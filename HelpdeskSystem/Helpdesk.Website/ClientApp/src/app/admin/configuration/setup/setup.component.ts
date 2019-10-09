import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl } from '@angular/forms';
import { AuthenticationService } from 'src/app/authentication/authentication.service';
import { ActivatedRoute, Router, RoutesRecognized } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { SetUpService } from './setup.service';
import { UpdateHelpdeskRequest } from 'src/app/data/requests/configuration/update-request';
import { Helpdesk } from '../../../data/DTOs/helpdesk.dto';

@Component({
  selector: 'app-admin-setup',
  templateUrl: './setup.component.html'
})
export class SetUpComponent implements OnInit {

  private returnUrl;
  private id;
  configForm;

  private helpdesk: Helpdesk;

  constructor(private builder: FormBuilder,
    private service: AuthenticationService,
    private configService: SetUpService,
    private route: ActivatedRoute,
    private _router: Router,
    private notifier: NotifierService) {

    this.configForm = builder.group({
      name: new FormControl(),
      hasQueue: ['false'],
      hasCheck: ['false'],
      isDisabled: ['false']
    })

    this.helpdesk = new Helpdesk();
  }


  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.returnUrl = params.return;
    });

    this.route.parent.params.subscribe(params => {
      this.id = +params["id"];
    });

    this.configService.GetHelpdesk(this.id).subscribe(
      result => {
        this.helpdesk = result.helpdesk;

        this.configForm.patchValue({
          name: this.helpdesk.name,
          hasQueue: this.helpdesk.hasQueue.toString(),
          hasCheck: this.helpdesk.hasCheckIn.toString(),
          isDisabled: this.helpdesk.isDisabled.toString(),
        });
      },
      error => {
        this.notifier.notify('error', 'Unable to retreive helpdesk configuration, pleae contact admin');
      }
    )
  }

  // UpdateHelpdesk() {
  //
  //   var data = this.configForm.value;
  //   var isValid: boolean = true;
  //
  //   if (!data.name) {
  //     this.notifierSerive.notify('warning', 'You must enter in a helpdesk name.');
  //     isValid = false;
  //   }
  //
  //   if (!isValid)
  //     return;
  //
  //   // var updateHelpdeskRequest = new UpdateHelpdeskRequest();
  //   // updateHelpdeskRequest.name = data.name;
  //   // updateHelpdeskRequest.hasCheckIn = data.hasCheck;
  //   // updateHelpdeskRequest.hasQueue = data.hasQueue;
  //   // updateHelpdeskRequest.isDisabled = data.isDisabled;
  //
  //   this.configService.UpdateHelpdesk(this.id, updateHelpdeskRequest).subscribe(result => {
  //     if (result.status == 200) {
  //       this.notifierSerive.notify('success', 'Helpdesk updated successfully.');
  //     }
  //   },
  //     error => {
  //       this.notifierSerive.notify('error', 'Unable to update helpdesk, please contact admin.');
  //     });
  // }

  // TODO: update helpdesk list on update/create
  updateHelpdesk(form) {

    let allowSubmit = true;
    const updateRequest: UpdateHelpdeskRequest = this.helpdesk;
    // delete updateRequest.id;

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
        // $('#modal-helpdesk-add').modal('hide');
        // form.reset();

        this.notifier.notify('success', 'Helpdesk edited successfully!');
      }, error => {
        console.log(error);
        this.notifier.notify('error', 'Could not edit helpdesk, please contact helpdesk admin.');
      }
    );


  }

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

