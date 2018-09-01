using Marten;
using Marten.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PickEmServer.Heart
{
    public class GameService
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<GameService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ReferenceService _referenceSevice;

        public GameService(IDocumentStore documentStore, ILogger<GameService> logger, ReferenceService referenceSevice, IServiceProvider serviceProvider)
        {
            _documentStore = documentStore;
            _logger = logger;
            _referenceSevice = referenceSevice;
            _serviceProvider = serviceProvider;
        }

        public async Task<Game> AddGame(string seasonCode, int weekNumber, GameAdd newGame)
        {
            if (newGame == null)
            {
                throw new ArgumentException("No newGame parameter input for AddGame (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                _referenceSevice.ThrowIfNonexistantSeason(seasonCode);

                // verify the team codes exist
                var teams = await dbSession
                    .Query<TeamData>()
                    .Where(team => team.TeamCode == newGame.AwayTeamCode || team.TeamCode == newGame.HomeTeamCode)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if ( teams.Count != 2 )
                {
                    string matchedAwayTeamCode = teams.SingleOrDefault(team => team.TeamCode == newGame.AwayTeamCode)?.TeamCode;
                    string matchedHomeTeamCode = teams.SingleOrDefault(team => team.TeamCode == newGame.HomeTeamCode)?.TeamCode;

                    throw new ArgumentException($"Input away team code ({newGame.AwayTeamCode}) home team code ({newGame.HomeTeamCode}) not valid. Matched only away team code ({matchedAwayTeamCode}) home team code ({matchedHomeTeamCode})");
                }

                // verify game does not already exist by id or by season/week/awayteam/hometeam
                var game = await dbSession
                   .Query<GameData>()
                   .Where(g => 
                        g.GameId == newGame.GameId 
                        || 
                        ( 
                            g.SeasonCodeRef == seasonCode
                            &&
                            g.WeekNumberRef == weekNumber
                            &&
                            g.AwayTeam.TeamCodeRef == newGame.AwayTeamCode 
                            && 
                            g.HomeTeam.TeamCodeRef == newGame.HomeTeamCode ) 
                        )
                   .SingleOrDefaultAsync()
                   .ConfigureAwait(false);

                if ( game != null )
                {
                    throw new ArgumentException($"Matching game already exists by Id: {newGame.GameId} or Season: {seasonCode}, Week: {weekNumber}, Away Team: {newGame.AwayTeamCode} and Home Team: {newGame.HomeTeamCode}");
                }

                GameData newGameData = new GameData
                {
                    GameId = newGame.GameId,
                    GameStart = newGame.GameStart,
                    LastUpdated = DateTime.Now,
                    NeutralField = newGame.NeutralField,
                    SeasonCodeRef = seasonCode,
                    WeekNumberRef = weekNumber,
                    GameState = GameStates.SpreadNotSet,
                    AwayTeam = new GameTeamData { TeamCodeRef = newGame.AwayTeamCode, Score = 0, ScoreAfterSpread = 0, Winner = false },
                    HomeTeam = new GameTeamData { TeamCodeRef = newGame.HomeTeamCode, Score = 0, ScoreAfterSpread = 0, Winner = false },
                    // default spread (no data in this input)
                    Spread = new SpreadData { PointSpread = 0.0M, SpreadDirection = SpreadDirections.None }
                };

                dbSession.Store(newGameData);
                dbSession.SaveChanges();
            }

            // read back out to return
            return await this.ReadGame(newGame.GameId);
        }

        public string BuildGameDescription(GameData gameData)
        {
            switch ( gameData.GameState )
            {
                case GameStates.Cancelled:
                    return "Cancelled";

                case GameStates.Final:
                    return "Final";

                case GameStates.InGame:
                    if (gameData.CurrentPeriod.Length > 0 && Regex.IsMatch(gameData.CurrentPeriod, "^[0-9]"))
                    {
                        var timeClockFormatted = gameData.TimeClock.ToString(@"mm\:ss");
                        return $"{timeClockFormatted} - {gameData.CurrentPeriod}";
                    }
                    else
                        return gameData.CurrentPeriod;

                case GameStates.SpreadLocked:
                case GameStates.SpreadNotSet:
                    return gameData.GameStart.ToString("dddd, M/d - hh:mm tt");
            }
            return null;
        }


        internal async Task<Game> LockSpread(string seasonCode, int weekNumber, int gameId)
        {
            using (var dbSession = _documentStore.LightweightSession())
            {
                var game = await dbSession
                   .Query<GameData>()
                   .Where(g => g.GameId == gameId && g.SeasonCodeRef == seasonCode && g.WeekNumberRef == weekNumber)
                   .SingleOrDefaultAsync()
                   .ConfigureAwait(false);

                if (game == null)
                {
                    throw new ArgumentException($"No matching game found to update. Criteria Id: {gameId} or Season: {seasonCode}, Week: {weekNumber}");
                }

                GameChanger gameChanger = new GameChanger(game, dbSession);
                gameChanger.LockSpread();
                dbSession.Store(game);
                dbSession.SaveChanges();
            }

            // read back out to return
            return await this.ReadGame(gameId);
        }

        internal async Task<List<Game>> ReadGames(string seasonCode, int weekNumber)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var reffedAwayTeams = new Dictionary<string, TeamData>();
                var reffedHomeTeams = new Dictionary<string, TeamData>();

                var allGamesInWeek = await dbSession
                    .Query<GameData>()
                    .Include(g => g.AwayTeam.TeamCodeRef, reffedAwayTeams)
                    .Include(g => g.HomeTeam.TeamCodeRef, reffedHomeTeams)
                    .ToListAsync()
                    .ConfigureAwait(false);

                var apiGames = new List<Game>();

                foreach (var gameData in allGamesInWeek)
                {
                    apiGames.Add(MapGameDataToApi(gameData, reffedAwayTeams[gameData.AwayTeam.TeamCodeRef], reffedHomeTeams[gameData.HomeTeam.TeamCodeRef]));
                }

                return apiGames;
            }
        }

        public async Task<Game> UpdateGame(string SeasonCode, int WeekNumber, int GameId, GameUpdate gameUpdates)
        {
            if (gameUpdates == null)
            {
                throw new ArgumentException("No gameUpdates parameter input for UpdateGame (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var game = await dbSession
                   .Query<GameData>()
                   .Where(g => g.GameId == GameId && g.SeasonCodeRef == SeasonCode && g.WeekNumberRef == WeekNumber)
                   .SingleOrDefaultAsync()
                   .ConfigureAwait(false);

                if (game == null)
                {
                    throw new ArgumentException($"No matching game found to update. Criteria Id: {GameId} or Season: {SeasonCode}, Week: {WeekNumber}");
                }

                GameChanger gameChanger = new GameChanger(game, dbSession);
                var gameChanges = gameChanger.ApplyChanges(gameUpdates);
                dbSession.Store(game);

                if ( gameChanges.GameStateChanged || gameChanges.ScoreChanged )
                {
                    // score and/or game state changed, let all the leagues know.

                    // TODO - avoid direct new up of league service. Cannot IoC do to circlies because game service uses league. What to do?
                    var leagueService = _serviceProvider.GetService<LeagueService>();
                    var leaguesData = await leagueService.ApplyGameChanges(game, gameChanges, dbSession);

                    foreach ( var leagueData in leaguesData )
                    {
                        dbSession.Store(leagueData);
                    }
                }

                dbSession.SaveChanges();
            }

            // read back out to return
            return await this.ReadGame(GameId);
        }

        public async Task<Game> UpdateSpread(string SeasonCode, int WeekNumber, int GameId, SpreadUpdate spreadUpdates)
        {
            if (spreadUpdates == null)
            {
                throw new ArgumentException("No spreadUpdates parameter input for UpdateSpread (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var game = await dbSession
                   .Query<GameData>()
                   .Where(g => g.GameId == GameId && g.SeasonCodeRef == SeasonCode && g.WeekNumberRef == WeekNumber)
                   .SingleOrDefaultAsync()
                   .ConfigureAwait(false);

                if (game == null)
                {
                    throw new ArgumentException($"No matching game found to update. Criteria Id: {GameId} or Season: {SeasonCode}, Week: {WeekNumber}");
                }

                GameChanger gameChanger = new GameChanger(game, dbSession);
                gameChanger.ApplySpread(spreadUpdates);
                dbSession.Store(game);
                dbSession.SaveChanges();
            }

            // read back out to return
            return await this.ReadGame(GameId);
        }

        public async Task<Game> ReadGame(int gameId)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                TeamData awayTeam = null;
                TeamData homeTeam = null;

                var game = await dbSession
                    .Query<GameData>()
                    .Include<TeamData>(g => g.AwayTeam.TeamCodeRef, team => awayTeam = team)
                    .Include<TeamData>(g => g.HomeTeam.TeamCodeRef, team => homeTeam = team)
                    .Where(g => g.GameId == gameId)
                    .SingleAsync()
                    .ConfigureAwait(false);

                return MapGameDataToApi(game, awayTeam, homeTeam);

            }
        }

        public async Task<List<Game>> ReadGamesInAnyLeague(string seasonCode, int weekNumber)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                // get all leagues in season
                var leagues = await dbSession
                    .Query<LeagueData>()
                    .Where(l => l.SeasonCodeRef == seasonCode)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if ( leagues.Count == 0 )
                {
                    return new List<Game>();
                }

                Dictionary<int, int> distinctGameIds = new Dictionary<int, int>();

                foreach ( var league in leagues )
                {
                    var leagueWeek = league.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);

                    // ignore leagues that don't have this week
                    if ( leagueWeek != null )
                    {
                        foreach ( var game in leagueWeek.Games )
                        {
                            if ( !distinctGameIds.ContainsKey(game.GameIdRef) )
                            {
                                distinctGameIds.Add(game.GameIdRef, game.GameIdRef);
                            }
                        }
                    }
                }

                if ( distinctGameIds.Count == 0 )
                {
                    return new List<Game>();
                }

                var gameIdArray = distinctGameIds.Keys.ToArray();

                var reffedAwayTeams = new Dictionary<string, TeamData>();
                var reffedHomeTeams = new Dictionary<string, TeamData>();

                var allGamesInWeek = await dbSession
                    .Query<GameData>()
                    .Include(g => g.AwayTeam.TeamCodeRef, reffedAwayTeams)
                    .Include(g => g.HomeTeam.TeamCodeRef, reffedHomeTeams)
                    .Where(g => g.GameId.IsOneOf(gameIdArray))
                    .ToListAsync()
                    .ConfigureAwait(false);

                var apiGames = new List<Game>();

                foreach ( var gameData in allGamesInWeek )
                {
                    apiGames.Add(MapGameDataToApi(gameData, reffedAwayTeams[gameData.AwayTeam.TeamCodeRef], reffedHomeTeams[gameData.HomeTeam.TeamCodeRef]));
                }

                return apiGames;
                
            }
        }

        private Game MapGameDataToApi(GameData gameData, TeamData awayTeamData, TeamData homeTeamData)
        {
            return new Game
            {
                AwayTeam = new TeamScore
                {
                    Score = gameData.AwayTeam.Score,
                    ScoreAfterSpread = gameData.AwayTeam.ScoreAfterSpread,
                    Team = new Team
                    {
                        icon24FileName = awayTeamData.icon24FileName,
                        LongName = awayTeamData.LongName,
                        NcaaNameSeo = awayTeamData.NcaaNameSeo,
                        ShortName = awayTeamData.ShortName,
                        TeamCode = awayTeamData.TeamCode,
                        theSpreadName = awayTeamData.theSpreadName
                    },
                    Winner = gameData.AwayTeam.Winner
                },
                CurrentPeriod = gameData.CurrentPeriod,
                GameId = gameData.GameId,
                GameStart = gameData.GameStart,
                HomeTeam = new TeamScore
                {
                    Score = gameData.HomeTeam.Score,
                    ScoreAfterSpread = gameData.HomeTeam.ScoreAfterSpread,
                    Team = new Team
                    {
                        icon24FileName = homeTeamData.icon24FileName,
                        LongName = homeTeamData.LongName,
                        NcaaNameSeo = homeTeamData.NcaaNameSeo,
                        ShortName = homeTeamData.ShortName,
                        TeamCode = homeTeamData.TeamCode,
                        theSpreadName = homeTeamData.theSpreadName
                    },
                    Winner = gameData.HomeTeam.Winner
                },
                GameState = gameData.GameState,
                LastUpdated = gameData.LastUpdated,
                NeutralField = gameData.NeutralField,
                Spread = new Spread
                {
                    PointSpread = gameData.Spread.PointSpread,
                    SpreadDirection = gameData.Spread.SpreadDirection
                },
                TimeClock = gameData.TimeClock
            };

        }
    }
}
