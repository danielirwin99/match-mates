import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
} from '@angular/common/http';
import { Observable, delay, finalize } from 'rxjs';
import { LoaderService } from '../_services/loader.service';

@Injectable()
export class LoadingInterceptor implements HttpInterceptor {

  // Passing through our module package
  constructor(private loaderService: LoaderService) {}

  intercept(
    request: HttpRequest<unknown>,
    next: HttpHandler
  ): Observable<HttpEvent<unknown>> {

    // Loading our Spinner Service
    this.loaderService.loader();

    return next.handle(request).pipe(
      // Our Timeout Delay
      delay(1000),
      // Turns OFF the Spinner once the request has been completed
      finalize(() => {
        this.loaderService.idle();
      })
    );
  }
}
