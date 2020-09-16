import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { Utils } from '../core/utils';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-short-link-create-dialog',
  templateUrl: './short-link-create-dialog.component.html',
  styleUrls: ['./short-link-create-dialog.component.css'],
  providers: [HttpService, DataService]
})
export class ShortLinkCreateDialogComponent implements OnInit {

  createShortLinkForm: FormGroup;

  constructor(fb: FormBuilder,
    public dialogRef: MatDialogRef<ShortLinkCreateDialogComponent>,
    private httpService: HttpService,
    private dataService: DataService,
    private _snackBar: MatSnackBar,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.createShortLinkForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.createShortLinkForm = new FormGroup({
      url: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(2048)
        ]),
      isPublic: new FormControl()
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.createShortLinkForm.controls[controlName].hasError(errorName);
  }

  createShortLink() {
    if (this.createShortLinkForm.valid) {
      let url = this.createShortLinkForm.value.url;
      let isPublic = this.createShortLinkForm.value.isPublic;

      var variables = null;
      if (isPublic) {
        variables = {
          "shortLink": {
            "full": url,
            "companyId": this.dataService.getCompanyId()
          }
        };
      }
      else {
        variables = {
          "shortLink": {
            "full": url,
            "ownerId": this.dataService.getUserId()
          }
        };
      }

      this.httpService.createShortLink(variables).subscribe(data => {
        if (data["data"] != undefined) {
          if (data["data"].createShortLink != null) {
            this._snackBar.open(`Короткая ссылка была успешно создана`, null, {
              duration: 3000,
            });

            this.dialogRef.close({ newLink: data["data"].createShortLink });
          }
          else {
            this._snackBar.open('При создании ссылки произошла ошибка!!!', null, {
              duration: 3000,
            });
          }
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("createShortLink Error", error);

        this._snackBar.open(`При создании ссылки произошла ошибка!!! Подробнее: ${error.message}`, null, {
          duration: 3000,
        });
      });
    }
  }

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
