import { Component } from '@angular/core';
import { PasswordResetService } from './password-reset.service';

@Component({
  selector: 'app-password-reset',
  templateUrl: './password-reset.component.html'
})

export class PasswordResetComponent {

  constructor(public passwordService: PasswordResetService) {
  }

}
