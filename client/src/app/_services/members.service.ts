import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  // Pulled from our environment file
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(private http: HttpClient) {}

  // Getting a list of members
  getMembers(page?: number, itemsPerPage?: number) {
    // HttpParams --> Allows us to set query string parameters along with our HTTP Request
    let params = new HttpParams();

    if (page && itemsPerPage) {
      // If we do have the page and itemsPerPage --> We want to set a query string that goes along with the Request
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    // Pulling through our Interface Type List from member.ts
    // Adding on our endpoint to the url as an array of users
    // We want to it to observe our response and pass up the params to the url body
    return this.http
      .get<Member[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map((response) => {
          // If we do get the response.body back
          if (response.body) {
            // Linking our response to the members
            this.paginatedResult.result = response.body;
          }
          // Accessing our Server Headers Response
          const pagination = response.headers.get('Pagination');
          if (pagination) {
            // Turning our result into an object
            this.paginatedResult.pagination = JSON.parse(pagination);
          }
          return this.paginatedResult;
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

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId, {});
  }
}
