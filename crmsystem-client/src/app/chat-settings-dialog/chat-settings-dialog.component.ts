import { Component, OnInit, Inject, ViewChild, ElementRef } from '@angular/core';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import { MatSnackBar, MatDialogRef, MAT_DIALOG_DATA, MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material';
import { Observable } from 'rxjs';
import { Employee } from '../models/employeeModel';
import { startWith, map } from 'rxjs/operators';
import { Utils } from '../core/utils';
import { ChatService } from '../services/chat.service';
import { HttpService } from '../services/http.service';
import { DataService } from '../services/data.service';
@Component({
  selector: 'app-chat-settings-dialog',
  templateUrl: './chat-settings-dialog.component.html',
  styleUrls: ['./chat-settings-dialog.component.css'],
  providers: [HttpService, DataService]
})
export class ChatSettingsDialogComponent implements OnInit {
  changeChatform: FormGroup;
  chatService: ChatService;
  chatId: number;
  chatName: string;
  chatParticipants: any[];
  selectedEmployee: Employee;
  allEmployees: Employee[];

  employeeCtrl = new FormControl();
  filteredEmployees: Observable<Employee[]>;

  resultUpdateChatName: any = null;
  removedChatId: any = null;

  @ViewChild('auto', null) matAutocomplete: MatAutocomplete;

  constructor(fb: FormBuilder,
    private _snackBar: MatSnackBar,
    private httpService: HttpService,
    private dataService: DataService,
    public dialogRef: MatDialogRef<ChatSettingsDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {

    this.chatService = data.chatService;
    this.chatId = data.chatId;
    this.chatName = data.chatName;
    this.chatParticipants = data.chatParticipants;

    this.changeChatform = fb.group({
      chatName: new FormControl(this.chatName,
        [
          Validators.required,
          Validators.maxLength(120)
        ]),
      hideRequired: false,
      floatLabel: 'auto',
    });

    this.filteredEmployees = this.employeeCtrl.valueChanges.pipe(
      startWith(null),
      map((employee: string | null) => employee ? this._filter(employee) : []));
  }

  ngOnInit() {
    let context = this;
    this.httpService.loadEmployees(this.dataService.getCompanyId()).subscribe(data => {
      this.allEmployees = data["data"].employees;
    },
      error => {
        Utils.printValueWithHeaderToConsole("[loadEmployees] Error", error);
      });

    this.chatService.hubConnection.on('UpdateChatNameResult', function (data) {
      try {
        let parsedData = JSON.parse(data);
        context.resultUpdateChatName = parsedData;

        context._snackBar.open(parsedData.message, null, {
          duration: 3000,
        });
      }
      catch (err) {
        Utils.printValueWithHeaderToConsole("UpdateChatNameResult Error ", err);
      }
    });

    this.chatService.hubConnection.on('AddParticipantTоChatResult', function (data) {
      try {
        if (data) {
          context.chatParticipants.push({
            Id: context.selectedEmployee.id,
            LastName: context.selectedEmployee.lastName,
            FirstName: context.selectedEmployee.firstName,
            Patronymic: context.selectedEmployee.patronymic
          });

          context._snackBar.open("Указанный участник был успешно добавлен!!!", null, {
            duration: 3000,
          });
        }
        else {
          context._snackBar.open("При добавлении указанного участника произошла ошибка!!!", null, {
            duration: 3000,
          });
        }

        context.employeeCtrl.setValue('');
      }
      catch (err) {
        Utils.printValueWithHeaderToConsole("AddParticipantTоChatResult Error ", err);
      }
    });

    this.chatService.hubConnection.on('RemoveParticipantFromChatResult', function (data) {
      try {
        let parsedData = JSON.parse(data);

        context._snackBar.open(parsedData.message, null, {
          duration: 3000,
        });

        if (parsedData.status) {
          let participantIndex = context.chatParticipants.findIndex(p => p.Id.indexOf(parsedData.participantId) === 0);

          if (participantIndex >= 0) {
            context.chatParticipants.splice(participantIndex, 1);
          }
        }
      }
      catch (err) {
        Utils.printValueWithHeaderToConsole("RemoveParticipantFromChatResult Error", err);
      }
    });

    this.chatService.hubConnection.on('RemoveParticipantFromList', function (data) {
      try {
        let parsedData = JSON.parse(data);

        context._snackBar.open(parsedData.message, null, {
          duration: 3000,
        });

        if (parsedData.status) {
          let participantIndex = context.chatParticipants.findIndex(p => p.Id.indexOf(parsedData.participantId) === 0);

          if (participantIndex >= 0) {
            context.chatParticipants.splice(participantIndex, 1);
          }
        }
      }
      catch (err) {
        Utils.printValueWithHeaderToConsole("RemoveParticipantFromChatResult Error", err);
      }
    });

    this.chatService.hubConnection.on('LeaveСhatResult', function (data) {
      try {
        let parsedData = JSON.parse(data);

        if (parsedData.status) {
          context.removedChatId = parsedData.chat;

          context.dialogRef.close({
            chatId: context.chatId,
            removed: context.removedChatId,
            updateName: context.resultUpdateChatName,
            participants: context.chatParticipants
          });
        }
      }
      catch (err) {
        Utils.printValueWithHeaderToConsole("LeaveСhatResult Error", err);
      }
    });
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.changeChatform.controls[controlName].hasError(errorName);
  }

  selectedEmployeeEvent(event: MatAutocompleteSelectedEvent): void {
    this.selectedEmployee = this.allEmployees.find(x => x.id === event.option.value);
    this.employeeCtrl.setValue(`${this.selectedEmployee.lastName} ${this.selectedEmployee.firstName} ${this.selectedEmployee.patronymic}`);
  }

  private _filter(value: string): Employee[] {
    if (!value.trim())
      return [];

    const filterValue = value.toLowerCase();

    return this.allEmployees.filter(employee => employee.id.toLowerCase().indexOf(filterValue) === 0 ||
      `${employee.lastName.toLowerCase()} ${employee.firstName.toLowerCase()} ${employee.patronymic.toLowerCase()}`.toLowerCase().indexOf(filterValue) === 0);
  }

  changeChatData() {
    if (this.changeChatform.valid) {
      this.chatService.updateChatName(this.chatId, this.changeChatform.value.chatName);
    }
  };

  addSelectEmployeeToChat() {
    if (this.selectedEmployee !== null) {
      let participant = this.chatParticipants.find(e => {
        return e.Id === this.selectedEmployee.id;
      });

      if (!participant) {
        let obj = {
          ChatId: this.chatId,
          Id: this.selectedEmployee.id
        };

        let datas = [obj];

        this.chatService.hubConnection.invoke('AddParticipantTоChat', this.chatId, datas)
          .catch(err => Utils.printValueWithHeaderToConsole("AddParticipantTоChat Error", err));
      }
      else {
        this._snackBar.open("Указанный пользователь уже участник чата!!!", null, {
          duration: 3000,
        });
      }
    }
    else {
      this._snackBar.open("Пожалуйста укажите сотрудника которого хотите добавить к чату!!!", null, {
        duration: 3000,
      });
    }
  }

  removeParticipantFromChat(participant) {
    this.chatService.removeChatParticipant(this.chatId, participant.Id);
  }

  leaveFromChat() {
    this.chatService.leaveChat(this.chatId);
  }

  onCloseClick(): void {
    this.dialogRef.close({
      chatId: this.chatId,
      removed: this.removedChatId,
      updateName: this.resultUpdateChatName,
      participants: this.chatParticipants
    });
  }
}
