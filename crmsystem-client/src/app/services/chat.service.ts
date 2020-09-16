import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HttpTransportType } from '@aspnet/signalr';
import { Utils } from '../core/utils';
import { ChatServiceStatus } from '../models/chatServiceStatus';
import { DataService } from './data.service';

@Injectable()
export class ChatService {
  status: ChatServiceStatus;
  hubConnection: HubConnection;

  constructor(private dataService: DataService, ) {

  }

  public startConnection = () => {
    const options = {
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets,
      accessTokenFactory: () => this.dataService.getToken(),
    };

    Utils.printValueToConsole(`${Utils.BASE_URL}/chatHub`);

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(`${Utils.BASE_URL}/chatHub`, options)
      .build();

    this.hubConnection
      .start()
      .then(() => {
        console.log('Connection started');
        this.status = ChatServiceStatus.Connected;
      })
      .catch(err => { console.log('Error while starting connection: ' + err); this.status = ChatServiceStatus.Failure; });
  }

  public loadChatMessages = (chatId: number) => {
    this.hubConnection.invoke('LoadChatMessages', chatId)
      .catch(err => Utils.printValueWithHeaderToConsole("[LoadChatMessages] Error", err));
  };

  public loadChatParticipants = (chatId: number) => {
    this.hubConnection.invoke('LoadChatParticipant', chatId)
      .catch(err => Utils.printValueWithHeaderToConsole("[loadChatParticipants] Error", err));
  }

  public sendMessage = (chatId: number, companyId: number, message: string) => {
    this.hubConnection.invoke("SendMessage", chatId, companyId, message)
      .catch(err => Utils.printValueWithHeaderToConsole("[sendMessage] Error", err));
  }

  public updateChatName = (chatId: number, newChatName: string) => {
    this.hubConnection.invoke("UpdateChatName", chatId, newChatName)
      .catch(err => Utils.printValueWithHeaderToConsole("[updateChatName] Error", err));
  }

  public removeChatParticipant = (chatId: number, participantId: string) => {
    this.hubConnection.invoke("RemoveParticipantFromChat", chatId, participantId)
      .catch(err => Utils.printValueWithHeaderToConsole("[removeChatParticipant] Error", err));
  }

  public leaveChat = (chatId: number) => {
    this.hubConnection.invoke("LeaveСhat", chatId)
      .catch(err => Utils.printValueWithHeaderToConsole("[leaveChat] Error", err));
  }

  public removeChatMessages = (chatId: number, messages: any[]) => {
    this.hubConnection.invoke("RemoveMessageFromMe", chatId, messages)
      .catch(err => Utils.printValueWithHeaderToConsole("[removeChatMessages] Error", err));
  }

  public joinToChats = (companyId: number) => {
    this.hubConnection.invoke("JoinToChats", companyId)
      .then(() => {
        console.log('Join to chats is successfully');
      })
      .catch(err => { console.log('Join to chats is successfully'); })
  }
}
