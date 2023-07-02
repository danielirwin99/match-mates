import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit {
  // When we want to click each tab and load it individually rather than load all of them at once
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  // Member could be a member type interface or undefined (not a member)
  member: Member | undefined;
  // Our third party gallery package
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];

  // ActivatedRoute --> When a user clicks on the link it will activate the route
  // We can access the route parameter from this
  constructor(
    private memberService: MembersService,
    private route: ActivatedRoute,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadMember();

    this.route.queryParams.subscribe({
      // We have access to our params here
      next: (params) => {
        // First part is checking to see if we have the tab parameter --> if we do then we want to use it
        params['tab'] && this.selectTab(params['tab']);
      },
    });
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

  selectTab(heading: string) {
    if (this.memberTabs) {
      // Tabs returns an array so we can use .find to look for heading that is equal to the one we are looking for
      this.memberTabs.tabs.find((x) => x.heading === heading)!.active = true;
    }
  }

  loadMessages() {
    // If we do have the username / member
    if (this.member) {
      // Pulling through our API Request
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: (messages) => (this.messages = messages),
      });
    }
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    // When we click on Messages heading THEN we load the messages
    if (this.activeTab.heading === 'Messages') {
      this.loadMessages();
    }
  }
}
