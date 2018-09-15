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

  tryLogin(username: string, password: string) {

    this.inputsInvalid = false;

    // TODO: oof. this is rough, nested... so all will return before going to player
    // probably should do async awaits and change the calls to return Promises with no data etc.
    this.userService.login(username, password).subscribe(response => 
      {
        // result will be true if succesful. If false is 401, bad pwd. All other issues are thrown.
        this.inputsInvalid = false;
        
        this.userService.setupUser(username).subscribe(response => 
          {
              this.leagueService.loadPlayers(this.statusService.seasonCode, this.statusService.leagueCode).subscribe(response => 
                {     
                    this.leagueService.loadWeeks(this.statusService.seasonCode, this.statusService.leagueCode).subscribe(response => 
                      { 
                        this.leagueService.loadPlayerScoreboard(
                          this.statusService.seasonCode, 
                          this.statusService.leagueCode, 
                          this.statusService.weekNumberFilter,
                          this.statusService.playerTagFilter);

                        this.leagueService.loadWeekScoreboard(
                          this.statusService.seasonCode, 
                          this.statusService.leagueCode, 
                          this.statusService.weekNumberFilter
                          );

                        this.leagueService.loadLeagueScoreboard(
                          this.statusService.seasonCode, 
                          this.statusService.leagueCode
                          );

                        // user fully setup go to player view
                        this.statusService.userLoggedInAndInitialized = true;
                        this.router.navigate(['/player'], { skipLocationChange: true });   
                      },
                      errors => { this.inputsInvalid = true; this.loginErrors = errors; }
                    );
                },
                errors => { this.inputsInvalid = true; this.loginErrors = errors; }
              );
            },
            errors => { this.inputsInvalid = true; this.loginErrors = errors; }
          );
      },
      errors => { this.inputsInvalid = true; this.loginErrors = errors; }
    );

  }
}
