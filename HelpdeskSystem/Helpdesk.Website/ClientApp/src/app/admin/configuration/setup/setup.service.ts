import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { GetHelpdeskResponse } from "src/app/data/responses/configuration/get-response";
import { Observable } from "rxjs/internal/Observable";
import { UpdateHelpdeskRequest } from "src/app/data/requests/configuration/update-request";
import { UpdateHelpdeskResponse } from "src/app/data/responses/configuration/update-response";

@Injectable({
  providedIn: 'root',
})
export class SetUpService {
  constructor(private http: HttpClient) { }

    GetHelpdesk(id: number): Observable<GetHelpdeskResponse> {
        return this.http.get<GetHelpdeskResponse>('/api/helpdesk/' + id);
    }

    UpdateHelpdesk(id: number, request: UpdateHelpdeskRequest)
    {
      return this.http.patch<UpdateHelpdeskResponse>('/api/helpdesk/' + id, request);
    }

  /**
   * HTTP request to create a helpdesk
   * @param helpdesk
   */
  createHelpdesk(helpdesk: UpdateHelpdeskRequest) {
    return this.http.post<UpdateHelpdeskRequest>('api/helpdesk', helpdesk);
  }
}
