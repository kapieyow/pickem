import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ButtonsModule } from 'ngx-bootstrap/buttons';
import { CollapseModule } from 'ngx-bootstrap/collapse'
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { AppRoutingModule } from './app-routing/app-routing.module';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { TopNavComponent } from './top-nav/top-nav.component';
import { PlayerComponent } from './player/player.component';
import { WeekComponent } from './week/week.component';
import { SeasonComponent } from './season/season.component';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    TopNavComponent,
    PlayerComponent,
    WeekComponent,
    SeasonComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BsDropdownModule.forRoot(),
    ButtonsModule.forRoot(),
    CollapseModule.forRoot(),
    ModalModule.forRoot(),
    TabsModule.forRoot(),
  ],
  providers: [],
  bootstrap: [ AppComponent ]
})
export class AppModule { }
