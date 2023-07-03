import {
  Directive,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]', // *appHasRole="['Admin', 'Moderator']"
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  user: User = {} as User;

  constructor(
    private viewContainerRef: ViewContainerRef,
    private templateRef: TemplateRef<any>,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      // If we get the user set it to the user
      next: (user) => {
        if (user) this.user = user;
      },
    });
  }

  // On load of the browser
  ngOnInit(): void {
    // If the user has the roles that are required to access / view it
    if (this.user.roles.some((r) => this.appHasRole.includes(r))) {
      // Display this if we do have the role authorisation
      this.viewContainerRef.createEmbeddedView(this.templateRef);

      // If we don't have the appropriate role --> Clear the component from viewing
    } else {
      this.viewContainerRef.clear();
    }
  }
}
