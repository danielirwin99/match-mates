import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  // Our Member can be a type or undefined
  @Input() member: Member | undefined;

  constructor(
    private memberService: MembersService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
  }

  // Likes function
  addLike(member: Member) {
    // Adding the like to the username that we click on
    this.memberService.addLike(member.userName).subscribe({
      // Prompt alert
      next: () => this.toastr.success('You have liked ' + member.knownAs),
    });
  }
}
