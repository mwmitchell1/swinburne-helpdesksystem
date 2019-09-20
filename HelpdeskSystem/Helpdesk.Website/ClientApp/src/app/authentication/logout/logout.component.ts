import { Component, OnInit } from '@angular/core';
import { RouterStateSnapshot, Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { HelpdeskService } from '../../helpdesk/helpdesk.service';
import { NotifierService } from 'angular-notifier';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
/**
 * This class is used to handle the logic related to the user logout component
 */
export class LogoutComponent implements OnInit {

  constructor(private authService: AuthenticationService,
    private router: Router,
    private helpdeskData: HelpdeskService,
    private notifierService: NotifierService) { }

  /**
   * This function calls the log out function in the auth service
   */
  ngOnInit() {
    this.authService.logout().subscribe(
      result => {
        // Navigate to helpdesk view of active helpdesk
        this.router.navigateByUrl('');
      },
      error => {
        this.notifierService.notify('error', 'Unable to log out.')
      }
    );
  }
}
