import { Component, OnInit } from '@angular/core';
import { RouterStateSnapshot, Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
/**
 * This class is used to handle the logic related to the user logout component
 */
export class LogoutComponent implements OnInit {

  constructor(private userService: AuthenticationService, private router: Router) { }

  /**
   * This function calls the log out function in the auth service 
   */
  ngOnInit() {
    this.userService.logout();
    this.router.navigateByUrl('/');
  }
}
