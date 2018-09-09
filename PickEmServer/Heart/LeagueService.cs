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
        public LeagueService(IDocumentStore documentStore, ILogger<LeagueService> logger, ReferenceService referenceService, GameService gameSevice, UserManager<PickEmUser> userManager)
        {
            _documentStore = documentStore;
            _gameSevice = gameSevice;
            _logger = logger;
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
            }

            // read back out to return
            return await this.ReadLeague(newLeague.LeagueCode);

        }

        internal async Task<League> AddLeagueGame(string seasonCode, string leagueCode, int weekNumber, LeagueGameAdd newLeagueGame)
        {
            if (newLeagueGame == null)
            {
                throw new ArgumentException("No newLeagueGame parameter input for AddLeagueGame (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = dbSession
                    .Query<LeagueData>()
                    .Where(l => l.LeagueCode == leagueCode)
                    .SingleOrDefault();

                if (leagueData == null)
                {
                    throw new ArgumentException($"No league exists with league code: {leagueCode}");
                }

                if (leagueData.SeasonCodeRef != seasonCode)
                {
                    throw new ArgumentException($"No league exists with league code: {leagueCode} for season: {seasonCode}");
                }

                var leagueWeek = leagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);
                if (leagueWeek == null)
                {
                    throw new ArgumentException($"League with league code: {leagueCode} does not contain week: {weekNumber}");
                }

                if (leagueWeek.Games.Exists(g => g.GameIdRef == newLeagueGame.GameId))
                {
                    throw new ArgumentException($"League with league code: {leagueCode} already has game with gameid: {newLeagueGame.GameId} for week: {weekNumber}");
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
                    throw new ArgumentException($"Game week must match League week and they do not. League with league code: {leagueCode} has week: {weekNumber}. Game with game id: {newLeagueGame.GameId} has week {game.WeekNumberRef}");
                }

                // whew we can add this now.
                leagueWeek.Games.Add(new LeagueGameData { GameIdRef = newLeagueGame.GameId, PlayerPicks = new List<PlayerPickData>() });

                SynchGamesAndPlayers(leagueData);

                dbSession.Store(leagueData);
                dbSession.SaveChanges();
            }

            // read back out to return
            return await this.ReadLeague(leagueCode);
        }

        internal async Task<League> AddLeaguePlayer(string leagueCode, LeaguePlayerAdd newLeaguePlayer)
        {
            if (newLeaguePlayer == null)
            {
                throw new ArgumentException("No newLeagueGame parameter input for AddLeaguePlayer (is null)");
            }

            if ( await _userManager.FindByNameAsync(newLeaguePlayer.UserName) == null )
            {
                throw new ArgumentException($"No user with username (id) : {newLeaguePlayer.UserName}. Cannot add league player"); 
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = dbSession
                    .Query<LeagueData>()
                    .Where(l => l.LeagueCode == leagueCode)
                    .SingleOrDefault();

                if (leagueData == null)
                {
                    throw new ArgumentException($"No league exists with league code: {leagueCode}");
                }

                
                if (leagueData.Players.Exists(p => p.PlayerTag == newLeaguePlayer.PlayerTag))
                {
                    throw new ArgumentException($"League with league code: {leagueCode} already has player with player tag: {newLeaguePlayer.PlayerTag}");
                }
                
                // whew we can add this now.
                leagueData.Players.Add(new LeaguePlayerData { PlayerTag = newLeaguePlayer.PlayerTag, UserNameRef = newLeaguePlayer.UserName });

                SynchGamesAndPlayers(leagueData);

                dbSession.Store(leagueData);
                dbSession.SaveChanges();
            }

            // read back out to return
            return await this.ReadLeague(leagueCode);
        }

        internal async Task<List<LeagueData>> ApplyGameChanges(GameData updatedGame, GameChanges gameChanges, IDocumentSession runningDbSession)
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

            foreach ( var leagueData in associatedLeagues )
            {
                // Checking both change flags and updating player picks. 
                if ( gameChanges.ScoreChanged || gameChanges.GameStateChanged )
                {
                    // game score changed : update pick status(es) for related games
                    // TODO: ToArray is dumb. There will only be one. How to in Linq?
                    var assocatedLeagueGameData = leagueData.Weeks.SelectMany(w => w.Games.Where(g => g.GameIdRef == updatedGame.GameId)).ToArray();

                    foreach (var playerPickData in assocatedLeagueGameData[0].PlayerPicks)
                    {
                        switch (updatedGame.GameState)
                        {
                            case GameStates.Cancelled:
                                playerPickData.PickStatus = PickStates.None;
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

                if ( gameChanges.GameStateChanged )
                {
                    if ( updatedGame.GameState == GameStates.Final || updatedGame.GameState == GameStates.Cancelled )
                    {
                        this.SynchScoreboards(leagueData);
                    }
                }
            }

            return associatedLeagues.ToList();
        }

        internal async Task<int> SetCurrentWeek(string seasonCode, string leagueCode, int currentWeekNumber)
        {
            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = await this.GetLeagueData(dbSession, seasonCode, leagueCode);

                var weekData = leagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == currentWeekNumber);
                if (weekData == null)
                {
                    throw new ArgumentException($"League: {leagueCode} for season: {seasonCode} does not contain a week: {currentWeekNumber}");
                }

                leagueData.CurrentWeekRef = currentWeekNumber;

                dbSession.Store(leagueData);
                dbSession.SaveChanges();

                return currentWeekNumber;
            }
        }

        public async Task<Player> ReadLeaguePlayer(string seasonCode, string leagueCode, string userName)
        {
            var leagueData = await this.GetLeagueData(seasonCode, leagueCode);

            var playerData = leagueData.Players.SingleOrDefault(p => p.UserNameRef == userName);
            if ( playerData == null )
            {
                throw new ArgumentException($"User name: {userName} is not references in league: {leagueCode}, season: {seasonCode}");
            }

            return new Player
            {
                PlayerTag = playerData.PlayerTag,
                UserName = playerData.UserNameRef
            };
        }

        public async Task<List<Player>> ReadLeaguePlayers(string seasonCode, string leagueCode)
        {
            var leagueData = await this.GetLeagueData(seasonCode, leagueCode);

            var resultPlayers = new List<Player>();

            foreach ( var playerData in leagueData.Players )
            {
                resultPlayers.Add(new Player
                {
                    PlayerTag = playerData.PlayerTag,
                    UserName = playerData.UserNameRef
                });
            }

            return resultPlayers;
        }

        public async Task<LeagueWeeks> ReadLeagueWeeks(string seasonCode, string leagueCode)
        {
            var leagueData = await this.GetLeagueData(seasonCode, leagueCode);

            var leagueWeeks = new LeagueWeeks();
            leagueWeeks.WeekNumbers = new List<int>();

            leagueWeeks.CurrentWeekNumber = leagueData.CurrentWeekRef;

            foreach (var weekData in leagueData.Weeks)
            {
                leagueWeeks.WeekNumbers.Add(weekData.WeekNumberRef);
            }

            return leagueWeeks;
        }

        public async Task<WeekScoreboard> ReadWeekScoreboard(string seasonCode, string leagueCode, int weekNumber, string authenticatedUserName)
        {
            // determine if the authenticated user has this player tag (if not hide picks for games not started)
            var authenticatedPlayer = await this.ReadLeaguePlayer(seasonCode, leagueCode, authenticatedUserName);
           
            var leagueWithExtendedData = await this.ReadLeagueWithWeekGamesExpanded(seasonCode, leagueCode, weekNumber);

            // get all players and loop over to map for full week
            var playerTags = leagueWithExtendedData.LeagueData.Players.Select(p => p.PlayerTag);

            var weekScoreboard = new WeekScoreboard();
            weekScoreboard.PlayerScoreboards = new List<PlayerScoreboard>();

            foreach ( var playerTag in playerTags )
            {
                var nextPlayerScoreboard = new PlayerScoreboard();
                nextPlayerScoreboard.PlayerTag = playerTag;
                nextPlayerScoreboard.PlayerScoreboardPicks = this.MapDataToPlayerPicks(
                    seasonCode, 
                    leagueCode, 
                    weekNumber, 
                    playerTag, 
                    (authenticatedPlayer.PlayerTag == playerTag), 
                    leagueWithExtendedData
                    );

                nextPlayerScoreboard.Wins = leagueWithExtendedData.LeagueData
                    .Weeks.Single(w => w.WeekNumberRef == weekNumber)
                    .PlayerWeekScores.Single(pws => pws.PlayerTagRef == playerTag)
                    .Points;

                weekScoreboard.PlayerScoreboards.Add(nextPlayerScoreboard);
            }

            return weekScoreboard;
        }

        // TODO: this probably should be spread between league and game services?
        public async Task<List<PlayerScoreboardPick>> ReadPlayerScoreboard(string seasonCode, string leagueCode, int weekNumber, string playerTag, string authenticatedUserName)
        {
            // determine if the authenticated user has this player tag (if not hide picks for games not started)
            var authenticatedPlayer = await this.ReadLeaguePlayer(seasonCode, leagueCode, authenticatedUserName);
            bool readingPlayersOwnScoreboard = (authenticatedPlayer.PlayerTag == playerTag);

            var leagueWithExtendedData = await this.ReadLeagueWithWeekGamesExpanded(seasonCode, leagueCode, weekNumber);

            return this.MapDataToPlayerPicks(seasonCode, leagueCode, weekNumber, playerTag, readingPlayersOwnScoreboard, leagueWithExtendedData);
        }

        private async Task<LeagueWithGamesAndTeamDataForWeek> ReadLeagueWithWeekGamesExpanded(string seasonCode, string leagueCode, int weekNumber)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var leagueWithExtendedData = new LeagueWithGamesAndTeamDataForWeek();

                // get league
                leagueWithExtendedData.LeagueData = await this.GetLeagueData(dbSession, seasonCode, leagueCode);

                // get games in league for week
                var leagueWeek = leagueWithExtendedData.LeagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);

                if (leagueWeek == null)
                {
                    throw new ArgumentException($"League with league code: {leagueCode} does not contain week: {weekNumber}");
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

        private List<PlayerScoreboardPick> MapDataToPlayerPicks(string seasonCode, string leagueCode, int weekNumber, string playerTag, bool readingPlayersOwnScoreboard, LeagueWithGamesAndTeamDataForWeek leagueWithExtendedData)
        {
            var playerScoreboard = new List<PlayerScoreboardPick>();

            foreach (var gameData in leagueWithExtendedData.GameDataForWeek.OrderBy(game => game.GameStart))
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

                var playerPickData = pickemGameData.PlayerPicks.SingleOrDefault(pp => pp.PlayerTagRef == playerTag);
                if (playerPickData == null)
                {
                    throw new ArgumentException($"League: {leagueCode} for season: {seasonCode}, week: {weekNumber}, game {gameData.GameId}, has no player pick for {playerTag}. Is {playerTag} in this league?");
                }


                var playerScoreboardPick = new PlayerScoreboardPick
                {
                    AwayTeamIconFileName = leagueWithExtendedData.referencedAwayTeamData[gameData.AwayTeam.TeamCodeRef].icon24FileName,
                    AwayTeamLongName = string.IsNullOrEmpty(leagueWithExtendedData.referencedAwayTeamData[gameData.AwayTeam.TeamCodeRef].LongName) ? gameData.AwayTeam.TeamCodeRef : leagueWithExtendedData.referencedAwayTeamData[gameData.AwayTeam.TeamCodeRef].LongName,
                    AwayTeamLosses = 0, // TODO set
                    AwayTeamRank = 0, // TODO set
                    AwayTeamScore = gameData.AwayTeam.Score,
                    AwayTeamWins = 0, // TODO set
                    GameId = gameData.GameId,
                    GameState = gameData.GameState,
                    GameStatusDescription = _gameSevice.BuildGameDescription(gameData),
                    HomeTeamIconFileName = leagueWithExtendedData.referencedHomeTeamData[gameData.HomeTeam.TeamCodeRef].icon24FileName,
                    HomeTeamLongName = string.IsNullOrEmpty(leagueWithExtendedData.referencedHomeTeamData[gameData.HomeTeam.TeamCodeRef].LongName) ? gameData.HomeTeam.TeamCodeRef : leagueWithExtendedData.referencedHomeTeamData[gameData.HomeTeam.TeamCodeRef].LongName,
                    HomeTeamLosses = 0, // TODO set
                    HomeTeamRank = 0, // TODO set
                    HomeTeamScore = gameData.HomeTeam.Score,
                    HomeTeamWins = 0, // TODO set
                    Pick = CalculatePickState(playerPickData, gameData, readingPlayersOwnScoreboard),
                    PickState = playerPickData.PickStatus,
                    Spread = gameData.Spread.PointSpread,
                    SpreadDirection = gameData.Spread.SpreadDirection
                };

                playerScoreboard.Add(playerScoreboardPick);

            }

            return playerScoreboard;
        }

        private PickTypes CalculatePickState(PlayerPickData playerPickData, GameData gameData, bool readingPlayersOwnScoreboard)
        {
            if ( !readingPlayersOwnScoreboard && (gameData.GameState == GameStates.SpreadLocked || gameData.GameState == GameStates.SpreadNotSet) )
            {
                // pick for another player (not the one logged in) and the game has not started)
                return PickTypes.Hidden;
            }
            else
            {
                return playerPickData.Pick;
            }
        }

        internal async Task<Player> SetPlayer(string seasonCode, string leagueCode, string userName, PlayerUpdate playerUpdate)
        {
            if (playerUpdate == null)
            {
                throw new ArgumentException("No playerUpdate parameter input for SetPlayer (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = await this.GetLeagueData(dbSession, seasonCode, leagueCode);

                var playerData = leagueData.Players.SingleOrDefault(p => p.UserNameRef == userName);
                if (playerData == null)
                {
                    throw new ArgumentException($"User name: {userName} is not references in league: {leagueCode}, season: {seasonCode}");
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

                return await this.ReadLeaguePlayer(seasonCode, leagueCode, userName);
            }
        }

        internal async Task<PlayerPick> SetPlayerPick(string seasonCode, string leagueCode, int weekNumber, string playerTag, int gameId, PlayerPickUpdate newPlayerPick)
        {
            if (newPlayerPick == null)
            {
                throw new ArgumentException("No newPlayerPick parameter input for SetPlayerPick (is null)");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var leagueData = await this.GetLeagueData(dbSession, seasonCode, leagueCode);

                var weekData = leagueData.Weeks.SingleOrDefault(w => w.WeekNumberRef == weekNumber);
                if (weekData == null)
                {
                    throw new ArgumentException($"League: {leagueCode} for season: {seasonCode} does not contain a week: {weekNumber}");
                }

                var pickemGameData = weekData.Games.SingleOrDefault(g => g.GameIdRef == gameId);
                if (pickemGameData == null)
                {
                    throw new ArgumentException($"League: {leagueCode} for season: {seasonCode}, week: {weekNumber} does not have a game with gameid: {gameId}");
                }

                var playerPickData = pickemGameData.PlayerPicks.SingleOrDefault(pp => pp.PlayerTagRef == playerTag);
                if (playerPickData == null)
                {
                    throw new ArgumentException($"League: {leagueCode} for season: {seasonCode}, week: {weekNumber}, game {gameId}, has no player pick for {playerTag}. Is {playerTag} in this league?");
                }

                // get associated game to make sure the player can update the pick
                var gameData = await _gameSevice.ReadGame(gameId);

                switch ( gameData.GameState )
                {
                    case GameStates.Cancelled:
                    case GameStates.Final:
                    case GameStates.InGame:
                        throw new Exception($"Player: {playerTag} in league: {leagueCode} cannot make a pick for game: {gameId} because the game is in the following game state: {gameData.GameState}");
                }

                playerPickData.Pick = newPlayerPick.Pick;

                dbSession.Store(leagueData);
                dbSession.SaveChanges();

                return new PlayerPick { Pick = playerPickData.Pick };
            }
        }

        private async Task<LeagueData> GetLeagueData(string seasonCode, string leagueCode)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                return await GetLeagueData(dbSession, seasonCode, leagueCode);
            }
        }

        private async Task<LeagueData> GetLeagueData(IQuerySession runningDocumentSession, string seasonCode, string leagueCode)
        {
            var leagueData = await runningDocumentSession
                .Query<LeagueData>()
                .Where(l => l.LeagueCode == leagueCode && l.SeasonCodeRef == seasonCode)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (leagueData == null)
            {
                throw new ArgumentException($"No league exists with league code: {leagueCode} for season: {seasonCode}");
            }

            return leagueData;
        }

        private async Task<League> ReadLeague(string leagueCode)
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var leagueData = await dbSession
                    .Query<LeagueData>()
                    .Where(l => l.LeagueCode == leagueCode)
                    .SingleAsync()
                    .ConfigureAwait(false);

                // TODO: fill in other league data to API
                League apiLeague = new League
                {
                    LeagueCode = leagueData.LeagueCode,
                    LeagueTitle = leagueData.LeagueTitle
                };

                return apiLeague;
            }
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
                }
            }

            // whole season
            foreach (var playerTag in playerTags)
            {
                var playerSeasonScoreData = leagueData.PlayerSeasonScores.Single(pss => pss.PlayerTagRef == playerTag);
                playerSeasonScoreData.Points = leagueData.Weeks.SelectMany(w => w.Games.SelectMany(g => g.PlayerPicks.Where(pp => pp.PlayerTagRef == playerTag && pp.PickStatus == PickStates.Won))).Count();
            }
        }
    }
}
