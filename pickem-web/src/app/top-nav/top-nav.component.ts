import { Component, OnInit } from '@angular/core';
import { Router } from '../../../node_modules/@angular/router';
import { Observable, forkJoin, EMPTY } from 'rxjs';
import { of } from 'rxjs/observable/of';
import { switchMap, debounceTime, map, retryWhen, tap, delay, catchError } from "rxjs/operators";

import { environment } from '../../environments/environment';
import { VERSION } from '../../environments/version';

import { StatusService } from '../sub-system/services/status.service';
import { UserService } from '../sub-system/services/user.service';
import { LeagueScoreboard } from '../sub-system/models/api/league-scoreboard';
import { PlayerScoreboard } from '../sub-system/models/api/player-scoreboard';
import { WeekScoreboard } from '../sub-system/models/api/week-scoreboard';
import { LeagueService } from '../sub-system/services/league.service';
import { LoggerService } from '../sub-system/services//logger.service';

import { QueueingSubject } from 'queueing-subject'
import websocketConnect from 'rxjs-websockets'

class StatusValue
{
  FieldValue: string;
  FieldName: string;
}

class Scoreboards
{
  leagueScoreboard: LeagueScoreboard;
  playerScoreboard: PlayerScoreboard; 
  weekScoreboard: WeekScoreboard;
}


@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.css']
})
export class TopNavComponent implements OnInit {

  isCollapsed = true;
  refreshInProcess = false;
  socketConnected = false;
  StatusValues: StatusValue[] = [];
  private _socketSubscription;
  private _lastWebSocketsConnectedCount = 0;

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
          this.StatusValues.push({ FieldName: "Web Service REST URL", FieldValue: environment.pickemRestServerBaseUrl });
          this.StatusValues.push({ FieldName: "Web Socket URL", FieldValue: environment.pickemWebSocketUrl });
          this.StatusValues.push({ FieldName: "Web Runtime Environment", FieldValue: ( environment.production ? "Production" : "Non-Prod" ) });
          this.StatusValues.push({ FieldName: "Web Version", FieldValue: VERSION.version });
        }
      )

    // TODO: Move web socket client to a service or something
    const socketInput = new QueueingSubject<string>()
    const { messages, connectionStatus } = websocketConnect(environment.pickemWebSocketUrl, socketInput);

    const connectionStatusSubscription = connectionStatus
      .subscribe(numberConnected => 
        {
          this.logger.debug('number of connected websockets: ' + numberConnected);
          this.socketConnected = numberConnected > 0;
          if ( this._lastWebSocketsConnectedCount == 0 && numberConnected > 0 )
          {
            this.reloadScoreboards();
          }
          this._lastWebSocketsConnectedCount = numberConnected;
        }
      );

    const pipedMessages = messages
      .pipe(
        retryWhen(errors => 
          errors.pipe(
            tap(error => this.logger.debug("Socket error. Delayed retry. Error: " + error)),
            delay(5000)
          )
        ),
        tap(message => this.logger.debug('received message:' + message)),
        debounceTime(2000),
        tap(() => 
            {
              this.logger.debug("debounced scoreboard poll");
              this.refreshInProcess = true;
            }
        ),
        switchMap(() => 
            {
              return this.readScoreboards()
                .pipe(
                  catchError(error => 
                    {
                      console.error("Caught readScoreboard failure. Error: " + error); // NOTE: not using this.logger.error as it could fail.
                      // readscoreboards blew chunks, likely net drop
                      // return the existing scoreboard data that was loaded earlier so the UI won't blank
                      let sameScoreboard = new Scoreboards();
                      sameScoreboard.leagueScoreboard = this.leagueService.leagueScoreboard;
                      sameScoreboard.playerScoreboard = this.leagueService.playerScoreboard;
                      sameScoreboard.weekScoreboard = this.leagueService.weekScoreboard;
                      return of(sameScoreboard);
                    }
                  )
                );
            }
        )
      )

    this._socketSubscription = pipedMessages
      .subscribe(responses => 
        {
          this.leagueService.leagueScoreboard = responses.leagueScoreboard;
          this.leagueService.playerScoreboard = responses.playerScoreboard;
          this.leagueService.weekScoreboard = responses.weekScoreboard;
          this.refreshInProcess = false;
        }
      );
  }

  ngOnDestroy()
  {
    if ( this._socketSubscription != null )
      this._socketSubscription.unsubscribe();
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

  async changeLeague(league: string)
  {
    this.statusService.leagueCode = league;
    this.refreshInProcess = true;

    this.leagueService.playerScoreboard = null;
    this.leagueService.weekScoreboard = null;
    this.leagueService.leagueScoreboard = null;

    await this.leagueService.loadLeagueMetaData(this.statusService.leagueCode, this.statusService.userName);

    this.leagueService.loadPlayerScoreboard(
      this.statusService.leagueCode, 
      this.statusService.weekNumberFilter,
      this.statusService.playerTagFilter);

    this.leagueService.loadWeekScoreboard(
      this.statusService.leagueCode, 
      this.statusService.weekNumberFilter
      );

    this.leagueService.loadLeagueScoreboard(
      this.statusService.leagueCode
      );

    this.statusService.userLoggedInAndInitialized = true;
    this.refreshInProcess = false;  
  }

  readScoreboards(): Observable<Scoreboards>
  {

    if ( this.statusService.userLoggedInAndInitialized ) 
    {
      // TODO: move all this to a central location? league service?
      return forkJoin(
        this.leagueService.readLeagueScoreboard(
          this.statusService.leagueCode
        ),
        this.leagueService.readPlayerScoreboard(
          this.statusService.leagueCode,
          this.statusService.weekNumberFilter,
          this.statusService.playerTagFilter
        ),
        this.leagueService.readWeekScoreboard(
          this.statusService.leagueCode, 
          this.statusService.weekNumberFilter
        )
      ).pipe(
        map(([leagueScoreboard, playerScoreboard, weekScoreboard]) => {
          // forkJoin returns an array of values, here we map those values to an object
          return { leagueScoreboard, playerScoreboard, weekScoreboard };
        })
      );
    }
    else
    {
      return of(new Scoreboards());
    }
  }

  reloadScoreboards()
  {
    this.refreshInProcess = true;

    this.readScoreboards().subscribe( responses => {
      this.leagueService.leagueScoreboard = responses.leagueScoreboard;
      this.leagueService.playerScoreboard = responses.playerScoreboard;
      this.leagueService.weekScoreboard = responses.weekScoreboard;
      this.refreshInProcess = false;
    })

  }
}
