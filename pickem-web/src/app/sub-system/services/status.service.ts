import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { ThrowStmt, ERROR_COMPONENT_TYPE } from '@angular/compiler';

import { LoggerService } from './logger.service';
import { League } from '../models/api/league';
import { PickEmStatus } from '../models/api/pickem-status';

//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({
  providedIn: 'root',
})
export class StatusService {

  private _leagueCode: string;

  // TODO: probably can get rid of leagueCode altogether AND straighten out the "league set" logic 
  // to set the current league directly, instead of through an accessor
  public get leagueCode(): string
  {
    return this._leagueCode;
  }
  public set leagueCode(newLeagueCode: string)
  {
    this._leagueCode = newLeagueCode;

    if ( newLeagueCode && this.userLeagues )
    {
      this.currentLeague = this.userLeagues.find(league => league.leagueCode == newLeagueCode);
    }
    else
    {    
      this.currentLeague = null;
    }
  }

  public currentLeague: League;
  public weekNumberFilter: number;
  public playerTagFilter: string;

  // TODO: this is probably terrible. Some other flag to show when active
  public userLoggedInAndInitialized: boolean;
  public userName: string;
  public userPlayerTag: string;
  public userLeagues: League[];

  constructor(private logger: LoggerService, private http: HttpClient) {
    this.userLoggedInAndInitialized = false;
  }

  clearAllSessionState()
  {
    localStorage.removeItem("JWT");
    this.userLoggedInAndInitialized = false;
    this.userName = null;
    this.userPlayerTag = null;
    this.currentLeague = null;
    this.leagueCode = null;
    this.playerTagFilter = null;
    this.weekNumberFilter = null;
  }

  readPickEmStatus(): Observable<PickEmStatus> {
    // build get URL
    let readUrl = environment.pickemRestServerBaseUrl  + "/status";

    return this.http.get<PickEmStatus>(readUrl)
      .pipe(
          tap(response => this.logger.debug(`read pickem status`)),
          catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); })
        );        
  }
}
