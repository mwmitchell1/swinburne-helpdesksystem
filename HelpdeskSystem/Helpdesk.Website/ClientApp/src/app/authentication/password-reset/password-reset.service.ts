import { Injectable } from '@angular/core';
import {UpdateUserRequest} from '../../data/requests/users/update-request';


@Injectable()
export class PasswordResetService {

  private resetRequest: UpdateUserRequest;
  private userId: number;
  public passwordNew: string;
  public passwordConfirm: string;

  constructor() {
    this.resetRequest = new UpdateUserRequest();
  }

  setupPasswordReset(user) {
    // console.log('password reset - ', user);
    this.userId = user.id;
  }

  setPassword() {
    console.log('request', this.resetRequest);
  }

}
