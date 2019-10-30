import { Component, OnInit } from '@angular/core';
import { NicknameService } from './nickname.service';
import { ActivatedRoute } from '@angular/router';
import { NotifierService } from 'angular-notifier';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Nickname } from 'src/app/data/DTOs/nickname.dto';
import { EditStudentNicknameRequest } from 'src/app/data/requests/student/edit-student-nickname-request';

@Component({
  selector: 'app-admin-nicknames',
  templateUrl: './nicknames.component.html'
})
/**
 * Used to handle Get & Edit funtionality of the nickname administration
 */
export class NicknamesComponent implements OnInit {

  public nicknames: Nickname[] = [];
  public editForm: FormGroup = this.builder.group({
    modalEditId: new FormControl(''),
    modalEditNickname: new FormControl('', [Validators.required, Validators.max(20)])
  });

  constructor(private service: NicknameService,
    private route: ActivatedRoute,
    private notifier: NotifierService,
    private builder: FormBuilder) {
  }

  ngOnInit() {
    this.getNicknames();
  }

  /**
   * This used to get all of the nick names for display
   */
  getNicknames() {
    this.service.getNickames().subscribe(
      result => {
        this.nicknames = result.nicknames;
      },
      error => {
        if (error.status !== 404) {
          this.notifier.notify('error', 'Unable to retreive nicknames, please contact admin');
        }
      }
    );
  }

  /**
   * This is used to populate the edit modal for used nickname edit
   * @param id The system id of the nickname to be editied
   */
  setUpEdit(id: number) {
    const nickname = this.nicknames.find(n => n.id === id);

    this.editForm.controls.modalEditId.setValue(nickname.id);
    this.editForm.controls.modalEditNickname.setValue(nickname.nickname);
  }

  /**
   * used to generate a random nickname on the edit page
   */
  generateNickname() {
    this.service.generateNickname().subscribe(
      result => {
        this.editForm.controls.modalEditNickname.setValue(result.nickname);
      },
      error => {
        this.notifier.notify('error', 'Unable to generate nickname, please enter manually or contact admin');
      }
    );
  }

  /**
   * Used to edit a nickname
   */
  editNickname() {
    event.preventDefault();

    if (this.editForm.invalid) {
      if ((!this.editForm.controls.modalEditNickname) || (this.editForm.controls.modalEditNickname.value.length > 20)) {
        this.notifier.notify('warning', 'You must enter in a nickame, that has 20 or less characters');
      }
    }

    const request = new EditStudentNicknameRequest();
    request.nickname = this.editForm.controls.modalEditNickname.value;

    this.service.editNickname(this.editForm.controls.modalEditId.value, request).subscribe(
      result => {
        this.getNicknames();
        this.editForm.reset();
        $('#modal-student-edit').modal('hide');
        this.notifier.notify('success', 'Student nickname updated');
      },
      error => {

        if (error.status === 400) {
          this.notifier.notify('warning', 'Nickname already exists please choose another.');
        } else {
          this.notifier.notify('error', 'Unable to save nickname, please contact admin');
        }
      }
    );
  }

  /**
   * Used to reset the edit modal when closing
   */
  closeEdit() {
    this.editForm.reset();
  }
}
