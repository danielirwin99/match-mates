import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, RouteReuseStrategy, Router } from '@angular/router';
import {
  NgxGalleryAnimation,
  NgxGalleryImage,
  NgxGalleryOptions,
} from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  // When we want to click each tab and load it individually rather than load all of them at once
  @ViewChild('memberTabs', { static: true }) memberTabs?: TabsetComponent;
  // Member could be a member type interface or undefined (not a member)
  member: Member = {} as Member;
  // Our third party gallery package
  galleryOptions: NgxGalleryOptions[] = [];
  galleryImages: NgxGalleryImage[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;

  // ActivatedRoute --> When a user clicks on the link it will activate the route
  // We can access the route parameter from this
  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService,
    public presenceService: PresenceService,
    private accountService: AccountService,
    private routeReuseStrategy: RouteReuseStrategy
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) this.user = user;
      },
    });
    // Updates the messages when you click the message notification
    this.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: (data) => (this.member = data['member']),
    });

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

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
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
    if (this.activeTab.heading === 'Messages' && this.user) {
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();
    }
  }
}
