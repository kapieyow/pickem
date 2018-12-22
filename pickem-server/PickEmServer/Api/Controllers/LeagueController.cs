using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.App.Models;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class LeagueController : Controller
    {
        private readonly LeagueService _leagueService;

        public LeagueController(UserManager<PickEmUser> userManager, LeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        [Authorize]
        [HttpGet]
        [Route("api/{LeagueCode}")]
        public async Task<League> GetLeague(string LeagueCode)
        {
            return await _leagueService.ReadLeague(LeagueCode);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{LeagueCode}/scoreboard")]
        public async Task<LeagueScoreboard> GetLeagueScoreboard(string LeagueCode)
        {
            return await _leagueService.ReadLeagueScoreboard(LeagueCode);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{LeagueCode}/players")]
        public async Task<List<Player>> GetPlayers(string LeagueCode)
        {
            return await _leagueService.ReadLeaguePlayers(LeagueCode);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{LeagueCode}/players/{UserName}")]
        public async Task<Player> GetPlayer(string LeagueCode, string UserName)
        {
            return await _leagueService.ReadLeaguePlayer(LeagueCode, UserName);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPut]
        [Route("api/{LeagueCode}/players/{UserName}")]
        public async Task<Player> PutPlayer(string LeagueCode, string UserName, [FromBody] PlayerUpdate playerUpdate)
        {
            return await _leagueService.SetPlayer(LeagueCode, UserName, playerUpdate);
        }

        [HttpPut]
        [Route("api/{LeagueCode}/players/")]
        public async Task<Player> PutPlayer(string LeagueCode, [FromBody] PlayerUpdate playerUpdate)
        {
            var leaguePlayer = await _leagueService.ReadLeaguePlayer(LeagueCode, this.User.Identity.Name);

            return await _leagueService.SetPlayer(LeagueCode, leaguePlayer.UserName, playerUpdate);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{LeagueCode}/{WeekNumber}/scoreboard")]
        public async Task<WeekScoreboard> GetWeekScoreboard(string LeagueCode, int WeekNumber)
        {
            return await _leagueService.ReadWeekScoreboard(LeagueCode, WeekNumber, User.Identity.Name);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard")]
        public async Task<PlayerScoreboard> GetPlayerWeekScoreboard(string LeagueCode, int WeekNumber, string PlayerTag)
        {
            return await _leagueService.ReadPlayerScoreboard(LeagueCode, WeekNumber, PlayerTag, User.Identity.Name);
        }

        [Authorize]
        [HttpPut]
        [Route("api/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard/{GameId}/pick")]
        public async Task<IActionResult> PutPlayerPick(string LeagueCode, int WeekNumber, string PlayerTag, int GameId, [FromBody] PlayerPickUpdate newPlayerPick)
        {
            var leaguePlayer = await _leagueService.ReadLeaguePlayer(LeagueCode, this.User.Identity.Name);

            if (leaguePlayer.PlayerTag.Equals(PlayerTag, StringComparison.OrdinalIgnoreCase))
            {
                var playerPickResult =  await _leagueService.SetPlayerPick(LeagueCode, WeekNumber, PlayerTag, GameId, newPlayerPick);
                return new OkObjectResult(playerPickResult);
            }
            else
            {
                return new UnauthorizedResult();
            }
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPost]
        [Route("api/{LeagueCode}/players")]
        public async Task<League> AddLeaguePlayer(string LeagueCode, [FromBody] LeaguePlayerAdd newLeaguePlayer)
        {
            return await _leagueService.AddLeaguePlayer(LeagueCode, newLeaguePlayer);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{LeagueCode}/weeks")]
        public async Task<LeagueWeeks> GetLeagueWeeks(string LeagueCode)
        {
            return await _leagueService.ReadLeagueWeeks(LeagueCode);
        }

        [Authorize]
        [HttpPost]
        [Route("api/leagues")]
        public async Task<League> AddLeague([FromBody] LeagueAdd newLeague)
        {
            return await _leagueService.AddLeague(newLeague);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPost]
        [Route("api/{LeagueCode}/{weekNumber}/pickemgames")]
        public async Task<League> AddLeagueGame(string LeagueCode, int weekNumber, [FromBody] LeagueGameAdd newLeagueGame)
        {
            return await _leagueService.AddLeagueGame(LeagueCode, weekNumber, newLeagueGame);
        }

        [Authorize(Policy = "IsAGod")]
        [HttpPut]
        [Route("api/{LeagueCode}")]
        public async Task<League> UpdateLeague(string LeagueCode, [FromBody] LeagueUpdate leagueUpdate)
        {
            return await _leagueService.UpdateLeague(LeagueCode, leagueUpdate);
        }

    }
}