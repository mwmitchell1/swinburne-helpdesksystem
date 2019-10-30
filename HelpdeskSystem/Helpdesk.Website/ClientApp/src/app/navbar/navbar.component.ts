import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService } from '../authentication/authentication.service';

import { HelpdeskService } from '../helpdesk/helpdesk.service';
import { RouteStateService } from '../helpers/route-state.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
/**
 * Used to handle the UI logic of the main nav bar
 */
export class NavbarComponent implements OnInit, OnDestroy {
  isExpanded = false;
  public authenticationService: AuthenticationService;
  public userIsAuthorized: boolean;

  private dropdownLabel: string;
  private dropdownPrefix: string;
  private helpdeskLink: string;
  private adminLink: string;
  private logoutLink: string;
  public isAdminRoute: boolean;

  private activeHelpdeskSub;
  private adminRouteSub;

  constructor(private service: AuthenticationService,
              private helpdeskData: HelpdeskService,
              private routeState: RouteStateService) {
    this.authenticationService = service;
    this.userIsAuthorized = this.authenticationService.isLoggedIn();

    this.dropdownLabel = 'No helpdesk selected';
    this.dropdownPrefix = 'helpdesk';
    this.adminLink = 'admin';
    this.logoutLink = 'logout';
  }

  ngOnInit(): void {
    this.adminRouteSub = this.routeState.adminRouteChange.subscribe((isAdmin) => {
      this.isAdminRoute = isAdmin;
      this.dropdownPrefix = isAdmin ? 'admin' : 'helpdesk';
    });
  }

  ngOnDestroy(): void {
    this.activeHelpdeskSub.unsubscribe();
    this.adminRouteSub.unsubscribe();
  }

}
