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
    public class WeekController : Controller
    {
        [Authorize]
        [HttpGet]
        [Route("api/{SeasonCode}/{LeagueCode}/{WeekNumber}/scoreboard")]
        public async Task<WeekScoreboard> GetScoreboard(string SeasonCode, int WeekNumber, string LeagueCode)
        {
            WeekScoreboard weekScoreboard = new WeekScoreboard();

            weekScoreboard.Season = new Season { SeasonCode = SeasonCode, SeasonTitle = "2018" };
            weekScoreboard.League = new League { LeagueCode = LeagueCode, LeagueTitle = "Get NeOnYa" };
            weekScoreboard.WeekNumber = WeekNumber;

            weekScoreboard.PlayersPicks = new List<PlayerPicks>();

            PlayerPicks playerPicks;
            GamePick gamePick;

            playerPicks = new PlayerPicks();
            weekScoreboard.PlayersPicks.Add(playerPicks);
            playerPicks.Player = new Player { PlayerTag = "sigterm" };
            playerPicks.Picks = new List<GamePick>();

            gamePick = new GamePick { PickId = 1, Pick = PickTypes.Away, PickStatus = PickStates.None };
            gamePick.Game = new Game { GameId = 1, CurrentPeriod = "1", GameStart = DateTime.Now, GameState = GameStates.SpreadNotSet, LastUpdated = DateTime.Now, NeutralField = false, TimeClock = new TimeSpan(0, 15, 0) };
            gamePick.Game.AwayTeam = new TeamScore { Score = 0, ScoreAfterSpread = 3.5M, Winner = false };
            gamePick.Game.AwayTeam.Team = new Team { TeamCode = "Clemson", LongName = "Clemson University", ShortName = "Clemson", NcaaNameSeo = "Clemson", theSpreadName = "Clemson", icon24FileName = "clemson.24.png" };
            gamePick.Game.HomeTeam = new TeamScore { Score = 0, ScoreAfterSpread = 0M, Winner = false };
            gamePick.Game.HomeTeam.Team = new Team { TeamCode = "UNC", LongName = "University of North Carolina", ShortName = "UNC", NcaaNameSeo = "north-carolina", theSpreadName = "UNC", icon24FileName = "north-carolina.24.png" };
            gamePick.Game.Spread = new Spread { PointSpread = 3.5M, SpreadDirection = SpreadDirections.ToAway };
            playerPicks.Picks.Add(gamePick);

            gamePick = new GamePick { PickId = 2, Pick = PickTypes.Away, PickStatus = PickStates.Won };
            gamePick.Game = new Game { GameId = 1, CurrentPeriod = "1", GameStart = DateTime.Now, GameState = GameStates.Final, LastUpdated = DateTime.Now, NeutralField = false, TimeClock = new TimeSpan(0, 15, 0) };
            gamePick.Game.AwayTeam = new TeamScore { Score = 0, ScoreAfterSpread = 3.5M, Winner = false };
            gamePick.Game.AwayTeam.Team = new Team { TeamCode = "OregonSt", LongName = "Oregon State University", ShortName = "Oregon St", NcaaNameSeo = "oregon-st", theSpreadName = "Oregon St", icon24FileName = "oregon-st.24.png" };
            gamePick.Game.HomeTeam = new TeamScore { Score = 0, ScoreAfterSpread = 0M, Winner = false };
            gamePick.Game.HomeTeam.Team = new Team { TeamCode = "USoCarolina", LongName = "University of South Carolina", ShortName = "USC", NcaaNameSeo = "south-carolina", theSpreadName = "USoCarolina", icon24FileName = "south-carolina.24.png" };
            gamePick.Game.Spread = new Spread { PointSpread = 3.5M, SpreadDirection = SpreadDirections.ToAway };

            playerPicks.Picks.Add(gamePick);
            playerPicks.WeekPoints = playerPicks.Picks.Count(p => p.PickStatus == PickStates.Won);


            playerPicks = new PlayerPicks();
            weekScoreboard.PlayersPicks.Add(playerPicks);
            playerPicks.Player = new Player { PlayerTag = "cushyGoblin" };
            playerPicks.Picks = new List<GamePick>();

            gamePick = new GamePick { PickId = 1, Pick = PickTypes.Away, PickStatus = PickStates.None };
            gamePick.Game = new Game { GameId = 1, CurrentPeriod = "1", GameStart = DateTime.Now, GameState = GameStates.SpreadNotSet, LastUpdated = DateTime.Now, NeutralField = false, TimeClock = new TimeSpan(0, 15, 0) };
            gamePick.Game.AwayTeam = new TeamScore { Score = 0, ScoreAfterSpread = 3.5M, Winner = false };
            gamePick.Game.AwayTeam.Team = new Team { TeamCode = "Clemson", LongName = "Clemson University", ShortName = "Clemson", NcaaNameSeo = "Clemson", theSpreadName = "Clemson", icon24FileName = "clemson.24.png" };
            gamePick.Game.HomeTeam = new TeamScore { Score = 0, ScoreAfterSpread = 0M, Winner = false };
            gamePick.Game.HomeTeam.Team = new Team { TeamCode = "UNC", LongName = "University of North Carolina", ShortName = "UNC", NcaaNameSeo = "north-carolina", theSpreadName = "UNC", icon24FileName = "north-carolina.24.png" };
            gamePick.Game.Spread = new Spread { PointSpread = 3.5M, SpreadDirection = SpreadDirections.ToAway };
            playerPicks.Picks.Add(gamePick);

            gamePick = new GamePick { PickId = 2, Pick = PickTypes.Away, PickStatus = PickStates.None };
            gamePick.Game = new Game { GameId = 1, CurrentPeriod = "1", GameStart = DateTime.Now, GameState = GameStates.SpreadNotSet, LastUpdated = DateTime.Now, NeutralField = false, TimeClock = new TimeSpan(0, 15, 0) };
            gamePick.Game.AwayTeam = new TeamScore { Score = 0, ScoreAfterSpread = 3.5M, Winner = false };
            gamePick.Game.AwayTeam.Team = new Team { TeamCode = "OregonSt", LongName = "Oregon State University", ShortName = "Oregon St", NcaaNameSeo = "oregon-st", theSpreadName = "Oregon St", icon24FileName = "oregon-st.24.png" };
            gamePick.Game.HomeTeam = new TeamScore { Score = 0, ScoreAfterSpread = 0M, Winner = false };
            gamePick.Game.HomeTeam.Team = new Team { TeamCode = "USoCarolina", LongName = "University of South Carolina", ShortName = "USC", NcaaNameSeo = "south-carolina", theSpreadName = "USoCarolina", icon24FileName = "south-carolina.24.png" };
            gamePick.Game.Spread = new Spread { PointSpread = 3.5M, SpreadDirection = SpreadDirections.ToAway };

            playerPicks.Picks.Add(gamePick);
            playerPicks.WeekPoints = playerPicks.Picks.Count(p => p.PickStatus == PickStates.Won);

            return weekScoreboard;

        }
    }
}