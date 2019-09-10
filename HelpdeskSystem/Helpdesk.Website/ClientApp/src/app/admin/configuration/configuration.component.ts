import { Component, OnInit } from '@angular/core';
import { Helpdesk } from 'src/app/helpdesk-data/helpdesk-data.model';
import { FormBuilder, FormControl } from '@angular/forms';
import { AuthenticationService } from 'src/app/authentication/authentication.service';
import { ActivatedRoute, Router, RoutesRecognized } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { ConfigurationService } from './configuration.service';
import { UpdateHelpdeskRequest } from 'src/app/data/requests/configuration/update-request';

@Component({
  selector: 'app-admin-configuration',
  templateUrl: './configuration.component.html'
})
export class ConfigurationComponent implements OnInit {

  private returnUrl;
  private id;
  configForm

  constructor(private builder: FormBuilder,
    private service: AuthenticationService,
    private configService: ConfigurationService,
    private route: ActivatedRoute,
    private _router: Router,
    private notifierSerive: NotifierService) {

    this.configForm = builder.group({
      name: new FormControl(),
      hasQueue: ['false'],
      hasCheck: ['false'],
      isDisabled: ['false']
    })
  }

  public helpdesk;

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
        this.notifierSerive.notify('error', 'Unable to retreive helpdesk configuration, pleae contact admin');
      }
    )
  }

  UpdateHelpdesk() {

    var data = this.configForm.value;
    var isValid: boolean = true;

    if (!data.name) {
      this.notifierSerive.notify('warning', 'You must enter in a helpdesk name.');
      isValid = false;
    }

    if (!isValid)
      return;

    var updateHelpdeskRequest = new UpdateHelpdeskRequest();
    updateHelpdeskRequest.name = data.name;
    updateHelpdeskRequest.hasCheckIn = data.hasCheck;
    updateHelpdeskRequest.hasQueue = data.hasQueue;
    updateHelpdeskRequest.isDisabled = data.isDisabled;

    this.configService.UpdateHelpdesk(this.id, updateHelpdeskRequest).subscribe(result => {
      if (result.status == 200) {
        this.notifierSerive.notify('success', 'Helpdesk updated successfully.');
      }
    },
      error => {
        this.notifierSerive.notify('error', 'Unable to update helpdesk, please contact admin.');
      });
  }
}