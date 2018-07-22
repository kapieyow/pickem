import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StatusService {

  // TODO: this is probably terrible. Some other flag to show when active
  public userLoggedIn: boolean;
  public userName: string;

  constructor() {
    this.userLoggedIn = false;
  }

  

}
