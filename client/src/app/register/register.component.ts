import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

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

  constructor(private accountService: AccountService) {}
  ngOnInit(): void {}

  // Register Function
  register() {
    this.accountService.register(this.model).subscribe({
      next: () => {
        this.cancel();
      },
    });
  }

  // Our Cancel function
  cancel() {
    this.cancelRegister.emit(false);
  }
}
