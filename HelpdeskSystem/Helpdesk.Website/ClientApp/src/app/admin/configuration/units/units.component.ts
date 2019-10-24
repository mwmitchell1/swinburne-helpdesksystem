import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Unit } from '../../../data/DTOs/unit.dto';
import { UnitsService } from './units.service';
import { NotifierService } from 'angular-notifier';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { Topic } from 'src/app/data/DTOs/topic.dto';
import { AddUpdateUnitRequest } from 'src/app/data/requests/units/add-unit-response';

@Component({
  selector: 'app-admin-units',
  templateUrl: './units.component.html'
})
/**
 * This component handles the CRUD actions for Units and Topics
 */
export class UnitsComponent {
  private errorMsg: string;
  public units: Unit[];
  public newTopics: Topic[];
  public editTopics: Topic[];
  public deleteForm: FormGroup = this.builder.group({
    unitId: new FormControl('')
  });
  public addForm: FormGroup = this.builder.group({
    unitName: new FormControl('', [Validators.required]),
    unitCode: new FormControl('', [Validators.required]),
    unitTopic: new FormControl('')
  });
  public editForm: FormGroup = this.builder.group({
    editUnitId: new FormControl('', [Validators.required]),
    editUnitName: new FormControl('', [Validators.required]),
    editUnitCode: new FormControl('', [Validators.required]),
    editUnitTopic: new FormControl('')
  });

  constructor(private unitsService: UnitsService,
    private route: ActivatedRoute,
    private notifier: NotifierService,
    private builder: FormBuilder) {

    this.units = [];
    this.updateUnitsList();
    this.newTopics = [];
    this.editTopics = [];
  }


  /**
   * Uses HelpdeskService to get units of selected helpdesk
   */
  updateUnitsList() {
    this.unitsService.getUnitsByHelpdeskId(this.route.parent.snapshot.params.id).subscribe(
      result => {
        this.units = result.units;
        // console.log('units', this.units);
      }, error => {
        if (error.status === 404) {
          this.errorMsg = 'This helpdesk does not have any units.';
        } else {
          this.notifier.notify('error', 'Unable to get units, please contact helpdesk admin.');
        }
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
        if (result.status === 200) {
          this.notifier.notify('success', 'Unit deleted successfully.');
          this.updateUnitsList();
          $('#modal-unit-delete').modal('hide');
        }
      },
      error => {
        if (error.status === 500) {
          this.notifier.notify('error', 'Unable to delete user please contact helpdesk admin');
        }
      });
  }

  /**
   * used to add the topic selected to the units array for the helpdesk on the page
   */
  addNewTopic() {
    const newTopicName: string = this.addForm.controls.unitTopic.value;

    if (!newTopicName) {
      this.notifier.notify('warning', 'You must enter a topic to add it.');
      return;
    }

    const existingTopic: Topic = this.newTopics.find(t => t.name === newTopicName);

    if (existingTopic) {
      this.notifier.notify('warning', 'Unit already has ' + newTopicName + ' topic.');
      return;
    }

    const topic: Topic = new Topic();
    topic.name = newTopicName;
    this.newTopics.push(topic);
    this.addForm.controls.unitTopic.setValue(null);
  }

  /**
   * used to remove the topic selected from the new unit information
   * @param topicToRemove the topi to be removed
   */
  removeNewTopic(topicToRemove: Topic) {
    event.preventDefault();
    const topic: Topic = this.newTopics.find(t => t.name === topicToRemove.name);

    this.newTopics.splice(this.newTopics.indexOf(topic), 1);
  }

  /**
   * used to close the add unit modal
   */
  closeAdd() {
    this.newTopics = [];
    this.addForm.reset();
  }

  /**
   * called to add a unit to the helpdesk
   */
  addUnit() {
    event.preventDefault();

    if (this.addForm.invalid) {
      if (!this.addForm.controls.unitName.value) {
        this.notifier.notify('warning', 'You must enter in a unit name.');
      }

      if ((!this.addForm.controls.unitCode.value) || this.addForm.controls.unitCode.value.length !== 8) {
        this.notifier.notify('warning', 'You must enter in a 8 character unit code.');
      }
    }

    const request = new AddUpdateUnitRequest();
    const id = 0;
    let helpdeskId: number;

    this.route.parent.params.subscribe(params => {
      helpdeskId = +params['id'];
    });
    request.Name = this.addForm.controls.unitName.value;
    request.Code = this.addForm.controls.unitCode.value;
    request.HelpdeskID = helpdeskId;
    this.newTopics.forEach(t => {
      request.Topics.push(t.name);
    });

    this.unitsService.addUpdateUnit(id, request).subscribe(result => {
      this.notifier.notify('success', 'Unit added successfully');
      $('#modal-unit-add').modal('hide');
      this.addForm.reset();
      this.newTopics = [];
      this.updateUnitsList();
    },
      error => {
        this.notifier.notify('error', 'Unable to add unit to helpdesk, please contact admin.');
      });
  }

  /**
   * Prepares the edit modal by prefilling he unit info using it's id
   * @param id the id of the unit
   */
  setUpEdit(id: number) {
    const unit: Unit = this.units.find(u => u.unitId === id);

    this.editForm.controls.editUnitName.setValue(unit.name);
    this.editForm.controls.editUnitCode.setValue(unit.code);
    this.editTopics = unit.topics;
    this.editForm.controls.editUnitId.setValue(unit.unitId);
  }

  /**
   * used to add a new topic to the system
   */
  addEditTopic() {
    const newTopicName = this.editForm.controls.editUnitTopic.value;
    if (!newTopicName) {
      this.notifier.notify('warning', 'You must enter a topic to add it.');
      return;
    }

    const existingTopic = this.editTopics.find(t => t.name === newTopicName);

    if (existingTopic) {
      this.notifier.notify('warning', 'Unit already has ' + newTopicName + ' topic.');
      return;
    }

    const topic = new Topic();
    topic.name = newTopicName;
    this.editTopics.push(topic);
    this.addForm.controls.unitTopic.setValue(null);
  }

  /**
   * Used to remove the topic selected from the existing units information
   * @param topicToRemove the topic to be removed
   */
  removeEditTopic(topicToRemove: Topic) {
    event.preventDefault();
    const topic: Topic = this.editTopics.find(t => t.name === topicToRemove.name);

    this.editTopics.splice(this.editTopics.indexOf(topic), 1);
  }

  /**
   * Used to close the edit unit modal
   */
  closeEdit() {
    this.editTopics = [];
    this.editForm.reset();
  }

  /**
   * Used to edit a unit
   */
  editUnit() {
    if (this.editForm.invalid) {

      if (!this.editForm.controls.unitId.value) {
        this.notifier.notify('error', 'UnitId not found, please contact admin.');
      } else {
        if (!this.addForm.controls.unitName.value) {
          this.notifier.notify('warning', 'You must enter in a unit name.');
        }

        if ((!this.addForm.controls.unitCode.value) || this.addForm.controls.unitCode.value.length !== 8) {
          this.notifier.notify('warning', 'You must enter in a 8 character unit code.');
        }
      }
    }

    const request = new AddUpdateUnitRequest();
    let helpdeskId: number;

    this.route.parent.params.subscribe(params => {
      helpdeskId = +params['id'];
    });
    request.Name = this.editForm.controls.editUnitName.value;
    request.Code = this.editForm.controls.editUnitCode.value;
    request.HelpdeskID = helpdeskId;
    this.editTopics.forEach(t => {
      request.Topics.push(t.name);
    });

    this.unitsService.addUpdateUnit(this.editForm.controls.editUnitId.value, request).subscribe(result => {
      this.notifier.notify('success', 'Unit saved successfully');
      $('#modal-unit-edit').modal('hide');
      this.editForm.reset();
      this.editTopics = [];
      this.updateUnitsList();
    },
      error => {
        this.notifier.notify('error', 'Unable to save unit, please contact admin.');
      });
  }
}
