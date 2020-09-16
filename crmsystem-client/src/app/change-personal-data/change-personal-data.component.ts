import { Component, OnInit, NgModule, Output, EventEmitter, Input } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DateAdapter } from '@angular/material/core';
import { HttpService } from '../services/http.service';
import { Utils } from '../core/utils';
import { ProfileModel } from '../models/profileModel';
import { GenderModel } from '../models/genderModel';
import { PositionModel } from '../models/positionModel';
import { DepartmentModel } from '../models/departmentModel';
import { Phone } from '../models/phone';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-change-personal-data',
  templateUrl: './change-personal-data.component.html',
  styleUrls: ['./change-personal-data.component.css'],
  providers: [HttpService]
})

export class ChangePersonalDataComponent implements OnInit {
  model: ProfileModel = new ProfileModel();
  form: FormGroup;
  showHeader: boolean = true;

  minDate = new Date(1900, 0, 1);
  maxDate = new Date();

  genders: GenderModel[] = [
    { name: "Мужской", id: 1 },
    { name: "Женский", id: 2 },
    { name: "Другой", id: 3 }
  ];

  @Input()
  employeeId: string;

  @Output() updateUserData = new EventEmitter<boolean>();

  constructor(fb: FormBuilder,
    private _httpService: HttpService,
    private _dataService: DataService,
    private _adapter: DateAdapter<any>,
    private _snackBar: MatSnackBar) {

    this.form = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });

    this._adapter.setLocale('ru');
  }

  ngOnInit() {

    this.form = new FormGroup({
      lastName: new FormControl(this.model.lastName,
        [
          Validators.required
        ]),
      firstName: new FormControl(this.model.firstName,
        [
          Validators.required
        ]),
      patronymic: new FormControl(this.model.patronymic,
        [
          Validators.required
        ]),
      dateOfBirth: new FormControl(this.model.dayOfBirth,
        [
          this.dateIsNotEmpty
        ]),
      gender: new FormControl('',
        [
          Validators.required
        ]),
      department: new FormControl(this.model.department,
        [
          Validators.required
        ]),
      position: new FormControl(this.model.position,
        [
          Validators.required
        ]),
      phones: new FormArray([])
    }, () => {
      return this.dateIsNotEmpty;
    });

    if (this.employeeId != undefined && this.employeeId != null) {

      this.showHeader = false;

      this._httpService.loadEmployeeInfo(this.employeeId).subscribe(
        data => {
          let employee = data["data"].user;

          this.model.id = employee.id;
          this.form.patchValue({
            lastName: employee.lastName,
            firstName: employee.firstName,
            patronymic: employee.patronymic,
            dateOfBirth: employee.dateOfBirth
          });

          if (employee.gender != null) {
            this.form.patchValue({
              gender: employee.gender.genderId
            });
          }

          if (employee.department) {
            this.form.patchValue({
              department: employee.department.name
            });
          }

          if (employee.position) {
            this.form.patchValue({
              position: employee.position.name
            });
          }

          if (employee.phones) {
            for (let i = 0; i < employee.phones.length; i++) {
              (<FormArray>this.form.controls["phones"]).push(new FormControl(employee.phones[i].phoneNumber, [
                Validators.required,
                Validators.pattern(/^(\+\d{2}[\- ]?)?(\(\d{2,3}\)|\d{3}[\- ]?)?([\d\- ]{7,10})$/)]));
            }
          }
        }, error => {
          Utils.printValueWithHeaderToConsole("Error", error.message);
        }
      );
    }
    else {
      this._httpService.loadUserInfo().subscribe(
        data => {
          this.model.id = data.id;
          this.form.patchValue({
            lastName: data.lastName,
            firstName: data.firstName,
            patronymic: data.patronymic,
            dateOfBirth: data.dateOfBirth
          });

          if (data.gender != null) {
            this.form.patchValue({
              gender: data.gender.genderId
            });
          }

          if (data.department) {
            this.form.patchValue({
              department: data.department.name
            });
          }

          if (data.position) {
            this.form.patchValue({
              position: data.position.name
            });
          }

          if (data.phones) {
            for (let i = 0; i < data.phones.length; i++) {
              (<FormArray>this.form.controls["phones"]).push(new FormControl(data.phones[i].phoneNumber, [
                Validators.required,
                Validators.pattern(/^(\+\d{2}[\- ]?)?(\(\d{2,3}\)|\d{3}[\- ]?)?([\d\- ]{7,10})$/)]));
            }
          }
        },
        error => {
          Utils.printValueWithHeaderToConsole("Error", error.message);
        }
      );
    }
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.form.controls[controlName].hasError(errorName);
  }

  dateIsNotEmpty(control: FormControl) {
    let value = control.value;

    if (value) {
      return null;
    }

    return { "isEmpty": true };
  }

  addPhone() {
    (<FormArray>this.form.controls["phones"]).push(new FormControl("+380 ", [
      Validators.required,
      Validators.pattern(/^(\+\d{2}[\- ]?)?(\(\d{2,3}\)|\d{3}[\- ]?)?([\d\- ]{7,10})$/)
    ]));
  }

  removePhone(index: number) {
    (<FormArray>this.form.controls["phones"]).removeAt(index);
  }

  public changePersonalData = () => {
    if (this.form.valid) {
      let model: ProfileModel = new ProfileModel();
      model.id = this.model.id;
      model.lastName = this.form.value.lastName;
      model.firstName = this.form.value.firstName;
      model.patronymic = this.form.value.patronymic;
      model.dayOfBirth = this.form.value.dateOfBirth;

      let gender: GenderModel = new GenderModel();
      gender.id = this.form.value.gender;
      model.gender = gender;

      let department: DepartmentModel = new DepartmentModel();
      department.name = this.form.value.department;
      model.department = department;

      let position: PositionModel = new PositionModel();
      position.name = this.form.value.position;
      model.position = position;

      let phones: Array<Phone> = [];
      let phonesControls = (<FormArray>this.form.controls["phones"]);
      for (let i = 0; i < phonesControls.length; i++) {
        phones.push(new Phone(phonesControls.at(i).value, this._dataService.getUserId()));
      }
      model.phones = phones;

      this._httpService.updateProfile(model, this._dataService.getCompanyId()).subscribe(
        data => {
          if (data["data"].updateUser.status) {
            this._snackBar.open("Данные были успешно обновлены", null, {
              duration: 3000,
            });

            this.updateUserData.emit(true);
          }
          else {
            this._snackBar.open(`При обновлении данных произошла ошибка! Подробнее: ${data["data"].updateUser.message}`, null, {
              duration: 3000,
            });
          }
        },
        error => {
          Utils.printValueWithHeaderToConsole("error", error);
          this._snackBar.open(`При обновлении данных произошла ошибка! Подробнее: ${error.message}`, null, {
            duration: 3000,
          });
        }
      );
    }
  }
}
