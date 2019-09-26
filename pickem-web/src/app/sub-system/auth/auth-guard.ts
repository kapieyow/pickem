import { JwtHelperService } from '@auth0/angular-jwt';
import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';

import { LoggerService } from '../services/logger.service';

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private jwtHelper: JwtHelperService, private router: Router, private logger: LoggerService) {
  }
  canActivate() {
    var token = localStorage.getItem("JWT");

    if ( token ) {
      if ( !this.jwtHelper.isTokenExpired(token) ) {
        return true;
      }
      else {
        this.logger.warn("JWT expired. Rerouting to login.")
      }
    }
    else
    {
      this.logger.warn("No JWT present. Rerouting to login.")
    }
    this.router.navigate(['/'], { skipLocationChange: true });
    return false;
  }
}