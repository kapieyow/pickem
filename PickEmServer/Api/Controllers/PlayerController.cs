using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PickEmServer.Api.Models;
using PickEmServer.App;

namespace PickEmServer.Api.Controllers
{
    [Produces("application/json")]
    public class PlayerController : Controller
    {

        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard")]
        public async Task<PlayerScoreboard> GetScoreboard(string SeasonCode, int WeekNumber, string LeagueCode, string PlayerTag)
        {
            PlayerScoreboard playerScoreboard = new PlayerScoreboard();

            playerScoreboard.Season = new Season { SeasonCode = SeasonCode, SeasonTitle = "2018" };
            playerScoreboard.League = new League { LeagueCode = LeagueCode, LeagueTitle = "Get NeOnYa" };
            playerScoreboard.WeekNumber = WeekNumber;
            playerScoreboard.Player = new Player { PlayerTag = PlayerTag };

            playerScoreboard.Picks = new List<GamePick>();

            GamePick gamePick;

            gamePick = new GamePick { PickId = 1, Pick = PickTypes.Away, PickStatus = PickStatuses.None };
            gamePick.Game = new Game { GameId = 1, CurrentPeriod = "1", GameStart = DateTime.Now, GameState = GameStates.SpreadNotSet, LastUpdated = DateTime.Now, NeutralField = false, TimeClock = new TimeSpan(0, 15, 0) };
            gamePick.Game.AwayTeam = new TeamScore { Score = 0, ScoreAfterSpread = 3.5M, Winner = false };
            gamePick.Game.AwayTeam.Team = new Team { TeamCode = "Clemson", LongName = "Clemson University", ShortName = "Clemson", NcaaNameSeo = "Clemson", theSpreadName = "Clemson", icon24FileName = "clemson.24.png" };
            gamePick.Game.HomeTeam = new TeamScore { Score = 0, ScoreAfterSpread = 0M, Winner = false };
            gamePick.Game.HomeTeam.Team = new Team { TeamCode = "UNC", LongName = "University of North Carolina", ShortName = "UNC", NcaaNameSeo = "north-carolina", theSpreadName = "UNC", icon24FileName = "north-carolina.24.png" };
            gamePick.Game.Spread = new Spread { PointSpread = 3.5M, SpreadDirection = SpreadDirections.ToAway };

            playerScoreboard.Picks.Add(gamePick);


            gamePick = new GamePick { PickId = 2, Pick = PickTypes.Away, PickStatus = PickStatuses.None };
            gamePick.Game = new Game { GameId = 1, CurrentPeriod = "1", GameStart = DateTime.Now, GameState = GameStates.SpreadNotSet, LastUpdated = DateTime.Now, NeutralField = false, TimeClock = new TimeSpan(0, 15, 0) };
            gamePick.Game.AwayTeam = new TeamScore { Score = 0, ScoreAfterSpread = 3.5M, Winner = false };
            gamePick.Game.AwayTeam.Team = new Team { TeamCode = "OregonSt", LongName = "Oregon State University", ShortName = "Oregon St", NcaaNameSeo = "oregon-st", theSpreadName = "Oregon St", icon24FileName = "oregon-st.24.png" };
            gamePick.Game.HomeTeam = new TeamScore { Score = 0, ScoreAfterSpread = 0M, Winner = false };
            gamePick.Game.HomeTeam.Team = new Team { TeamCode = "USoCarolina", LongName = "University of South Carolina", ShortName = "USC", NcaaNameSeo = "south-carolina", theSpreadName = "USoCarolina", icon24FileName = "south-carolina.24.png" };
            gamePick.Game.Spread = new Spread { PointSpread = 3.5M, SpreadDirection = SpreadDirections.ToAway };

            playerScoreboard.Picks.Add(gamePick);


            return playerScoreboard;
        }
    }
}