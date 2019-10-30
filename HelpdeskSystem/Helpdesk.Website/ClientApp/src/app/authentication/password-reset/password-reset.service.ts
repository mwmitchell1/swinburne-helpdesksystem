import { Injectable } from '@angular/core';
import { UpdateUserRequest } from '../../data/requests/users/update-request';
import { UsersService } from '../../admin/users/users.service';
import { NotifierService } from 'angular-notifier';


@Injectable()
export class PasswordResetService {

  private resetRequest: UpdateUserRequest;
  private userId: number;
  public passwordNew: string;
  public passwordConfirm: string;
  public passwordsMatch: boolean;
  public pwErrMsg: string;

  constructor(private userService: UsersService, private notifier: NotifierService) {
    this.resetRequest = new UpdateUserRequest();
    this.pwErrMsg = '';
  }

  setupPasswordReset(user) {
    // console.log('password reset - ', user);
    this.userId = user.id;
    this.resetRequest.Username = user.username;
  }

  resetErrMsg() {
    this.pwErrMsg = '';
  }

  setPassword(form) {

    this.passwordsMatch = false;

    console.log('request', this.passwordNew);

    // check if password is populated
    if (this.passwordNew === undefined || this.passwordNew === '') {
      form.controls['password-new'].markAsDirty();
      return;
    }

    // check if confirm is populated
    if (this.passwordConfirm === undefined || this.passwordConfirm === '') {
      form.controls['password-confirm'].markAsDirty();
      this.pwErrMsg = 'Please confirm the password';
      return;
    }

    // check if passwords match
    if (this.passwordNew !== this.passwordConfirm) {
      // error message
      // console.log('no go here')
      console.log('passwords not match');
      form.controls['password-confirm'].markAsDirty();
      this.pwErrMsg = 'Passwords do not match';
      // this.passwordsMatch = true;
      return;
    }

    // validation passed, set up request object
    this.resetRequest.Password = this.passwordNew;

    console.log('send request', this.userId, this.resetRequest);

    this.userService.updateUser(this.resetRequest, this.userId).subscribe(
      result => {
        if (result.status === 200) {
          this.notifier.notify('success', 'Password reset successfully!');

          $('#modal-set-password').modal('hide');
          form.reset();
        }
      }, error => {
        this.notifier.notify('error', 'Unable to reset password, please contact helpdesk admin');
      }
    );


  }

}
