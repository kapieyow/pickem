import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LeagueService } from '../sub-system/services/league.service';
import { LoggerService } from '../sub-system/services//logger.service';
import { StatusService } from '../sub-system/services/status.service';
import { Log } from '../sub-system/models/api/log';

@Component({
  selector: 'app-test-driver',
  templateUrl: './test-driver.component.html',
  styleUrls: ['./test-driver.component.css']
})
export class TestDriverComponent implements OnInit {

  constructor(private router: Router, private leagueService: LeagueService, private logger: LoggerService, private statusService: StatusService) { }

  ngOnInit() {
  }

  debug(logMessage: string) { this.logger.debug(logMessage); }
  info(logMessage: string) { this.logger.info(logMessage); }
  warn(logMessage: string) { this.logger.warn(logMessage); }
  error(logMessage: string) { this.logger.error(logMessage); }
  wtf(logMessage: string) { this.logger.wtf(logMessage); }
  
  changeToLeague(league: string)
  {
    this.statusService.leagueCode = league;
    
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
            errors => { this.wtf(errors); }
          );
      },
      errors => { this.wtf(errors); }
    );

  }

  logs: Log[];
  errorMessages: string[] = [];

  readLogs ()
  {
    this.logs = [];

    this.logger.readLogs()
      .subscribe(
        response => {this.logs = response},
        errors => { 
          this.errorMessages = errors;
        }
      )
  }

}
