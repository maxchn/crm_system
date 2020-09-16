import { Component, OnInit, Inject } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';

export interface DialogData {
  id: number;
  name: string;
  url: string;
}

@Component({
  selector: 'app-edit-company-data-dialog',
  templateUrl: './edit-company-data-dialog.component.html',
  styleUrls: ['./edit-company-data-dialog.component.css'],
  providers: [HttpService]
})

export class EditCompanyDataDialogComponent implements OnInit {
  changeCompanyForm: FormGroup;

  constructor(fb: FormBuilder,
    private _snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<EditCompanyDataDialogComponent>,
    private httpService: HttpService,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {

    this.changeCompanyForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.changeCompanyForm = new FormGroup({
      name: new FormControl(this.data.name,
        [
          Validators.required,
          Validators.maxLength(120)
        ]),
      url: new FormControl(this.data.url,
        [
          Validators.required,
          Validators.pattern('^[a-zA-Z0-9_]+$'),
          Validators.maxLength(120)
        ])
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.changeCompanyForm.controls[controlName].hasError(errorName);
  }

  changeCompany = () => {
    this.httpService.updateCompany(this.data.id,
      this.changeCompanyForm.value.name,
      this.changeCompanyForm.value.url).subscribe(data => {
        if (data["data"].updateCompany.status) {
          this._snackBar.open("Данные были успешно обновлены", null, {
            duration: 3000,
          });

          let result = {
            status: data["data"].updateCompany.status,
            name: this.changeCompanyForm.value.name,
            url: this.changeCompanyForm.value.url
          };

          this.dialogRef.close(result);
        }
        else {
          this._snackBar.open(`При обновлении данных произошла ошибка!!! Подробнее: `, null, {
            duration: 4000,
          });
        }
      },
        error => {
          Utils.printValueWithHeaderToConsole("Error", error);

          this._snackBar.open("При обновлении данных произошла ошибка!!!", null, {
            duration: 4000,
          });
        });
  }

  onCancelClick(): void {
    this.dialogRef.close();
  }
}