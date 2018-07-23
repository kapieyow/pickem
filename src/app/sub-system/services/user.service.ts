import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { of } from 'rxjs/observable/of';
import { catchError, map, tap } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';

import { UserCredentials } from '../models/api/user-credentials';
import { LoggerService } from './logger.service';
import { environment } from '../../../environments/environment';
import { StatusService } from './status.service';
import { UserRegistration } from '../models/api/user-registration';
import { User } from '../models/api/user';


//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};


@Injectable()
  // TODO: Should this service be broken out into "authentication/login" and "user like registration" like the server API is?
export class UserService {

  constructor(private logger: LoggerService, private http: HttpClient, private statusService: StatusService) { }

  login (username: string, password: string) : Observable<any>
  {
    this.statusService.userLoggedIn = false;

    let credentials = new UserCredentials();
    credentials.userName = username;
    credentials.password = password;

    return this.http.post<UserCredentials>(environment.pickemRestServerBaseUrl + "/auth/login", credentials, httpOptions)
      .pipe(
        tap(response => 
          {
            let token = (<any>response).token;
            localStorage.setItem("JWT", token);
            this.statusService.userName = username;
            this.statusService.userLoggedIn = true;
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
    localStorage.removeItem("JWT");
    this.statusService.userLoggedIn = false;
  }

  register (userName: string, password: string, email: string) : Observable<User>
  {
    let userRegistration = new UserRegistration();
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
