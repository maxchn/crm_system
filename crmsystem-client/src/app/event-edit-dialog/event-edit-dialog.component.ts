import { Component, OnInit, Inject } from '@angular/core';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Utils } from '../core/utils';

@Component({
  selector: 'app-event-edit-dialog',
  templateUrl: './event-edit-dialog.component.html',
  styleUrls: ['./event-edit-dialog.component.css'],
  providers: [HttpService, DataService]
})
export class EventEditDialogComponent implements OnInit {

  editEventForm: FormGroup;
  id: number;
  event: any;

  constructor(fb: FormBuilder,
    public dialogRef: MatDialogRef<EventEditDialogComponent>,
    private httpService: HttpService,
    private _snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.event = data;

    this.editEventForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.editEventForm = new FormGroup({
      title: new FormControl(this.event.title,
        [
          Validators.required,
          Validators.maxLength(120)
        ])
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.editEventForm.controls[controlName].hasError(errorName);
  }

  onCloseClick(): void {
    this.dialogRef.close();
  }

  editCalendarEvent(): void {
    if (this.editEventForm.valid) {
      let text = this.editEventForm.value.title;

      this.httpService.updateCalendarEvent(text, this.event.start, this.event.end, this.event.id)
        .subscribe(data => {
          this._snackBar.open("Событие было успешно обновленно", null, {
            duration: 3000,
          });

          this.dialogRef.close({
            updateId: data["data"].updateCalendarEvent.calendarEventId,
            updateTitle: data["data"].updateCalendarEvent.text
          });
        }, error => {
          Utils.printValueWithHeaderToConsole("updateCalendarEvent Error", error);

          this._snackBar.open("При обновлении события произошла ошибка!!!", null, {
            duration: 3000,
          });
        })
    }
  }

  removeCalendarEvent(): void {
    this.httpService.removeCalendarEvent(this.event.id)
      .subscribe(data => {
        this._snackBar.open("Событие было успешно удалено", null, {
          duration: 3000,
        });

        this.dialogRef.close({ removeId: data["data"].deleteCalendarEvent });
      }, error => {
        Utils.printValueWithHeaderToConsole("removeCalendarEvent Error", error);

        this._snackBar.open("При удалении события произошла ошибка!!!", null, {
          duration: 3000,
        });
      })
  }
}
