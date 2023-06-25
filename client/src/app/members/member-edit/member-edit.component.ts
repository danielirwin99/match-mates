import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  // Allows to access the form as a child of the component
  @ViewChild('editForm') editForm: NgForm | undefined;

  // Browser event when you reload --> Prompts an alert
  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm?.dirty) {
      $event.returnValue = true;
    }
  }

  // Pulling through our models
  member: Member | undefined;
  user: User | null = null;

  // Pulling through our services
  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private toastr: ToastrService
  ) {
    // Stops after one request
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => (this.user = user),
    });
  }

  ngOnInit(): void {
    // Loads this data to use for HTML
    this.loadMember();
  }

  loadMember() {
    // Checking to see if have the user
    if (!this.user) return;

    // Getting the member
    this.memberService.getMember(this.user.username).subscribe({
      next: (member) => (this.member = member),
    });
  }

  updateMember() {
    // Pulls through the updateMember function from MemberService
    // Returns an observable --> We need to subscribe
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next: (_) => {
        this.toastr.success('Profile updated successfully');
        // Accessing for the form and resetting it to the member page
        this.editForm?.reset(this.member);
      },
    });
  }
}
