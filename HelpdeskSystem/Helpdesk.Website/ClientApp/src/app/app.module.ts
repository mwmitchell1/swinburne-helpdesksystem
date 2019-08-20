import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

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
import { ConfigurationComponent } from './admin/configuration/configuration.component';

import { HelpdeskDataService } from './helpdesk-data/helpdesk-data.service';
import { RouteStateService } from './helpers/route-state.service';
import { UnitsComponent } from './admin/units/units.component';
import { UsersComponent } from './admin/users/users.component';
import { ReportingComponent } from './admin/reporting/reporting.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HomeComponent,
    LoginComponent,
    LogoutComponent,
    AdminComponent,
    ConfigurationComponent,
    UnitsComponent,
    UsersComponent,
    ReportingComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'login', component: LoginComponent, pathMatch: 'full' },
      { path: 'logout', component: LogoutComponent, pathMatch: 'full' },
      { path: 'admin', component: AdminComponent, pathMatch: 'full' },
      { path: ':helpdesk', component: HomeComponent, pathMatch: 'full' },
      { path: ':helpdesk/admin', component: AdminComponent,
        children: [
          { path: '', redirectTo: 'configuration', pathMatch: 'full' },
          { path: 'configuration', component: ConfigurationComponent, pathMatch: 'full' },
          { path: 'units', component: UnitsComponent, pathMatch: 'full' },
          { path: 'users', component: UsersComponent, pathMatch: 'full' },
          { path: 'reporting', component: ReportingComponent, pathMatch: 'full' }
        ]
      },
    ])
  ],
  providers: [CookieService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    AuthGuardService,
    AuthenticationService,
    HelpdeskDataService,
    RouteStateService],
  bootstrap: [AppComponent]
})
export class AppModule { }
