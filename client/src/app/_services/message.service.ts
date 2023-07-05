import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.apiUrl;
  // Need to connect to our message hub
  hubUrl = environment.hubUrl;

  private hubConnection?: HubConnection;

  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) {}

  // Getting the other username FROM the MEMBER DETAILED COMPONENT
  createHubConnection(user: User, otherUsername: string) {
    // Our API SignalR Call from the server
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(
        this.hubUrl + 'message?user=' + otherUsername,
        // Getting our token
        {
          accessTokenFactory: () => user.token,
        }
      )
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('ReceiveMessageThread', (messages) => {
      // Creating an observable to store the messages
      // We get back the array of messages
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('NewMessage', (message) => {
      this.messageThread$.pipe(take(1)).subscribe({
        next: (messages) => {
          this.messageThreadSource.next([...messages, message]);
        },
      });
    });
  }

  // Stops the Hub Connection
  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop();
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    // Creating our params from the shared pagination file
    let params = getPaginationHeaders(pageNumber, pageSize);

    // Adds the container onto the params
    params = params.append('Container', container);

    // Our Request to the Server API
    return getPaginatedResult<Message[]>(
      this.baseUrl + 'messages',
      params,
      this.http
    );
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'messages/thread/' + username
    );
  }

  // This returns a Promise so we need to make it async
  async sendMessage(username: string, content: string) {
    // "SendMessage" --> What we called the function in our API
    return this.hubConnection?.invoke('SendMessage', {
        // Passing through the username and the content
        recipientUsername: username,
        content,
      })
      .catch((error) => console.log(error));
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
