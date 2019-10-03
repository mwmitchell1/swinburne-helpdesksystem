import { Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { NotifierService } from 'angular-notifier';
import { pipe } from 'rxjs';
import { DatePipe } from '@angular/common';
import { ReportingService } from './reporting.service';

@Component({
  selector: 'app-admin-reporting',
  templateUrl: './reporting.component.html'
})
export class ReportingComponent {

  constructor (private service: ReportingService, private notifier: NotifierService) {

  }

  ExportDatabase() {
    this.service.exportDatabase().subscribe(
      result => {
        const blob = new Blob([result], {
          type: 'application/zip',
        });

        const url = window.URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.setAttribute("href", url);

        var pipe = new DatePipe('en-AU');
        var now = new Date();

        link.setAttribute("download", "database_export_" + pipe.transform(now, 'yyyyMMdd_hhmm') + ".zip");
        link.style.display = "none";
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      },
      error => {
          this.notifier.notify('error', 'Unable to export database, please contact administrator')
      }
    )
  }
}
