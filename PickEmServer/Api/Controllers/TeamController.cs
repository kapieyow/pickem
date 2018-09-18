using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class TeamController : Controller
    {
        private readonly TeamService _teamService;

        public TeamController(TeamService teamService)
        {
            _teamService = teamService;
        }

        [Authorize]
        [HttpGet]
        [Route("api/teams")]
        public async Task<List<Team>> ReadTeams()
        {
            return await _teamService.ReadTeams();
        }

        [Authorize]
        [HttpPut]
        [Route("api/teams/{TeamCode}/{SeasonCode}/{WeekNumber}/stats")]
        public async Task<Team> PutTeamStats(string TeamCode, string SeasonCode, int WeekNumber, [FromBody] TeamStatsUpdate teamStatsUpdate)
        {
            return await _teamService.SetTeamStats(TeamCode, SeasonCode, WeekNumber, teamStatsUpdate);
        }

    }
}