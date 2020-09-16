import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from "@angular/router";
import { CanActivate } from '@angular/router';
import { Observable } from 'rxjs';
import { DataService } from '../services/data.service';
import { Injectable } from '@angular/core';

@Injectable()
export class RoleGuard implements CanActivate {

    constructor(private dataService: DataService,
        private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean | UrlTree | Observable<boolean | UrlTree> | Promise<boolean | UrlTree> {

        let result = this.isAuthorized();

        if (!result) {
            this.router.navigate(['/login']);
        }

        return result;
    }

    isAuthorized(): boolean {
        return this.dataService.isAuthorized();
    }
}