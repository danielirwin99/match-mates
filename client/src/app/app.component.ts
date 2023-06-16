import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

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
  constructor(private http: HttpClient) {}

  // METHOD
  // ---------------
  // Getting the API --> subscribe so it does more than observing and gets it
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/users').subscribe({
      // What we want to do with the response back
      next: (response) => (this.users = response),
      error: (error) => console.log(error),
      complete: () => console.log('Request completed'),
    });
  }
}
