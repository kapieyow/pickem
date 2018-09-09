using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.App;
using PickEmServer.Heart;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class LeagueController : Controller
    {
        private readonly LeagueService _leagueService;

        public LeagueController(LeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/scoreboard")]
        public async Task<LeagueScoreboard> GetScoreboard(string SeasonCode, string LeagueCode)
        {
            LeagueScoreboard leagueScoreboard = new LeagueScoreboard();

            leagueScoreboard.League = new League { LeagueCode = LeagueCode, LeagueTitle = "Burlington Mafia" };
            leagueScoreboard.Season = new Season { SeasonCode = SeasonCode, SeasonTitle = "This Season" };

            leagueScoreboard.PlayerScores = new List<PlayerSeasonScore>();

            PlayerSeasonScore playerSeasonScore;

            playerSeasonScore = new PlayerSeasonScore();
            playerSeasonScore.Player = new Player { PlayerTag = "bewwew" };
            playerSeasonScore.WeeklyScores = new List<WeekScore>();
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 1, Points = 5 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 2, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 3, Points = 8 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 4, Points = 14 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 5, Points = 5 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 6, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 7, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 8, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 9, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 10, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 11, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 12, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 13, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 14, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 15, Points = 0 });
            playerSeasonScore.Points = playerSeasonScore.WeeklyScores.Select(ws => ws.Points).Sum();
            leagueScoreboard.PlayerScores.Add(playerSeasonScore);

            playerSeasonScore = new PlayerSeasonScore();
            playerSeasonScore.Player = new Player { PlayerTag = "cush" };
            playerSeasonScore.WeeklyScores = new List<WeekScore>();
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 1, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 2, Points = 12 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 3, Points = 10 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 4, Points = 5 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 5, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 6, Points = 11 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 7, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 8, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 9, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 10, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 11, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 12, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 13, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 14, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 15, Points = 0 });
            playerSeasonScore.Points = playerSeasonScore.WeeklyScores.Select(ws => ws.Points).Sum();
            leagueScoreboard.PlayerScores.Add(playerSeasonScore);

            playerSeasonScore = new PlayerSeasonScore();
            playerSeasonScore.Player = new Player { PlayerTag = "kapieyow" };
            playerSeasonScore.WeeklyScores = new List<WeekScore>();
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 1, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 2, Points = 6 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 3, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 4, Points = 11 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 5, Points = 10 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 6, Points = 6 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 7, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 8, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 9, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 10, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 11, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 12, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 13, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 14, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 15, Points = 0 });
            playerSeasonScore.Points = playerSeasonScore.WeeklyScores.Select(ws => ws.Points).Sum();
            leagueScoreboard.PlayerScores.Add(playerSeasonScore);

            playerSeasonScore = new PlayerSeasonScore();
            playerSeasonScore.Player = new Player { PlayerTag = "kapieyow" };
            playerSeasonScore.WeeklyScores = new List<WeekScore>();
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 1, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 2, Points = 6 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 3, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 4, Points = 11 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 5, Points = 10 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 6, Points = 6 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 7, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 8, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 9, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 10, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 11, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 12, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 13, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 14, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 15, Points = 0 });
            playerSeasonScore.Points = playerSeasonScore.WeeklyScores.Select(ws => ws.Points).Sum();
            leagueScoreboard.PlayerScores.Add(playerSeasonScore);

            playerSeasonScore = new PlayerSeasonScore();
            playerSeasonScore.Player = new Player { PlayerTag = "samSpade" };
            playerSeasonScore.WeeklyScores = new List<WeekScore>();
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 1, Points = 6 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 2, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 3, Points = 8 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 4, Points = 11 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 5, Points = 9 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 6, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 7, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 8, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 9, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 10, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 11, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 12, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 13, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 14, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 15, Points = 0 });
            playerSeasonScore.Points = playerSeasonScore.WeeklyScores.Select(ws => ws.Points).Sum();
            leagueScoreboard.PlayerScores.Add(playerSeasonScore);

            playerSeasonScore = new PlayerSeasonScore();
            playerSeasonScore.Player = new Player { PlayerTag = "sigterm" };
            playerSeasonScore.WeeklyScores = new List<WeekScore>();
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 1, Points = 3 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 2, Points = 4 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 3, Points = 5 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 4, Points = 6 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 5, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 6, Points = 8 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 7, Points = 7 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 8, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 9, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 10, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 11, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 12, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 13, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 14, Points = 0 });
            playerSeasonScore.WeeklyScores.Add(new WeekScore { WeekNumber = 15, Points = 0 });
            playerSeasonScore.Points = playerSeasonScore.WeeklyScores.Select(ws => ws.Points).Sum();
            leagueScoreboard.PlayerScores.Add(playerSeasonScore);

            return leagueScoreboard;
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/players")]
        public async Task<List<Player>> GetPlayers(string SeasonCode, string LeagueCode)
        {
            return await _leagueService.ReadLeaguePlayers(SeasonCode, LeagueCode);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/players/{UserName}")]
        public async Task<Player> GetPlayer(string SeasonCode, string LeagueCode, string UserName)
        {
            return await _leagueService.ReadLeaguePlayer(SeasonCode, LeagueCode, UserName);
        }

        [Authorize]
        [HttpPut]
        [Route("api/{SeasonCode}/{LeagueCode}/weeks/current")]
        public async Task<int> PutCurrentWeek(string SeasonCode, string LeagueCode, [FromBody] int currentWeekNumber)
        {
            return await _leagueService.SetCurrentWeek(SeasonCode, LeagueCode, currentWeekNumber);
        }

        [Authorize]
        [HttpPut]
        [Route("api/{SeasonCode}/{LeagueCode}/players/{UserName}")]
        public async Task<Player> PutPlayer(string SeasonCode, string LeagueCode, string UserName, [FromBody] PlayerUpdate playerUpdate)
        {
            return await _leagueService.SetPlayer(SeasonCode, LeagueCode, UserName, playerUpdate);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/{WeekNumber}/scoreboard")]
        public async Task<WeekScoreboard> GetWeekScoreboard(string SeasonCode, string LeagueCode, int WeekNumber)
        {
            return await _leagueService.ReadWeekScoreboard(SeasonCode, LeagueCode, WeekNumber, User.Identity.Name);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard")]
        public async Task<PlayerScoreboard> GetPlayerWeekScoreboard(string SeasonCode, string LeagueCode, int WeekNumber, string PlayerTag)
        {
            return await _leagueService.ReadPlayerScoreboard(SeasonCode, LeagueCode, WeekNumber, PlayerTag, User.Identity.Name);
        }

        [Authorize]
        [HttpPut]
        [Route("api/{SeasonCode}/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard/{GameId}/pick")]
        public async Task<PlayerPick> PutPlayerPick(string SeasonCode, string LeagueCode, int WeekNumber, string PlayerTag, int GameId, [FromBody] PlayerPickUpdate newPlayerPick)
        {
            return await _leagueService.SetPlayerPick(SeasonCode, LeagueCode, WeekNumber, PlayerTag, GameId, newPlayerPick);
        }

        [Authorize]
        [HttpPost]
        [Route("api/{SeasonCode}/{LeagueCode}/players")]
        public async Task<League> AddLeaguePlayer(string LeagueCode, [FromBody] LeaguePlayerAdd newLeaguePlayer)
        {
            return await _leagueService.AddLeaguePlayer(LeagueCode, newLeaguePlayer);
        }

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/weeks")]
        public async Task<LeagueWeeks> GetLeagueWeeks(string SeasonCode, string LeagueCode)
        {
            return await _leagueService.ReadLeagueWeeks(SeasonCode, LeagueCode);
        }

        [Authorize]
        [HttpPost]
        [Route("api/{SeasonCode}/leagues")]
        public async Task<League> AddLeague(string SeasonCode, [FromBody] LeagueAdd newLeague)
        {
            return await _leagueService.AddLeague(SeasonCode,  newLeague);
        }

        [Authorize]
        [HttpPost]
        [Route("api/{SeasonCode}/{LeagueCode}/{weekNumber}/games")]
        public async Task<League> AddLeagueGame(string SeasonCode, string LeagueCode, int weekNumber, [FromBody] LeagueGameAdd newLeagueGame)
        {
            return await _leagueService.AddLeagueGame(SeasonCode, LeagueCode, weekNumber, newLeagueGame);
        }

       

    }
}