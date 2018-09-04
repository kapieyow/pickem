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
import { PickTypes, PickStates, GameStates } from '../models/api/enums';
import { PlayerPick } from '../models/api/player-pick';
import { PlayerPickUpdate } from '../models/api/player-pick-update';

//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()

export class LeagueService {

  
  currentPlayerScoreboardPicks: PlayerScoreboardPick[] = [];

  constructor(private logger: LoggerService, private statusService: StatusService, private http: HttpClient) { }

  public loadPlayerScoreboard(seasonCode: string, leagueCode: string, weekNumber: number, playerTag: string)
  {
    this.readPlayerScoreboard(seasonCode, leagueCode, weekNumber, playerTag)
      .subscribe(
        response => this.currentPlayerScoreboardPicks = response,
        errors => this.currentPlayerScoreboardPicks = [] // note readPlayerScoreboard already logged whatever barfed.
      );
  }

  public readPlayerScoreboard(seasonCode: string, leagueCode: string, weekNumber: number, playerTag: string): Observable<PlayerScoreboardPick[]>
  {
    //{SeasonCode}/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard
    return this.http.get<PlayerScoreboardPick[]>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/" + weekNumber + "/" + playerTag + "/scoreboard", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.length}) player scoreboard picks`)),
        catchError(error => 
          { 
            this.currentPlayerScoreboardPicks = [];
            return throwError(this.logger.logAndParseHttpError(error)); 
          })
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

  public readPlayers(seasonCode: string, leagueCode: string) : Observable<Player[]>
  {
    // /api/:SeasonCode/:LeagueCode/players
    return this.http.get<Player[]>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/players", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.length}) players`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }

  public readWeeks(seasonCode: string, leagueCode: string) :Observable<number[]>
  {
    // /api/:SeasonCode/:LeagueCode/weeks
    return this.http.get<number[]>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/weeks", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.length}) weeks`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }

  public setPlayerPick(seasonCode: string, leagueCode: string, weekNumber: number, playerTag: string, gameId: number, pick: PickTypes)
  {
    var playerPickUpdate = new PlayerPickUpdate();

    playerPickUpdate.pick = pick;

    // /api/{SeasonCode}/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard/{GameId}/pick
    return this.http.put<PlayerPick>(
        environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/" + weekNumber + "/" + playerTag + "/scoreboard/" + gameId + "/pick", 
        playerPickUpdate, 
        httpOptions
      )
      .subscribe(
        response => 
          {
            // find same game
            var playerPick = this.currentPlayerScoreboardPicks.find(psp => psp.gameId == gameId);
            playerPick.pick = pick;
          },
        errors => 
          { 
            this.currentPlayerScoreboardPicks = [];
            return throwError(this.logger.logAndParseHttpError(errors));
          }  
      );
     
  }
}
