import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from './user.model';

@Injectable()
export class UsersService {
  constructor(private http: HttpClient) { }

  /**
   * GET Request to update this.users array
   */
  getUsers() {
    return this.http.get<User>('/api/users');
  }

}
