import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
@Injectable({ providedIn: 'root' })
export class ReportingService {
  constructor(private http: HttpClient) { }
  exportDatabase() {
    return this.http.get('/api/exportdatabase', { responseType: 'blob' });
  }
}
