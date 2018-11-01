import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { LogAdd } from '../models/api/log-add';
import { environment } from '../../../environments/environment';
import { Observable, throwError } from 'rxjs';
import { Log } from '../models/api/log';
import { tap, catchError } from 'rxjs/operators';
import { ThrowStmt, ERROR_COMPONENT_TYPE } from '@angular/compiler';

//"Content-Type", "application/json-patch+json"
const httpOptions = {
  headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable()
export class LoggerService {

  constructor(private http: HttpClient) { }

  debug(logMessage: string)
  {
    console.debug(logMessage);
    // TODO: configure this somewhere
    //  this.postToServer("Debug", logMessage);
  }

  info(logMessage: string)
  {
    console.info(logMessage);
    this.postToServer("Information", logMessage);
  }

  warn(logMessage: string)
  {
    console.warn(logMessage);
    this.postToServer("Warning", logMessage);
  }
  
  error(logMessage: string)
  {
    console.error(logMessage);
    this.postToServer("Error", logMessage);
  }

  wtf(logMessage: string)
  {
    console.error("[WTF]-> " + logMessage);
    this.postToServer("WTF", logMessage);
  }

  public logAndParseHttpError(error: HttpErrorResponse) : string[] 
  {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      var errorMessage = `[client side]: ${error.error.message}`;
      this.error(errorMessage);
      return [errorMessage];
    } 
    else if ( error.status == 400 )
    {
      // bad request / validation
      // break out the validation messages
      try 
      {
        var errorMessages: string[] = [];

        // this is a bit nutz. Trying to get the error messages out from serial json, which is an array of arrays
        var errorProperties = Object
          .keys(error.error)
          .forEach(key => errorMessages = errorMessages.concat(error.error[key]));

        var errorBodyJson = JSON.stringify(error.error);
        var errorMessage = `${error.message}: ${errorBodyJson}`;
        
        // only a validation so log as warning
        this.warn(errorMessage);
        return errorMessages;
      }
      catch (e)
      {
          // log as if unknown.
          return [ "Oops! Didn't expect this", "Unable to digest 400 error", this.logAsUnhandledHttpError(error) ];
      }
    }
    else if ( error.status == 401 )
    {
        // unauthorized
        var errorBodyJson = JSON.stringify(error.error);
        var errorMessage = `${error.message}: ${errorBodyJson}`;
        
        // only a validation so log as warning
        this.warn(errorMessage);
        return [ "You are not authorized for this action"];
    }
    else
    {
      return [ "Oops! Didn't expect this", this.logAsUnhandledHttpError(error) ];
    }
  };

  public readLogs(): Observable<Log[]>
  {
    return this.http.get<Log[]>(environment.pickemRestServerBaseUrl + "/logs", httpOptions)
      .pipe(
        tap(response => this.debug(`read (${response.length}) logs`)),
        catchError(error => 
          {
            return throwError([this.logAsUnhandledHttpError(error)]);
          })
      );
  }

  private logAsUnhandledHttpError(error: HttpErrorResponse) : string 
  {
    var errorBodyJson = JSON.stringify(error.error);
    var errorMessage = `${error.message}: ${errorBodyJson}`;
    
    this.error(errorMessage);
    return `${error.message}`;
  }

  private postToServer(logLevel: string, logMessage: string)
  {
    // attempt to post to the server. 
    let logAdd = new LogAdd();
    logAdd.component = "Pick'Em Web Client" 
    logAdd.logLevel = logLevel;
    logAdd.logMessage = logMessage;
      
    this.http.post<LogAdd>(environment.pickemRestServerBaseUrl + "/logs", logAdd, httpOptions)
      .subscribe( 
        response => { /* thumbs up */ },
        error => { 
          var errorBodyJson = JSON.stringify(error.error);
          var serverError = `${error.message}: ${errorBodyJson}`;
          console.error(`Unable to log message (${logMessage}) at level (${logLevel}) to server due to (${serverError})`); }
        );
  }

}
