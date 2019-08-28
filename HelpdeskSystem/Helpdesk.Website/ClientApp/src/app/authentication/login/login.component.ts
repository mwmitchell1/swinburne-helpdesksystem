import { Component, OnInit } from '@angular/core';
import { LoginRequest } from 'src/app/data/requests/login-request';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';

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

  constructor(private builder: FormBuilder,
              private service: AuthenticationService,
              private route: ActivatedRoute,
              private router: Router) {
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
    const loginRequest = new LoginRequest;
    loginRequest.Password = data.password;
    loginRequest.Username = data.username;

    this.service.loginUser(loginRequest).subscribe(result => {
      console.log(result);
      this.router.navigateByUrl(this.returnUrl);
    });
  }
}
