import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { ToastrModule } from 'ngx-toastr';

// Stored Modules to remove clutter in app.module

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    BsDropdownModule.forRoot(),
    // Our Client Error
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
    }),
  ],
  exports: [BsDropdownModule, ToastrModule],
})
export class SharedModule {}
