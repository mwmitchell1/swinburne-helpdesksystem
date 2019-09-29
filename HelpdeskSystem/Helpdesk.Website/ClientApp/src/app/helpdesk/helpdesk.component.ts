import { Component, OnInit } from "@angular/core";
import { HelpdeskService } from "./helpdesk.service";
import { NotifierService } from "angular-notifier";
import { Helpdesk } from "../data/DTOs/helpdesk.dto";
import { ActivatedRoute } from "@angular/router";
import { CheckIn } from "../data/DTOs/check-in.dto";
import { FormBuilder, FormControl, FormGroup } from "@angular/forms";
import { Unit } from "../data/DTOs/unit.dto";
import { CheckInRequest } from "../data/requests/check-in/chek-in-request";

@Component({
  selector: 'app-helpdesk',
  templateUrl: './helpdesk.component.html',
  styleUrls: ['./helpdesk.component.css']
})
export class HelpdeskComponent implements OnInit {
  public checkInForm: FormGroup;
  checkOutForm;
  public helpdesk: Helpdesk = new Helpdesk();
  public helpdeskId: Number;
  public checkIns: CheckIn[] = [];
  public units: Unit[] = [];

  constructor(private service: HelpdeskService
    , private notifier: NotifierService
    , private route: ActivatedRoute
    , private builder: FormBuilder) {
    this.checkInForm = this.builder.group({
      modalSID: new FormControl(''),
      modalStudentId: new FormControl(''),
      modalNickname: new FormControl(''),
      modalUnitId: new FormControl(''),
    });
    this.checkOutForm = this.builder.group({
      checkOutStudentId: new FormControl()
    });
  }

  ngOnInit() {

    
    this.service.getHelpdeskById(this.route.snapshot.params.id).subscribe(
      result => {
        this.helpdesk = result.helpdesk;

        if (this.helpdesk.hasCheckIn) {
          this.service.getCheckInsByHelpdesk(this.route.snapshot.params.id).subscribe(
            result => {
              this.checkIns = result.CheckIns;
            },
            error => {
              if (error.status != 404)
              {
                this.notifier.notify('error', "Unable to retreive check ins, please contact admin");
              }
            }
          )
        }
      },
      error => {
        this.notifier.notify('error', "Unable to retreive helpdesk information, please contact admin");
      }
    );

    this.service.getActiveUnitsByHelpdeskId(this.route.snapshot.params.id).subscribe(
      result => {
        this.units = result.units;
      },
      error => {
        this.notifier.notify('error', 'Unable to retreive the units for the helpdesk.');
      }
    )
  }

  checkIn(data) {
    var request = new CheckInRequest();
    request.Nickname = this.checkInForm.controls.modalNickname.value;
    request.SID = this.checkInForm.controls.modalSID.value;
    request.StudentID = this.checkInForm.controls.modalStudentId.value;
    request.UnitID = this.checkInForm.controls.modalUnitId.value;
    this.service.checkIn(request).subscribe(
      result => {
        var checkIn = new CheckIn();
        checkIn.checkInId = result.checkInId;
        checkIn.nickname = request.Nickname;
        this.checkIns.push(checkIn);
      },
      error => {
        this.notifier.notify('error', "Unable to check in.");
      }
    );
  }
}