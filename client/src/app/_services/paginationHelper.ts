// ----------------------------------
// This is a shared service type file
//-----------------------------------

import { HttpClient, HttpParams } from '@angular/common/http';
import { map } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
// Pulling through our Interface Type List from member.ts
// Adding on our endpoint to the url as an array of users
// We want to it to observe our response and pass up the params to the url body
export function getPaginatedResult<T>(
  url: string,
  params: HttpParams,
  http: HttpClient
) {
  const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
  return http.get<T>(url, { observe: 'response', params }).pipe(
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

export function getPaginationHeaders(pageNumber: number, pageSize: number) {
  // HttpParams --> Allows us to set query string parameters along with our HTTP Request
  let params = new HttpParams();

  // If we do have the pageNumber and pageSize --> We want to set a query string that goes along with the Request
  params = params.append('pageNumber', pageNumber);
  params = params.append('pageSize', pageSize);
  return params;
}
