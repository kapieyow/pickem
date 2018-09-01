import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { ThrowStmt, ERROR_COMPONENT_TYPE } from '@angular/compiler';
import { Player } from '../models/api/player';
import { PlayerScoreboardPick } from '../models/api/player-scoreboard-pick';
import { LoggerService } from './logger.service';
import { StatusService } from './status.service';

//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()

export class LeagueService {

  constructor(private logger: LoggerService, private statusService: StatusService, private http: HttpClient) { }

  public readPlayerScoreboard(seasonCode: string, leagueCode: string, weekNumber: number, playerTag: string): Observable<PlayerScoreboardPick[]>
  {
    //{SeasonCode}/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard
    return this.http.get<PlayerScoreboardPick[]>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/" + weekNumber + "/" + playerTag + "/scoreboard", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.length}) player scoreboard picks`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }

  public readPlayer(seasonCode: string, leagueCode: string, userName: string): Observable<Player>
  {
    // /api/{SeasonCode}/{LeagueCode}/players/{UserName}
    return this.http.get<Player>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/players/" + userName, httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${userName}) player`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }
}
