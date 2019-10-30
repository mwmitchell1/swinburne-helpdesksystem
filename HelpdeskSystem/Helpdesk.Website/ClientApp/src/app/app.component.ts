import { Component } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
/**
 * The main app component
 */
export class AppComponent {
  title = 'app';
  public userIsAuthorized: boolean;

  constructor(private cookieService: CookieService) {
  }
}

