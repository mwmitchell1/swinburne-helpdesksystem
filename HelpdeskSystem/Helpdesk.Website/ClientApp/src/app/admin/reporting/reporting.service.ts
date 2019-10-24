import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
@Injectable({ providedIn: 'root' })
/**
 * Used to call the API for reporting
 */
export class ReportingService {
  constructor(private http: HttpClient) { }

  /**
   * Used to call the API to get a full database export
   */
  exportDatabase() {
    return this.http.get('/api/exportdatabase', { responseType: 'blob' });
  }
}
