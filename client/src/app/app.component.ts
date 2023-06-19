import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  // PROPERTIES
  // -----------
  title = 'MatchMates';
  users: any;

  // Constructor
  // ------------
  // We want to inject our HTTP client into this component NOT the module
  // What we are going to use in here is only going to be available inside this class --> Private
  // Injecting our AccountService so that the data passes through all of our app
  constructor(private accountService: AccountService) {}

  // METHOD
  // ---------------
  // Getting the API --> subscribe so it does more than observing and gets it
  ngOnInit(): void {
    // Our functions below
    this.setCurrentUser();
  }

  // Getting the user for the whole app
  setCurrentUser() {
    const userString = localStorage.getItem('user');
    // If we don't have the user just return / quiet the function
    if (!userString) return;
    // If we do have a user
    const user: User = JSON.parse(userString);
    // Runs this if we do
    this.accountService.setCurrentUser(user);
  }
}
