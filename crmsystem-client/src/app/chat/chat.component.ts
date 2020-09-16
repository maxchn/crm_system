import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CreateChatDialogComponent } from '../create-chat-dialog/create-chat-dialog.component';
import { Utils } from '../core/utils';
import { ChatService } from '../services/chat.service';
import { Chat } from '../models/chat';
import { HttpService } from '../services/http.service';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ChatServiceStatus } from '../models/chatServiceStatus';
import { ChatSettingsDialogComponent } from '../chat-settings-dialog/chat-settings-dialog.component';
import { DataService } from '../services/data.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
  providers: [ChatService, HttpService, DataService]
})

export class ChatComponent implements OnInit {
  chats: Array<Chat> = [];
  chatMessages: Array<any> = [];
  selectedChat: Chat;
  currentUserId: string;
  chatParticipants: Array<any> = [];
  chatForm: FormGroup;
  selectionMode: boolean = false;
  selectedMessages: Array<any> = [];

  @ViewChild('scrollContainer', null) private scrollContainer: ElementRef;

  constructor(public dialog: MatDialog,
    public chatService: ChatService,
    private httpService: HttpService,
    private dataService: DataService,
    private fb: FormBuilder) {

    this.currentUserId = dataService.getUserId();

    this.chatForm = fb.group({
      message: new FormControl('',
        [
          Validators.required
        ]),
      hideRequired: false,
      floatLabel: 'auto',
    });
  }

  ngOnInit() {
    this.chatService.startConnection();

    this.httpService.loadAllChats(this.dataService.getCompanyId(), this.dataService.getUserId()).subscribe(data => {
      if (data["data"].chats) {
        this.chats = data["data"].chats;
      }
    },
      error => {
        Utils.printValueWithHeaderToConsole("getAllChats Error", error);
      });

    this.receiveMessage();

    let context = this;
    setTimeout(function () {
      if (context.chatService.status == ChatServiceStatus.Connected) {
        context.chatService.joinToChats(context.dataService.getCompanyId());
      }
    }, 5000);
  }

  public hasError = (controlName: string, errorName: string) => {
    return this.chatForm.controls[controlName].hasError(errorName);
  }

  showCreateChatDialog() {
    const dialogRef = this.dialog.open(CreateChatDialogComponent, {
      width: '500px',
      data: { userId: this.dataService.getUserId(), companyId: this.dataService.getCompanyId() }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.chats.push(result);
      }
    });
  }

  showChatSettingsDialog() {
    const dialogRef = this.dialog.open(ChatSettingsDialogComponent, {
      width: '500px',
      data: {
        chatService: this.chatService,
        chatId: this.selectedChat.chatId,
        chatName: this.selectedChat.name,
        chatParticipants: this.chatParticipants
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        if (!result.removed) {
          let chat = this.chats.find(c => c.chatId == result.chatId);

          if (chat && result.updateName && result.updateName.status) {
            chat.name = result.updateName.newChatName;
          }

          this.chatParticipants = result.participants;
        }
        else {
          let index = this.chats.findIndex(c => c.chatId == result.removed);

          if (index >= 0) {
            this.chats.splice(index, 1);
            this.chatMessages.length = 0;
          }
        }
      }
    });
  }

  selectChat(chat: Chat) {
    if (this.selectedChat) {
      this.selectedChat.isSelected = false;
    }

    this.selectedChat = chat;
    this.selectedChat.isSelected = true;

    this.chatMessages.length = 0;
    this.chatService.loadChatMessages(chat.chatId);
    this.chatService.loadChatParticipants(chat.chatId);

    this.clearSelectedMessages();
  }

  clearSelectedMessages() {
    this.selectedMessages.length = 0;
    this.selectionMode = false;
  }

  sendMessage() {
    if (this.chatForm.valid) {
      let message = this.chatForm.value.message;

      if (this.chatService.status == ChatServiceStatus.Connected) {
        this.chatService.sendMessage(this.selectedChat.chatId, this.dataService.getCompanyId(), message);
      }
    }
  }

  receiveMessage() {
    let context = this;

    this.chatService.hubConnection.on('ReceiveChatMessage', function (data) {

      if (context.selectedChat) {
        let parseData = JSON.parse(data);

        if (Array.isArray(parseData) && parseData.length == 0) {
          // Show no items
        }
        else if (Array.isArray(parseData) && parseData.length > 0) {
          parseData.forEach(function (value) {
            context.chatMessages.push(value);
          });
        } else {
          context.chatMessages.push(parseData);
        }
      }
      else {
        // FIXME: Show Notification
      }

      context.chatForm.patchValue({
        message: ''
      });
      setTimeout(function () {
        context.scrollToBottom();
      }, 500);
    });

    this.chatService.hubConnection.on('ReceiveChatMessageAsSender', function (data) {

      if (context.selectedChat) {
        let parseData = JSON.parse(data);
        if (Array.isArray(parseData) && parseData.length == 0) {
          // Show no items
        }
        else if (Array.isArray(parseData) && parseData.length > 0) {
          parseData.forEach(function (value) {
            context.chatMessages.push(value);
          });
        } else {
          context.chatMessages.push(parseData);
        }
      }
      else {
        // FIXME: Show Notification
      }

      context.chatForm.patchValue({
        message: ''
      });
      setTimeout(function () {
        context.scrollToBottom();
      }, 500);
    });

    this.chatService.hubConnection.on('LoadChatParticipantResult', function (data) {

      try {
        let parseData = JSON.parse(data);

        if (Array.isArray(parseData) && parseData.length == 0) {
          // Show no items

        }
        else if (Array.isArray(parseData) && parseData.length > 0) {
          context.chatParticipants.length = 0;

          parseData.forEach(function (value) {
            context.chatParticipants.push(value);
          });
        } else {
          context.chatMessages.push(parseData);
        }
      }
      catch (err) {
      }
    });

    this.chatService.hubConnection.on('RemoveMessageFromMeResult', function (data) {
      try {
        let parsedData = JSON.parse(data);

        if (parsedData) {

          let index = context.chatMessages.findIndex(x => x.id == parsedData.id);
          if (index >= 0) {
            context.chatMessages.splice(index, 1);
          }

          index = context.selectedMessages.findIndex(x => x.id == parsedData.id);
          if (index >= 0) {
            context.selectedMessages.splice(index, 1);
          }
        }
      }
      catch (err) {
        Utils.printValueWithHeaderToConsole("RemoveMessageFromMeResult Error", err);
      }
    });
  }

  scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    } catch (err) {
      Utils.printValueWithHeaderToConsole("Scroll Error", err);
    }
  }

  getChatItemClass(chat: Chat) {
    return chat.isSelected ? 'chat_name selected_chat_item' : 'chat_name';
  }

  checkClick(isChecked: boolean) {
    this.selectionMode = isChecked;

    if (!this.selectionMode) {
      this.selectedMessages.forEach(function (value) {
        value.isSelected = false;
      });

      this.selectedMessages.length = 0;
    }
  }

  messageClick(message: any) {

    if (this.selectionMode) {
      if (this.selectChat) {
        if (message.isSelected == null || message.isSelected == undefined) {
          message.isSelected = true;

          this.selectedMessages.push(message);
        }
        else {
          if (message.isSelected == true) {
            message.isSelected = false;

            let index = this.selectedMessages.findIndex(m => m.Id == message.Id);

            if (index >= 0) {
              this.selectedMessages.splice(index, 1);
            }
          }
          else {
            message.isSelected = true;
            this.selectedMessages.push(message);
          }
        }
      }
    }
  }

  getChatMessageClass(message: any) {
    if (message.isSelected == null || message.isSelected == undefined) {
      return '';
    }
    else {
      return message.isSelected ? 'chat_selected_message' : '';
    }
  }

  removeSelectedImages() {
    let messages = [];

    this.selectedMessages.forEach(function (value) {
      messages.push({
        ChatMessageParticipantId: value.id
      });
    });

    this.chatService.removeChatMessages(this.selectedChat.chatId, messages);
  }

  parseDateTime(dateTime): string {
    return Utils.formatDateTime(dateTime);
  }
}