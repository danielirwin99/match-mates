import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';

// Our Register Functionality

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

  // Our Type for the register input
  registerForm: FormGroup = new FormGroup({});

  maxDate: Date = new Date();

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initializeForm();
    // This only allows us to select anyone over 18 years old
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  // Our Register Form Function
  initializeForm() {
    // this.fb.group allows us to use array brackets to group everything
    this.registerForm = this.fb.group({
      // Validators are things the inputs must oblige by
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      // Put the second argument in an array to have multiple validators
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(4),
          Validators.maxLength(12),
        ],
      ],
      confirmPassword: [
        '',
        [
          Validators.required,
          // Our Function below as a manually made validator --> must pass in what you want to match ("password")
          this.matchPasswords('password'),
        ],
      ],
    });
    // This stops a buggy password going through by making sure the parent equals the child too
    this.registerForm.controls['password'].valueChanges.subscribe({
      next: () =>
        this.registerForm.controls['confirmPassword'].updateValueAndValidity(),
    });
  }

  // Must include the type inside and what we want to return (Validator Function)
  matchPasswords(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      // Seeing if our confirm password (left side) = our password (parent right side)
      return control.value === control.parent?.get(matchTo)?.value
        ? // If they do match we return null --? VALIDATED
          null
        : {
            // If they don't match we return this --> NOT VALIDATED / MATCHING
            notMatching: true,
          };
    };
  }

  // Register Function
  register() {
    console.log(this.registerForm?.value);
    // this.accountService.register(this.model).subscribe({
    //   next: () => {
    //     this.cancel();
    //   },
    //   error: (error) => {
    //     console.log(error);
    //     this.toastr.error(error.error);
    //   },
    // });
  }

  // Our Cancel function
  cancel() {
    this.cancelRegister.emit(false);
  }
}
