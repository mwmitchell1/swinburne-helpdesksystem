export class AddUserRequest {
  public Username: string;

  // TODO: Modify POST /api/users endpoint to only require username
  public Password: string;
  public PasswordConfirm: string;

  constructor() {
    this.Password = 'Password1';
    this.PasswordConfirm = 'Password1';
  }

}
