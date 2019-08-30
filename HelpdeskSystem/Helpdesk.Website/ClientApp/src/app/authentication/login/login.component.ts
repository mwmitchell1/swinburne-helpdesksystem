import { Component, OnInit } from '@angular/core';
import { LoginRequest } from 'src/app/data/requests/login-request';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { NotifierService } from 'angular-notifier';

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
  private returnUrl;
  private readonly notifier: NotifierService;

  constructor(private builder: FormBuilder,
              private service: AuthenticationService,
              private route: ActivatedRoute,
              private router: Router,
              notifierSerive: NotifierService) {

    this.notifier = notifierSerive;
    
    this.loginForm = this.builder.group({
      username: '',
      password: ''
    });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      console.log(params);
      this.returnUrl = params.return;
    });
  }

  /**
   * This function calls the log in function in the auth service
   * @param data The login inforamtion of the user
   */
  logIn(data) {

    var isValid: boolean = true;

    if (!data.username)
    {
      this.notifier.notify('warning', 'You must enter your username');
      isValid = false;
    }

    if (!data.password)
    {
      this.notifier.notify('warning', 'You must enter your password');
      isValid = false;
    }

    if (!isValid)
      return;

    const loginRequest = new LoginRequest;
    loginRequest.Password = data.password;
    loginRequest.Username = data.username;

    this.service.loginUser(loginRequest).subscribe(result => {
      this.router.navigateByUrl(this.returnUrl);
    },
    error => {
      if (error.status == 400)
      {
        this.notifier.notify('warning', 'Username or password is incorrect.');
      }
      else
      {
        this.notifier.notify('error', 'Unable to login please contact admin.')
      }
    });
  }
}
