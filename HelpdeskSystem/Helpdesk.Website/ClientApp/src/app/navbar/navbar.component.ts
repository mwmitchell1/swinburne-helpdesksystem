import { Component } from '@angular/core';
import { AuthenticationService } from '../authentication/authentication.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
  // styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  isExpanded = false;
  public authenticationService: AuthenticationService;
  public userIsAuthorized: boolean;

  public selectedHelpdesk: object;

  private helpdesks = [
    {
      id: '01',
      name: 'helpdesk-1'
    },
    {
      id: '02',
      name: 'helpdesk-2'
    },
    {
      id: '03',
      name: 'helpdesk-3'
    }
  ];

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

  /**
   * This function sets the currently selected helpdesk
   * @param selected The name of the selected helpdesk
   */
  setSelectedHelpdesk(selected: string) {
    this.helpdesks.forEach((helpdesk) => {
      if (helpdesk.name.toLowerCase() === selected.toLowerCase()) {
        this.selectedHelpdesk = helpdesk;
        return;
      }
    });
  }

  /**
   * This function checks if a helpdesk is currently selected
   * @return boolean
   */
  helpdeskIsSelected(): boolean {
    return !(typeof this.selectedHelpdesk === 'undefined');
  }

}
