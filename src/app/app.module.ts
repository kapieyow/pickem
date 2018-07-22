import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';

import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { CollapseModule } from 'ngx-bootstrap/collapse'
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { JwtHelper } from '../../node_modules/angular2-jwt';

import { AppRoutingModule } from './app-routing/app-routing.module';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { TopNavComponent } from './top-nav/top-nav.component';
import { PlayerComponent } from './player/player.component';
import { WeekComponent } from './week/week.component';
import { SeasonComponent } from './season/season.component';
import { TestDriverComponent } from './test-driver/test-driver.component';

import { AuthGuard } from './sub-system/auth/auth-guard';
import { LoggerService } from './sub-system/services/logger.service';
import { StatusService } from './sub-system/services/status.service';
import { UserService } from './sub-system/services/user.service';



@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    TopNavComponent,
    PlayerComponent,
    WeekComponent,
    SeasonComponent,
    TestDriverComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BsDropdownModule.forRoot(),
    ButtonsModule.forRoot(),
    CollapseModule.forRoot(),
    HttpClientModule,
    ModalModule.forRoot(),
    TabsModule.forRoot(),
  ],
  providers: [ AuthGuard, JwtHelper, LoggerService, StatusService, UserService ],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
