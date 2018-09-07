import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { LeagueService } from '../sub-system/services/league.service';
import { UserService } from '../sub-system/services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private router: Router, private leagueService: LeagueService, private userService: UserService) { }

  inputsInvalid: boolean;
  loginErrors: string[] = [];

  ngOnInit() {
  }

  tryLogin(username: string, password: string) {

    this.inputsInvalid = false;

    this.userService.login(username, password)
      .subscribe(
        result => {
          // result will be true if succesful. If false is 401, bad pwd. All other issues are thrown.
          this.inputsInvalid = false;
          // chain to user setup. is async will flips screens on success.
          this.setupUser(username);
        },
        errors => {
          this.inputsInvalid = true;
          this.loginErrors = errors;
        }
      );

  }

  setupUser(username: string)
  {
    this.userService.setupUser(username)
      .subscribe(
        result => {
          this.leagueService.setupLeagueFilters();
          // user fully setup go to player view
          this.router.navigate(['/player'], { skipLocationChange: true });
        },
        errors => {
          this.inputsInvalid = true;
          this.loginErrors = errors;
        }
      );
  }
}
