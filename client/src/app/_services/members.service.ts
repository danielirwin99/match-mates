import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  // Pulled from our environment file
  baseUrl = environment.apiUrl;
  members: Member[] = [];

  constructor(private http: HttpClient) {}

  // Getting a list of members
  getMembers(userParams: UserParams) {
    // Our HTTP params are populated from here
    let params = this.getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    // Our params
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    // Sorting on the buttons
    params = params.append('orderBy', userParams.orderBy);

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'users', params);
  }

  // Pulling through our Interface Type List from member.ts
  // Adding on our endpoint to the url as an array of users
  // We want to it to observe our response and pass up the params to the url body
  private getPaginatedResult<T>(url: string, params: HttpParams) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map((response) => {
        // If we do get the response.body back
        if (response.body) {
          // Linking our response to the members
          paginatedResult.result = response.body;
        }
        // Accessing our Server Headers Response
        const pagination = response.headers.get('Pagination');
        if (pagination) {
          // Turning our result into an object
          paginatedResult.pagination = JSON.parse(pagination);
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    // HttpParams --> Allows us to set query string parameters along with our HTTP Request
    let params = new HttpParams();

    // If we do have the pageNumber and pageSize --> We want to set a query string that goes along with the Request
    params = params.append('pageNumber', pageNumber);
    params = params.append('pageSize', pageSize);
    return params;
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
