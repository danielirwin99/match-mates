import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { map, of } from 'rxjs';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  // Pulled from our environment file
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  // This is to store a key-value pair --> Gives us access to get and set properties
  memberCache = new Map();
  user: User | undefined;
  userParams: UserParams | undefined;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    // Making an observable request
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      // If we get our user back from the request
      next: (user) => {
        if (user) {
          this.userParams = new UserParams(user);
          this.user = user;
        }
      },
    });
  }

  // This is to store our desired params after we change pages
  getUserParams() {
    return this.userParams;
  }

  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  // Resetting the User Params Service to use elsewhere
  resetUserParams() {
    if (this.user) {
      // If we do have the User --> Set it to their params selected when reset
      this.userParams = new UserParams(this.user);
      return this.userParams;
    }
    return;
  }

  // Getting a list of members
  getMembers(userParams: UserParams) {
    // Getting our key for the cache
    const response = this.memberCache.get(Object.values(userParams).join('-'));

    // If we do have something inside the response i.e a key
    if (response) return of(response);

    // Our HTTP params are populated from here
    let params = getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );

    // Our params
    params = params.append('minAge', userParams.minAge);
    params = params.append('maxAge', userParams.maxAge);
    params = params.append('gender', userParams.gender);
    // Sorting on the buttons
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(
      this.baseUrl + 'users',
      params,
      this.http
    ).pipe(
      // Using the pipe(map) method we can project the results of our response and set them inside our member cache (see response above)
      // Purpose: Saving our cache so we don't have to request to the API every single refresh after have requested it once
      map((response) => {
        // Setting the response to the result of this and then returning it
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }

  // Getting a individual member
  getMember(username: string) {
    // Using the reduce method we can turn the result into ONE flattened array
    const member = [...this.memberCache.values()]
      // Empty array is our initial value
      .reduce((arr, elem) => arr.concat(elem.result), [])
      // Finding the Member to see if its store in our cache already
      .find((member: Member) => member.userName === username);

    // If it is stored in our cache already return it without HTTP / API request
    if (member) return of(member);

    // If it isn't stored --> return it with API Request
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

  // Setting Main Photo request
  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  // Deleting photo request
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photoId, {});
  }

  // Add Like Request
  // Taking in the username that is about to be liked as a parameter
  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  // Getting our likes request
  getLikes(predicate: string, pageNumber: number, pageSize: number) {
    // Passing in our PaginationHeaders to use for pages on our List page for the likes
    let params = getPaginationHeaders(pageNumber, pageSize);

    params = params.append('predicate', predicate);

    // Add type of Member[] to return more than just an object (contains the properties)
    return getPaginatedResult<Member[]>(
      this.baseUrl + 'likes',
      params,
      this.http
    );
  }
}
