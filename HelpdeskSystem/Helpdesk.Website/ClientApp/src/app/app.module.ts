import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NotifierModule, NotifierOptions } from 'angular-notifier'
import * as $ from 'jquery';
import * as bootstrap from 'bootstrap';

import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './authentication/login/login.component';
import { CookieService } from 'ngx-cookie-service';
import { AuthInterceptor } from './helpers/auth.inteceptor';
import { AuthGuardService } from './helpers/auth.guard.service';
import { AuthenticationService } from './authentication/authentication.service';
import { LogoutComponent } from './authentication/logout/logout.component';
import { AdminComponent } from './admin/admin.component';

import { HelpdeskService } from './helpdesk/helpdesk.service';
import { RouteStateService } from './helpers/route-state.service';

import { SetUpComponent } from './admin/configuration/setup/setup.component';
import { UnitsComponent } from './admin/configuration/units/units.component';
import { UsersComponent } from './admin/users/users.component';
import { ReportingComponent } from './admin/reporting/reporting.component';
import { NicknamesComponent } from './admin/nicknames/nicknames.component';
import { ConfigurationComponent } from './admin/configuration/configuration.component';
import { HelpdeskComponent } from './helpdesk/helpdesk.component';
import { DataTablesModule } from 'angular-datatables';

import { UsersService } from './admin/users/users.service';
import { UnitsService } from './admin/configuration/units/units.service';
import {PasswordResetComponent} from './authentication/password-reset/password-reset.component';
import {PasswordResetService} from './authentication/password-reset/password-reset.service';


@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HomeComponent,
    LoginComponent,
    LogoutComponent,
    AdminComponent,
    SetUpComponent,
    UnitsComponent,
    UsersComponent,
    NicknamesComponent,
    ReportingComponent,
    ConfigurationComponent,
    HelpdeskComponent,
    PasswordResetComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    DataTablesModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'login', component: LoginComponent, pathMatch: 'full' },
      { path: 'logout', component: LogoutComponent, pathMatch: 'full' },
      { path: 'helpdesk/:id', component: HelpdeskComponent, pathMatch: 'full' },
      {
        path: 'admin', component: AdminComponent, canActivate: [AuthGuardService],
        children: [
          { path: 'users', component: UsersComponent, pathMatch: 'full' },
          { path: 'nicknames', component: NicknamesComponent, pathMatch: 'full' },
          { path: 'reporting', component: ReportingComponent, pathMatch: 'full' },
          {
            path: ':id', component: ConfigurationComponent,
            children: [
              { path: 'setup', component: SetUpComponent, pathMatch: 'full' },
              { path: 'units', component: UnitsComponent, pathMatch: 'full' }
            ]
          }
        ]
      },

    ]),
    NotifierModule
  ],
  providers: [CookieService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    AuthGuardService,
    AuthenticationService,
    HelpdeskService,
    RouteStateService,
    UsersService,
    UnitsService,
    PasswordResetService],
  bootstrap: [AppComponent]
})
export class AppModule { }
