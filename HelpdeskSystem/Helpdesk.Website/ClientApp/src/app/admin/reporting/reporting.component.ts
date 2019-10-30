import { Component } from '@angular/core';
import { NotifierService } from 'angular-notifier';
import { DatePipe } from '@angular/common';
import { ReportingService } from './reporting.service';

@Component({
  selector: 'app-admin-reporting',
  templateUrl: './reporting.component.html'
})
/**
 * Used to handle UI functionality for reporting
 */
export class ReportingComponent {

  constructor (private service: ReportingService, private notifier: NotifierService) {

  }

  /**
   * Used to get a full database copy as a ZIP file of CSVs
   */
  ExportDatabase() {
    this.service.exportDatabase().subscribe(
      result => {
        const blob = new Blob([result], {
          type: 'application/zip',
        });

        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.setAttribute('href', url);

        const pipe: DatePipe = new DatePipe('en-AU');
        const now = new Date();

        link.setAttribute('download', 'database_export_' + pipe.transform(now, 'yyyyMMdd_hhmm') + '.zip');
        link.style.display = 'none';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      },
      error => {
          this.notifier.notify('error', 'Unable to export database, please contact administrator');
      }
    );
  }
}
