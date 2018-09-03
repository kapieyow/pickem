import { Component, OnInit } from '@angular/core';
import { Router } from '../../../node_modules/@angular/router';
import { Observable, throwError } from 'rxjs';

import { StatusService } from '../sub-system/services/status.service';
import { UserService } from '../sub-system/services/user.service';
import { Player } from '../sub-system/models/api/player';
import { LeagueService } from '../sub-system/services/league.service';
import { LoggerService } from '../sub-system/services//logger.service';



@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.css']
})
export class TopNavComponent implements OnInit {

  weeks: number[] = [];
  players: Player[] = [];

  constructor(public statusService: StatusService, private leagueService: LeagueService, private router: Router, private userService: UserService, private logger: LoggerService, ) { }

  ngOnInit() {

    // load players
    this.leagueService.readPlayers(this.statusService.seasonCode, this.statusService.leagueCode)
      .subscribe(
        response => { 
          this.players = response 
        },
        errors => { return throwError(this.logger.logAndParseHttpError(errors)); }
      );

    // load weeks
    this.leagueService.readWeeks(this.statusService.seasonCode, this.statusService.leagueCode)
      .subscribe(
        response => { 
          this.weeks = response 
        },
        errors => { return throwError(this.logger.logAndParseHttpError(errors)); }
      );
  }

  logout ()
  {
    this.userService.logout();
    this.router.navigate(['/'], { skipLocationChange: true });
  }

  changeWeek(newWeek: number)
  {
    this.statusService.weekNumberFilter = newWeek;
    this.reloadScoreboards();
  }

  changePlayer(newPlayerTag: string)
  {
    this.statusService.playerTagFilter = newPlayerTag;
    this.reloadScoreboards();
  }

  reloadScoreboards()
  {
    this.leagueService.loadPlayerScoreboard(
      this.statusService.seasonCode, 
      this.statusService.leagueCode,
      this.statusService.weekNumberFilter,
      this.statusService.playerTagFilter
    );
  }
}
