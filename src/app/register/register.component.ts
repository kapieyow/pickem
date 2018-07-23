import { Component, OnInit } from '@angular/core';
import { UserService } from '../sub-system/services/user.service';
import { Router } from '@angular/router';
import { LoggerService } from '../sub-system/services/logger.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private router: Router, private userService: UserService, private logger: LoggerService) { }

  inputsInvalid: boolean = false;
  registrationErrors: string[];

  ngOnInit() {
  }

  tryRegistration(userName: string, password: string, email: string) {

    this.inputsInvalid = false;

    this.userService.register(userName, password, email)
      .subscribe(
        result => 
        { 
            this.inputsInvalid = false;
            this.router.navigate(['/'], { skipLocationChange: true });
        },
        errors => 
          {
            this.inputsInvalid = true;
            this.registrationErrors = errors;
          }
        );

  }

}
