import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';

import { StudentSessionService } from '../session/student-session.service';

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const session = inject(StudentSessionService).session();
  if (!session?.accessToken) {
    return next(request);
  }

  return next(
    request.clone({
      setHeaders: {
        Authorization: `Bearer ${session.accessToken}`
      }
    })
  );
};
