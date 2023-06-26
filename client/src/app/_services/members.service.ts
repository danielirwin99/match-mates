import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  // Pulled from our environment file
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) {}

  // Getting a list of members
  getMembers() {
    // If we have loaded a member already (greater than 0) --> return an observable of the members
    if (this.members.length > 0) return of(this.members);
    // Pulling through our Interface Type List from member.ts
    // Adding on our endpoint to the url as an array of users
    return this.http.get<Member[]>(this.baseUrl + 'users').pipe(
      // Maps over the members to display them
      map((members) => {
        this.members = members;
        return members;
      })
    );
  }

  // Getting a individual member
  getMember(username: string) {
    // Finds the username that we clicked on
    const member = this.members.find((x) => x.userName === username);
    // If do have the member then return an observable of it
    if (member) return of(member);
    return this.http.get<Member>(
      // Going inside our API URL to grab the username with Authorization of the http
      this.baseUrl + 'users/' + username
    );
  }

  // Updating the Member from API to Client
  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      map(() => {
        // Tells us the index of the elements of the member in this members array
        const index = this.members.indexOf(member);
        // Accessing the member at the index we received
        // Spread operator spreads all the elements of the member
        // i.e. id, name, city
        this.members[index] = { ...this.members[index], ...member };
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId:number) {
    return this.http.delete(this.baseUrl + "users/delete-photo/" + photoId, {});
  }
}
