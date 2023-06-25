import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

// When we update our changes we want this guard to trigger
// It basically prompts an alert to warn the User what happens

export const preventUnsavedChangesGuard: CanDeactivateFn<
  MemberEditComponent
> = (component: MemberEditComponent): boolean => {
  if (component.editForm?.dirty) {
    return confirm(
      'Are you sure you want to continue? Any unsaved changes will be lost'
    );
  }
  return true;
};
