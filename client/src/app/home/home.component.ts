import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  registerMode = false;
  users: any;

  constructor() {}

  ngOnInit(): void {
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }



  // What we want our cancelRegister to do
  // Passes through the event --> Our new EventEmitter from register.component --> Initial = false (default)
  cancelRegisterMode(event:boolean) {
    this.registerMode = event;
  }
}
