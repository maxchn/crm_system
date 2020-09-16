import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-event-create-dialog',
  templateUrl: './event-create-dialog.component.html',
  styleUrls: ['./event-create-dialog.component.css'],
  providers: [HttpService, DataService]
})
export class EventCreateDialogComponent implements OnInit {

  startDate: string;
  endDate: string;
  createEventForm: FormGroup;
  newEvent: any = null;

  constructor(fb: FormBuilder,
    public dialogRef: MatDialogRef<EventCreateDialogComponent>,
    private httpService: HttpService,
    private dataService: DataService,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.startDate = data.startDate;
    this.endDate = data.endDate;

    this.createEventForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.createEventForm = new FormGroup({
      title: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(120)
        ])
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.createEventForm.controls[controlName].hasError(errorName);
  }

  createCalendarEvent(): void {
    if (this.createEventForm.valid) {
      let text = this.createEventForm.value.title;

      this.httpService.addCalendarEvent(text, this.startDate, this.endDate, this.dataService.getUserId())
        .subscribe(data => {
          if (data["data"].createCalendarEvent.calendarEventId > 0) {
            this.newEvent = data["data"].createCalendarEvent;
            this.dialogRef.close(this.newEvent);
          }
        }, error => {
          Utils.printValueWithHeaderToConsole("addCalendarEvent Error", error);
        });
    }
  }

  onCloseClick(): void {
    this.dialogRef.close(this.newEvent);
  }
}