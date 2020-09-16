import { Component, OnInit, Inject } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProfileModel } from '../models/profileModel';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { DataService } from '../services/data.service';

export interface DialogData {
  name: string;
  url: string;
}

@Component({
  selector: 'app-employee-invitation-dialog',
  templateUrl: './employee-invitation-dialog.component.html',
  styleUrls: ['./employee-invitation-dialog.component.css'],
  providers: [HttpService, DataService]
})

export class EmployeeInvitationDialogComponent implements OnInit {
  invitationForm: FormGroup;
  employeeRegisterForm: FormGroup;

  constructor(fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private _httpService: HttpService,
    private dataService: DataService,
    public dialogRef: MatDialogRef<EmployeeInvitationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData) {

    this.invitationForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });

    this.employeeRegisterForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.invitationForm = new FormGroup({
      email: new FormControl('',
        [
          Validators.required,
          Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+?\.[a-zA-Z0-9-.]{2,5}$')
        ])
    });

    this.employeeRegisterForm = new FormGroup({
      email: new FormControl('',
        [
          Validators.required,
          Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+?\.[a-zA-Z0-9-.]{2,5}$')
        ]),
      lastName: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(256)
        ]),
      firstName: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(256)
        ]),
      patronymic: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(256)
        ]),
      department: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(256)
        ]),
      position: new FormControl('',
        [
          Validators.required,
          Validators.maxLength(256)
        ]),
      isSendLoginPasswordOnEmail: new FormControl('')
    });
  }

  public hasErrorInvitationForm = (controlName: string, errorName: string) => {
    return this.invitationForm.controls[controlName].hasError(errorName);
  }

  public hasErrorRegisterForm = (controlName: string, errorName: string) => {
    return this.employeeRegisterForm.controls[controlName].hasError(errorName);
  }

  onCancelClick(): void {
    this.dialogRef.close(null);
  }

  sendInvitation() {
    if (this.invitationForm.valid) {
      let email = this.invitationForm.value.email;

      this._httpService.sendInvitation(email, this.dataService.getCompanyId())
        .subscribe(data => {
          this._snackBar.open("Приглашение с инструкцией успешно было отправлено на указанный email", null, {
            duration: 2000,
          });

          this.dialogRef.close(null);
        }, error => {
          this._snackBar.open(`При отправки приглашения произошла ошибка!!! Подробнее: ${error.message}`, null, {
            duration: 3000,
          });
        });
    }
  }

  registerEmployee(): void {
    if (this.employeeRegisterForm.valid) {

      let model: ProfileModel = new ProfileModel();
      model.email = this.employeeRegisterForm.value.email;
      model.lastName = this.employeeRegisterForm.value.lastName;
      model.firstName = this.employeeRegisterForm.value.firstName;
      model.patronymic = this.employeeRegisterForm.value.patronymic;
      model.department = this.employeeRegisterForm.value.department;
      model.position = this.employeeRegisterForm.value.position;

      let companyId = this.dataService.getCompanyId();
      let isSendLoginPasswordOnEmail = this.employeeRegisterForm.value.isSendLoginPasswordOnEmail;
      isSendLoginPasswordOnEmail = isSendLoginPasswordOnEmail == null || isSendLoginPasswordOnEmail == undefined || isSendLoginPasswordOnEmail == false ? false : true;

      this._httpService.registerNewEmployee(model, companyId, isSendLoginPasswordOnEmail).subscribe(data => {

        if (data["data"].createEmployee.status) {
          this._snackBar.open("Новый сотрудник был успешно зарегистрирован", null, {
            duration: 3000,
          });

          this.dialogRef.close(data["data"].createEmployee.employee == null ? null : data["data"].createEmployee.employee);
        }
      },
        error => {
          Utils.printValueWithHeaderToConsole("Error", error);

          this._snackBar.open(`При регистрации нового сотрудника произошла ошибка!!! Подробнее: ${error.message}`, null, {
            duration: 3000,
          });
        });
    }
  }

  clearEmployeeRegisterForm(): void {
    this.employeeRegisterForm.patchValue({
      email: '',
      lastName: '',
      firstName: '',
      patronymic: '',
      department: '',
      position: ''
    });
  }
}