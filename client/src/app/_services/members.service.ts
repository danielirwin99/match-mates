import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  // Pulled from our environment file
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Getting a list of members
  getMembers() {
    // Pulling through our Interface Type List from member.ts
    // Adding on our endpoint to the url
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  // Getting a individual member
  getMember(username: string) {
    return this.http.get<Member>(
      // Going inside our API URL to grab the username with Authorization of the http
      this.baseUrl + 'users/' + username
    );
  }
}
