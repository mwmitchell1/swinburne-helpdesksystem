import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import {GetUnitsByHelpdeskIdResponse} from '../../../data/responses/units/get-by-help-id.response';
import { DeleteUnitResponse } from 'src/app/data/responses/units/delete-unit..response';
import { AddUpdateUnitRequest } from 'src/app/data/requests/units/add-unit-response';
import { AddUpdateUnitResponse } from 'src/app/data/responses/units/add-update-unit-response';

/**
 * Used to call the CRUD APIs for units
 */
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

  deleteUnit(id: number) {
    return this.http.delete<DeleteUnitResponse>('/api/units/' + id);
  }

  addUpdateUnit(id: number, request: AddUpdateUnitRequest) {
    return this.http.post<AddUpdateUnitResponse>('/api/units/' + id, request);
  }
}
