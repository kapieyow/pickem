import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { of } from 'rxjs/observable/of';
import { catchError, map, tap } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

import { UserCredentials } from '../models/api/user-credentials';
import { LoggerService } from './logger.service';
import { environment } from '../../../environments/environment';
import { LeagueService } from './league.service';
import { StatusService } from './status.service';
import { UserRegistration } from '../models/api/user-registration';
import { UserLoggedIn } from '../models/api/user-logged-in';
import { User } from '../models/api/user';


//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};


@Injectable()
  // TODO: Should this service be broken out into "authentication/login" and "user like registration" like the server API is?
export class UserService {

  constructor(private logger: LoggerService, private http: HttpClient, private statusService: StatusService, private leagueService: LeagueService) { }

  login (username: string, password: string) : Observable<UserLoggedIn>
  {
    this.statusService.userLoggedInAndInitialized = false;

    let credentials = new UserCredentials();
    credentials.userName = username;
    credentials.password = password;

    return this.http.post<UserLoggedIn>(environment.pickemRestServerBaseUrl + "/auth/login", credentials, httpOptions)
      .pipe(
        tap(response => 
          {
            let token = response.token;
            localStorage.setItem("JWT", token);
            this.statusService.userName = username;
            
            if ( response.defaultLeagueCode )
            {
              this.statusService.leagueCode = response.defaultLeagueCode;
            }
            else
            {
              // TODO: unhack this
              this.statusService.leagueCode = "Default";
            }
          }),
        catchError(error =>
          {
            if ( error.status == 401 )
            {
              // not authorized, not a error, bad user or password
              return throwError(["User name and password are not valid"]);
            }

            // some other error throw it!
            return throwError(this.logger.logAndParseHttpError(error));
          })
      );
  }

  setupUser (userName: string) : Observable<any>
  {
    return this.leagueService.readPlayer(this.statusService.seasonCode, this.statusService.leagueCode, userName)
    .pipe(
      tap(response => 
        {
          this.statusService.userPlayerTag = response.playerTag
          this.statusService.playerTagFilter = response.playerTag;
        }),
      catchError(error =>
        {
          return throwError(this.logger.logAndParseHttpError(error));
        })
    );
  }

  getUsersPlayerTag(userName: string) : string
  {
    var playerTag = "";
    this.leagueService.readPlayer(this.statusService.seasonCode, this.statusService.leagueCode, userName)
      .subscribe(
        response => { 
          playerTag = response.playerTag;
        },
        errors => { 
          return throwError(this.logger.logAndParseHttpError(errors)); 
        }
      );

    return playerTag;
  }

  logout ()
  {
    localStorage.removeItem("JWT");
    this.statusService.userLoggedInAndInitialized = false;
  }

  register (userName: string, password: string, email: string, defaultLeagueCode: string) : Observable<User>
  {
    let userRegistration = new UserRegistration();
    userRegistration.defaultLeagueCode = defaultLeagueCode;
    userRegistration.email = email;
    userRegistration.password = password;
    userRegistration.userName = userName;

    return this.http.post<User>(environment.pickemRestServerBaseUrl + "/useraccounts", userRegistration, httpOptions)
      .pipe(
        tap(response => this.logger.info(`User (${response.userName}) registered.`)),
        catchError(error =>
          {
            return throwError(this.logger.logAndParseHttpError(error));
          })
      );
  }
}
