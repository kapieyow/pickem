import { Component, OnInit } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { GameScoreboard } from '../sub-system/models/api/game-scoreboard';
import { LeagueService } from '../sub-system/services/league.service';
import { LoggerService } from '../sub-system/services//logger.service';
import { StatusService } from '../sub-system/services/status.service';
import { PickTypes, PickStates, GameStates } from '../sub-system/models/api/enums';
import { formatDate } from '@angular/common';



@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class PlayerComponent implements OnInit {

  constructor(public leagueService: LeagueService, private logger: LoggerService, private statusService: StatusService) { }

  ngOnInit() { }

  setPick(gameScoreboard: GameScoreboard, newPick: PickTypes)
  {
    if ( gameScoreboard.pickScoreboards[0].pick == newPick )
    {
      this.logger.debug(`Game ${gameScoreboard.gameId}, already has pick ${newPick} so click ignored`);
    }
    else
    {
      // only ok to make picks if the game has not started
      if ( this.gamePickableByCurrentPlayer(gameScoreboard) )
      {  
        this.logger.debug(`Game ${gameScoreboard.gameId}, pick type ${newPick}`);
        this.leagueService.setPlayerPick(
            this.statusService.seasonCode, 
            this.statusService.leagueCode, 
            this.statusService.weekNumberFilter,
            this.statusService.userPlayerTag,
            gameScoreboard.gameId,
            newPick
          );
      }
      else
      {
        this.logger.debug(`Game is not pickable by user (${this.statusService.userPlayerTag}), player filter (${this.statusService.playerTagFilter}), game state  (${gameScoreboard.gameState})`);
      }
    }
  }

  gamePickableByCurrentPlayer(gameScoreboard: GameScoreboard) : boolean
  {
    if ( this.statusService.userPlayerTag == this.statusService.playerTagFilter )
    {
      switch (gameScoreboard.gameState)
      {
        case GameStates.SpreadLocked:
        case GameStates.SpreadNotSet:
          return true;
      }
      return false; // was not in a pickable state
    }
    else
    {
      // not viewing the logged in player. i.e. can't change others picks
      return false;
    }
  }

  gameStatusDescriptionFirstPart(gameScoreboard: GameScoreboard) : string
  {
    switch ( gameScoreboard.gameState )
    {
        case GameStates.Cancelled:
            return "CANCELLED";

        case GameStates.Final:
            return "FINAL";

        case GameStates.InGame:

            let periodRegEx = new RegExp("^[0-9]");

            if (gameScoreboard.gameCurrentPeriod.length > 0 && periodRegEx.test(gameScoreboard.gameCurrentPeriod))
            {
                let timeParts = gameScoreboard.gameTimeClock.split(":")
                let justMinuteSeconds = timeParts[1] + ":" + timeParts[2];
                return justMinuteSeconds;
            }
            else
                return gameScoreboard.gameCurrentPeriod;

        case GameStates.SpreadLocked:
        case GameStates.SpreadNotSet:
            return formatDate(gameScoreboard.gameStart, "M/d", "en-US");
            
    }
    return null;
  }

  gameStatusDescriptionSecondPart(gameScoreboard: GameScoreboard) : string
  {
    switch ( gameScoreboard.gameState )
    {
        case GameStates.Cancelled:
        case GameStates.Final:
          return null;

        case GameStates.InGame:
          return gameScoreboard.gameCurrentPeriod;
           
        case GameStates.SpreadLocked:
        case GameStates.SpreadNotSet:
          return formatDate(gameScoreboard.gameStart, "h:mm a", "en-US");
            
    }
    return null;
  }
}
