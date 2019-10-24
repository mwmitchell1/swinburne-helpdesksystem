import { Component } from '@angular/core';
import { HelpdeskService } from '../helpdesk/helpdesk.service';
import { Helpdesk } from '../data/DTOs/helpdesk.dto';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  public helpdesks: Helpdesk[];

  constructor(private helpdeskService: HelpdeskService, private notifier: NotifierService) {
    helpdeskService.getActiveHelpdesks().subscribe(
      result => {
        this.helpdesks = result.helpdesks;
      },
      error => {
        if (error.status !== 404) {
          this.notifier.notify('error', 'Unable to load dashboard, please contact administrators');
        }
      }
    );
  }
}
