import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  // Type of Member from member.ts interface
  // Member is now an observable of the Members
  // members$: Observable<Member[]> | undefined;
  members: Member[] = [];
  pagination: Pagination | undefined;
  // Our pageSize and items per page stored in here
  userParams: UserParams | undefined;

  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];

  constructor(private memberService: MembersService) {
    // Coming from our memberService functions of params
    this.userParams = this.memberService.getUserParams();
  }

  // When we call our members we want to load it onto the component
  ngOnInit(): void {
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  // Loads the members
  loadMembers() {
    // Stops our strict mode not letting us
    if (this.userParams) {
      // Checking to see if we have these userParams
      this.memberService.setUserParams(this.userParams);
      this.memberService.getMembers(this.userParams).subscribe({
        next: (response) => {
          if (response.result && response.pagination) {
            this.members = response.result;
            this.pagination = response.pagination;
          }
        },
      });
    }
  }

  resetFilters() {
    // Our Member Service has the functionality
    this.userParams = this.memberService.resetUserParams();
    // Resetting the the load of members based on the default parameters
    this.loadMembers();
  }

  // Changing the page function
  // Takes in an event
  pageChanged(event: any) {
    // This check stops any bugs happening with requests
    if (this.userParams && this.userParams?.pageNumber !== event.page) {
      this.userParams.pageNumber = event.page;
      // Pulling through our Member Service Function
      this.memberService.setUserParams(this.userParams);
      this.loadMembers();
    }
  }
}
