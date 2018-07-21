import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from '../login/login.component';
import { PlayerComponent } from '../player/player.component';
import { SeasonComponent } from '../season/season.component';
import { WeekComponent } from '../week/week.component';
import { TestDriverComponent } from '../test-driver/test-driver.component';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'player', component: PlayerComponent },
  { path: 'season', component: SeasonComponent },
  { path: 'week', component: WeekComponent },
  { path: 'testdriver', component: TestDriverComponent },
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }
