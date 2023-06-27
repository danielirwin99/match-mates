import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
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

  // What number we want to show first
  pageNumber = 1;
  // How many we want to show per page
  pageSize = 5;

  constructor(private memberService: MembersService) {}

  // When we call our members we want to load it onto the component
  ngOnInit(): void {
    // this.members$ = this.memberService.getMembers();
    this.loadMembers();
  }

  // Loads the members
  loadMembers() {
    this.memberService.getMembers(this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        if (response.result && response.pagination) {
          this.members = response.result;
          this.pagination = response.pagination;
        }
      },
    });
  }

  // Changing the page function
  // Takes in an event
  pageChanged(event: any) {
    // This check stops any bugs happening with requests
    if (this.pageNumber !== event.page) {
      this.pageNumber = event.page;
      this.loadMembers();
    }
  }
}
