using Marten;
using Marten.Linq;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Heart
{
    public class GameService
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<GameService> _logger;


        public GameService(IDocumentStore documentStore, ILogger<GameService> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        public async Task<Game> AddGame(string SeasonCode, int WeekNumber, GameAdd newGame)
        {
            using (var dbSession = _documentStore.LightweightSession())
            {
                // verify the team codes exist
                var teams = await dbSession
                    .Query<TeamData>()
                    .Where(team => team.TeamCode == newGame.AwayTeamCode || team.TeamCode == newGame.HomeTeamCode)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if ( teams.Count != 2 )
                {
                    string matchedAwayTeamCode = teams.SingleOrDefault(team => team.TeamCode == newGame.AwayTeamCode)?.TeamCode;
                    string matchedHomeTeamCode = teams.SingleOrDefault(team => team.TeamCode == newGame.AwayTeamCode)?.TeamCode;

                    throw new ArgumentException($"Input away team code ({newGame.AwayTeamCode}) home team code ({newGame.HomeTeamCode}) not valid. Matched only away team code ({matchedAwayTeamCode}) home team code ({matchedHomeTeamCode})");
                }

                // verify game does not already exist by id or by season/week/awayteam/hometeam
                var game = await dbSession
                   .Query<GameData>()
                   .Where(g => 
                        g.GameId == newGame.GameId 
                        || 
                        ( 
                            g.SeasonCodeRef == SeasonCode
                            &&
                            g.WeekNumberRef == WeekNumber
                            &&
                            g.AwayTeam.TeamCodeRef == newGame.AwayTeamCode 
                            && 
                            g.HomeTeam.TeamCodeRef == newGame.HomeTeamCode ) 
                        )
                   .SingleOrDefaultAsync()
                   .ConfigureAwait(false);

                if ( game != null )
                {
                    throw new ArgumentException($"Matching game already exists by Id: {newGame.GameId} or Season: {SeasonCode}, Week: {WeekNumber}, Away Team: {newGame.AwayTeamCode} and Home Team: {newGame.HomeTeamCode}");
                }

                GameData newGameData = new GameData
                {
                    GameId = newGame.GameId,
                    GameStart = newGame.GameStart,
                    LastUpdated = DateTime.Now,
                    NeutralField = newGame.NeutralField,
                    SeasonCodeRef = SeasonCode,
                    WeekNumberRef = WeekNumber,
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


        public async Task<Game> UpdateGame(string SeasonCode, int WeekNumber, int GameId, GameUpdate gameUpdates)
        {
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
                gameChanger.ApplyChanges(gameUpdates);
                gameChanger.SaveChanges();

            }

            // read back out to return
            return await this.ReadGame(GameId);
        }

        public async Task<Game> UpdateSpread(string SeasonCode, int WeekNumber, int GameId, SpreadUpdate spreadUpdates)
        {
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
                gameChanger.SaveChanges();
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

                Game apiGame = new Game
                {
                    AwayTeam = new TeamScore
                    {
                        Score = game.AwayTeam.Score,
                        ScoreAfterSpread = game.AwayTeam.ScoreAfterSpread,
                        Team = new Team
                        {
                            icon24FileName = awayTeam.icon24FileName,
                            LongName = awayTeam.LongName,
                            NcaaNameSeo = awayTeam.NcaaNameSeo,
                            ShortName = awayTeam.ShortName,
                            TeamCode = awayTeam.TeamCode,
                            theSpreadName = awayTeam.theSpreadName
                        },
                        Winner = game.AwayTeam.Winner
                    },
                    CurrentPeriod = game.CurrentPeriod,
                    GameId = game.GameId,
                    GameStart = game.GameStart,
                    HomeTeam = new TeamScore
                    {
                        Score = game.HomeTeam.Score,
                        ScoreAfterSpread = game.HomeTeam.ScoreAfterSpread,
                        Team = new Team
                        {
                            icon24FileName = homeTeam.icon24FileName,
                            LongName = homeTeam.LongName,
                            NcaaNameSeo = homeTeam.NcaaNameSeo,
                            ShortName = homeTeam.ShortName,
                            TeamCode = homeTeam.TeamCode,
                            theSpreadName = homeTeam.theSpreadName
                        },
                        Winner = game.HomeTeam.Winner
                    },
                    GameState = game.GameState,
                    LastUpdated = game.LastUpdated,
                    NeutralField = game.NeutralField,
                    Spread = new Spread
                    {
                        PointSpread = game.Spread.PointSpread,
                        SpreadDirection = game.Spread.SpreadDirection
                    },
                    TimeClock = game.TimeClock
                };

                return apiGame;
            }
        }
    }
}
