import { Component } from '@angular/core';
import { Helpdesk } from '../data/DTOs/helpdesk.dto';
import { HelpdeskService } from '../helpdesk/helpdesk.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})

export class AdminComponent {
  public helpdesks: Helpdesk[];

  constructor(private helpdeskService: HelpdeskService, private notifier: NotifierService) {
    helpdeskService.getActiveHelpdesks().subscribe(
      result => {
        this.helpdesks = result.helpdesks;
      },
      error => {
        this.notifier.notify('error', "Unable to load dashboard, please contact administrators");
      }
    );
  }
}