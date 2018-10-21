import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from '../login/login.component';
import { PlayerComponent } from '../player/player.component';
import { TestDriverComponent } from '../test-driver/test-driver.component';
import { RegisterComponent } from '../register/register.component';

import { AuthGuard } from '../sub-system/auth/auth-guard';

const routes: Routes = [
  { path: '', component: LoginComponent },
  { path: 'player', component: PlayerComponent, canActivate: [AuthGuard] },
  { path: 'register/:leagueCode', component: RegisterComponent  },
  { path: 'testdriver', component: TestDriverComponent  },
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }
