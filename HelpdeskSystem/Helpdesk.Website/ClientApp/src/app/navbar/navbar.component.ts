import { Component } from '@angular/core';
import { AuthenticationService } from '../authentication/authentication.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
  //styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  isExpanded = false;
  public authenticationService: AuthenticationService;
  public userIsAuthorized: boolean;

  constructor(private service: AuthenticationService) {
    this.authenticationService = service;
    this.userIsAuthorized = this.authenticationService.isLoggedIn();
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }
}
