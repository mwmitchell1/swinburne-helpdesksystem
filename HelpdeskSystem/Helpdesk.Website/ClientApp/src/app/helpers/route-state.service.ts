import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { HelpdeskDataService } from '../helpdesk-data/helpdesk-data.service';

@Injectable()
export class RouteStateService {
  adminRouteChange: Subject<boolean> = new Subject<boolean>();

  constructor(private router: Router, helpdesks: HelpdeskDataService) {
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        const url = event.url.split('/').splice(1);

        switch (url.length) {
          case 0:
            helpdesks.clearActiveHelpdesk();
            this.adminRouteChange.next(false);
            break;
          case 1:
            if (!this.isAdmin(url[0])) { helpdesks.setActiveHelpdesk(url[0]); }
            this.adminRouteChange.next(false);
            break;
          case 2:
          case 3: {
            if (this.isAdmin(url[1])) {
              this.adminRouteChange.next(true);
              helpdesks.setActiveHelpdesk(url[0]);
            }
          }
        }

      }
    });
  }

  /**
   * Check if string equals 'admin'
   * @param route String to check
   */
  isAdmin(route: string): boolean {
    return (route.toLowerCase() === 'admin');
  }


}


