import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GetHelpdeskResponse } from 'src/app/data/responses/configuration/get-response';
import { Observable } from 'rxjs/internal/Observable';
import { UpdateHelpdeskRequest } from 'src/app/data/requests/configuration/update-request';
import { UpdateHelpdeskResponse } from 'src/app/data/responses/configuration/update-response';
import { ForceCheckoutQueueRemoveResponse } from 'src/app/data/responses/helpdesk/force-checkout-queue-remove-response';

@Injectable({
  providedIn: 'root',
})
/**
 * used to call the CRUD APIs for Set Up
 */
export class SetUpService {
  constructor(private http: HttpClient) { }

    /**
     * Used to call the get the helpdesk information by id api
     * @param id The id of the helpdesk
     */
    GetHelpdesk(id: number): Observable<GetHelpdeskResponse> {
        return this.http.get<GetHelpdeskResponse>('/api/helpdesk/' + id);
    }

    /**
     * Used to call the update helpdesks setings api
     * @param id The id of the helpdesk
     * @param request the settings to be saved
     */
    updateHelpdesk(id: number, request: UpdateHelpdeskRequest) {
      return this.http.patch<UpdateHelpdeskResponse>('/api/helpdesk/' + id, request);
    }


    /**
     * HTTP request to create a helpdesk
     * @param helpdesk
     */
    createHelpdesk(helpdesk: UpdateHelpdeskRequest) {
      return this.http.post<UpdateHelpdeskRequest>('api/helpdesk', helpdesk);
    }

    /**
     * Used to call the api that will check out all students and/or remove all queue
     * items for the helpdesk
     * @param id the id of the helpdesk
     */
    ClearHelpdesk(id: number) {
      return this.http.delete<ForceCheckoutQueueRemoveResponse>('/api/helpdesk/' + id + '/clear');
    }
}
