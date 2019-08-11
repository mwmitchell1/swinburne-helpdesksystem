import { Component } from '@angular/core';
import { AuthenticationService } from '../authentication/authentication.service';
import { NavigationStart, Router } from '@angular/router';
import { filter } from 'rxjs/operators';


@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html'
  // styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  isExpanded = false;
  public authenticationService: AuthenticationService;
  public userIsAuthorized: boolean;
  public isAdminSection: boolean;

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

  constructor(private service: AuthenticationService, private router: Router) {
    this.authenticationService = service;
    this.userIsAuthorized = this.authenticationService.isLoggedIn();

    // Router navigation event
    router.events.pipe(
      filter(e => e instanceof NavigationStart)
    ).subscribe(e => {
      console.log(e);
      // @ts-ignore
      const url = e.url.split('/').splice(1);
      console.log(url);

      // check if first section of url is 'admin'
      this.isAdminSection = (url[0].toLowerCase() === 'admin');

      // if 3 sections and second
      if (url.length > 1 && url[1] !== '') { this.setSelectedHelpdesk(url[1]); }
    });
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
