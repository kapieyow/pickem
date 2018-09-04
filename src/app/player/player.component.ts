import { Component, OnInit } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { PlayerScoreboardPick } from '../sub-system/models/api/player-scoreboard-pick';
import { LeagueService } from '../sub-system/services/league.service';
import { LoggerService } from '../sub-system/services//logger.service';
import { StatusService } from '../sub-system/services/status.service';
import { PickTypes, PickStates, GameStates } from '../sub-system/models/api/enums';



@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.css']
})
export class PlayerComponent implements OnInit {

  constructor(public leagueService: LeagueService, private logger: LoggerService, private statusService: StatusService) { }

  ngOnInit() {
    this.getPlayerPicks();
  }

  getPlayerPicks()
  {
    this.leagueService.loadPlayerScoreboard(
      this.statusService.seasonCode, 
      this.statusService.leagueCode, 
      this.statusService.weekNumberFilter, 
      this.statusService.playerTagFilter);
  }

  setPick(playerScoreboardPick: PlayerScoreboardPick, newPick: PickTypes)
  {
    if ( playerScoreboardPick.pick == newPick )
    {
      this.logger.debug(`Game ${playerScoreboardPick.gameId}, already has pick ${newPick} so click ignored`);
    }
    else
    {
      // only ok to make picks if the game has not started
      if ( this.gamePickableByCurrentPlayer(playerScoreboardPick) )
      {  
        this.logger.debug(`Game ${playerScoreboardPick.gameId}, pick type ${newPick}`);
        this.leagueService.setPlayerPick(
            this.statusService.seasonCode, 
            this.statusService.leagueCode, 
            this.statusService.weekNumberFilter,
            this.statusService.userPlayerTag,
            playerScoreboardPick.gameId,
            newPick
          );
      }
      else
      {
        this.logger.debug(`Game is not pickable by user (${this.statusService.userPlayerTag}), player filter (${this.statusService.playerTagFilter}), game state  (${playerScoreboardPick.gameState})`);
      }
    }
  }

  gamePickableByCurrentPlayer(playerScoreboardPick: PlayerScoreboardPick) : boolean
  {
    if ( this.statusService.userPlayerTag == this.statusService.playerTagFilter )
    {
      switch (playerScoreboardPick.gameState)
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

}
