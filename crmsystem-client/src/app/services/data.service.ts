import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Utils } from '../core/utils';
import { NullLogger } from '@aspnet/signalr';

@Injectable()
export class DataService {

  constructor(private cookieService: CookieService) { }

  public getFullToken(): string {
    const tokenType: string = this.cookieService.get(Utils.ACCESS_TOKEN_TYPE);
    const token: string = this.cookieService.get(Utils.ACCESS_TOKEN);

    return `${tokenType} ${token}`;
  }

  public getToken(): string {
    const token: string = this.cookieService.get(Utils.ACCESS_TOKEN);
    return token;
  }

  public getUserId(): string {
    const userId: string = this.cookieService.get(Utils.USER_ID);
    return userId;
  }

  public getCompanyId(): number {
    const companyId: number = parseInt(this.cookieService.get(Utils.COMPANY_ID));
    return companyId;
  }

  public isAuthorized(): boolean {
    const token: string = this.cookieService.get(Utils.ACCESS_TOKEN);

    return token != null && token != undefined && token != '';
  }
}