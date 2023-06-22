import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, take } from 'rxjs';
import { AccountService } from '../_services/account.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    // When we have what we get back from our AccountService --> Will complete and stop consuming resources
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: (user) => {
        // If we have the user to see if its null
        if (user) {
          // If we DO have the user we want to pass through the user.token
          request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`,
            },
          });
        }
      },
    });
    return next.handle(request);
  }
}
