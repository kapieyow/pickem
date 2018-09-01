import { Component, OnInit } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { PlayerScoreboardPick } from '../sub-system/models/api/player-scoreboard-pick';
import { LeagueService } from '../sub-system/services/league.service';
import { LoggerService } from '../sub-system/services//logger.service';
import { StatusService } from '../sub-system/services/status.service';

class Pick {
  awayTeamIconFileName: string;
  awayTeamLosses: number;
  awayTeamLongName: string;
  awayTeamRank: number;
  awayTeamScore: number;
  awayTeamWins: number;
  gameStatusDescription: string;
  gameState: gameStatusDescription;
  homeTeamIconFileName: string;
  homeTeamLosses: number;
  homeTeamLongName: string;
  homeTeamRank: number;
  homeTeamScore: number;
  homeTeamWins: number;
  gameId: number;
  pick: PickCodes;
  pickState: PickStatus;
  pickToSpreadNeutral: number;
  spread: number;
  spreadDirection: SpreadDirections;
}

enum gameStatusDescription {
  Final = "Final",
  InGame = "InGame",
  SpreadNotSet = "SpreadNotSet",
  SpreadLocked = "SpreadLocked"
}

enum PickCodes {
  Away = "Away",
  Home = "Home",
  None = "None"
}

enum PickStatus {
  Losing = "Losing",
  Lost = "Lost",
  None = "None",
  Pushing = "Pushing",
  Pushed = "Pushed",
  Winning = "Winning",
  Won = "Won"
}

enum SpreadDirections {
  None = "None",
  ToAway = "ToAway",
  ToHome = "ToHome"
}

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class PlayerComponent implements OnInit {

  //Picks: Pick[] = [];
  Picks: PlayerScoreboardPick[] = [];


  constructor(private leagueService: LeagueService, private logger: LoggerService, private statusService: StatusService) { }

  ngOnInit() {
    this.getPlayerPicks();
  }

  getPlayerPicks()
  {
    this.leagueService.readPlayerScoreboard(this.statusService.seasonCode, this.statusService.leagueCode, this.statusService.weekNumberFilter, this.statusService.playerTagFilter)
      .subscribe(
        response => { 
          this.Picks = response 
        },
        errors => { return throwError(this.logger.logAndParseHttpError(errors)); }
      );
  }

  loadFakePicks() {

    // spread not set (none at all yet)
    /*
    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "No spread",
        awayTeamRank: 0,
        awayTeamScore: null,
        awayTeamWins: 3,
        gameStatusDescription: "Saturday 9/27 - 8:00PM EDT",
        gameState: gameStatusDescription.SpreadNotSet,
        homeTeamIconFileName: "north-carolina.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Home Team",
        homeTeamRank: 15,
        homeTeamScore: null,
        homeTeamWins: 4,
        gameId: 1,
        pick: PickCodes.None,
        pickState: PickStatus.None,
        pickToSpreadNeutral: null,
        spread: null,
        spreadDirection: SpreadDirections.None
      });

    // spread not set (pending spread)
    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "spread not set, but have draft",
        awayTeamRank: 0,
        awayTeamScore: null,
        awayTeamWins: 3,
        gameStatusDescription: "Saturday 9/27 - 8:00PM EDT",
        gameState: gameStatusDescription.SpreadNotSet,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Home Team",
        homeTeamRank: 15,
        homeTeamScore: null,
        homeTeamWins: 4,
        gameId: 2,
        pick: PickCodes.None,
        pickState: PickStatus.None,
        pickToSpreadNeutral: null,
        spread: 6,
        spreadDirection: SpreadDirections.ToAway
      });

    // spread set:  no pick (away spread)
    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "spread set no pick",
        awayTeamRank: 0,
        awayTeamScore: null,
        awayTeamWins: 3,
        gameStatusDescription: "Saturday 9/27 - 8:00PM EDT",
        gameState: gameStatusDescription.SpreadLocked,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Home Team",
        homeTeamRank: 15,
        homeTeamScore: null,
        homeTeamWins: 4,
        gameId: 3,
        pick: PickCodes.None,
        pickState: PickStatus.None,
        pickToSpreadNeutral: null,
        spread: 6,
        spreadDirection: SpreadDirections.ToAway
      });

    // spread set:  no pick (home spread)
    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "spread set no pick",
        awayTeamRank: 0,
        awayTeamScore: null,
        awayTeamWins: 3,
        gameStatusDescription: "Saturday 9/27 - 8:00PM EDT",
        gameState: gameStatusDescription.SpreadLocked,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Home Team",
        homeTeamRank: 15,
        homeTeamScore: null,
        homeTeamWins: 4,
        gameId: 4,
        pick: PickCodes.None,
        pickState: PickStatus.None,
        pickToSpreadNeutral: null,
        spread: 6,
        spreadDirection: SpreadDirections.ToHome
      });

    // spread set:  picked away
    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "Away Team",
        awayTeamRank: 0,
        awayTeamScore: null,
        awayTeamWins: 3,
        gameStatusDescription: "Saturday 9/27 - 8:00PM EDT",
        gameState: gameStatusDescription.SpreadLocked,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Home Team",
        homeTeamRank: 15,
        homeTeamScore: null,
        homeTeamWins: 4,
        gameId: 9,
        pick: PickCodes.Away,
        pickState: PickStatus.None,
        pickToSpreadNeutral: null,
        spread: 6,
        spreadDirection: SpreadDirections.ToHome
      });

    // spread set:  picked home
    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "Away Team",
        awayTeamRank: 0,
        awayTeamScore: null,
        awayTeamWins: 3,
        gameStatusDescription: "Saturday 9/27 - 8:00PM EDT",
        gameState: gameStatusDescription.SpreadLocked,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Home Team",
        homeTeamRank: 15,
        homeTeamScore: null,
        homeTeamWins: 4,
        gameId: 10,
        pick: PickCodes.Home,
        pickState: PickStatus.None,
        pickToSpreadNeutral: null,
        spread: 6,
        spreadDirection: SpreadDirections.ToHome
      });

    // in game - picked away, losing
    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "In Game Away Team",
        awayTeamRank: 0,
        awayTeamScore: 15,
        awayTeamWins: 3,
        gameStatusDescription: "5:18 in 3rd Quarter",
        gameState: gameStatusDescription.InGame,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "In Game Home Team",
        homeTeamRank: 15,
        homeTeamScore: 28,
        homeTeamWins: 4,
        gameId: 11,
        pick: PickCodes.Away,
        pickState: PickStatus.Losing,
        pickToSpreadNeutral: -20.5,
        spread: 7.5,
        spreadDirection: SpreadDirections.ToHome
      });

    // in game: push, winning, losing X away and home  
    // in game: no pick
    // in game: pushed, won, lost X away and home  
    // final: no pick

    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 2,
        awayTeamLongName: "Away Team",
        awayTeamRank: 0,
        awayTeamScore: 3,
        awayTeamWins: 3,
        gameStatusDescription: "Final",
        gameState: gameStatusDescription.Final,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Home Team",
        homeTeamRank: 15,
        homeTeamScore: 18,
        homeTeamWins: 4,
        gameId: 5,
        pick: PickCodes.Away,
        pickState: PickStatus.Lost,
        pickToSpreadNeutral: -7,
        spread: 8,
        spreadDirection: SpreadDirections.ToAway
      });

    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 0,
        awayTeamLongName: "Other Away Team",
        awayTeamRank: 4,
        awayTeamScore: 35,
        awayTeamWins: 4,
        gameStatusDescription: "Final",
        gameState: gameStatusDescription.Final,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Other Home Team",
        homeTeamRank: 22,
        homeTeamScore: 28,
        homeTeamWins: 4,
        gameId: 6,
        pick: PickCodes.Away,
        pickState: PickStatus.Won,
        pickToSpreadNeutral: 3,
        spread: 4,
        spreadDirection: SpreadDirections.ToHome
      });

    this.Picks.push(
      {
        awayTeamIconFileName: "clemson.24.png",
        awayTeamLosses: 0,
        awayTeamLongName: "Whatever",
        awayTeamRank: 4,
        awayTeamScore: 22,
        awayTeamWins: 4,
        gameStatusDescription: "Final",
        gameState: gameStatusDescription.Final,
        homeTeamIconFileName: "clemson.24.png",
        homeTeamLosses: 1,
        homeTeamLongName: "Yeah Yeah Yeah",
        homeTeamRank: 22,
        homeTeamScore: 28,
        homeTeamWins: 4,
        gameId: 7,
        pick: PickCodes.Home,
        pickState: PickStatus.Won,
        pickToSpreadNeutral: 1.5,
        spread: 4.5,
        spreadDirection: SpreadDirections.ToAway
      });
      */
  }

}
