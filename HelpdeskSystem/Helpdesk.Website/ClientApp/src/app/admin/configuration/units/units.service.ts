import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Unit } from '../../../data/DTOs/unit.dto';
import {GetUnitsByHelpdeskIdResponse} from '../../../data/responses/units/get-by-help-id.response';

@Injectable()
export class UnitsService {
  constructor(private http: HttpClient) { }

  /**
   * HTTP request to get active helpdesk's units
   * @param id The id of the helpdesk to get units for
   */
  getUnitsByHelpdeskId(id: number) {
    return this.http.get<GetUnitsByHelpdeskIdResponse>('/api/units/helpdesk/' + id);
  }

}
