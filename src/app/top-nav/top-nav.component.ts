import { Component, OnInit } from '@angular/core';
import { StatusService } from '../sub-system/services/status.service';
import { UserService } from '../sub-system/services/user.service';
import { Router } from '../../../node_modules/@angular/router';

@Component({
  selector: 'app-top-nav',
  templateUrl: './top-nav.component.html',
  styleUrls: ['./top-nav.component.css']
})
export class TopNavComponent implements OnInit {

  constructor(public statusService: StatusService, private router: Router, private userService: UserService) { }

  ngOnInit() {
  }

  logout ()
  {
    this.userService.logout();
    this.router.navigate(['/'], { skipLocationChange: true });
  }
}
