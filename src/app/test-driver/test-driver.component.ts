import { Component, OnInit } from '@angular/core';
import { LoggerService } from '../sub-system/services/logger.service';

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
  
}
