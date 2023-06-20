import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-server-error',
  templateUrl: './server-error.component.html',
  styleUrls: ['./server-error.component.css']
})
export class ServerErrorComponent implements OnInit {
  error: any;

  // We want to get access to the Router State --> So we inject it
  constructor(private router: Router) {
    const navigation = this.router.getCurrentNavigation();
    // This could be undefined so we need to use optional chaining
    // Looking inside the error to access --> "error" from the state
    this.error = navigation?.extras?.state?.["error"]
  }

  ngOnInit(): void {

  }
}
