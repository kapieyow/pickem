import { Component, OnInit } from '@angular/core';

class Pick {
  AwayTeamIconImage: string;
  AwayTeamLosses: number;
  AwayTeamName: string;
  AwayTeamRank: number;
  AwayTeamScore: number;
  AwayTeamWins: number;
  GameChannel: string;
  GameLocation: string;
  GameStatus: string;
  GameStatusCode: GameStatus;
  HomeTeamIconImage: string;
  HomeTeamLosses: number;
  HomeTeamName: string;
  HomeTeamRank: number;
  HomeTeamScore: number;
  HomeTeamWins: number;
  PickCode: PickCodes;
  PickState: PickStatus;
  PickToSpreadNeatral: number;
  SpreadToAwayTeam: number;
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

    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 2,
        AwayTeamName: "Away Team",
        AwayTeamRank: 0,
        AwayTeamScore: 3,
        AwayTeamWins: 3,
        GameChannel: null,
        GameLocation: "PS backyard",
        GameStatus: "Final",
        GameStatusCode: GameStatus.Final,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Home Team",
        HomeTeamRank: 15,
        HomeTeamScore: 18,
        HomeTeamWins: 4,
        PickCode: PickCodes.Away,
        PickState: PickStatus.Lost,
        PickToSpreadNeatral: -7,
        SpreadToAwayTeam: 8
      });

    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 0,
        AwayTeamName: "Other Away Team",
        AwayTeamRank: 4,
        AwayTeamScore: 35,
        AwayTeamWins: 4,
        GameChannel: "Smell-o-vision",
        GameLocation: "yer house",
        GameStatus: "Final",
        GameStatusCode: GameStatus.Final,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 1,
        HomeTeamName: "Other Home Team",
        HomeTeamRank: 22,
        HomeTeamScore: 28,
        HomeTeamWins: 4,
        PickCode: PickCodes.Away,
        PickState: PickStatus.Won,
        PickToSpreadNeatral: 3,
        SpreadToAwayTeam: -4
      });

    this.Picks.push(
      {
        AwayTeamIconImage: "clemson.24.png",
        AwayTeamLosses: 3,
        AwayTeamName: "Away hasn't started",
        AwayTeamRank: 0,
        AwayTeamScore: null,
        AwayTeamWins: 2,
        GameChannel: "no TV",
        GameLocation: "elsewhere",
        GameStatus: "Saturday 9/27 - 8:00PM EDT",
        GameStatusCode: GameStatus.SpreadSet,
        HomeTeamIconImage: "clemson.24.png",
        HomeTeamLosses: 2,
        HomeTeamName: "Home hasn't started",
        HomeTeamRank: 25,
        HomeTeamScore: null,
        HomeTeamWins: 3,
        PickCode: PickCodes.None,
        PickState: PickStatus.None,
        PickToSpreadNeatral: null,
        SpreadToAwayTeam: 11
      });

  }



}
