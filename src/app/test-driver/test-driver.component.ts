import { Component, OnInit } from '@angular/core';
import { LoggerService } from '../sub-system/services/logger.service';
import { Log } from '../sub-system/models/api/log';

@Component({
  selector: 'app-test-driver',
  templateUrl: './test-driver.component.html',
  styleUrls: ['./test-driver.component.css']
})
export class TestDriverComponent implements OnInit {

  constructor(private logger: LoggerService) { }

  ngOnInit() {
  }

  debug(logMessage: string) { this.logger.debug(logMessage); }
  info(logMessage: string) { this.logger.info(logMessage); }
  warn(logMessage: string) { this.logger.warn(logMessage); }
  error(logMessage: string) { this.logger.error(logMessage); }
  wtf(logMessage: string) { this.logger.wtf(logMessage); }
  

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
