import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

import { Helpdesk } from './helpdesk-data.model';

@Injectable()
export class HelpdeskDataService {
  private helpdesks: Helpdesk[];
  private activeHelpdesk: Helpdesk;

  activeHelpdeskChange: Subject<Helpdesk> = new Subject<Helpdesk>();

  constructor() {
    // temporary - array will be filled out by API call
    this.helpdesks = [
      new Helpdesk({ id: 1, name: 'Helpdesk 1', hasCheckIn: false, hasQueue: true }),
      new Helpdesk({ id: 2, name: 'Helpdesk 2', hasCheckIn: true, hasQueue: false }),
      new Helpdesk({ id: 3, name: 'Helpdesk 3', hasCheckIn: true, hasQueue: true }),
    ];

    this.activeHelpdesk = this.helpdesks[0];
  }


  /**
   * Returns all helpdesks
   * @return Helpdesk[]
   */
  getHelpdesks(): Helpdesk[] {
    return this.helpdesks;
  }

  /**
   * Returns a specific helpdesk
   * @param name Normalized name of the helpdesk to return
   * @return Helpdesk Matching helpdesk
   * @return null If no helpdesks match
   */
  getHelpdeskByName(name: string): Helpdesk {
    let h = null;

    this.helpdesks.forEach((helpdesk) => {
      if (helpdesk.normalizedName.toLowerCase() === name.toLowerCase()) {
        h = helpdesk;
      }
    });

    return h;
  }

  /**
   * Set the active helpdesk
   * @param name Normalized name of the active helpdesk
   * @return boolean True if helpdesk exists and can be set as active, otherwise False
   */
  setActiveHelpdesk(name: string): boolean {
    const helpdesk = this.getHelpdeskByName(name);

    // if helpdesk was found by name, set active, return bool value of success
    if (helpdesk) {
      this.activeHelpdesk = helpdesk;
      this.activeHelpdeskChange.next(this.activeHelpdesk);
      return true;
    } else { return false; }
  }

  /**
   * Returns the active helpdesk
   * @return Helpdesk
   */
  getActiveHelpdesk(): Helpdesk {
    return this.activeHelpdesk;
  }

  /**
   * Clears the current active helpdesk
   */
  clearActiveHelpdesk(): void {
    this.activeHelpdesk = null;
    this.activeHelpdeskChange.next(null);
  }

  /**
   * Check if a helpdesk is active
   * @return boolean
   */
  helpdeskIsActive(): boolean {
    if (this.activeHelpdesk) { return true; } else { return false; }
  }

}
