export class AddUserRequest {
  public Username: string;

  // TODO: Modify POST /api/users endpoint to only require username
  public Password: string;
  public PasswordConfirm: string;

  constructor() {
    // this.Password = this.Username;
    // this.PasswordConfirm = this.Username;
  }

}
