import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Unit } from '../../../data/DTOs/unit.dto';
import { UnitsService } from './units.service';
import { NotifierService } from 'angular-notifier';
import { FormBuilder, FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-admin-units',
  templateUrl: './units.component.html'
})
export class UnitsComponent {

  private units: Unit[];
  public deleteForm: FormGroup = this.builder.group({
    unitId: new FormControl('')
  });

  constructor(private unitsService: UnitsService,
              private route: ActivatedRoute,
              private notifier: NotifierService,
              private builder: FormBuilder) {

    this.units = [];
    this.updateUnitsList();


  }


  /**
   * Uses HelpdeskService to get units of selected helpdesk
   */
  updateUnitsList() {
    this.unitsService.getUnitsByHelpdeskId(this.route.parent.snapshot.params.id).subscribe(
      result => {
        this.units = result.units;
      }, error => {
        this.notifier.notify('error', 'Unable to get units, please contact helpdesk admin.');
      }
    );
  }

   /**
   * Prepares hidden delete form
   * @param id Id of unit to delete
   */
  setupDelete(id: number) {
    this.deleteForm.controls.unitId.setValue(id);
  }

    /**
   * Unit method to delete unit
   * @param data Form data
   */
  deleteUnit(data) {
    this.unitsService.deleteUnit(data.unitId).subscribe(
      result => {
        if (result.status == 200) {
          this.notifier.notify('success', 'Unit deleted successfully.');
          this.updateUnitsList();
          var modal = $('#modal-user-delete').modal('hide');
        }
      },
      error => {
        if (error.status == 500) {
          this.notifier.notify('error', 'Unable to delete user please contact helpdesk admin');
        }
      });
  }

}
