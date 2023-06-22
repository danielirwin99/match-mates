import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  // Type of Member from member.ts interface
  members: Member[] = [];
  constructor(private memberService: MembersService) {}

  // When we call our members we want to load it onto the component
  ngOnInit(): void {
    this.loadMembers();
  }

  // Grabbing the members from members.service
  loadMembers() {
    // Our API request function
    this.memberService.getMembers().subscribe({
      next: (members) => (this.members = members),
    });
  }
}
