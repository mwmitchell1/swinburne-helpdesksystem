export class User {
  id: number;
  username: string;
  password: string;

  constructor(id: number, username: string) {
    this.id = id;
    this.username = username;
    this.password = '';
  }
}
