import { Component } from '@angular/core';
import { Utils } from './core/utils';
import { DataService } from './services/data.service';
import { Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';
import { HttpService } from './services/http.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  providers: [HttpService, DataService]
})

export class AppComponent {
  title = 'crmsystem-client';
  pageTitle: string = "Главная";

  appitems = [];

  authorizedMenuItems = [
    {
      label: 'Dashboard',
      link: '/dashboard',
      icon: 'dashboard'
    },
    {
      label: 'Задачи',
      link: '/task/index',
      icon: 'home'
    },
    {
      label: 'Сотрудники',
      link: '/employee/index',
      icon: 'people'
    },
    {
      label: 'Чат',
      link: '/chat',
      icon: 'chat'
    },
    {
      label: 'Облако',
      link: '/cloud',
      icon: 'cloud'
    },
    {
      label: 'Календарь',
      link: '/calendar',
      icon: 'calendar_today'
    },
    {
      label: 'Cсылки',
      link: '/link_shortener',
      icon: 'link'
    },
    {
      label: 'О компании',
      link: '/company/details',
      icon: 'domain'
    },
    {
      label: 'Профиль',
      link: '/profile',
      icon: 'person'
    },
    {
      label: 'Выход',
      link: '/logout',
      icon: 'exit_to_app'
    },
  ]

  constructor(private _dataService: DataService,
    private _httpService: HttpService,
    private _cookieService: CookieService,
    private _router: Router) {
    if (_dataService.isAuthorized()) {
      this.appitems = this.authorizedMenuItems;
    }
  }

  selectedItem(event) {
    this.pageTitle = event.label;

    if (event.link === '/logout') {

      this._httpService.logout().subscribe(data => {
        this.logout();
        window.location.reload(true);
      }, error => {
        Utils.printValueWithHeaderToConsole("Logout Error", error);

        this.logout();
        window.location.reload(true);
      });
    }
    else {
      this._router.navigate([event.link]);
    }
  }

  isLargeScreen(): boolean {
    const screenWith = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;

    return screenWith > 720;
  }

  logout(): void {
    this._cookieService.delete(Utils.ACCESS_TOKEN_TYPE);
    this._cookieService.delete(Utils.ACCESS_TOKEN);
    this._cookieService.delete(Utils.EXPIRES);
    this._cookieService.delete(Utils.USER_ID);
    this._cookieService.delete(Utils.COMPANY_ID);
  }

  setTitle(title: string) {
    this.pageTitle = title;
  }

  isAuthorized(): boolean {
    return this._dataService.isAuthorized();
  }
}