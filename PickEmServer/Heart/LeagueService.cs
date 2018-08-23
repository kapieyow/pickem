using Marten;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
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
        private readonly ReferenceService _referenceSevice;
        private readonly UserManager<PickEmUser> _userManager;

        public LeagueService(IDocumentStore documentStore, ILogger<LeagueService> logger, ReferenceService referenceService, UserManager<PickEmUser> userManager)
        {
            _documentStore = documentStore;
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
                    // TODO: verify week exists?
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

        public async Task<League> ReadLeague(string leagueCode)
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

                // TODO: need to sync players?

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

                dbSession.Store(leagueData);
                dbSession.SaveChanges();
            }

            // read back out to return
            return await this.ReadLeague(leagueCode);
        }
        
    }
}
