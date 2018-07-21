import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { LogAdd } from '../models/log-add';
import { environment } from '../../../environments/environment';
import { basename } from 'path';

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
    this.postToServer("DEBUG", logMessage);
  }

  info(logMessage: string)
  {
    console.info(logMessage);
    this.postToServer("INFO", logMessage);
  }

  warn(logMessage: string)
  {
    console.warn(logMessage);
    this.postToServer("WARN", logMessage);
  }
  
  error(logMessage: string)
  {
    console.error(logMessage);
    this.postToServer("ERROR", logMessage);
  }

  wtf(logMessage: string)
  {
    console.error("[WTF]-> " + logMessage);
    this.postToServer("WTF", logMessage);
  }

  public buildErrorMessage(error: HttpErrorResponse) : string {

    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      return `[client side]: ${error.error.message}`
    } 
    else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      var errorBodyJson = JSON.stringify(error.error);
      return `${error.message}: ${errorBodyJson}`;
    }
  };

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
        error => { console.error(`Unable to log message (${logMessage}) at level (${logLevel}) to server due to (${this.buildErrorMessage(error)})`); }
        );
  }

}
