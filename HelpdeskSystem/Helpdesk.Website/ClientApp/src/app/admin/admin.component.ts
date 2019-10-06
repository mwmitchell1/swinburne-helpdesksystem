import { Component } from '@angular/core';
import { Helpdesk } from '../data/DTOs/helpdesk.dto';
import { HelpdeskService } from '../helpdesk/helpdesk.service';
import { NotifierService } from 'angular-notifier';
import { UpdateHelpdeskRequest } from '../data/requests/configuration/update-request';
import { UnitsService } from './configuration/units/units.service';
import {SetUpService} from "./configuration/setup/setup.service";

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})

export class AdminComponent {
  public helpdesks: Helpdesk[];
  private createRequest: UpdateHelpdeskRequest = new UpdateHelpdeskRequest();

  constructor(private helpdeskService: HelpdeskService, private notifier: NotifierService, private setupService: SetUpService) {
    helpdeskService.getActiveHelpdesks().subscribe(
      result => {
        this.helpdesks = result.helpdesks;
      },
      error => {
        this.notifier.notify('error', "Unable to load dashboard, please contact administrators");
      }
    );
  }


  createHelpdesk(form) {

    this.setupService.createHelpdesk(this.createRequest).subscribe(
      result => {
        console.log(result);
      }, error => {
        console.log(error);
      }
    );

  }
}
