import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthenticationService } from '../authentication/authentication.service';

import { HelpdeskDataService } from '../helpdesk-data/helpdesk-data.service';
import { RouteStateService } from '../helpers/route-state.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
  // styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit, OnDestroy {
  isExpanded = false;
  public authenticationService: AuthenticationService;
  public userIsAuthorized: boolean;

  private dropdownLabel: string;
  private adminLink: string;
  private isAdminRoute: boolean;

  private activeHelpdeskSub;
  private adminRouteSub;

  constructor(private service: AuthenticationService,
              private helpdeskData: HelpdeskDataService,
              private routeState: RouteStateService) {
    this.authenticationService = service;
    this.userIsAuthorized = this.authenticationService.isLoggedIn();

    this.dropdownLabel = 'No helpdesk selected';
    this.adminLink = 'admin';
  }

  ngOnInit(): void {
    this.activeHelpdeskSub = this.helpdeskData.activeHelpdeskChange.subscribe((helpdesk) => {
      this.dropdownLabel = helpdesk.name;
      // this.adminLink = helpdesk.normalizedName + '/admin';
    });

    this.adminRouteSub = this.routeState.adminRouteChange.subscribe((isAdmin) => {
      this.isAdminRoute = isAdmin;
    });
  }

  ngOnDestroy(): void {
    this.activeHelpdeskSub.destroy();
    this.adminRouteSub.destroy();
  }

}
