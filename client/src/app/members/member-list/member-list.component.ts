import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  // Type of Member from member.ts interface
  // Member is now an observable of the Members
  members$: Observable<Member[]> | undefined;

  constructor(private memberService: MembersService) {}

  // When we call our members we want to load it onto the component
  ngOnInit(): void {
    this.members$ = this.memberService.getMembers();
  }
}
