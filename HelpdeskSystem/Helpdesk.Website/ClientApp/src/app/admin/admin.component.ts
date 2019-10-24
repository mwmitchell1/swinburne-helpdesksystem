import { Component, ViewChild } from '@angular/core';
import { Helpdesk } from '../data/DTOs/helpdesk.dto';
import { HelpdeskService } from '../helpdesk/helpdesk.service';
import { NotifierService } from 'angular-notifier';
import { UpdateHelpdeskRequest } from '../data/requests/configuration/update-request';
import { SetUpService } from './configuration/setup/setup.service';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
/**
 * This is the component used to handle the top level of the administration UI logic
 */
export class AdminComponent {
  public helpdesks: Helpdesk[];
  public createRequest: UpdateHelpdeskRequest = new UpdateHelpdeskRequest();

  constructor(private helpdeskService: HelpdeskService, private notifier: NotifierService, private setupService: SetUpService) {
    this.getHelpdesks();
  }

  /**
   * Used to retreive all helpdesks for admin setion
   */
  getHelpdesks() {
    this.helpdeskService.getActiveHelpdesks().subscribe(
      result => {
        this.helpdesks = result.helpdesks;
      },
      error => {
        if (error.status !== 404) {
          this.notifier.notify('error', 'Unable to helpdesks, please contact administrators');
        }
      }
    );
  }

  /**
   * Used to add a new helpdesk to the system
   * @param form the information for the helpdesk from the UI form
   */
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
        this.getHelpdesks();
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
