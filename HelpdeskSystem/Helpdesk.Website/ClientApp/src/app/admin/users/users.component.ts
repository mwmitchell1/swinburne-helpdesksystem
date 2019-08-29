import { Component } from '@angular/core';

import { UsersService } from './users.service';
import { User } from './user.model';

@Component({
  selector: 'app-admin-users',
  templateUrl: './users.component.html'
})
export class UsersComponent {
  private users: User[];

  constructor(private usersService: UsersService) {
    // this.users = usersService.getUsers();
    // this.users = [];
    this.updateUserList();
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


}
