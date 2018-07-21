import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UserService } from '../sub-system/services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private router: Router, private userService: UserService) { }

  inputsInvalid: boolean = false;

  ngOnInit() {
  }

  tryLogin(username: string, password: string) {

    this.inputsInvalid = false;

    this.userService.login(username, password)
      .subscribe(success => 
        { 
          this.inputsInvalid = !success;
          if ( success ) {
            this.router.navigate(['/player'], { skipLocationChange: true });
          }
        });

  }
}
