import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Unit } from '../../../data/DTOs/unit.dto';
import { UnitsService } from './units.service';
import { HelpdeskService } from '../../../helpdesk/helpdesk.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-admin-units',
  templateUrl: './units.component.html'
})
export class UnitsComponent {

  private units: Unit[];

  constructor(private unitsService: UnitsService,
              private helpdeskService: HelpdeskService,
              private route: ActivatedRoute,
              private notifier: NotifierService) {

    this.units = [];
    this.updateUnitsList();


  }


  /**
   * Uses HelpdeskService to get units of selected helpdesk
   */
  updateUnitsList() {
    this.helpdeskService.getUnitsByHelpdeskId(this.route.parent.snapshot.params.id).subscribe(
      result => {
        this.units = result.units;
      }, error => {
        this.notifier.notify('error', 'Unable to get units, please contact helpdesk admin.');
      }
    );
  }



}
