import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
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
  // Our third party gallery package
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];

  // ActivatedRoute --> When a user clicks on the link it will activate the route
  // We can access the route parameter from this
  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadMember();
    // Styling for the Image Gallery
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        // See the full image
        imagePercent: 100,
        // How many thumbnails underneath the main image
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false,
      },
    ];
  }

  getImages() {
    // If there is no member just exit out
    if (!this.member) return [];
    const imageUrls = [];
    // We need to loop over the photos in member.photos
    for (const photo of this.member.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
      });
    }
    // Pushes it to the client (Website)
    return imageUrls;
  }

  loadMember() {
    // Combining the member to the route of the username
    const username = this.route.snapshot.paramMap.get('username');

    // We need to check the username
    if (!username) return;

    // Now get the Member and push it to the route
    this.memberService.getMember(username).subscribe({
      next: (member) => {
        (this.member = member),
          // Syncing the styling above to our images from the member (see below)
          (this.galleryImages = this.getImages());
      },
    });
  }
}
