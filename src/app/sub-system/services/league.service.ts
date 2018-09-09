import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { ThrowStmt, ERROR_COMPONENT_TYPE } from '@angular/compiler';
import { Player } from '../models/api/player';
import { PlayerScoreboard } from '../models/api/player-scoreboard';
import { LoggerService } from './logger.service';
import { StatusService } from './status.service';
import { PickTypes, PickStates, GameStates } from '../models/api/enums';
import { PlayerPick } from '../models/api/player-pick';
import { PlayerPickUpdate } from '../models/api/player-pick-update';
import { LeagueWeeks } from '../models/api/league-weeks';
import { WeekScoreboard } from '../models/api/week-scoreboard';

//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()

export class LeagueService {

  weekNumbers: number[] = [];
  players: Player[] = [];  
  playerScoreboard: PlayerScoreboard = new PlayerScoreboard();
  weekScoreboard: WeekScoreboard;

  constructor(private logger: LoggerService, private statusService: StatusService, private http: HttpClient) { }

  public loadPlayerScoreboard(seasonCode: string, leagueCode: string, weekNumber: number, playerTag: string)
  {
    this.readPlayerScoreboard(seasonCode, leagueCode, weekNumber, playerTag)
      .subscribe(
        response => this.playerScoreboard = response,
        errors => this.playerScoreboard = new PlayerScoreboard() // note readPlayerScoreboard already logged whatever barfed.
      );
  }

  public loadWeekScoreboard(seasonCode: string, leagueCode: string, weekNumber: number)
  {
    this.readWeekScoreboard(seasonCode, leagueCode, weekNumber)
      .subscribe(
        response => this.weekScoreboard = response,
        errors => this.weekScoreboard = null
      );
  }

  public readPlayerScoreboard(seasonCode: string, leagueCode: string, weekNumber: number, playerTag: string): Observable<PlayerScoreboard>
  {
    //{SeasonCode}/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard
    return this.http.get<PlayerScoreboard>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/" + weekNumber + "/" + playerTag + "/scoreboard", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.gamePickScoreboards.length}) player scoreboard picks`)),
        catchError(error => 
          { 
            this.playerScoreboard = new PlayerScoreboard();
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

  public readWeeks(seasonCode: string, leagueCode: string) :Observable<LeagueWeeks>
  {
    // /api/:SeasonCode/:LeagueCode/weeks
    return this.http.get<LeagueWeeks>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/weeks", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.weekNumbers.length}) weeks`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }

  public readWeekScoreboard(seasonCode: string, leagueCode: string, weekNumber: number): Observable<WeekScoreboard>
  {
    // /api/{SeasonCode}/{LeagueCode}/{WeekNumber}/scoreboard
    return this.http.get<WeekScoreboard>(environment.pickemRestServerBaseUrl + "/" + seasonCode + "/" + leagueCode + "/" + weekNumber + "/scoreboard", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read week (${weekNumber}) scoreboard for league (${leagueCode})`)),
        catchError(error => 
          { 
            this.weekScoreboard = null;
            return throwError(this.logger.logAndParseHttpError(error)); 
          })
      );
  }

  public setupLeagueFilters()
  {
    this.readPlayers(this.statusService.seasonCode, this.statusService.leagueCode)
      .subscribe(
        response => { 
          this.players = response 
        },
        errors => { return throwError(this.logger.logAndParseHttpError(errors)); }
      );

    // load weeks
    this.readWeeks(this.statusService.seasonCode, this.statusService.leagueCode)
      .subscribe(
        response => { 
          this.weekNumbers = response.weekNumbers;
          this.statusService.weekNumberFilter = response.currentWeekNumber;
        },
        errors => { return throwError(this.logger.logAndParseHttpError(errors)); }
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
            var gamePickScoreboard = this.playerScoreboard.gamePickScoreboards.find(psp => psp.gameId == gameId);
            var pickScoreboard = gamePickScoreboard.pickScoreboards.find(ps => ps.playerTag == playerTag);
            pickScoreboard.pick = pick;
          },
        errors => 
          { 
            this.playerScoreboard = new PlayerScoreboard();
            return throwError(this.logger.logAndParseHttpError(errors));
          }  
      );
     
  }
}
