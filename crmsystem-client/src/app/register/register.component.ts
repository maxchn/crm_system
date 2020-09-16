import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { PasswordValidator } from '../validators/password.validator';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { Utils } from '../core/utils';


export class RegistrationModel {
  login: string;
  password: string;
  confirmPassword: string;
}

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  providers: [HttpService]
})

export class RegisterComponent implements OnInit {

  model: RegistrationModel = new RegistrationModel();
  registerForm: FormGroup;
  ref: string = null;
  token: string = null;

  hide = true;
  error: any;

  constructor(private _router: Router,
    private _activatedRoute: ActivatedRoute,
    fb: FormBuilder,
    private _httpService: HttpService,
    private _dataService: DataService) {

    if (_dataService.isAuthorized()) {
      this._router.navigate(['/dashboard']);
    }

    this.registerForm = fb.group({
      login: new FormControl("",
        [
          Validators.required,
          Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+?\.[a-zA-Z0-9-.]{2,4}$')
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

  ngOnInit(): void {
    this._activatedRoute.queryParams.subscribe(params => {
      this.registerForm.patchValue({
        "login": params["email"]
      });

      this.ref = params['ref'];
      this.token = params['token'];
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.registerForm.controls[controlName].hasError(errorName);
  }

  public signUp = () => {
    if (this.registerForm.valid) {
      if (this.ref == null || this.token != null) {
        this.executeSignUp();
      }
      else if (this.ref != null) {
        this.executeExtendedSignUp();
      }
    }
  }

  private executeSignUp() {
    const model: RegistrationModel = Object.assign({}, this.registerForm.value);
    this._httpService.signUp(model, this.token).subscribe(
      data => { this._router.navigate(['/login']); },
      error => { this.error = error.message; }
    );
  }

  private executeExtendedSignUp() {
    const body = {
      email: this.registerForm.value.login,
      password: this.registerForm.value.password,
      confirmPassword: this.registerForm.value.confirmPassword,
      ref: this.ref
    };

    this._httpService.extendedSignUp(body).subscribe(data => {
      this._router.navigate(['/login']);
    }, error => {
      Utils.printValueWithHeaderToConsole("[extendedSignUp][error]", error)
      this.error = error.message;
    });
  }
}