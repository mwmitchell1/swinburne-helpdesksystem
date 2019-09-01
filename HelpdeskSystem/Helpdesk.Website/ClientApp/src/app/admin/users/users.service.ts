import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from './user.model';
import { DeleteUserResponse } from 'src/app/data/responses/users/delete-response';
import { UpdateUserRequest } from 'src/app/data/requests/users/update-request';
import { UpdateUserResponse } from 'src/app/data/responses/users/update-response';
import { AddUserRequest } from '../../data/requests/users/add-request';
import { AddUserResponse } from "../../data/responses/users/add-response";

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


  addUser(request: AddUserRequest) {
    return this.http.post<AddUserResponse>('/api/users/', request);
  }


  updateUser(request: UpdateUserRequest, id: number) {
    return this.http.patch<UpdateUserResponse>('/api/users/' + id, request);
  }
}
