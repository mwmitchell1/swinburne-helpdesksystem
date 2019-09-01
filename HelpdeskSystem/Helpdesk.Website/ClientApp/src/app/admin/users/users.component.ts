import { Component } from '@angular/core';
import { UsersService } from './users.service';
import { User } from './user.model';
import { NotifierService } from 'angular-notifier';
import { FormBuilder, FormControl } from '@angular/forms';

@Component({
  selector: 'app-admin-users',
  templateUrl: './users.component.html'
})
export class UsersComponent {
  private users: User[];
  deleteForm;

  constructor(private usersService: UsersService
    , private notifierService: NotifierService
    , private builder: FormBuilder) {
    // this.users = usersService.getUsers();
    // this.users = [];
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

  setupDelete(id: number) {
    this.deleteForm.patchValue({userId: id});
  }

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
}
