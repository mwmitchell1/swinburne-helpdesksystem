import { Component, OnInit, Inject } from '@angular/core';
import { LoginRequest } from 'src/app/data/requests/login-request';
import { HttpClient } from '@angular/common/http';
import { CookieService } from 'ngx-cookie-service';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  public loginRequest: LoginRequest;
  private client: HttpClient;
  private baseUrl: string;
  loginForm;

  constructor(private builder: FormBuilder, private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private cookieService: CookieService) {
    this.client = http;
    this.baseUrl = baseUrl;
    this.loginForm = this.builder.group({
      username: '',
      password: ''
    });
  }

  ngOnInit() {
  }

  logIn(data) {
    var loginRequest = new LoginRequest()
    loginRequest.Password = data.password;
    loginRequest.Username = data.username
    this.client.post<LoginRequest>(this.baseUrl + 'api/users/login', JSON.stringify(this.loginRequest)).subscribe();
  }

}
