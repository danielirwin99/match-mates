import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  // Member could be a member type interface or undefined (not a member)
  member: Member | undefined;

  // ActivatedRoute --> When a user clicks on the link it will activate the route
  // We can access the route parameter from this
  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadMember();
  }

  loadMember() {
    // Combining the member to the route of the username
    const username = this.route.snapshot.paramMap.get('username');

    // We need to check the username
    if (!username) return;

    // Now get the Member and push it to the route
    this.memberService.getMember(username).subscribe({
      next: (member) => (this.member = member),
    });
  }
}
