import { Component } from '@angular/core';
import { PasswordResetService } from './password-reset.service';
import { UpdateUserRequest } from '../../data/requests/users/update-request';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html'
})

export class PasswordResetComponent {

  private new: string;
  private confirm: string;

  // private updateRequest: UpdateUserRequest;

  constructor(private passwordService: PasswordResetService) {
    // this.updateRequest = new UpdateUserRequest();
  }


  setPassword(user) {

  }

}
