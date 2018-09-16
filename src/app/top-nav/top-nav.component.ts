import { Component, OnInit } from '@angular/core';
import { Router } from '../../../node_modules/@angular/router';
import { Observable, throwError } from 'rxjs';

import { environment } from '../../environments/environment';
import { VERSION } from '../../environments/version';

import { StatusService } from '../sub-system/services/status.service';
import { UserService } from '../sub-system/services/user.service';
import { Player } from '../sub-system/models/api/player';
import { LeagueService } from '../sub-system/services/league.service';
import { LoggerService } from '../sub-system/services//logger.service';



class StatusValue
{
  FieldValue: string;
  FieldName: string;
}


@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.css']
})
export class TopNavComponent implements OnInit {

  isCollapsed = true;
  StatusValues: StatusValue[] = [];

  constructor(public statusService: StatusService, public leagueService: LeagueService, private router: Router, private userService: UserService, private logger: LoggerService, ) { }

  ngOnInit() {

    // get server status values, then build them plus this one.
    this.statusService.readPickEmStatus()
      .subscribe(pickemeStatus => 
        {
          //statusValues.push(new StatusValue(FieldName: "Database", FieldValue: nickServerStatus.database))
          this.StatusValues.push({ FieldName: "Authenticated User", FieldValue: pickemeStatus.authenticatedUserName });
          this.StatusValues.push({ FieldName: "Database", FieldValue: pickemeStatus.database });
          this.StatusValues.push({ FieldName: "Database Host", FieldValue: pickemeStatus.databaseHost });
          this.StatusValues.push({ FieldName: "Service Product", FieldValue: pickemeStatus.product });
          this.StatusValues.push({ FieldName: "Service Version", FieldValue: pickemeStatus.productVersion });
          this.StatusValues.push({ FieldName: "Service Runtime Environment", FieldValue: pickemeStatus.runtimeEnvironment });
          this.StatusValues.push({ FieldName: "Web to Service REST URL", FieldValue: environment.pickemRestServerBaseUrl });
          this.StatusValues.push({ FieldName: "Web Runtime Environment", FieldValue: ( environment.production ? "Production" : "Non-Prod" ) });
          this.StatusValues.push({ FieldName: "Web Version", FieldValue: VERSION.version });
        }
      )
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

    this.leagueService.loadWeekScoreboard(
      this.statusService.seasonCode, 
      this.statusService.leagueCode, 
      this.statusService.weekNumberFilter);

    this.leagueService.loadLeagueScoreboard(
      this.statusService.seasonCode, 
      this.statusService.leagueCode);
  }
}
