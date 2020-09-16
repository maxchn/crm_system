import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from "@angular/router";
import { CanActivate } from '@angular/router';
import { Observable } from 'rxjs';
import { DataService } from '../services/data.service';
import { Injectable } from '@angular/core';
import { HttpService } from '../services/http.service';
import { reject } from 'q';

@Injectable()
export class ProfileGuard implements CanActivate {

    constructor(
        private _httpService: HttpService,
        private _dataService: DataService,
        private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {

        let router = this.router;

        if (this._dataService.isAuthorized()) {
            let res = this._httpService.getIsFullProfile(this._dataService.getUserId());
            res.then(data => {
                if (data == false) {
                    router.navigate(['extended_registration']);
                }
            }).catch(err => {
                reject(err);
            });

            return res;
        }
        else {
            return true;
        }
    }
}