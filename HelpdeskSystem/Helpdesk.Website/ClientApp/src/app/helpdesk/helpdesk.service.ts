import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

import { Helpdesk } from '../data/DTOs/helpdesk.dto';
import { HttpClient } from '@angular/common/http';
import { GetHelpdesksResponse } from '../data/responses/helpdesk/get-all-response';
import { GetHelpdeskResponse } from '../data/responses/configuration/get-response';

@Injectable()
export class HelpdeskService {
  private helpdesks: Helpdesk[];
  private activeHelpdesk: Helpdesk;

  activeHelpdeskChange: Subject<Helpdesk> = new Subject<Helpdesk>();

  constructor(private client: HttpClient) {
    // temporary - array will be filled out by API call
    this.helpdesks = [];

    this.activeHelpdesk = this.helpdesks[0];
  }

  /**
   * Returns all helpdesks
   * @return Helpdesk[]
   */
  getHelpdesks() {
    return this.client.get<GetHelpdesksResponse>("/api/helpdesk/")
  }

  /**
   * Returns all active helpdesks
   * @return Helpdesk[]
   */
  getActiveHelpdesks() {
    return this.client.get<GetHelpdesksResponse>("/api/helpdesk/active")
  }

  /**
   * Returns a specific helpdesk
   * @param id Unique Id of the helpdesk to return
   * @return Helpdesk Matching helpdesk
   * @return null If no helpdesks match
   */
  getHelpdeskById(id: number) {
    return this.client.get<GetHelpdeskResponse>("/api/helpdesk/" + id);
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
