
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { Player } from '../models/api/player';
import { PlayerScoreboard } from '../models/api/player-scoreboard';
import { LoggerService } from './logger.service';
import { StatusService } from './status.service';
import { PickTypes, PickStates, GameStates } from '../models/api/enums';
import { PlayerPick } from '../models/api/player-pick';
import { PlayerPickUpdate } from '../models/api/player-pick-update';
import { LeagueWeeks } from '../models/api/league-weeks';
import { LeagueScoreboard } from '../models/api/league-scoreboard'
import { WeekScoreboard } from '../models/api/week-scoreboard';

//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()

export class LeagueService {

  weekNumbers: number[] = [];
  players: Player[] = [];  

  playerScoreboard: PlayerScoreboard = null;
  weekScoreboard: WeekScoreboard = null;
  leagueScoreboard: LeagueScoreboard = null;


  constructor(private logger: LoggerService, private statusService: StatusService, private http: HttpClient) { }

  public loadLeagueScoreboard(leagueCode: string)
  {
    this.readLeagueScoreboard(leagueCode).subscribe( response => { this.leagueScoreboard = response; } );
  }

  public loadPlayerScoreboard(leagueCode: string, weekNumber: number, playerTag: string)
  {
    this.readPlayerScoreboard(leagueCode, weekNumber, playerTag).subscribe( response => { this.playerScoreboard = response; } );
  }

  public loadWeekScoreboard(leagueCode: string, weekNumber: number)
  {
    this.readWeekScoreboard(leagueCode, weekNumber).subscribe( response => { this.weekScoreboard = response; } );
  }

  public async loadLeagueMetaData(leagueCode: string, userName: string)
  {
    var player = await this.readPlayer(leagueCode, userName).toPromise();
    this.statusService.playerTagFilter = player.playerTag;
    this.statusService.userPlayerTag = player.playerTag;

    var leaguePlayers = await this.readLeaguePlayers(leagueCode).toPromise();
    this.players = leaguePlayers;

    var leagueWeeks = await this.readLeagueWeeks(leagueCode).toPromise();
    this.weekNumbers = leagueWeeks.weekNumbers;
    this.statusService.weekNumberFilter = leagueWeeks.currentWeekNumber;
  }


  public readLeagueScoreboard(leagueCode: string): Observable<LeagueScoreboard>
  {
    // api/{LeagueCode}/scoreboard
    return this.http.get<LeagueScoreboard>(environment.pickemRestServerBaseUrl + "/" + leagueCode + "/scoreboard", httpOptions)
      .pipe(
        tap(response => 
          { 
            this.logger.debug(`read scoreboard for league (${leagueCode})`);
          }),
        catchError(error => 
          { 
            this.leagueScoreboard = null;
            return throwError(this.logger.logAndParseHttpError(error)); 
          })
      );
  }

  public readPlayerScoreboard(leagueCode: string, weekNumber: number, playerTag: string): Observable<PlayerScoreboard>
  {
    //{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard
    return this.http.get<PlayerScoreboard>(environment.pickemRestServerBaseUrl + "/" + leagueCode + "/" + weekNumber + "/" + playerTag + "/scoreboard", httpOptions)
      .pipe(
        tap(response => 
          { 
            this.logger.debug(`read (${response.gamePickScoreboards.length}) player scoreboard picks`);
          }),
        catchError(error => 
          { 
            this.playerScoreboard = null;
            return throwError(this.logger.logAndParseHttpError(error)); 
          })
      );
  }

  private readPlayer(leagueCode: string, userName: string): Observable<Player>
  {
    // /api/{LeagueCode}/players/{UserName}
    return this.http.get<Player>(environment.pickemRestServerBaseUrl + "/" + leagueCode + "/players/" + userName, httpOptions)
      .pipe(
        tap(() => this.logger.debug(`read (${userName}) player`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }

  private readLeaguePlayers(leagueCode: string) : Observable<Player[]>
  {
    // /api/:LeagueCode/players
    return this.http.get<Player[]>(environment.pickemRestServerBaseUrl + "/" + leagueCode + "/players", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.length}) players`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }

  private readLeagueWeeks(leagueCode: string) : Observable<LeagueWeeks>
  {
    // /api/:LeagueCode/weeks
    return this.http.get<LeagueWeeks>(environment.pickemRestServerBaseUrl + "/" + leagueCode + "/weeks", httpOptions)
      .pipe(
        tap(response => this.logger.debug(`read (${response.weekNumbers.length}) weeks`)),
        catchError(error => { return throwError(this.logger.logAndParseHttpError(error)); } )
      );
  }

  public readWeekScoreboard(leagueCode: string, weekNumber: number): Observable<WeekScoreboard>
  {
    // /api/{LeagueCode}/{WeekNumber}/scoreboard
    return this.http.get<WeekScoreboard>(environment.pickemRestServerBaseUrl + "/" + leagueCode + "/" + weekNumber + "/scoreboard", httpOptions)
      .pipe(
        tap(response => 
          {
            this.logger.debug(`read week (${weekNumber}) scoreboard for league (${leagueCode})`);
          }),
        catchError(error => 
          { 
            this.weekScoreboard = null;
            return throwError(this.logger.logAndParseHttpError(error)); 
          })
      );
  }

  public setPlayerPick(leagueCode: string, weekNumber: number, playerTag: string, gameId: number, pick: PickTypes)
  {
    var playerPickUpdate = new PlayerPickUpdate();

    playerPickUpdate.pick = pick;

    // /api/{LeagueCode}/{WeekNumber}/{PlayerTag}/scoreboard/{GameId}/pick
    return this.http.put<PlayerPick>(
        environment.pickemRestServerBaseUrl + "/" + leagueCode + "/" + weekNumber + "/" + playerTag + "/scoreboard/" + gameId + "/pick", 
        playerPickUpdate, 
        httpOptions
      )
      .subscribe(
        response => 
          {
            this.playerScoreboard.gamesPending = response.gamesPending;
            this.playerScoreboard.gamesPicked = response.gamesPicked;
            // find same game
            var gamePickScoreboard = this.playerScoreboard.gamePickScoreboards.find(psp => psp.gameId == gameId);
            var pickScoreboard = gamePickScoreboard.pickScoreboards.find(ps => ps.playerTag == playerTag);
            pickScoreboard.pick = response.pick;
          },
        errors => 
          { 
            this.playerScoreboard = new PlayerScoreboard();
            return throwError(this.logger.logAndParseHttpError(errors));
          }  
      );
     
  }
}
