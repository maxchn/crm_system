import { Component, OnInit } from '@angular/core';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { Utils } from '../core/utils';
import { MatDialog } from '@angular/material/dialog';
import { PrivateNotificationDialogComponent } from '../private-notification-dialog/private-notification-dialog.component';
import { Notification } from '../models/notification'
import { MatSnackBar } from '@angular/material/snack-bar';
import { TaskStatistics } from '../models/taskStatistics';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  birthDayNotifications: Array<any> = [];
  notifications: Array<Notification> = [];
  taskStatistics: TaskStatistics = new TaskStatistics();

  constructor(private _httpService: HttpService,
    private _dataService: DataService,
    public dialog: MatDialog,
    private _snackBar: MatSnackBar) { }

  ngOnInit() {
    this._httpService.getAllNotifications(this._dataService.getCompanyId())
      .subscribe(data => {
        if (data["data"]) {

          this.birthDayNotifications = data["data"].birthDayNotifications;

          for (let i = 0; i < data["data"].taskNotifications.length; i++) {
            this.notifications.push({ type: 1, data: data["data"].taskNotifications[i] });
          }

          for (let i = 0; i < data["data"].companyNotifications.length; i++) {
            this.notifications.push({ type: 2, data: data["data"].companyNotifications[i] });
          }

          for (let i = 0; i < data["data"].privateNotifications.length; i++) {
            this.notifications.push({ type: 3, data: data["data"].privateNotifications[i] });
          }

          this.sortNotifications();
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("[getAllNotifications][error]", error);
      });

    this._httpService.getStatistics(this._dataService.getCompanyId())
      .subscribe(data => {
        this.taskStatistics = data["data"].getTaskStatistics;
      }, error => {
        Utils.printValueWithHeaderToConsole("[loadStatistics][error]", error);
      });
  }

  getTaskNotificationTypeText(type) {
    switch (type) {
      case 'DOING':
        return 'начал выполнение задачи';
      case 'TO_DO':
        return 'приостановил выполнение задачи';
      case 'CLOSED':
        return 'изменил статус задачи на "Выполнена"';
      case 'REOPEN':
        return 'вновь открыл задачу';
      default:
        return '';
    }
  }

  showPrivateMessageDialog() {
    const dialogRef = this.dialog.open(PrivateNotificationDialogComponent, {
      width: '650px',
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result != null) {
        if (result.id == 0) {
          this.notifications.push({ type: 3, data: result.data });
          this.sortNotifications();
        }
      }
    });
  }

  sortNotifications() {
    this.notifications = this.notifications.sort((a, b) => {
      if (a.data.dateTime > b.data.dateTime) {
        return -1;
      }

      if (a.data.dateTime < b.data.dateTime) {
        return 1;
      }

      return 0;
    });
  }

  getFormatDateTime(value): string {
    return Utils.formatDateTime(value);
  }

  getPermissionOnPrivateNotification(id: string): boolean {
    return id == this._dataService.getUserId();
  }

  deletePrivateNotification(id) {
    this._httpService.removePrivateNotification(id).subscribe(data => {
      if (data["data"].deletePrivateNotification > 0) {
        let index = this.notifications.findIndex(n => n.type == 3 && n.data.privateNotificationId == data["data"].deletePrivateNotification);

        if (index >= 0) {
          this.notifications.splice(index, 1);
        }
      }
      else {
        this._snackBar.open(`При удалении уведомления произошла ошибка!!!`, null, {
          duration: 3000,
        });
      }
    }, error => {
      Utils.printValueWithHeaderToConsole("[removePrivateNotification][error]", error);
      this._snackBar.open(`При удалении уведомления произошла ошибка!!!`, null, {
        duration: 3000,
      });
    });
  }

  editPrivateNotification(notification) {
    const dialogRef = this.dialog.open(PrivateNotificationDialogComponent, {
      width: '650px',
      data: notification
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result != null) {
        if (result.id > 0) {
          let index = this.notifications.findIndex(n => n.type == 3 && n.data.privateNotificationId == result.id);

          if (index >= 0) {
            this.notifications[index].data = result.data;
            this.sortNotifications();
          }
        }
      }
    });
  }

  calcPercentTasksCompleted(): number {
    if (this.taskStatistics.countAllTasksCompletedPerMonth > 0) {
      return (this.taskStatistics.countTasksCompletedPerMonth / this.taskStatistics.countAllTasksCompletedPerMonth) * 100;
    }
    else {
      return 0;
    }
  }
}