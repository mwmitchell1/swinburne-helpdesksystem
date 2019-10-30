import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { User } from './user.model';
import { DeleteUserResponse } from 'src/app/data/responses/users/delete-response';
import { UpdateUserRequest } from 'src/app/data/requests/users/update-request';
import { UpdateUserResponse } from 'src/app/data/responses/users/update-response';
import { AddUserRequest } from '../../data/requests/users/add-request';
import { AddUserResponse } from '../../data/responses/users/add-response';

@Injectable()
/**
 * Used to call the APIs for Users
 */
export class UsersService {
  constructor(private http: HttpClient) { }

  /**
   * HTTP Request to get all users
   */
  getUsers() {
    return this.http.get<User>('/api/users');
  }

  /**
   * HTTP request to delete user
   * @param id Id of user to delete
   */
  deleteUser(id: number) {
    return this.http.delete<DeleteUserResponse>('/api/users/' + id)
  }

  /**
   * HTTP request to add new user
   * @param request Data of user to add
   */
  addUser(request: AddUserRequest) {
    return this.http.post<AddUserResponse>('/api/users/', request);
  }

  /**
   * HTTP request to update existing user
   * @param request Data to update
   * @param id Id of user to update
   */
  updateUser(request: UpdateUserRequest, id: number) {
    return this.http.patch<UpdateUserResponse>('/api/users/' + id, request);
  }
}
