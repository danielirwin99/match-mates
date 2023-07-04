import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection?: HubConnection;

  constructor(private toastr: ToastrService) {}

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
      this.toastr.info(username + ' has connected'); // Displaying this
    });

    // When we disconnect --> Display
    this.hubConnection.off('UserIsOffline', (username) => {
      this.toastr.warning(username + 'has disconnected');
    });
  }

  // Stops the connection
  // If it can't stop it will spit out an error --> console.log it
  stopHubConnection() {
    this.hubConnection?.stop().catch((error) => console.log(error));
  }
}
