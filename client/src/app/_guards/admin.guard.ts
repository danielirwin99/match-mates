import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

// This stops "Member" role or any Unauthorised role enter the route

export const adminGuard: CanActivateFn = () => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  return accountService.currentUser$.pipe(
    map((user) => {
      if (!user) return false;
      if (user.roles.includes('Admin') || user.roles.includes('Moderator')) {
        return true;
      } else {
        toastr.error('You do not have permissions to enter this area');
        return false;
      }
    })
  );
};
