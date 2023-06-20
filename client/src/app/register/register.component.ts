import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  // This is for our child component to parent component --> Emits an event
  @Output() cancelRegister = new EventEmitter();

  // Our property
  model: any = {};

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}
  ngOnInit(): void {}

  // Register Function
  register() {
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
      error: (error) => {
        console.log(error);
        this.toastr.error(error.error);
      },
    });
  }

  // Our Cancel function
  cancel() {
    this.cancelRegister.emit(false);
  }
}
