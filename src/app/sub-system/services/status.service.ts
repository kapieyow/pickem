import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class StatusService {

  public seasonCode: string;
  public leagueCode: string;
  public weekNumberFilter: number;
  public playerTagFilter: string;

  // TODO: this is probably terrible. Some other flag to show when active
  public userLoggedIn: boolean;
  public userName: string;
  public userPlayerTag: string;

  constructor() {
    this.userLoggedIn = false;

    // TODO: make work for other seasons
    this.seasonCode = "18";
    // TODO: tighter league handling
    this.leagueCode = "Default";
    // TODO: do better
    this.weekNumberFilter = 1;

  }

  

}
