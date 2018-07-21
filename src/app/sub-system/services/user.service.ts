import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Credentials } from '../models/credentials';
import { BaseService } from './base.service';
import { LoggerService } from './logger.service';

@Injectable()
export class UserService extends BaseService {

  constructor(logger: LoggerService, private http: HttpClient) { 
    super(logger);
  }

  login (username: string, password: string) 
  {
    let credentials = new Credentials();
    credentials.userName = username;
    credentials.password = password;


  }
}
