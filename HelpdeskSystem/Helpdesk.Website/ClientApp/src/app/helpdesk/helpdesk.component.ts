import { Component, OnInit } from '@angular/core';
import { HelpdeskService } from './helpdesk.service';
import { NotifierService } from 'angular-notifier';
import { Helpdesk } from '../data/DTOs/helpdesk.dto';
import { ActivatedRoute } from '@angular/router';
import { CheckIn } from '../data/DTOs/check-in.dto';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Unit } from '../data/DTOs/unit.dto';
import { CheckInRequest } from '../data/requests/check-in/chek-in-request';
import { ValidateNicknameRequest } from '../data/requests/student/validate-nickname-request';
import { NicknameService } from '../admin/nicknames/nickname.service';
import { CheckOutRequest } from '../data/requests/check-in/check-out-request';
import { QueueItem } from '../data/DTOs/queue-item.dto';
import { Topic } from '../data/DTOs/topic.dto';
import { AddToQueueRequest } from '../data/requests/queue/add-to-queue-request';
import { UpdateQueueItemStatusRequest } from '../data/requests/queue/update-queue-item-status-request';
import { UpdateQueueItemRequest } from '../data/requests/queue/update-queue-item-request';
import { delay } from 'rxjs/operators';

@Component({
  selector: 'app-helpdesk',
  templateUrl: './helpdesk.component.html',
  styleUrls: ['./helpdesk.component.css']
})
/**
 * Used to handle all UI logic for the helpdesk UI page
 */
export class HelpdeskComponent implements OnInit {
  public checkInForm: FormGroup;
  public checkOutForm: FormGroup;
  public joinForm: FormGroup;
  public editQueueForm: FormGroup;
  public helpdesk: Helpdesk = new Helpdesk();
  public helpdeskId: Number;
  public checkIns: CheckIn[] = [];
  public units: Unit[] = [];
  public topics: Topic[] = [];
  public queue: QueueItem[] = [];
  public showTopic = false;
  public ding: HTMLAudioElement;

  constructor(private service: HelpdeskService
    , private notifier: NotifierService
    , private route: ActivatedRoute
    , private builder: FormBuilder
    , private nicknameService: NicknameService) {

    this.ding = new Audio('../../../assets/sounds/ding.wav');
    this.ding.load();

    this.checkInForm = this.builder.group({
      modalSID: new FormControl('', [Validators.required]),
      modalStudentId: new FormControl(''),
      modalNickname: new FormControl('', [Validators.required]),
      modalUnitId: new FormControl(''),
    });
    this.checkOutForm = this.builder.group({
      checkOutStudentId: new FormControl('', [Validators.required])
    });
    this.joinForm = this.builder.group({
      modalJoinCheckId: new FormControl(''),
      modalJoinStudentId: new FormControl(''),
      modalJoinSID: new FormControl(''),
      modalJoinNickname: new FormControl(''),
      modalJoinUnitId: new FormControl(''),
      modalJoinTopicId: new FormControl('', [Validators.required]),
      modalJoinDescription: new FormControl('', [Validators.required])
    });

    this.editQueueForm = this.builder.group({
      modalEditItemId: new FormControl(''),
      modalEditNickname: new FormControl(''),
      modalEditUnitId: new FormControl(''),
      modalEditTopicId: new FormControl(''),
      modalEditDescription: new FormControl('', [Validators.required])
    });
  }

  ngOnInit() {
    this.service.getHelpdeskById(this.route.snapshot.params.id).subscribe(
      result => {
        this.helpdesk = result.helpdesk;
        if (this.helpdesk.hasCheckIn) {
          this.service.getCheckInsByHelpdesk(this.route.snapshot.params.id).subscribe(
            result => {
              this.checkIns = result.checkIns;
            },
            error => {
              if (error.status !== 404) {
                this.notifier.notify('error', 'Unable to retreive check ins, please contact admin');
              }
            }
          );
        }

        if (this.helpdesk.hasQueue) {
          this.getQueueItems();
        }

        if (this.helpdesk.hasQueue && (!this.helpdesk.hasCheckIn)) {
          $(document).on('shown.bs.modal', '#modal-join-queue', function () {
            $('#modalJoinSID').focus();
          });
        }
      },
      error => {
        this.notifier.notify('error', 'Unable to retreive helpdesk information, please contact admin');
      }
    );

    this.service.getActiveUnitsByHelpdeskId(this.route.snapshot.params.id).subscribe(
      result => {
        this.units = result.units;
      },
      error => {
        if (error.status === 404) {
          this.notifier.notify('warning', 'There are no units for this helpdesk, please talk to admin');
        } else {
          this.notifier.notify('error', 'Unable to retreive the units for the helpdesk.');
        }
      }
    );

    $(document).on('shown.bs.modal', '#modal-check-in', function () {
      $('#modalSID').focus();
    });
  }

  /**
   * Used to check in a student
   */
  checkIn() {

    if (!this.checkInForm.valid) {

      if (!this.checkInForm.controls.modalSID.value) {
        this.notifier.notify('warning', 'You must enter in your Student ID.');
      }

      if (!this.checkInForm.controls.modalNickname.value) {
        this.notifier.notify('warning', 'You must enter in a nickname');
      }

      return;
    }

    if ((!this.checkInForm.controls.modalUnitId.value) && (!this.helpdesk.hasQueue)) {
      this.notifier.notify('warning', 'You must select a unit.');
      return;
    }

    const request = new CheckInRequest();
    request.Nickname = this.checkInForm.controls.modalNickname.value;
    request.StudentId = this.checkInForm.controls.modalStudentId.value;
    request.SID = this.checkInForm.controls.modalSID.value;
    request.UnitID = this.checkInForm.controls.modalUnitId.value;
    this.service.checkIn(request).subscribe(
      result => {
        this.notifier.notify('success', 'Check in successful');
        const checkIn = new CheckIn();
        checkIn.checkInId = result.checkInID;
        checkIn.nickname = request.Nickname;
        checkIn.unitId = request.UnitID;
        checkIn.studentId = result.studentID;
        this.checkIns.push(checkIn);
        $('#modal-check-in').modal('hide');
        this.checkInForm.reset();
        this.checkInForm.controls.modalUnitId.setValue('');
      },
      error => {
        this.notifier.notify('error', 'Unable to check in.');
      }
    );
  }

  /**
   * Used to reset the check in modal
   */
  closeCheckIn() {
    this.checkInForm.reset();
    this.checkInForm.controls.modalUnitId.setValue('');
  }

  /**
   * Used to checkout a student from the helpdesk
   */
  checkOut() {
    if (!this.checkOutForm.valid) {

      if (!this.checkOutForm.controls.checkOutStudentId.value) {
        this.notifier.notify('warning', 'You must select your username.');
      }

      return;
    }

    const request = new CheckOutRequest();
    request.ForcedCheckout = false;
    const id = this.checkOutForm.controls.checkOutStudentId.value;
    this.service.checkOut(id, request).subscribe(
      result => {

        if (this.helpdesk.hasQueue) {
          this.getQueueItems();
        }

        this.notifier.notify('success', 'Checkout successful');
        $('#modal-check-out').modal('hide');
        this.checkOutForm.reset();
        const checkIn = this.checkIns.find(c => c.checkInId === id);
        this.checkIns.splice(this.checkIns.indexOf(checkIn), 1);
        this.checkOutForm.controls.checkOutStudentId.setValue('');
      },
      error => {
        if (error.status !== 404) {
          this.notifier.notify('error', 'Unable to check you out, please contact admin.');
        }
      }
    );
  }

  /**
   * Used to reset the checkout modal when closed
   */
  closeCheckOut() {
    this.checkOutForm.reset();
    this.checkOutForm.controls.checkOutStudentId.setValue('');
  }

    /**
   * Used to ensure that the nickname entered is unique or to retreive the students information
   * if they are already in the system
   */
  validateNickname() {

    const request = new ValidateNicknameRequest();
    request.Name = this.checkInForm.controls.modalNickname.value;
    request.SID = this.checkInForm.controls.modalSID.value;

    if ((!request.SID) && (!request.Name)) {
      return;
    }

    this.nicknameService.validateNickname(request).subscribe(
      result => {
        if (result.status === 202) {
          if (result.sid) {
            this.checkInForm.controls.modalSID.setValue(result.studentId);
          }

          if (result.nickname) {
            this.checkInForm.controls.modalNickname.setValue(result.nickname);
          }

          if (result.studentId) {
            this.checkInForm.controls.modalStudentId.setValue(result.sid);
          }
        }
      },
      error => {
        if (error.status === 400) {
          this.notifier.notify('warning', 'This nickname is already taken, please choose another.');
        } else if (error.sttaus !== 404) {
          this.notifier.notify('error', 'Unable to validate nickname, please contact admin.');
        }
      }
    );
  }

  /**
   * Used to ensure that the nickname entered is unique or to retreive the students information
   * if they are already in the system
   */
  validateQueueNickname() {

    const request = new ValidateNicknameRequest();
    request.Name = this.joinForm.controls.modalJoinNickname.value;
    request.SID = this.joinForm.controls.modalJoinSID.value;

    if ((!request.SID) && (!request.Name)) {
      return;
    }

    this.nicknameService.validateNickname(request).subscribe(
      result => {
        if (result.status === 202) {
          if (result.sid) {
            this.joinForm.controls.modalJoinSID.setValue(result.studentId);
          }

          if (result.nickname) {
            this.joinForm.controls.modalJoinNickname.setValue(result.nickname);
          }

          if (result.studentId) {
            this.joinForm.controls.modalJoinStudentId.setValue(result.sid);
          }
        }
      },
      error => {
        if (error.status === 400) {
          this.notifier.notify('warning', 'This nickname is already taken, please choose another.');
        } else if (error.status !== 404) {
          this.notifier.notify('error', 'Unable to validate nickname, please contact admin.');
        }
      }
    );
  }

  /**
   * Used to retreive the items currently queued at the helpdesk
   */
  getQueueItems() {
    this.queue = [];

    this.service.getQueueItemsByHelpdesk(this.route.snapshot.params.id).subscribe(
      result => {
        this.queue = result.queueItems;
      },
      error => {
        if (error.status !== 404) {
          this.notifier.notify('error', 'Unable to retreive queue items, please contact admin');
        }
      }
    );
  }

  /**
   * Used to populate the topics in the various modals
   * @param value the information required for populating the topics
   */
  populateTopics(value?: number) {

    if (value !== null) {
      if (this.helpdesk.hasCheckIn) {
        const checkIn = this.checkIns.find(c => c.checkInId == value);
        this.topics = this.units.find(u => u.unitId === checkIn.unitId).topics;
        this.showTopic = true;
      } else {
        this.showTopic = true;
        this.topics = this.units.find(u => u.unitId == value).topics;
      }
    } else {
      this.showTopic = false;
      this.topics = [];
    }
  }

  /**
   * Used to join the helpdesk queue
   */
  joinQueue() {
    let valid = true;

    if (this.helpdesk.hasCheckIn) {
      if (!this.joinForm.controls.modalJoinCheckId.value) {
        this.notifier.notify('warning', 'You must select your nickname.');
        valid = false;
      }
    } else if (
      (!this.joinForm.controls.modalJoinStudentId.value)
      && ((!this.joinForm.controls.modalJoinNickname.value)
      || (!this.joinForm.controls.modalJoinSID.value))
      ) {
        this.notifier.notify('warning', 'You must enter a nickname and your student id.');
        valid = false;
    }

    if (!this.joinForm.controls.modalJoinDescription.value) {
      this.notifier.notify('warning', 'You must enter in a description');
      valid = false;
    }

    if ((!this.joinForm.controls.modalJoinUnitId.value) && (!this.helpdesk.hasCheckIn)) {
      this.notifier.notify('warning', 'You must select a unit.');
      valid = false;
    }

    if (!this.joinForm.controls.modalJoinTopicId.value) {
      this.notifier.notify('warning', 'You must select a topic.');
      valid = false;
    }

    if (!valid) {
      return;
    }

    const request = new AddToQueueRequest();
    request.nickname = this.joinForm.controls.modalJoinNickname.value;
    request.sid = this.joinForm.controls.modalJoinSID.value;
    request.description = this.joinForm.controls.modalJoinDescription.value;

    if (this.helpdesk.hasCheckIn) {
      request.checkInID = this.joinForm.controls.modalJoinCheckId.value;
      request.studentID = this.checkIns.find(c => c.checkInId == request.checkInID).studentId;
    } else {
      request.studentID = this.joinForm.controls.modalJoinStudentId.value;
    }
    request.topicID = this.joinForm.controls.modalJoinTopicId.value;

    this.service.addToQueue(request).subscribe(
      result => {
        this.notifier.notify('success', 'Successfully joined queue.');
        this.getQueueItems();
        $('#modal-join-queue').modal('hide');
        this.joinForm.reset();
        this.ding.play();
        this.joinForm.controls.modalJoinTopicId.setValue('');
        this.joinForm.controls.modalJoinCheckId.setValue('');
        this.joinForm.controls.modalJoinUnitId.setValue('');
      },
      error => {
        this.notifier.notify('error', 'Unable to join queue, please contact admin.');
      }
    );
  }

  /**
   * Used to reset join queue modal upon close
   */
  closeJoinQueue() {
    this.joinForm.reset();
    this.joinForm.controls.modalJoinCheckId.setValue('');
    this.joinForm.controls.modalJoinUnitId.setValue('');
    this.joinForm.controls.modalJoinTopicId.setValue('');
    this.topics = [];
    this.showTopic = false;
  }

  /**
   * Used to prepare the edit modal for editing a queue item
   * @param id the id of the queue item to be edited
   */
  setupEdit(id: number) {
    const item = this.queue.find(i => i.itemId === id);

    this.editQueueForm.controls.modalEditItemId.setValue(item.itemId);
    this.editQueueForm.controls.modalEditNickname.setValue(item.nickname);
    this.editQueueForm.controls.modalEditNickname.setValue(item.description);

    if (!this.helpdesk.hasCheckIn) {
      const unitSelected = this.units.find(u => u.name === item.unit).unitId;
      this.populateTopics(unitSelected);
      this.editQueueForm.controls.modalEditUnitId.setValue(unitSelected);
    } else {
      this.populateTopics(item.checkInId);
    }

    const topicId = this.topics.find(t => t.name === item.topic).topicId;
    this.editQueueForm.controls.modalEditTopicId.setValue(topicId);
  }

  /**
   * Used to update a queue item
   */
  editQueue() {

    let valid = true;

    if (!this.helpdesk.hasCheckIn) {
      if (!this.editQueueForm.controls.modalEditUnitId.value) {
        this.notifier.notify('warning', 'You must select a Unit');
        valid = false;
      }
    }

    if (!this.editQueueForm.controls.modalEditDescription.value) {
      this.notifier.notify('warning', 'You must enter in a description');
        valid = false;
    }

    if (!this.editQueueForm.controls.modalEditTopicId.value) {
      this.notifier.notify('warning', 'You must select a Topic');
      valid = false;
    }

    if (!valid) {
      return false;
    }

    const request = new UpdateQueueItemRequest();
    request.topicId = this.editQueueForm.controls.modalEditTopicId.value;

    this.service.updateQueueItem(this.editQueueForm.controls.modalEditItemId.value, request).subscribe(
      result => {
        this.notifier.notify('success', 'Item change saved');
        this.getQueueItems();
        this.editQueueForm.reset();
        $('#modal-edit-queue').modal('hide');
      },
      error => {
        this.notifier.notify('error', 'Unable to update queue item, please contact admin.');
      }
    );
  }

  /**
   * Used to reset the edit queue item modal upon close
   */
  closeEditQueue() {
    this.editQueueForm.reset();
    this.topics = [];
    this.showTopic = false;
  }

  /**
   * Used to remove an item from the queue using it's id
   * @param id the id of the queue item to be removed
   */
  remove(id: number) {
    const request = new UpdateQueueItemStatusRequest();
    request.TimeRemoved = new Date();

    this.service.updateQueueItemStatus(id, request).pipe(delay(200)).subscribe(
      result => {
        this.notifier.notify('success', 'Item removed from queue');
        this.getQueueItems();
      },
      error => {
        this.notifier.notify('error', 'Unable to remove queue item, please contact admin');
      }
    );
  }

  /**
   * Used to indicate that a tutor is assisting a student
   * @param id the id of the queue item the tutor is asindicating the are assisting with
   */
  collect(id: number) {
    const request = new UpdateQueueItemStatusRequest();
    request.TimeHelped = new Date();

    this.service.updateQueueItemStatus(id, request).subscribe(
      result => {
        this.notifier.notify('success', 'Item collected');
        this.getQueueItems();
      },
      error => {
        this.notifier.notify('error', 'Unable to remove queue item, please contact admin');
      }
    );
  }
}
