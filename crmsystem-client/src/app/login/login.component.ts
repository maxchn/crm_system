import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { HttpService } from '../services/http.service';
import { CookieService } from 'ngx-cookie-service';
import { Token } from '../models/token';
import { Utils } from '../core/utils';
import { MatDialog } from '@angular/material/dialog';
import { ForgotPasswordDialogComponent } from '../forgot-password-dialog/forgot-password-dialog.component';
import { DataService } from '../services/data.service';

export class LoginModel {
  login: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [HttpService]
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  hide = true;
  error: any = null;

  constructor(private router: Router,
    private fb: FormBuilder,
    private httpService: HttpService,
    private cookieService: CookieService,
    private dataService: DataService,
    public dialog: MatDialog) {
    this.loginForm = fb.group({
      hideRequired: false,
      floatLabel: 'auto',
    });

    if (dataService.isAuthorized()) {
      this.router.navigate(['/dashboard']);
    }
  }

  ngOnInit() {
    this.loginForm = new FormGroup({
      login: new FormControl('',
        [
          Validators.required,
          Validators.pattern('^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+?\.[a-zA-Z0-9-.]{2,4}$')
        ]),
      password: new FormControl('',
        [
          Validators.required
        ]),
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.loginForm.controls[controlName].hasError(errorName);
  }

  public signIn = () => {
    if (this.loginForm.valid) {
      this.executeOwnerCreation();
    }
  }

  private executeOwnerCreation = () => {
    const model: LoginModel = Object.assign({}, this.loginForm.value);

    this.httpService.signIn(model).subscribe(
      data => {
        const token: Token = Object.assign({}, data);

        this.cookieService.set(Utils.ACCESS_TOKEN_TYPE, token.access_token_type, new Date(token.expires));
        this.cookieService.set(Utils.ACCESS_TOKEN, token.access_token, new Date(token.expires));
        this.cookieService.set(Utils.EXPIRES, token.expires, new Date(token.expires));

        this.httpService.loadUserInfo().subscribe(data => {

          if (data) {
            this.cookieService.set(Utils.USER_ID, data.id, new Date(token.expires));

            this.httpService.loadEmployeeCompany(data.id).subscribe(data => {
              try {
                this.cookieService.set(Utils.COMPANY_ID,
                  data["data"].employeeCompany.companyId,
                  new Date(token.expires));

                window.location.reload(true);
              }
              catch (err) {
                Utils.printValueWithHeaderToConsole("Error", err);
              }
            },
              error => {
                this.error = "Неверный логин или пароль";
              });
          }
        }, error => {
          this.error = "Неверный логин или пароль";
        });
      },
      error => {
        this.error = "Неверный логин или пароль";
      }
    );
  }

  showForgotPasswordDialog() {
    const dialogRef = this.dialog.open(ForgotPasswordDialogComponent, {
      width: '420px'
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }
}