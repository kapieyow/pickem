import { Component, OnInit } from '@angular/core';

class Pick {
  AwayTeamIconImage: string;
  AwayTeamLosses: number;
  AwayTeamName: string;
  AwayTeamRank: number;
  AwayTeamScore: number;
  AwayTeamWins: number;
  GameStatus: string;
  GameStatusCode: GameStatus;
  HomeTeamIconImage: string;
  HomeTeamLosses: number;
  HomeTeamName: string;
  HomeTeamRank: number;
  HomeTeamScore: number;
  HomeTeamWins: number;
  Id: number;
  PickCode: PickCodes;
  PickState: PickStatus;
  PickToSpreadNeatral: number;
  Spread: number;
  SpreadDirection: SpreadDirections;
}

enum GameStatus {
  Final = "FINAL",
  InGame = "IN_GAME",
  SpreadNotSet = "SPREAD_NOT_SET",
  SpreadSet = "SPREAD_SET"
}

enum PickCodes {
  Away = "AWAY",
  Home = "HOME",
  None = "NONE"
}

enum PickStatus {
  Losing = "LOSING",
  Lost = "LOST",
  None = "NONE",
  Pushing = "PUSHING",
  Pushed = "PUSHED",
  Winning = "WINNING",
  Won = "WON"
}

enum SpreadDirections {
  None = "NONE",
  ToAway = "TO_AWAY",
  ToHome = "TO_HOME"
}

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class PlayerComponent implements OnInit {

  Picks: Pick[] = [];

  constructor() { }

  ngOnInit() {
    this.loadFakePicks();
  }

  loadFakePicks() {

    

    // spread not set (none at all yet)
    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "No Spread",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 3,
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadNotSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: null,
        HomeTeamWins: 4,
        Id: 1,
        PickCode: PickCodes.None,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        Spread: null,
        SpreadDirection: SpreadDirections.None
      });

    // spread not set (pending spread)
    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "Spread not set, but have draft",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 3,
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadNotSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: null,
        HomeTeamWins: 4,
        Id: 2,
        PickCode: PickCodes.None,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        Spread: 6,
        SpreadDirection: SpreadDirections.ToAway
      });

    // spread set:  no pick (away spread)
    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "Spread set no pick",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 3,
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: null,
        HomeTeamWins: 4,
        Id: 3,
        PickCode: PickCodes.None,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        Spread: 6,
        SpreadDirection: SpreadDirections.ToAway
      });

     // spread set:  no pick (home spread)
     this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "Spread set no pick",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 3,
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: null,
        HomeTeamWins: 4,
        Id: 4,
        PickCode: PickCodes.None,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        Spread: 6,
        SpreadDirection: SpreadDirections.ToHome
      });

    // spread set:  picked away
    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "Away Team",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 3,
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: null,
        HomeTeamWins: 4,
        Id: 9,
        PickCode: PickCodes.Away,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        Spread: 6,
        SpreadDirection: SpreadDirections.ToHome
      });

    // spread set:  picked home
    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "Away Team",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 3,
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: null,
        HomeTeamWins: 4,
        Id: 10,
        PickCode: PickCodes.Home,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        Spread: 6,
        SpreadDirection: SpreadDirections.ToHome
      });

      // in game - picked away, losing
      this.Picks.push(
        {
          AwayTeamIconImage: "clemson.24.png",
          AwayTeamLosses: 2,
          AwayTeamName: "In Game Away Team",
          AwayTeamRank: 0,
          AwayTeamScore: 15,
          AwayTeamWins: 3,
          GameStatus: "5:18 in 3rd Quarter",
          GameStatusCode: GameStatus.InGame,
          HomeTeamIconImage: "clemson.24.png",
          HomeTeamLosses: 1,
          HomeTeamName: "In Game Home Team",
          HomeTeamRank: 15,
          HomeTeamScore: 28,
          HomeTeamWins: 4,
          Id: 11,
          PickCode: PickCodes.Away,
          PickState: PickStatus.Losing,
          PickToSpreadNeatral: -20.5,
          Spread: 7.5,
          SpreadDirection: SpreadDirections.ToHome
        });

    // in game: push, winning, losing X away and home  
    // in game: no pick
    // in game: pushed, won, lost X away and home  
    // final: no pick

    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "Away Team",
        AwayTeamRank: 0,
        AwayTeamScore: 3,
        AwayTeamWins: 3,
        GameStatus: "Final",
        GameStatusCode: GameStatus.Final,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: 18,
        HomeTeamWins: 4,
        Id: 5,
        PickCode: PickCodes.Away,
        PickState: PickStatus.Lost,
        PickToSpreadNeatral: -7,
        Spread: 8,
        SpreadDirection: SpreadDirections.ToAway
      });

    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 0,
        AwayTeamName: "Other Away Team",
        AwayTeamRank: 4,
        AwayTeamScore: 35,
        AwayTeamWins: 4,
        GameStatus: "Final",
        GameStatusCode: GameStatus.Final,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Other Home Team",
        HomeTeamRank: 22,
        HomeTeamScore: 28,
        HomeTeamWins: 4,
        Id: 6,
        PickCode: PickCodes.Away,
        PickState: PickStatus.Won,
        PickToSpreadNeatral: 3,
        Spread: 4,
        SpreadDirection: SpreadDirections.ToHome
      });

    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 0,
        AwayTeamName: "Whatever",
        AwayTeamRank: 4,
        AwayTeamScore: 22,
        AwayTeamWins: 4,
        GameStatus: "Final",
        GameStatusCode: GameStatus.Final,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Yeah Yeah Yeah",
        HomeTeamRank: 22,
        HomeTeamScore: 28,
        HomeTeamWins: 4,
        Id: 7,
        PickCode: PickCodes.Home,
        PickState: PickStatus.Won,
        PickToSpreadNeatral: 1.5,
        Spread: 4.5,
        SpreadDirection: SpreadDirections.ToAway
      });

    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 3,
        AwayTeamName: "Away hasn't started",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 2,
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 2,
        HomeTeamName: "Home hasn't started",
        HomeTeamRank: 25,
        HomeTeamScore: null,
        HomeTeamWins: 3,
        Id: 8,
        PickCode: PickCodes.None,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        Spread: 11,
        SpreadDirection: SpreadDirections.ToAway
      });

      

  }



}
