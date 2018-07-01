import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  tryLogin(email: string, password: string) {
    // TODO : really try to login
    this.router.navigate(['/player'], { skipLocationChange: true });
  }

}
