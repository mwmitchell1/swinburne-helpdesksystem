import { Component, OnInit } from '@angular/core';
import { LoginRequest } from 'src/app/data/requests/login-request';
import { FormBuilder, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { NotifierService } from 'angular-notifier';
import { UpdateUserRequest } from 'src/app/data/requests/users/update-request';
import { UsersService } from 'src/app/admin/users/users.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

/**
 * This class is used to handle the logic for the login component
 */
export class LoginComponent implements OnInit {
  loginForm;
  setPasswordForm;
  private returnUrl;
  private readonly notifier: NotifierService;

  constructor(private builder: FormBuilder,
    private service: AuthenticationService,
    private userService: UsersService,
    private route: ActivatedRoute,
    private router: Router,
    notifierSerive: NotifierService) {

    this.notifier = notifierSerive;

    this.setPasswordForm = this.builder.group({
      modalUsername: new FormControl(''),
      modalPassword: new FormControl(''),
      confirmPassword: new FormControl(''),
      modalUserId: new FormControl('')
    });

    this.loginForm = this.builder.group({
      username: '',
      password: ''
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.returnUrl = params.return;
    });
  }

  /**
   * This function calls the log in function in the auth service
   * @param data The login inforamtion of the user
   */
  logIn(data) {

    var isValid: boolean = true;

    if (!data.username) {
      this.notifier.notify('warning', 'You must enter your username');
      isValid = false;
    }

    if (!data.password) {
      this.notifier.notify('warning', 'You must enter your password');
      isValid = false;
    }

    if (!isValid)
      return;

    const loginRequest = new LoginRequest;
    loginRequest.Password = data.password;
    loginRequest.Username = data.username;

    this.service.loginUser(loginRequest).subscribe(result => {
      if (result.status == 200) {
        this.router.navigateByUrl(this.returnUrl);
      }
      else if (result.status == 202) {
        this.setPasswordForm.patchValue({ modalUsername: loginRequest.Username, modalUserId: result.userId });

        document.getElementById("open-set-password").click();
      }
    },
      error => {
        if (error.status == 400) {
          this.notifier.notify('warning', 'Username or password is incorrect.');
        }
        else {
          this.notifier.notify('error', 'Unable to login please contact admin.')
        }
      });
  }

  setPassword(data) {
    var isValid: boolean = true;

    if (!data.modalPassword) {
      this.notifier.notify('warning', 'You must enter your password');
      isValid = false;
    }

    if (!data.confirmPassword) {
      this.notifier.notify('warning', 'You must enter your password confirmation');
      isValid = false;
    }

    if (data.confirmPassword != data.modalPassword) {
      this.notifier.notify('warning', 'Passwords do not match.');
      isValid = false;
    }

    if (!isValid)
      return;

    const updateUserRequest = new UpdateUserRequest;
    updateUserRequest.Username = data.modalUsername;
    updateUserRequest.Password = data.modalPassword;

    this.userService.updateUser(updateUserRequest, data.modalUserId).subscribe(result => {
      if (result.status == 200) {
        $('#modal-set-password').modal('hide');
        this.loginForm.patchValue({username: data.modalUsername, password: ''})
      }
    },
      error => {
        this.notifier.notify('error', 'Unable to login please contact admin.')
      });
  }
}
