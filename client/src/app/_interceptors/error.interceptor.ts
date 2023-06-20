import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Injectable()
// Handles HTTP Requests / Responses
export class ErrorInterceptor implements HttpInterceptor {
  // We are using router just in case we need to redirect the User to another page from an error
  // Toaster is used for the notification for the errors
  constructor(private router: Router, private toastr: ToastrService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {
    // To transform or modify observables we need to use the pipe method
    return next.handle(request).pipe(
      // Using a built in error from rxjs --> We can grab any errors
      // We need to give the error a type so it knows what we want
      catchError((error: HttpErrorResponse) => {
        // If there is an error
        if (error) {
          // Switch allows us to change the control of where it goes
          switch (error.status) {
            case 400:
              // This is error.error inside our API Response --> Its also an array of objects
              if (error.error.errors) {
                // Create an empty array to store the result inside (Display it to the user)
                const modelStateErrors = [];

                // For in loop to find the key
                for (const key in error.error.errors) {
                  // Accessing the key
                  if (error.error.errors[key]) {
                    // IF we DO have the KEY --> Push it
                    modelStateErrors.push(error.error.errors[key]);
                  }
                }
                // Throw the error after we've found it --> Flat turns 2 arrays into 1
                throw modelStateErrors.flat();
              } else {
                this.toastr.error(error.error, error.status.toString());
              }
              break;

            // -----------------------------------------------------
            // break --> Stops the execution of the switch statement
            // -----------------------------------------------------

            // When 401 shows
            case 401:
              this.toastr.error('Unauthorised', error.status.toString());
              break;

            // When 404 shows
            case 404:
              this.router.navigateByUrl('/not-found');
              break;

            // When 500 shows
            case 500:
              // Setting this equal to the error response
              const navigationExtras: NavigationExtras = {
                // Router State
                state: { error: error.error },
              };
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;

            // IF none of these errors above are triggered then show this and console.log() it
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        throw error;
      })
    );
  }
}
