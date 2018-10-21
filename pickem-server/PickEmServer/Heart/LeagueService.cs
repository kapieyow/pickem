using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.App.Models;
using PickEmServer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Heart
{

    public class LeagueService
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<LeagueService> _logger;
        private readonly GameService _gameSevice;
        private readonly PickemEventer _pickemEventer;
        private readonly ReferenceService _referenceSevice;
        private readonly UserManager<PickEmUser> _userManager;

        private class LeagueGameComparer : IEqualityComparer<LeagueGameData>
        {
            public bool Equals(LeagueGameData x, LeagueGameData y)
            {
                return x.GameIdRef == y.GameIdRef;
            }

            public int GetHashCode(LeagueGameData obj)
            {
                return obj.GetHashCode();
            }
        }

        private class LeagueWithGamesAndTeamDataForWeek
        {
            public LeagueData LeagueData { get; set; }
            public List<GameData> GameDataForWeek { get; set; }
            public Dictionary<string, TeamData> referencedAwayTeamData { get; set; }
            public Dictionary<string, TeamData> referencedHomeTeamData { get; set; }
        }
        public LeagueService(
            IDocumentStore documentStore, 
            ILogger<LeagueService> logger, 
            PickemEventer pickemEventer, 
            ReferenceService referenceService, 
            GameService gameSevice, 
            UserManager<PickEmUser> userManager)
        {
            _documentStore = documentStore;
            _gameSevice = gameSevice;
            _logger = logger;
            _pickemEventer = pickemEventer;
            _referenceSevice = referenceService;
            _userManager = userManager;
        }

        public async Task<League> AddLeague(string seasonCode, LeagueAdd newLeague)
        {
            if (newLeague == null)
            {
                throw new ArgumentException("No newLeague parameter input for AddLeague (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                // verify season exists
                _referenceSevice.ThrowIfNonexistantSeason(seasonCode);

                // verify league does not already exist 
                var game = await dbSession
                   .Query<LeagueData>()
                   .Where(l => l.LeagueCode == newLeague.LeagueCode)
                   .SingleOrDefaultAsync()
                   .ConfigureAwait(false);

                if (game != null)
                {
                    throw new ArgumentException($"Matching league already exists with league code: {newLeague.LeagueCode}");
                }

                LeagueData newLeagueData = new LeagueData
                {
                    LeagueCode = newLeague.LeagueCode,
                    LeagueTitle = newLeague.LeagueTitle,
                    PlayerSeasonScores = new List<PlayerScoreSubtotalData>(),
                    Players = new List<LeaguePlayerData>(),
                    SeasonCodeRef = seasonCode,
                    Weeks = new List<LeagueWeekData>()
                };

                foreach (var weekNumberRef in newLeague.WeekNumbers)
                {
                    newLeagueData.Weeks.Add(new LeagueWeekData
                    {
                        Games = new List<LeagueGameData>(),
                        PlayerWeekScores = new List<PlayerScoreSubtotalData>(),
                        WeekNumberRef = weekNumberRef
                    });
                }

                dbSession.Store(newLeagueData);
                dbSession.SaveChanges();

                return this.MapLeagueData(newLeagueData);
            }
        }

        internal async Task<League> AddLeagueGame(string seasonCode, string uncheckedLeagueCode, int weekNumber, LeagueGameAdd newLeagueGame)
        {
            if (newLeagueGame == null)
            {
                throw new ArgumentException("No newLeagueGame parameter input for AddLeagueGame (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = await this.GetLeagueData(dbSession, uncheckedLeagueCode);
                var exactLeagueCode = leagueData.LeagueCode;

                var leagueWeek = leagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);
                if (leagueWeek == null)
                {
                    throw new ArgumentException($"League with league code: {exactLeagueCode} does not contain week: {weekNumber}");
                }

                if (leagueWeek.Games.Exists(g => g.GameIdRef == newLeagueGame.GameId))
                {
                    throw new ArgumentException($"League with league code: {exactLeagueCode} already has game with gameid: {newLeagueGame.GameId} for week: {weekNumber}");
                }

                var game = dbSession
                    .Query<GameData>()
                    .Where(g => g.GameId == newLeagueGame.GameId)
                    .SingleOrDefault();

                if (game == null)
                {
                    throw new ArgumentException($"Game with gameid: {newLeagueGame.GameId} does not exist");
                }

                if (game.WeekNumberRef != weekNumber)
                {
                    throw new ArgumentException($"Game week must match League week and they do not. League with league code: {exactLeagueCode} has week: {weekNumber}. Game with game id: {newLeagueGame.GameId} has week {game.WeekNumberRef}");
                }

                // whew we can add this now.
                leagueWeek.Games.Add(new LeagueGameData { GameIdRef = newLeagueGame.GameId, PlayerPicks = new List<PlayerPickData>() });

                SynchGamesAndPlayers(leagueData);

                dbSession.Store(leagueData);
                dbSession.SaveChanges();

                var pickemEvent = new PickemSystemEvent(PickemSystemEventTypes.LeagueGameAdded, seasonCode, exactLeagueCode, weekNumber, newLeagueGame.GameId);
                pickemEvent.LeagueCodesAffected.Add(exactLeagueCode);
                _pickemEventer.Emit(pickemEvent);

                return this.MapLeagueData(leagueData);
            }
        }

        internal async Task<League> AddLeaguePlayer(string uncheckedLeagueCode, LeaguePlayerAdd newLeaguePlayer)
        {
            if (newLeaguePlayer == null)
            {
                throw new ArgumentException("No newLeagueGame parameter input for AddLeaguePlayer (is null)");
            }

            PickEmUser pickEmUser = null;
            // NOTE: user manager lookup is case INsensitive. Use the result which is cased exactly.
            pickEmUser = await _userManager.FindByNameAsync(newLeaguePlayer.UserName);

            if (pickEmUser == null)
            {
                throw new ArgumentException($"No user with username (id) : {newLeaguePlayer.UserName}. Cannot add league player");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = dbSession
                    .Query<LeagueData>()
                    .Where(l => l.LeagueCode.Equals(uncheckedLeagueCode, StringComparison.Ordinal))
                    .SingleOrDefault();
                var exactLeagueCode = leagueData.LeagueCode;

                if (leagueData == null)
                {
                    throw new ArgumentException($"No league exists with league code: {exactLeagueCode}");
                }


                if (leagueData.Players.Exists(p => p.PlayerTag == newLeaguePlayer.PlayerTag))
                {
                    throw new ArgumentException($"League with league code: {exactLeagueCode} already has player with player tag: {newLeaguePlayer.PlayerTag}");
                }

                // whew we can add this now.
                leagueData.Players.Add(new LeaguePlayerData { PlayerTag = newLeaguePlayer.PlayerTag, UserNameRef = pickEmUser.UserName });

                SynchGamesAndPlayers(leagueData);

                dbSession.Store(leagueData);
                dbSession.SaveChanges();

                var pickemEvent = new PickemSystemEvent(PickemSystemEventTypes.LeaguePlayerAdded, null, exactLeagueCode, null, null);
                pickemEvent.DynamicKeys.playerTag = newLeaguePlayer.PlayerTag;
                pickemEvent.DynamicInformation.userName = pickEmUser.UserName;
                pickemEvent.LeagueCodesAffected.Add(exactLeagueCode);
                _pickemEventer.Emit(pickemEvent);

                return this.MapLeagueData(leagueData);
            }
        }

        internal List<LeagueData> ApplyGameChanges(GameData updatedGame, GameChanges gameChanges, IDocumentSession runningDbSession)
        {
            var leagueGameToMatch = new LeagueGameData { GameIdRef = updatedGame.GameId };

            // TODO: is there a better way to do this, native to Marten? Failed with the following
            //      .Where(l => l.Weeks.Any(w => w.Games.Any(g => g.GameIdRef == updatedGame.GameId)))
            //  .Where(l => l.Weeks.Any(w => w.Games.Contains(leagueGameToMatch, new LeagueGameComparer())))
            var directSql =
                $@"SELECT 
                    jsonb_pretty(l.data)
                FROM
                    public.mt_doc_leaguedata l,
                    jsonb_array_elements(data->'Weeks') weeks,
	                jsonb_array_elements(weeks->'Games') games
                WHERE
                    games->>'GameIdRef' = '{updatedGame.GameId}'"
                ;

            var associatedLeagues = runningDbSession.Query<LeagueData>(directSql).ToList();

            foreach (var leagueData in associatedLeagues)
            {
                // Checking both change flags and updating player picks. 
                if (gameChanges.ScoreChanged || gameChanges.GameStateChanged)
                {
                    // game score changed : update pick status(es) for related games
                    // TODO: ToArray is dumb. There will only be one. How to in Linq?
                    var assocatedLeagueGameData = leagueData.Weeks.SelectMany(w => w.Games.Where(g => g.GameIdRef == updatedGame.GameId)).ToArray();

                    foreach (var playerPickData in assocatedLeagueGameData[0].PlayerPicks)
                    {
                        switch (updatedGame.GameState)
                        {
                            case GameStates.Cancelled:
                                playerPickData.PickStatus = PickStates.Cancelled;
                                break;

                            case GameStates.Final:
                                if (updatedGame.AwayTeam.ScoreAfterSpread < updatedGame.HomeTeam.ScoreAfterSpread)
                                {
                                    // home team winning (on spread)
                                    switch (playerPickData.Pick)
                                    {
                                        case PickTypes.None:
                                            playerPickData.PickStatus = PickStates.None;
                                            break;

                                        case PickTypes.Away:
                                            playerPickData.PickStatus = PickStates.Lost;
                                            break;

                                        case PickTypes.Home:
                                            playerPickData.PickStatus = PickStates.Won;
                                            break;

                                        default:
                                            throw new ArgumentException($"Unknown PickTypes: {playerPickData.Pick}");
                                    }
                                }
                                else if (updatedGame.AwayTeam.ScoreAfterSpread > updatedGame.HomeTeam.ScoreAfterSpread)
                                {
                                    // away team winning (on spread)
                                    switch (playerPickData.Pick)
                                    {
                                        case PickTypes.None:
                                            playerPickData.PickStatus = PickStates.None;
                                            break;

                                        case PickTypes.Away:
                                            playerPickData.PickStatus = PickStates.Won;
                                            break;

                                        case PickTypes.Home:
                                            playerPickData.PickStatus = PickStates.Lost;
                                            break;

                                        default:
                                            throw new ArgumentException($"Unknown PickTypes: {playerPickData.Pick}");
                                    }
                                }
                                else
                                {
                                    // tied (on spread)
                                    if (playerPickData.Pick == PickTypes.None)
                                    {
                                        playerPickData.PickStatus = PickStates.None;
                                    }
                                    else
                                    {
                                        playerPickData.PickStatus = PickStates.Pushed;
                                    }
                                }
                                break;

                            case GameStates.InGame:
                                if (updatedGame.AwayTeam.ScoreAfterSpread < updatedGame.HomeTeam.ScoreAfterSpread)
                                {
                                    // home team winning (on spread)
                                    switch (playerPickData.Pick)
                                    {
                                        case PickTypes.None:
                                            playerPickData.PickStatus = PickStates.None;
                                            break;

                                        case PickTypes.Away:
                                            playerPickData.PickStatus = PickStates.Losing;
                                            break;

                                        case PickTypes.Home:
                                            playerPickData.PickStatus = PickStates.Winning;
                                            break;

                                        default:
                                            throw new ArgumentException($"Unknown PickTypes: {playerPickData.Pick}");
                                    }
                                }
                                else if (updatedGame.AwayTeam.ScoreAfterSpread > updatedGame.HomeTeam.ScoreAfterSpread)
                                {
                                    // away team winning (on spread)
                                    switch (playerPickData.Pick)
                                    {
                                        case PickTypes.None:
                                            playerPickData.PickStatus = PickStates.None;
                                            break;

                                        case PickTypes.Away:
                                            playerPickData.PickStatus = PickStates.Winning;
                                            break;

                                        case PickTypes.Home:
                                            playerPickData.PickStatus = PickStates.Losing;
                                            break;

                                        default:
                                            throw new ArgumentException($"Unknown PickTypes: {playerPickData.Pick}");
                                    }
                                }
                                else
                                {
                                    // tied (on spread)
                                    if (playerPickData.Pick == PickTypes.None)
                                    {
                                        playerPickData.PickStatus = PickStates.None;
                                    }
                                    else
                                    {
                                        playerPickData.PickStatus = PickStates.Pushing;
                                    }
                                }
                                break;

                            default:
                                throw new Exception($"Invalid game state to change score {updatedGame.GameState}");
                        }
                    }
                }

                if (gameChanges.GameStateChanged)
                {
                    if (updatedGame.GameState == GameStates.Final || updatedGame.GameState == GameStates.Cancelled)
                    {
                        this.SynchScoreboards(leagueData);
                    }
                }
            }

            return associatedLeagues.ToList();
        }

        public async Task<League> ReadLeague(string uncheckedLeagueCode)
        {
            var leagueData = await this.GetLeagueData(uncheckedLeagueCode);
            return MapLeagueData(leagueData);
        }

        public async Task<Player> ReadLeaguePlayer(string seasonCode, string uncheckedLeagueCode, string uncheckedUserName)
        {
            var leagueData = await this.GetLeagueData(uncheckedLeagueCode);
            var exactLeagueCode = leagueData.LeagueCode;

            var playerData = leagueData.Players.SingleOrDefault(p => p.UserNameRef.Equals(uncheckedUserName, StringComparison.OrdinalIgnoreCase));
            if (playerData == null)
            {
                throw new ArgumentException($"User name: {uncheckedUserName} is not references in league: {exactLeagueCode}, season: {seasonCode}");
            }

            return new Player
            {
                PlayerTag = playerData.PlayerTag,
                UserName = playerData.UserNameRef
            };
        }

        public async Task<List<Player>> ReadLeaguePlayers(string seasonCode, string uncheckedLeagueCode)
        {
            var leagueData = await this.GetLeagueData(uncheckedLeagueCode);
            var exactLeagueCode = leagueData.LeagueCode;

            var resultPlayers = new List<Player>();

            foreach (var playerData in leagueData.Players)
            {
                resultPlayers.Add(new Player
                {
                    PlayerTag = playerData.PlayerTag,
                    UserName = playerData.UserNameRef
                });
            }

            return resultPlayers.OrderBy(p => p.PlayerTag).ToList();
        }

        public async Task<LeagueWeeks> ReadLeagueWeeks(string seasonCode, string uncheckedLeagueCode)
        {
            var leagueData = await this.GetLeagueData(uncheckedLeagueCode);
            var exactLeagueCode = leagueData.LeagueCode;

            var leagueWeeks = new LeagueWeeks();
            leagueWeeks.WeekNumbers = new List<int>();

            foreach (var weekData in leagueData.Weeks)
            {
                leagueWeeks.WeekNumbers.Add(weekData.WeekNumberRef);
            }

            return leagueWeeks;
        }

        public async Task<WeekScoreboard> ReadWeekScoreboard(string seasonCode, string uncheckedLeagueCode, int weekNumber, string uncheckedAuthenticatedUserName)
        {
            // determine if the authenticated user has this player tag (if not hide picks for games not started)
            var authenticatedPlayer = await this.ReadLeaguePlayer(seasonCode, uncheckedLeagueCode, uncheckedAuthenticatedUserName);

            var leagueWithExtendedData = await this.ReadLeagueWithWeekGamesExpanded(seasonCode, uncheckedLeagueCode, weekNumber);
            var exactLeagueCode = leagueWithExtendedData.LeagueData.LeagueCode;

            // get all players and loop over to map for full week
            // ordered by week wins descending
            var playerTags = leagueWithExtendedData.LeagueData
                    .Weeks.Single(w => w.WeekNumberRef == weekNumber)
                    .PlayerWeekScores.OrderByDescending(pws => pws.Points)
                    .ThenBy(pws => pws.PlayerTagRef)
                    .Select(pws => pws.PlayerTagRef)
                    ;

            var weekScoreboard = new WeekScoreboard();
            weekScoreboard.PlayerTags = playerTags.ToList();
            weekScoreboard.PlayerWins = new List<PlayerWeekWins>();
            weekScoreboard.GamePickScoreboards = new List<GameScoreboard>();

            foreach (var playerTag in playerTags)
            {
                var playerWeekWins = new PlayerWeekWins();
                playerWeekWins.PlayerTag = playerTag;
                playerWeekWins.Wins = leagueWithExtendedData.LeagueData
                    .Weeks.Single(w => w.WeekNumberRef == weekNumber)
                    .PlayerWeekScores.Single(pws => pws.PlayerTagRef == playerTag)
                    .Points;

                weekScoreboard.PlayerWins.Add(playerWeekWins);
            }

            weekScoreboard.GamePickScoreboards = this.MapDataToGameScoreboards(
                seasonCode, 
                exactLeagueCode, 
                weekNumber, 
                weekScoreboard.PlayerTags, 
                authenticatedPlayer.PlayerTag, 
                leagueWithExtendedData);

            return weekScoreboard;
        }

        public async Task<PlayerScoreboard> ReadPlayerScoreboard(string seasonCode, string uncheckedLeagueCode, int weekNumber, string uncheckedPlayerTag, string uncheckedAuthenticatedUserName)
        {
            var leagueWithExtendedData = await this.ReadLeagueWithWeekGamesExpanded(seasonCode, uncheckedLeagueCode, weekNumber);
            var exactLeagueCode = leagueWithExtendedData.LeagueData.LeagueCode;

            // determine if the authenticated user has this player tag (if not hide picks for games not started)
            var authenticatedPlayer = await this.ReadLeaguePlayer(seasonCode, exactLeagueCode, uncheckedAuthenticatedUserName);

            var playerScoreboard = new PlayerScoreboard();

            playerScoreboard.GamePickScoreboards = this.MapDataToGameScoreboards(
                seasonCode, 
                exactLeagueCode, 
                weekNumber, 
                new List<string> { uncheckedPlayerTag }, 
                authenticatedPlayer.PlayerTag, 
                leagueWithExtendedData);

            var weekScoreSubtotals = leagueWithExtendedData.LeagueData
                .Weeks.Single(w => w.WeekNumberRef == weekNumber)
                .PlayerWeekScores.Single(pwss => pwss.PlayerTagRef.Equals(uncheckedPlayerTag, StringComparison.OrdinalIgnoreCase));

            // game counts
            playerScoreboard.Games = playerScoreboard.GamePickScoreboards.Count;
            playerScoreboard.GamesPicked = weekScoreSubtotals.GamesPicked;
            playerScoreboard.GamesWon = weekScoreSubtotals.Points;
            playerScoreboard.GamesLost = weekScoreSubtotals.GamesLost;
            playerScoreboard.GamesPending = weekScoreSubtotals.GamesPending;

            return playerScoreboard;
        }

        public async Task<List<League>> ReadUserLeagues(string userName)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var leagueData = await dbSession
                    .Query<LeagueData>()
                    .Where(l => l.Players.Any(p => p.UserNameRef == userName))
                    .ToListAsync()
                    .ConfigureAwait(false);

                return leagueData
                    .Select(ld => MapLeagueData(ld))
                    .OrderBy(l => l.LeagueTitle)
                    .ToList();
            }
        }


        internal async Task<LeagueScoreboard> ReadLeagueScoreboard(string seasonCode, string uncheckedLeagueCode)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var leagueData = await this.GetLeagueData(dbSession, uncheckedLeagueCode);

                var leagueScoreboard = new LeagueScoreboard();
                leagueScoreboard.WeekNumbers = leagueData.Weeks.Select(w => w.WeekNumberRef).OrderBy(wn => wn).ToList();
                leagueScoreboard.PlayerScoreboards = new List<PlayerSeasonScoreboard>();

                // get player tags in "winning" order
                var playerTags = leagueData
                   .PlayerSeasonScores.OrderByDescending(pss => pss.Points)
                   .ThenBy(pss => pss.PlayerTagRef)
                   .Select(pss => pss.PlayerTagRef)
                   ;

                foreach ( var playerTag in playerTags )
                {
                    var playerSeasonScoreboard = new PlayerSeasonScoreboard();
                    
                    playerSeasonScoreboard.PlayerTag = playerTag;
                    playerSeasonScoreboard.Wins = leagueData.PlayerSeasonScores.Single(pss => pss.PlayerTagRef == playerTag).Points;
                    playerSeasonScoreboard.WeeklyScores = new List<WeekScore>();

                    foreach ( var week in leagueScoreboard.WeekNumbers )
                    {
                        var weekScore = new WeekScore
                        {
                            Points = leagueData.Weeks.Single(w => w.WeekNumberRef == week).PlayerWeekScores.Single(pws => pws.PlayerTagRef == playerTag).Points,
                            WeekNumber = week
                        };

                        playerSeasonScoreboard.WeeklyScores.Add(weekScore);
                    }

                    leagueScoreboard.PlayerScoreboards.Add(playerSeasonScoreboard);
                }

                return leagueScoreboard;

            }
        }

        private async Task<LeagueWithGamesAndTeamDataForWeek> ReadLeagueWithWeekGamesExpanded(string seasonCode, string uncheckedLeagueCode, int weekNumber)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var leagueWithExtendedData = new LeagueWithGamesAndTeamDataForWeek();

                // get league
                leagueWithExtendedData.LeagueData = await this.GetLeagueData(dbSession, uncheckedLeagueCode);
                var exactLeagueCode = leagueWithExtendedData.LeagueData.LeagueCode;

                // get games in league for week
                var leagueWeek = leagueWithExtendedData.LeagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);

                if (leagueWeek == null)
                {
                    throw new ArgumentException($"League with league code: {exactLeagueCode} does not contain week: {weekNumber}");
                }

                var gameIdArray = leagueWeek.Games.Select(g => g.GameIdRef).ToArray();

                leagueWithExtendedData.referencedAwayTeamData = new Dictionary<string, TeamData>();
                leagueWithExtendedData.referencedHomeTeamData = new Dictionary<string, TeamData>();

                 var gamesForWeek = await dbSession
                    .Query<GameData>()
                    .Include(g => g.AwayTeam.TeamCodeRef, leagueWithExtendedData.referencedAwayTeamData)
                    .Include(g => g.HomeTeam.TeamCodeRef, leagueWithExtendedData.referencedHomeTeamData)
                    .Where(g => g.GameId.IsOneOf(gameIdArray))
                    .ToListAsync()
                    .ConfigureAwait(false);

                leagueWithExtendedData.GameDataForWeek = gamesForWeek.ToList();

                return leagueWithExtendedData;
            }

        }

        private List<GameScoreboard> MapDataToGameScoreboards(
            string seasonCode, 
            string leagueCode, 
            int weekNumber, 
            List<string> uncheckedPlayerTags, 
            string authenticatedPlayerTag, 
            LeagueWithGamesAndTeamDataForWeek leagueWithExtendedData)
        {
            if (uncheckedPlayerTags == null || uncheckedPlayerTags.Count == 0 )
            {
                throw new ArgumentException($"Must have at least 1 player tag uncheckedPlayerTags for MapDataToGameScoreboards");
            }

            var gameScoreboards = new List<GameScoreboard>();

            // game loop 
            foreach (var gameData in leagueWithExtendedData.GameDataForWeek.OrderBy(game => game.GameStart).ThenBy(game => game.AwayTeam.TeamCodeRef))
            {
                var weekData = leagueWithExtendedData.LeagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);
                if (weekData == null)
                {
                    throw new ArgumentException($"League: {leagueCode} for season: {seasonCode} does not contain a week: {weekNumber}");
                }

                var pickemGameData = weekData.Games.SingleOrDefault(g => g.GameIdRef == gameData.GameId);
                if (pickemGameData == null)
                {
                    throw new ArgumentException($"League: {leagueCode} for season: {seasonCode}, week: {weekNumber} does not have a game with gameid: {gameData.GameId}");
                }

                var awayTeamWeekStats = this.FindTeamWeekStats(leagueWithExtendedData.referencedAwayTeamData[gameData.AwayTeam.TeamCodeRef], seasonCode, weekNumber);
                var homeTeamWeekStats = this.FindTeamWeekStats(leagueWithExtendedData.referencedHomeTeamData[gameData.HomeTeam.TeamCodeRef], seasonCode, weekNumber);


                var gameScoreboard = new GameScoreboard
                {
                    AwayTeamIconFileName = leagueWithExtendedData.referencedAwayTeamData[gameData.AwayTeam.TeamCodeRef].icon24FileName,
                    AwayTeamLongName = string.IsNullOrEmpty(
                        leagueWithExtendedData.referencedAwayTeamData[gameData.AwayTeam.TeamCodeRef].LongName)
                        ? gameData.AwayTeam.TeamCodeRef
                        : leagueWithExtendedData.referencedAwayTeamData[gameData.AwayTeam.TeamCodeRef].LongName,
                    AwayTeamLosses = awayTeamWeekStats?.Losses ?? 0,
                    AwayTeamRank = awayTeamWeekStats?.FbsRank ?? 0,
                    AwayTeamScore = gameData.AwayTeam.Score,
                    AwayTeamWins = awayTeamWeekStats?.Wins ?? 0,
                    GameCurrentPeriod = gameData.CurrentPeriod,
                    GameId = gameData.GameId,
                    GameStart = gameData.GameStart,
                    GameState = gameData.GameState,
                    GameStatusDescription = _gameSevice.BuildGameDescription(gameData),
                    GameTimeClock = gameData.TimeClock,
                    HomeTeamIconFileName = leagueWithExtendedData.referencedHomeTeamData[gameData.HomeTeam.TeamCodeRef].icon24FileName,
                    HomeTeamLongName = string.IsNullOrEmpty(
                        leagueWithExtendedData.referencedHomeTeamData[gameData.HomeTeam.TeamCodeRef].LongName) 
                        ? gameData.HomeTeam.TeamCodeRef 
                        : leagueWithExtendedData.referencedHomeTeamData[gameData.HomeTeam.TeamCodeRef].LongName,
                    HomeTeamLosses = homeTeamWeekStats?.Losses ?? 0,
                    HomeTeamRank = homeTeamWeekStats?.FbsRank ?? 0,
                    HomeTeamScore = gameData.HomeTeam.Score,
                    HomeTeamWins = homeTeamWeekStats?.Wins ?? 0,
                    Spread = gameData.Spread.PointSpread,
                    SpreadDirection = gameData.Spread.SpreadDirection,
                    Leader = gameData.Leader,
                    LeaderAfterSpread = gameData.LeaderAfterSpread
                };

                gameScoreboard.PickScoreboards = new List<PickScoreboard>();

                // playa loop 
                foreach ( var uncheckedPlayerTag in uncheckedPlayerTags)
                {
                    var playerPickData = pickemGameData.PlayerPicks.SingleOrDefault(pp => pp.PlayerTagRef.Equals(uncheckedPlayerTag, StringComparison.OrdinalIgnoreCase));
                    if (playerPickData == null)
                    {
                        throw new ArgumentException($"League: {leagueCode} for season: {seasonCode}, week: {weekNumber}, game {gameData.GameId}, has no player pick for {uncheckedPlayerTag}. Is {uncheckedPlayerTag} in this league?");
                    }

                    bool readingPlayersOwnScoreboard = (authenticatedPlayerTag == playerPickData.PlayerTagRef);

                    string pickedTeamIconFileName = null;
                    string pickedTeamLongName = null;

                    // setup pick team info
                    switch (playerPickData.Pick)
                    {
                        case PickTypes.Away:
                            pickedTeamIconFileName = gameScoreboard.AwayTeamIconFileName;
                            pickedTeamLongName = gameScoreboard.AwayTeamLongName;
                            break;

                        case PickTypes.Hidden:
                            pickedTeamLongName = "Hidden";
                            break;

                        case PickTypes.Home:
                            pickedTeamIconFileName = gameScoreboard.HomeTeamIconFileName;
                            pickedTeamLongName = gameScoreboard.HomeTeamLongName;
                            break;

                        case PickTypes.None:
                            pickedTeamLongName = "None";
                            break;

                    }

                    PickTypes pickType = CalculatePickState(playerPickData, gameData, readingPlayersOwnScoreboard);

                    var playerGamePickScoreboard = new PickScoreboard
                    {
                        Pick = pickType,
                        PickState = playerPickData.PickStatus,
                        PlayerTag = playerPickData.PlayerTagRef,
                        PickedTeamIconFileName = ( pickType == PickTypes.Hidden ) ? null : pickedTeamIconFileName,
                        PickedTeamLongName = ( pickType == PickTypes.Hidden ) ? "Hidden" : pickedTeamLongName
                    };

                    gameScoreboard.PickScoreboards.Add(playerGamePickScoreboard);
                }

                gameScoreboards.Add(gameScoreboard);
            }

            return gameScoreboards;
        }

        private int CalculateGamesPending(LeagueWeekData leagueWeekData, string playerTag)
        {
            // assuming upstream verified this player is in this week data.
            return leagueWeekData.Games.SelectMany(g => g.PlayerPicks.Where(
                pp => pp.PlayerTagRef == playerTag 
                && pp.Pick != PickTypes.None 
                && pp.PickStatus != PickStates.Lost 
                && pp.PickStatus != PickStates.Won
                && pp.PickStatus != PickStates.Pushed
                && pp.PickStatus != PickStates.Cancelled)
                ).Count();
        }

        private PickTypes CalculatePickState(PlayerPickData playerPickData, GameData gameData, bool readingPlayersOwnScoreboard)
        {
            if ( !readingPlayersOwnScoreboard && (gameData.GameState == GameStates.SpreadLocked || gameData.GameState == GameStates.SpreadNotSet) && playerPickData.Pick != PickTypes.None )
            {
                // pick for another player (not the one logged in), the game has not started AND the other player has made a pick
                return PickTypes.Hidden;
            }
            else
            {
                return playerPickData.Pick;
            }
        }

        private TeamWeekStats FindTeamWeekStats(TeamData teamData, string seasonCode, int weekNumber)
        {
            if ( teamData.Seasons == null )
            {
                return null;
            }

            var teamSeason = teamData.Seasons.SingleOrDefault(s => s.SeasonCodeRef == seasonCode);

            if ( teamSeason == null )
            {
                return null;
            }

            return teamSeason.WeekStats.SingleOrDefault(w => w.WeekNumberRef == weekNumber);

        }

        internal async Task<Player> SetPlayer(string seasonCode, string uncheckedLeagueCode, string uncheckedUserName, PlayerUpdate playerUpdate)
        {
            if (playerUpdate == null)
            {
                throw new ArgumentException("No playerUpdate parameter input for SetPlayer (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = await this.GetLeagueData(dbSession, uncheckedLeagueCode);
                var exactLeagueCode = leagueData.LeagueCode;

                var playerData = leagueData.Players.SingleOrDefault(p => p.UserNameRef.Equals(uncheckedUserName, StringComparison.OrdinalIgnoreCase));
                if (playerData == null)
                {
                    throw new ArgumentException($"User name: {uncheckedUserName} is not references in league: {exactLeagueCode}, season: {seasonCode}");
                }

                var originalPlayerTag = playerData.PlayerTag;
                playerData.PlayerTag = playerUpdate.PlayerTag;

                // sync all refs
                var playerSeasonScoreboard = leagueData.PlayerSeasonScores.Single(pss => pss.PlayerTagRef == originalPlayerTag);
                playerSeasonScoreboard.PlayerTagRef = playerData.PlayerTag;

                foreach (var weekData in leagueData.Weeks)
                {
                    var playerWeekScore = weekData.PlayerWeekScores.Single(pws => pws.PlayerTagRef == originalPlayerTag);
                    playerWeekScore.PlayerTagRef = playerData.PlayerTag;

                    foreach (var gameData in weekData.Games)
                    {
                        var playerPickData = gameData.PlayerPicks.Single(pp => pp.PlayerTagRef == originalPlayerTag);
                        playerPickData.PlayerTagRef = playerData.PlayerTag;
                    }
                }

                dbSession.Store(leagueData);
                dbSession.SaveChanges();

                var pickemEvent = new PickemSystemEvent(PickemSystemEventTypes.LeaguePlayerChanged, null, exactLeagueCode, null, null);
                pickemEvent.DynamicKeys.playerTag = playerUpdate.PlayerTag;
                pickemEvent.DynamicInformation.userName = playerData.UserNameRef;
                pickemEvent.LeagueCodesAffected.Add(exactLeagueCode);
                _pickemEventer.Emit(pickemEvent);

                return await this.ReadLeaguePlayer(seasonCode, exactLeagueCode, playerData.UserNameRef);
            }
        }

        internal async Task<PlayerPick> SetPlayerPick(string seasonCode, string uncheckedLeagueCode, int weekNumber, string uncheckedPlayerTag, int gameId, PlayerPickUpdate newPlayerPick)
        {
            if (newPlayerPick == null)
            {
                throw new ArgumentException("No newPlayerPick parameter input for SetPlayerPick (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = await this.GetLeagueData(dbSession, uncheckedLeagueCode);
                var exactLeagueCode = leagueData.LeagueCode;

                var weekData = leagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);
                if (weekData == null)
                {
                    throw new ArgumentException($"League: {exactLeagueCode} for season: {seasonCode} does not contain a week: {weekNumber}");
                }

                var pickemGameData = weekData.Games.SingleOrDefault(g => g.GameIdRef == gameId);
                if (pickemGameData == null)
                {
                    throw new ArgumentException($"League: {exactLeagueCode} for season: {seasonCode}, week: {weekNumber} does not have a game with gameid: {gameId}");
                }

                var playerPickData = pickemGameData.PlayerPicks.SingleOrDefault(pp => pp.PlayerTagRef.Equals(uncheckedPlayerTag, StringComparison.OrdinalIgnoreCase));
                if (playerPickData == null)
                {
                    throw new ArgumentException($"League: {exactLeagueCode} for season: {seasonCode}, week: {weekNumber}, game {gameId}, has no player pick for {uncheckedPlayerTag}. Is {uncheckedPlayerTag} in this league?");
                }

                // get associated game to make sure the player can update the pick
                var gameData = await _gameSevice.ReadGame(gameId);

                switch ( gameData.GameState )
                {
                    case GameStates.Cancelled:
                    case GameStates.Final:
                    case GameStates.InGame:
                        throw new Exception($"Player: {playerPickData.PlayerTagRef} in league: {exactLeagueCode} cannot make a pick for game: {gameId} because the game is in the following game state: {gameData.GameState}");
                }

                int gamesPicked;
                int gamesPending = this.CalculateGamesPending(weekData, playerPickData.PlayerTagRef);

                // did the pick change?
                if (playerPickData.Pick != newPlayerPick.Pick)
                {
                    playerPickData.Pick = newPlayerPick.Pick;

                    // update pick count in week subtotals
                    var weekScoreSubtotal = weekData.PlayerWeekScores.Single(pwss => pwss.PlayerTagRef == playerPickData.PlayerTagRef);
                    gamesPicked = weekData.Games.SelectMany(g => g.PlayerPicks.Where(pp => pp.PlayerTagRef == playerPickData.PlayerTagRef && pp.Pick != PickTypes.None)).Count();
                    weekScoreSubtotal.GamesPicked = gamesPicked;
                    gamesPending = this.CalculateGamesPending(weekData, playerPickData.PlayerTagRef);
                    weekScoreSubtotal.GamesPending = gamesPending;

                    dbSession.Store(leagueData);
                    dbSession.SaveChanges();

                    var pickemEvent = new PickemSystemEvent(PickemSystemEventTypes.LeaguePlayerPickChanged, seasonCode, exactLeagueCode, weekNumber, gameId);
                    pickemEvent.DynamicKeys.playerTag = playerPickData.PlayerTagRef;
                    pickemEvent.LeagueCodesAffected.Add(exactLeagueCode);
                    _pickemEventer.Emit(pickemEvent);
                }
                else
                {
                    gamesPicked = weekData.Games.SelectMany(g => g.PlayerPicks.Where(pp => pp.PlayerTagRef == playerPickData.PlayerTagRef && pp.Pick != PickTypes.None)).Count();
                    gamesPending = this.CalculateGamesPending(weekData, playerPickData.PlayerTagRef);
                }

                return new PlayerPick {
                    GamesPending = gamesPending,
                    GamesPicked = gamesPicked,
                    Pick = playerPickData.Pick
                };
            }
        }

        private async Task<LeagueData> GetLeagueData(string uncheckedLeagueCode)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                return await GetLeagueData(dbSession, uncheckedLeagueCode);
            }
        }

        private async Task<LeagueData> GetLeagueData(IQuerySession runningDocumentSession, string uncheckedLeagueCode)
        {
            // trim inputs
            uncheckedLeagueCode = uncheckedLeagueCode.Trim();

            var leagueData = await runningDocumentSession
                .Query<LeagueData>()
                .Where(l => l.LeagueCode.Equals(uncheckedLeagueCode, StringComparison.OrdinalIgnoreCase)) // league insensitive search
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (leagueData == null)
            {
                throw new ArgumentException($"No league exists with league code: {uncheckedLeagueCode}");
            }

            return leagueData;
        }

        private League MapLeagueData(LeagueData leagueData)
        {
            return new League
            {
                LeagueCode = leagueData.LeagueCode,
                LeagueTitle = leagueData.LeagueTitle
            };
        }
       
        private void SynchGamesAndPlayers(LeagueData leagueData)
        {
            // TODO: is this a pig with cartisian-o-rama?

            foreach ( var playerData in leagueData.Players )
            {
                if ( !leagueData.PlayerSeasonScores.Any(pss => pss.PlayerTagRef == playerData.PlayerTag) )
                {
                    leagueData.PlayerSeasonScores.Add(new PlayerScoreSubtotalData
                    {
                        PlayerTagRef = playerData.PlayerTag,
                        Points = 0
                    });
                }

                foreach ( var weekData in leagueData.Weeks )
                {
                    if (!weekData.PlayerWeekScores.Any(pws => pws.PlayerTagRef == playerData.PlayerTag))
                    {
                        weekData.PlayerWeekScores.Add(new PlayerScoreSubtotalData
                        {
                            PlayerTagRef = playerData.PlayerTag,
                            Points = 0
                        });
                    }

                    foreach (var gameData in weekData.Games)
                    {
                        if (!gameData.PlayerPicks.Any(pp => pp.PlayerTagRef == playerData.PlayerTag))
                        {
                            gameData.PlayerPicks.Add(new PlayerPickData
                            {
                                Pick = App.PickTypes.None,
                                PickStatus = App.PickStates.None,
                                PlayerTagRef = playerData.PlayerTag
                            });
                        }
                    }
                }
            }
        }

        private void SynchScoreboards(LeagueData leagueData)
        {
            var playerTags = leagueData.Players.Select(p => p.PlayerTag);

            // by week
            foreach ( var weekData in leagueData.Weeks )
            {
                foreach ( var playerTag in playerTags )
                {
                    var playerWeekScoreData = weekData.PlayerWeekScores.Single(pws => pws.PlayerTagRef == playerTag);
                    playerWeekScoreData.Points = weekData.Games.SelectMany(g => g.PlayerPicks.Where(pp => pp.PlayerTagRef == playerTag && pp.PickStatus == PickStates.Won)).Count();

                    // TODO: this doesn't need to be here if all picks run through SetPlayerPick which should be the case. 
                    // during 1.7.x conversion this was not the case for historical data.
                    playerWeekScoreData.GamesPicked = weekData.Games.SelectMany(g => g.PlayerPicks.Where(pp => pp.PlayerTagRef == playerTag && pp.Pick != PickTypes.None)).Count();
                    playerWeekScoreData.GamesLost = weekData.Games.SelectMany(g => g.PlayerPicks.Where(pp => pp.PlayerTagRef == playerTag && pp.PickStatus == PickStates.Lost)).Count();
                    playerWeekScoreData.GamesPending = this.CalculateGamesPending(weekData, playerTag);
                }
            }

            // whole season
            foreach (var playerTag in playerTags)
            {
                var playerSeasonScoreData = leagueData.PlayerSeasonScores.Single(pss => pss.PlayerTagRef == playerTag);
                playerSeasonScoreData.Points = leagueData.Weeks.SelectMany(w => w.Games.SelectMany(g => g.PlayerPicks.Where(
                    pp => pp.PlayerTagRef == playerTag 
                    && pp.PickStatus == PickStates.Won)
                    )).Count();
            }
        }
    }
}
