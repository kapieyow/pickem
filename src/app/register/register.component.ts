import { Component, OnInit } from '@angular/core';
import { UserService } from '../sub-system/services/user.service';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { LoggerService } from '../sub-system/services/logger.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private route: ActivatedRoute, private router: Router, private userService: UserService, private logger: LoggerService) { }

  inputsInvalid: boolean = false;
  registrationErrors: string[];
  leagueCode: string;

  ngOnInit() {
    this.leagueCode = this.route.snapshot.params["leagueCode"];
  }

  tryRegistration(userName: string, password: string, email: string) {

    this.inputsInvalid = false;

    this.userService.register(userName, password, email, this.leagueCode)
      .subscribe(
        result => 
        { 
            this.inputsInvalid = false;
            this.router.navigate(['/']);
        },
        errors => 
          {
            this.inputsInvalid = true;
            this.registrationErrors = errors;
          }
        );

  }

}
