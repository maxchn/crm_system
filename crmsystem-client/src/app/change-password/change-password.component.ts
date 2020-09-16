import { Component } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PasswordValidator } from '../validators/password.validator';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';

export class ChangePasswordModel {
  oldPassword: string;
  password: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css'],
  providers: [HttpService]
})
export class ChangePasswordComponent {
  model: ChangePasswordModel = new ChangePasswordModel();
  form: FormGroup;
  hide = true;

  constructor(private httpService: HttpService,
    private _snackBar: MatSnackBar,
    private fb: FormBuilder) {

    this.form = fb.group({
      oldPassword: new FormControl('',
        [
          Validators.required
        ]),
      password: new FormControl('',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(100),
          Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#\\$%\\^&\\*])(?=.{8,}).*$')
        ]),
      confirmPassword: new FormControl(''),
      hideRequired: false,
      floatLabel: 'auto',
    },
      {
        validator: PasswordValidator.areEqual
      }
    );
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.form.controls[controlName].hasError(errorName);
  }

  public changePassword = () => {
    if (this.form.valid) {

      let oldPassword = this.form.value.oldPassword;
      let newPassword = this.form.value.password;
      let confirmPassword = this.form.value.confirmPassword;
      
      this.httpService.changePassword(oldPassword, newPassword, confirmPassword)
        .subscribe(data => {
          this._snackBar.open("Пароль был успешно изменен", null, {
            duration: 3000,
          });

          this.form.patchValue({
            oldPassword: '',
            password: '',
            confirmPassword: ''
          });
        },
          error => {
            Utils.printValueWithHeaderToConsole("Error", error);

            this._snackBar.open(`При изменении пароля произошла ошибка!!! Подробнее: ${error.error}`, null, {
              duration: 3000,
            });
          });
    }
  }
}
