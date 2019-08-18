using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.Extensions.Logging;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.App.Models;
using PickEmServer.Data.Models;

namespace PickEmServer.Heart
{
    public class TeamService
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<TeamService> _logger;
        private readonly PickemEventer _pickemEventer;

        public TeamService(IDocumentStore documentStore, ILogger<TeamService> logger, PickemEventer pickemEventer)
        {
            _documentStore = documentStore;
            _logger = logger;
            _pickemEventer = pickemEventer;
        }

        internal async Task<List<Team>> ReadTeams()
        {
            using (var dbSession = _documentStore.QuerySession())
            {
                var allTeams = await dbSession
                    .Query<TeamData>()
                    .ToListAsync()
                    .ConfigureAwait(false);

                var apiTeams = new List<Team>();

                foreach (var teamData in allTeams)
                {
                    apiTeams.Add(MapTeamData(teamData));
                }

                return apiTeams.OrderBy(t => t.TeamCode).ToList();
            }
        }

        internal async Task<Team> SetTeamStats(string uncheckedTeamCode, string seasonCode, int weekNumber, TeamStatsUpdate teamStatsUpdate)
        {
            if (teamStatsUpdate == null)
            {
                throw new ArgumentNullException("teamStatsUpdate");
            }

            using (var dbSession = _documentStore.LightweightSession())
            {
                var teamData = await this.GetTeam(dbSession, uncheckedTeamCode);

                // does season exist, if not add
                if (teamData.Seasons == null)
                {
                    teamData.Seasons = new List<TeamSeasonStats>();
                }

                TeamSeasonStats teamSeasonStats = teamData.Seasons.SingleOrDefault(s => s.SeasonCodeRef == seasonCode);
                if (teamSeasonStats == null)
                {
                    teamSeasonStats = new TeamSeasonStats();
                    teamSeasonStats.SeasonCodeRef = seasonCode;
                    teamData.Seasons.Add(teamSeasonStats);
                }

                if (teamSeasonStats.WeekStats == null)
                {
                    teamSeasonStats.WeekStats = new List<TeamWeekStats>();
                }

                TeamWeekStats teamWeekStats = teamSeasonStats.WeekStats.SingleOrDefault(w => w.WeekNumberRef == weekNumber);
                if (teamWeekStats == null)
                {
                    teamWeekStats = new TeamWeekStats();
                    teamWeekStats.WeekNumberRef = weekNumber;
                    teamSeasonStats.WeekStats.Add(teamWeekStats);
                }

                teamWeekStats.FbsRank = teamStatsUpdate.FbsRank;
                teamWeekStats.Losses = teamStatsUpdate.Losses;
                teamWeekStats.Wins = teamStatsUpdate.Wins;

                dbSession.Store(teamData);
                dbSession.SaveChanges();

                var pickemEvent = new PickemSystemEvent(PickemSystemEventTypes.TeamStatsChanged, seasonCode, weekNumber);
                pickemEvent.DynamicKeys.teamCode = teamData.TeamCode;
                _pickemEventer.Emit(pickemEvent);

                return MapTeamData(teamData);
            }
        }

        private async Task<TeamData> GetTeam(IQuerySession runningDocumentSession, string uncheckedTeamCode)
        {
            var teamData = await runningDocumentSession
                .Query<TeamData>()
                .Where(t => t.TeamCode.Equals(uncheckedTeamCode, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (teamData == null)
            {
                throw new ArgumentException($"No team exists with team code: {uncheckedTeamCode}");
            }

            return teamData;
        }

        private Team MapTeamData(TeamData teamData)
        {
            return new Team
            {
                icon24FileName = teamData.icon24FileName,
                LongName = teamData.LongName,
                EspnAbbreviation = teamData.EspnAbbreviation,
                EspnDisplayName = teamData.EspnDisplayName,
                NcaaNameSeo = teamData.NcaaNameSeo,
                ShortName = teamData.ShortName,
                TeamCode = teamData.TeamCode,
                theSpreadName = teamData.theSpreadName
            };
        }
    }
}
