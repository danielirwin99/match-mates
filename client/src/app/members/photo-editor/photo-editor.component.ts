import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { environment } from 'src/environments/environment';
import { take } from 'rxjs';
import { Photo } from 'src/app/_models/photo';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  @Input() member: Member | undefined;
  // This is for our HTML
  uploader: FileUploader | undefined;

  hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;
  user: User | undefined;

  constructor(
    private accountService: AccountService,
    // Pulling this through for our main photo
    private memberService: MembersService
  ) {
    // Setting our User property
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        if (user) {
          this.user = user;
        }
      },
    });
  }
  ngOnInit(): void {
    // Allows the function below to work in the HTML component
    this.initializeUploader();
  }

  // Our Set Main Photo Functionality
  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo.id).subscribe({
      next: () => {
        if (this.user && this.member) {
          this.user.photoUrl = photo.url;
          // Updates the main photo for all components displaying it (i.e the navbar)
          this.accountService.setCurrentUser(this.user);
          this.member.photoUrl = photo.url;
          this.member.photos.forEach((p) => {
            if (p.isMain) p.isMain = false;
            if (p.id === photo.id) p.isMain = true;
          });
        }
      },
    });
  }

  // Delete Photo Function
  deletePhoto(photoId: number) {
    // Pulling our Http Delete from memberService
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        // If this.member = true
        if (this.member) {
          // Filter to the photo that matches the id
          this.member.photos = this.member.photos.filter(
            (x) => x.id !== photoId
          );
        }
      },
    });
  }

  // Event
  fileOverBase(e: any) {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      // Adds the Photo
      url: this.baseUrl + 'users/add-photo',
      // Adds the token
      authToken: 'Bearer ' + this.user?.token,
      isHTML5: true,
      // Accepts all image types e.g. jpeg, png etc
      allowedFileType: ['image'],
      // Removes the photo after upload so it doesn't stay there
      removeAfterUpload: true,
      // The user must click upload photo
      autoUpload: false,
      // Max that Cloudinary can take on free subscription (10MB)
      maxFileSize: 10 * 1024 * 1024,
    });

    // Avoids the cors configuration
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      // If we do get a response --> parse it
      if (response) {
        const photo = JSON.parse(response);
        // Pushes the photo to the member photos collection
        this.member?.photos.push(photo);
      }
    };
  }
}
