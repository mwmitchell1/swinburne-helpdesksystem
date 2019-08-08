import { Component } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'app';

  //private _userService: UserService;
  public userIsAuthorized: boolean;

  /**
   * 
   */
  constructor(private cookieService: CookieService) {
    //this._userService = service;
  }
}

