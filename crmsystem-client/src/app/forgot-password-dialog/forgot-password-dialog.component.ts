import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';

@Component({
  selector: 'app-forgot-password-dialog',
  templateUrl: './forgot-password-dialog.component.html',
  styleUrls: ['./forgot-password-dialog.component.css'],
  providers: [HttpService]
})
export class ForgotPasswordDialogComponent implements OnInit {

  forgotPasswordForm: FormGroup;

  constructor(fb: FormBuilder,
    private _snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<ForgotPasswordDialogComponent>,
    private httpService: HttpService) {

    this.forgotPasswordForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.forgotPasswordForm = new FormGroup({
      email: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(120),
          Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+?\.[a-zA-Z0-9-.]{2,3}$')
        ])
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.forgotPasswordForm.controls[controlName].hasError(errorName);
  }

  forgotPassword() {
    if (this.forgotPasswordForm.valid) {
      let email = this.forgotPasswordForm.value.email;

      this.httpService.forgotPassword(email).subscribe(data => {
        this._snackBar.open(data, null, {
          duration: 3000,
        });

        this.dialogRef.close();
      }, error => {
        this._snackBar.open("Неверный пароль или email не подтвержден!!!", null, {
          duration: 3000,
        });
      });
    }
  }

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
