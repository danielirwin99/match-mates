import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
import { PresenceService } from './presence.service';

// This auto provides it in the app.module
@Injectable({
  providedIn: 'root',
})

// Responsible for making our HTTP Requests from server to client
// Using a service lets us centralise our http requests
export class AccountService {
  // Getting it from our dev environment file
  baseUrl = environment.apiUrl;

  // Sort of a global value that we can use elsewhere in our application
  // Can be User OR null
  private currentUserSource = new BehaviorSubject<User | null>(null); // Initially set to null

  // Dollar sign signifies it is an observable
  currentUser$ = this.currentUserSource;

  constructor(
    private http: HttpClient,
    private presenceService: PresenceService
  ) {}

  // Our Login Request
  // model is our body
  login(model: User) {
    // Using OBSERVABLES method for API request
    // For our post request we need to specify what we want to return --> <User>
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        // User is equal to the response we get back from the API
        const user = response;
        // Storing the user in our localStorage so its saved (see line 58)
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  // Our Register Request
  // Similar to the Log In Request
  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  // This function is called every time we refresh the browser or login
  setCurrentUser(user: User) {
    user.roles = [];
    // Accessing the roles inside our JWT Token --> Inside our API
    const roles = this.getDecodedToken(user.token).role;

    // This makes our roles an array regardless if there is only 1 role that would normally return NOT an array but an object
    Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);

    // If there was a successful register of a user --> Set this user in our localStorage
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);

    // Connects us to the hub every time
    this.presenceService.createHubConnection(user);
  }

  // Removing our user when hit logout
  logout() {
    localStorage.removeItem('user');
    // Sets it back to null when we logout
    this.currentUserSource.next(null);

    // Stops the hub connection
    this.presenceService.stopHubConnection();
  }

  // Getting the token for the Admin
  getDecodedToken(token: string) {
    // We are interested in the details (i.e. nameid) and NOT the credentials (i.e password of the object)
    return JSON.parse(atob(token.split('.')[1]));
  }
}
