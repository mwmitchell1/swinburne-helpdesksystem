import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from './user.model';
import { DeleteUserResponse } from 'src/app/data/responses/users/delete-response';
import { UpateUserRequest } from 'src/app/data/requests/users/update-request';
import { UpdateUserResponse } from 'src/app/data/responses/users/update-response';

@Injectable()
export class UsersService {
  constructor(private http: HttpClient) { }

  /**
   * GET Request to update this.users array
   */
  getUsers() {
    return this.http.get<User>('/api/users');
  }

  deleteUser(id: number)
  {
    return this.http.delete<DeleteUserResponse>('/api/users/' + id)
  }

  updateUser(request: UpateUserRequest, id: number) {
    return this.http.patch<UpdateUserResponse>('/api/users/' + id, request);
  }
}
