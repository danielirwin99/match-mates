import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  // Initially going to be an empty array
  private onlineUsersSource = new BehaviorSubject<string[]>([]);
  // Allows us to subscribe to this
  onlineUsers$ = this.onlineUsersSource.asObservable();

  constructor(private toastr: ToastrService, private router: Router) {}

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(
        // Our API URL Endpoint
        this.hubUrl + 'presence',
        // Accessing the token function
        { accessTokenFactory: () => user.token }
      )
      // If we lose connection with our server it will retry to connect it
      .withAutomaticReconnect()
      // Builds our Hub Connection
      .build();

    // This returns a promise that resolves when we connect or get rejected
    this.hubConnection.start().catch((error) => console.log(error));

    // Listening to the information from the API
    // Inside the string has to match whats on the Server (PresenceHub.cs)
    this.hubConnection.on('UserIsOnline', (username) => {
      // Observing the OnlineUsers
      this.onlineUsers$.pipe(take(1)).subscribe({
        // Replacing the old array with a new one
        next: (usernames) =>
          this.onlineUsersSource.next([...usernames, username]),
      });
    });

    this.hubConnection.on('GetOnlineUsers', (usernames) =>
      this.onlineUsersSource.next(usernames)
    );

    // When we disconnect --> Display
    this.hubConnection.off('UserIsOffline', (username) => {
      // Observing the OnlineUsers
      this.onlineUsers$.pipe(take(1)).subscribe({
        next: (usernames) =>
          // Chooses the one that does not equal the username
          this.onlineUsersSource.next(usernames.filter((x) => x !== username)),
      });
    });

    // Notification function
    // From this function we get back the username and knownAs properties
    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) =>
      this.toastr
        .info(knownAs + ' has sent you a new message! Click me to see it')
        .onTap.pipe(take(1))
        .subscribe({
          // Adding our endpoint when they click it
          next: () =>
            this.router.navigateByUrl('/members/' + username + '?tab=Messages'),
        })
    );
  }

  // Stops the connection
  // If it can't stop it will spit out an error --> console.log it
  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log(error));
  }
}
