import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from '../login/login.component';
import { PlayerComponent } from '../player/player.component';
import { SeasonComponent } from '../season/season.component';
import { WeekComponent } from '../week/week.component';
import { TestDriverComponent } from '../test-driver/test-driver.component';

import { AuthGuard } from '../sub-system/auth/auth-guard';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'player', component: PlayerComponent, canActivate: [AuthGuard] },
  { path: 'season', component: SeasonComponent, canActivate: [AuthGuard] },
  { path: 'week', component: WeekComponent, canActivate: [AuthGuard]  },
  { path: 'testdriver', component: TestDriverComponent  },
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }
