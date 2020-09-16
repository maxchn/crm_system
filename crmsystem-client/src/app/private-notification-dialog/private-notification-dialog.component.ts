import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { Utils } from '../core/utils';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

@Component({
  selector: 'app-private-notification-dialog',
  templateUrl: './private-notification-dialog.component.html',
  styleUrls: ['./private-notification-dialog.component.css']
})
export class PrivateNotificationDialogComponent implements OnInit {

  buttonIcon: string = 'add';
  buttonText: string = 'Отправить';

  @ViewChild('bodyEditor', null) bodyEditor: any;
  public Editor = ClassicEditor;
  body: string = "";

  @ViewChild('employeesList', null) employeesList: any;
  employees: Array<any> = [];
  notification: any = null;
  notificationEmployees: Array<any> = [];

  constructor(private _httpService: HttpService,
    private _dataService: DataService,
    private _snackBar: MatSnackBar,
    public dialogRef: MatDialogRef<PrivateNotificationDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    if (data != undefined && data != null) {
      this.notification = data;
    }
  }

  ngOnInit() {
    this._httpService.loadEmployees(this._dataService.getCompanyId()).subscribe(data => {
      this.employees = data["data"].employees;
    },
      error => {
        Utils.printValueWithHeaderToConsole("Error", error);
      });

    if (this.notification != null) {
      this._httpService.loadNotificationEmployees(this.notification.privateNotificationId, this._dataService.getCompanyId())
        .subscribe(data => {
          if (data["data"].privateNotificationEmployees != null) {
            this.notificationEmployees = data["data"].privateNotificationEmployees;
          }
        }, error => {
          Utils.printValueWithHeaderToConsole("[loadNotificationEmployees][error]", error);
        });

      this.body = this.notification.body;
      this.buttonIcon = 'edit';
      this.buttonText = 'Обновить';
    }
  }

  isSelectedEmployee(id) {
    let index = this.notificationEmployees.findIndex(n => n.employeeId == id);
    return index >= 0 ? true : false;
  }

  makeAction(selectedEmployees) {
    let body = this.bodyEditor.editorInstance.getData();

    if (body.length === 0 || !body.trim()) {
      this._snackBar.open(`Укажите сообщение!!!`, null, {
        duration: 3000,
      });
      return;
    }

    if (selectedEmployees.length == 0) {
      this._snackBar.open(`Укажите как минимум одного сотрудника!!!`, null, {
        duration: 3000,
      });
      return;
    }

    let privateNotification = {
      "privateNotificationId": this.notification == null ? 0 : this.notification.privateNotificationId,
      "authorId": this._dataService.getUserId(),
      "body": body
    };

    let privateNotificationEmployees: Array<any> = []
    for (let i = 0; i < selectedEmployees.length; i++) {
      privateNotificationEmployees.push({
        "employeeId": selectedEmployees[i].value.id,
        "companyId": this._dataService.getCompanyId()
      });
    }

    privateNotificationEmployees.push({
      "employeeId": this._dataService.getUserId(),
      "companyId": this._dataService.getCompanyId()
    });

    if (this.notification == null) {
      this._httpService.createPrivateNotification(privateNotification, privateNotificationEmployees).subscribe(data => {
        if (data["data"].createPrivateNotification != null) {
          this._snackBar.open(`Приватное уведомление было успешно создано`, null, {
            duration: 3000,
          });

          this.dialogRef.close({ id: 0, data: data["data"].createPrivateNotification });
        }
        else {
          this._snackBar.open(`При создании приватного уведомления произошла ошибка!!!`, null, {
            duration: 3000,
          });
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("[createPrivateNotification][error]", error);

        this._snackBar.open(`При создании приватного уведомления произошла ошибка!!!`, null, {
          duration: 3000,
        });
      });
    } else {
      this._httpService.updatePrivateNotification(privateNotification, privateNotificationEmployees).subscribe(data => {
        if (data["data"].updatePrivateNotification != null) {
          this._snackBar.open(`Уведомление было успешно обновлено`, null, {
            duration: 3000,
          });

          this.dialogRef.close({ id: this.notification.privateNotificationId, data: data["data"].updatePrivateNotification });
        }
        else {
          this._snackBar.open(`При создании приватного уведомления произошла ошибка!!!`, null, {
            duration: 3000,
          });
        }
      }, error => {
        Utils.printValueWithHeaderToConsole("[createPrivateNotification][error]", error);

        this._snackBar.open(`При создании приватного уведомления произошла ошибка!!!`, null, {
          duration: 3000,
        });
      });
    }
  }

  onCloseClick(): void {
    this.dialogRef.close();
  }
}