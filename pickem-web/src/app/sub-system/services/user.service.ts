import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

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

  login (uncheckedUsername: string, password: string) : Observable<UserLoggedIn>
  {
    this.statusService.userLoggedInAndInitialized = false;

    let credentials = new UserCredentials();
    credentials.userName = uncheckedUsername;
    credentials.password = password;

    return this.http.post<UserLoggedIn>(environment.pickemRestServerBaseUrl + "/auth/login", credentials, httpOptions)
      .pipe(
        tap(response => 
          {
            let token = response.token;
            localStorage.setItem("JWT", token);
            this.statusService.userName = response.userName;
            this.statusService.userLeagues = response.leagues;
            // TODO: this has to be after userLeagues set due to dumbness in statusService 
            // where it sets the current league using userLeagues on the setter of leagueCode.
            // See issue #26 to unwind.
            this.statusService.leagueCode = response.defaultLeagueCode;
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

  logout ()
  {
    this.statusService.clearAllSessionState();
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
