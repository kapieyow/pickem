import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import { LeagueService } from '../sub-system/services/league.service';
import { StatusService } from '../sub-system/services/status.service';
import { UserService } from '../sub-system/services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private router: Router, private leagueService: LeagueService, private userService: UserService, private statusService: StatusService) { }

  inputsInvalid: boolean;
  loginErrors: string[] = [];

  ngOnInit() {
  }

  async tryLogin(username: string, password: string) 
  {
    // trim inputs
    username = username.trim();
    password = password.trim();

    this.inputsInvalid = false;

    try
    {
      var userLoggedIn = await this.userService.login(username, password).toPromise();
    }
    catch (errors)
    {
      this.inputsInvalid = true; 
      this.loginErrors = errors;

      return;
    }

    await this.leagueService.loadLeagueMetaData(this.statusService.leagueCode, userLoggedIn.userName);

    this.leagueService.loadPlayerScoreboard(
      this.statusService.leagueCode, 
      this.statusService.weekNumberFilter,
      this.statusService.playerTagFilter);

    this.leagueService.loadWeekScoreboard(
      this.statusService.leagueCode, 
      this.statusService.weekNumberFilter
      );

    this.leagueService.loadLeagueScoreboard(
      this.statusService.leagueCode
      );

    // user fully setup go to player view
    this.statusService.userLoggedInAndInitialized = true;
    this.router.navigate(['/player'], { skipLocationChange: true });   
    
  } 
}
