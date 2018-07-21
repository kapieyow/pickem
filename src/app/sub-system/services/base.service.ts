import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { LoggerService } from './logger.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable()
export abstract class BaseService {

  constructor(protected logger: LoggerService) { }

  /**
  * Handle Http operation that failed.
  * Let the app continue.
  * @param operation - name of the operation that failed
  * @param result - optional value to return as the observable result
  */
  protected handleHttpError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: better job of transforming error for user consumption
      var errorMessage = this.logger.buildErrorMessage(error);

      this.logger.error(`Operation (${operation}) failed: Message (${errorMessage})`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}
