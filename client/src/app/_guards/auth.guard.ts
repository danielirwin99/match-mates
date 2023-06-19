import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { map } from 'rxjs';

// Our Client Error Handler
export const authGuard = () => {
  // Injecting our Services to use below
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  // Routing to our Account Service
  return accountService.currentUser$.pipe(
    // Mapping over the user
    map((user) => {
      // If there is a User return as normal
      if (user) return true;
      // If there is no User --> return this
      else {
        toastr.error('You shall not pass !');
        return false;
      }
    })
  );
};
