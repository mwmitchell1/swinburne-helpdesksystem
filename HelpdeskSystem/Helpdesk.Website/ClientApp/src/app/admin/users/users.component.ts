import { Component } from '@angular/core';

import { UsersService } from './users.service';
import { User } from './user.model';
import { NotifierService } from 'angular-notifier';
import { FormBuilder, FormControl } from '@angular/forms';
import { AddUserRequest } from '../../data/requests/users/add-request';

@Component({
  selector: 'app-admin-users',
  templateUrl: './users.component.html'
})
export class UsersComponent {
  private users: User[];
  deleteForm;
  private readonly userToAdd: AddUserRequest;

  constructor(private usersService: UsersService
    , private notifierService: NotifierService
    , private builder: FormBuilder) {

    this.userToAdd = new AddUserRequest();

    this.updateUserList();

    this.deleteForm = this.builder.group({
      userId: new FormControl('')
    });
  }


  /**
   * Calls UsersService.getUsers() to populate this.users array
   */
  updateUserList(): void {
    this.users = [];

    this.usersService.getUsers().subscribe(data => {
      // @ts-ignore
      data.users.forEach(user => {
        this.users.push(new User(user.userId, user.username));
      });
    });
  }

  /**
   * Prepares hidden delete form
   * @param id Id of user to delete
   */
  setupDelete(id: number) {
    this.deleteForm.patchValue({userId: id});
  }

  /**
   * User service method to delete user
   * @param data Form data
   */
  deleteUser(data) {
    this.usersService.deleteUser(data.userId).subscribe(
      result => {
        if (result.status == 200) {
          this.notifierService.notify('success', 'User deleted successfully.');
          this.updateUserList();
          var modal = $('#modal-user-delete').modal('hide');
        }
      },
      error => {
        if (error.status == 500) {
          this.notifierService.notify('error', 'Unable to delete user please contact helpdesk admin');
        }
        else if (error.status == 403) {
          this.notifierService.notify('warning', 'You cannot delete this user');
        }
      });
  }

  /**
   * User service method to add new user
   * @param form Add user form
   */
  addUser(form) {

    console.log('adding user', this.userToAdd);
    this.usersService.addUser(this.userToAdd).subscribe(
      result => {
        // console.log('result', result);
        this.notifierService.notify('success', 'User added successfully!');
        // TODO Refactor to avoid using updateUserList - getting all users to update one user
        this.updateUserList();

        // close modal
        $('#modal-user-add').modal('hide');

        // reset form
        form.reset();
      },
      error => {
        // console.log('error', error);

        if (error.status === 403) {
          this.notifierService.notify('warning', 'User already exists.');
        } else {
          this.notifierService.notify('error', 'Unable to add user, please contact helpdesk admin');
        }

      }
    );
  }
}
