import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

// This auto provides it in the app.module
@Injectable({
  providedIn: 'root',
})

// Responsible for making our HTTP Requests from server to client
// Using a service lets us centralise our http requests
export class AccountService {
  // Getting it from our dev environment file
  baseUrl = environment.apiUrl

  // Sort of a global value that we can use elsewhere in our application
  // Can be User OR null
  private currentUserSource = new BehaviorSubject<User | null>(null); // Initially set to null

  // Dollar sign signifies it is an observable
  currentUser$ = this.currentUserSource;

  constructor(private http: HttpClient) {}

  // Our Login Request
  // model is our body
  login(model: User) {
    // Using OBSERVABLES method for API request
    // For our post request we need to specify what we want to return --> <User>
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        // User is equal to the response we get back from the API
        const user = response;
        // Storing the user in our localStorage so its saved
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          // This fires the function above when we log in --> Line 20
          this.currentUserSource.next(user);
        }
      })
    );
  }

  // Our Register Request
  // Similar to the Log In Request
  register(model: any) {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((user) => {
        // If there was a successful register of a user --> Set this user in our localStorage
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
      })
    );
  }

  // Sets the User Data from localStorage to the one we log in as
  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  // Removing our user when hit logout
  logout() {
    localStorage.removeItem('user');
    // Sets it back to null when we logout
    this.currentUserSource.next(null);
  }
}
