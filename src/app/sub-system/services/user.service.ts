import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { of } from 'rxjs/observable/of';
import { catchError, map, tap } from 'rxjs/operators';
import { Observable } from 'rxjs/Observable';

import { UserCredentials } from '../models/user-credentials';
import { BaseService } from './base.service';
import { LoggerService } from './logger.service';
import { environment } from '../../../environments/environment';
import { StatusService } from './status.service';


//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class UserService extends BaseService {

  constructor(logger: LoggerService, private http: HttpClient, private statusService: StatusService) { 
    super(logger);
  }

  login (username: string, password: string) : Observable<boolean>
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
        map(response => true),
        catchError(error =>
          {
            var httpErrorMessage = this.logger.buildErrorMessage(error);
            this.logger.warn(`Failed login for (${username}) http error (${httpErrorMessage})`)
            return of(false);
          })
      );
  }

  logout ()
  {
    localStorage.removeItem("JWT");
    this.statusService.userLoggedIn = false;
  }
}
