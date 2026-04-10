import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

import { StudentSessionService } from './student-session.service';

export const portalGuard: CanActivateFn = () => {
  const session = inject(StudentSessionService).session();
  if (session) {
    return true;
  }

  return inject(Router).createUrlTree(['/login']);
};
