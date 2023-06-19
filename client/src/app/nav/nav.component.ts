import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};

  // This is our API Request that allows to login and out
  // This is now accessible in HTML
  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  login() {
    // What happens when we Log In
    this.accountService.login(this.model).subscribe({
      // Using the router from above --> When we Log In we are navigated to /members
      next: (_) => this.router.navigateByUrl('/members'),
      error: (error) => this.toastr.error(error.error),
    });
  }

  logout() {
    this.accountService.logout();
    // Takes us back to the homepage
    this.router.navigateByUrl('/');
  }
}
