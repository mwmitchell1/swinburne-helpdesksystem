export class Helpdesk {
  id: number;
  name: string;
  hasCheckIn: boolean;
  hasQueue: boolean;
  normalizedName: string;
  route: string;
  adminRoute: string;

  constructor(helpdesk) {
    this.id = helpdesk.id;
    this.name = helpdesk.name;
    this.hasCheckIn = helpdesk.hasCheckIn;
    this.hasQueue = helpdesk.hasQueue;

    this.normalizedName = this.name.toLowerCase().replace(/\s/g, '-');
    this.route = this.normalizedName;
    this.adminRoute = this.route + '/admin';
  }
}
