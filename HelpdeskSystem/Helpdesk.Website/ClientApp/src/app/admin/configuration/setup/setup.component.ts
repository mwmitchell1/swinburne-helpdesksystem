import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RoutesRecognized } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { SetUpService } from './setup.service';
import { UpdateHelpdeskRequest } from 'src/app/data/requests/configuration/update-request';
import { Helpdesk } from '../../../data/DTOs/helpdesk.dto';

@Component({
  selector: 'app-admin-setup',
  templateUrl: './setup.component.html'
})
/**
 * This component is used to perform CRUD functions and UI logic for the set up page
 */
export class SetUpComponent implements OnInit {

  private id;
  public helpdesk: Helpdesk;

  constructor(private configService: SetUpService,
              private route: ActivatedRoute,
              private notifier: NotifierService) {

    this.helpdesk = new Helpdesk();
  }

  ngOnInit() {
    this.route.parent.params.subscribe(params => {
      this.id = +params['id'];
    });

    this.configService.GetHelpdesk(this.id).subscribe(
      result => {
        this.helpdesk = result.helpdesk;
      },
      error => {
        this.notifier.notify('error', 'Unable to retrieve helpdesk configuration, please contact admin');
      }
    );
  }

  /**
   * Validates form data and sends request to update helpdesk settings
   * @param form ngForm passed from component
   */
  updateHelpdesk(form) {
    let allowSubmit = true;
    // Assign form data to request object
    const updateRequest: UpdateHelpdeskRequest = this.helpdesk;

    console.log(updateRequest);

    // Check if name has been touched - do not allow submit - required to show invalid msg on submit
    if (!updateRequest.hasOwnProperty('name')) {
      form.controls['settings-name'].markAsDirty();
      allowSubmit = false;
    }

    // Checkbox validation
    if (!updateRequest.isDisabled && !updateRequest.hasCheckIn && !updateRequest.hasQueue) {
      allowSubmit = false;
    }

    // If not allowed to submit, end function
    if (!allowSubmit) { return; }

    // Allowed to submit - send request
    this.configService.updateHelpdesk(this.id, updateRequest).subscribe(
      result => {
        this.notifier.notify('success', 'Helpdesk edited successfully!');
      }, error => {
        console.log(error);
        this.notifier.notify('error', 'Could not edit helpdesk, please contact helpdesk admin.');
      }
    );
  }

  /**
   * Used to clear all check ins and queue items for the helpdesk
   */
  ClearHelpdesk() {
    this.configService.ClearHelpdesk(this.id).subscribe(
      result => {
        this.notifier.notify('success', 'Helpdesk cleared');
      },
      error => {
        this.notifier.notify('error', 'Unable to clear helpdesk please contact admin');
      }
    );
  }
}

