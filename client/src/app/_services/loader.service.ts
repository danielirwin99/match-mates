import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class LoaderService {
  // Our Initial value for the spinner --> When the spinner > 0 --> Display it
  loaderRequestCount = 0;

  // Passing through our module package
  constructor(private spinnerService: NgxSpinnerService) {}

  loader() {
    // Increments the count by 1
    this.loaderRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-scale-party',
      // Background Colour (White)
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333',
    });
  }

  idle() {
    // Decrements the count by 1
    this.loaderRequestCount--;
    // If the Count is 0 or less --> Make it 0 again AND hide the animation spinner
    if (this.loaderRequestCount <= 0) {
      this.loaderRequestCount = 0;
      this.spinnerService.hide();
    }
  }
}
