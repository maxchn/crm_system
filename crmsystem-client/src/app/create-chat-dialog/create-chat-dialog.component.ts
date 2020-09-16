import { Component, OnInit, Inject } from '@angular/core';
import { Utils } from '../core/utils';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { MatSnackBar, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-create-chat-dialog',
  templateUrl: './create-chat-dialog.component.html',
  styleUrls: ['./create-chat-dialog.component.css'],
  providers: [HttpService, DataService]
})
export class CreateChatDialogComponent implements OnInit {
  createChatForm: FormGroup;
  companyId: number;
  userId: string;

  constructor(fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private httpService: HttpService,
    private dataService: DataService,
    public dialogRef: MatDialogRef<CreateChatDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.companyId = data.companyId;
    this.userId = data.userId;

    this.createChatForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.createChatForm = new FormGroup({
      name: new FormControl(this.data.name,
        [
          Validators.required,
          Validators.maxLength(120)
        ])
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.createChatForm.controls[controlName].hasError(errorName);
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }

  createChat() {
    if (this.createChatForm.valid) {

      let name = this.createChatForm.value.name;

      this.httpService.createNewChat({
        name: name
      }, this.dataService.getCompanyId(), this.dataService.getUserId()).subscribe(data => {
        if (data["data"].createChat) {
          this._snackBar.open("Чат был успешно создан", null, {
            duration: 3000,
          });

          this.dialogRef.close(data["data"].createChat);
        }
        else {
          this._snackBar.open("При создании нового чата произошла ошибка!!!", null, {
            duration: 3000,
          });
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("Error", error);

        this._snackBar.open("При создании нового чата произошла ошибка!!!", null, {
          duration: 3000,
        });
      });
    }
  }
}
