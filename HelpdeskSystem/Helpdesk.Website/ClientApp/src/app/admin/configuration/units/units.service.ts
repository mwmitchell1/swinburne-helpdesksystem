import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Unit } from '../../../data/DTOs/unit.dto';

@Injectable()
export class UnitsService {
  constructor(private http: HttpClient) { }

  /**
   * HTTP request to get active helpdesk's units
   */
  // getUnits() {
  //   return this.http.get<Unit>('/api/units/helpdesk/1');
  // }

}
