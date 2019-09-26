import { Component } from '@angular/core';

import { Unit } from './unit.model';
import { UnitsService } from './units.service';

@Component({
  selector: 'app-admin-units',
  templateUrl: './units.component.html'
})
export class UnitsComponent {

  private units: Unit[];

  constructor(private unitsService: UnitsService) {

    this.updateUnitsList();


  }


  updateUnitsList() {
    this.unitsService.getUnits().subscribe(
      result => {
        console.log('result', result);
        this.units = result.units;
      }, error => {
        console.log('error', error);
      }
    );
  }



}
