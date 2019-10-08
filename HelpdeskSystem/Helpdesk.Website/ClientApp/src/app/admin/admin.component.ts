import {Component, ViewChild} from '@angular/core';
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

    let allowSubmit = true;

    // Check if name has been touched - do not allow submit - required to show invalid msg on submit
    if (!this.createRequest.hasOwnProperty('name')) {
      form.controls['helpdesk-add-name'].markAsDirty();
      allowSubmit = false;
    }

    // Checkbox validation
    if (!this.createRequest.isDisabled && !this.createRequest.hasCheckIn && !this.createRequest.hasQueue) {
        allowSubmit = false;
    }

    // If not allowed to submit, end function
    if (!allowSubmit) { return; }

    // Allowed to submit - send request
    this.setupService.createHelpdesk(this.createRequest).subscribe(
      result => {
        $('#modal-helpdesk-add').modal('hide');
        form.reset();

        this.notifier.notify('success', 'Helpdesk added successfully!');
      }, error => {
        console.log(error);
        this.notifier.notify('error', 'Could not add new helpdesk, please contact helpdesk admin.');
      }
    );


  }
}
