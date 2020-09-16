import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from "@angular/router";
import { CanActivate } from '@angular/router';
import { Observable } from 'rxjs';
import { DataService } from '../services/data.service';
import { Injectable } from '@angular/core';
import { HttpService } from '../services/http.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(
        private _httpService: HttpService,
        private _dataService: DataService,
        private _router: Router) { }

    canActivate(route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {
        let result = this.isAuthorized();

        if (!result) {
            this._router.navigate(['/login']);
        }

        return result;
    }

    isAuthorized(): boolean {
        return this._dataService.isAuthorized();
    }
}