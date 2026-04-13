import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { StudentSessionService } from '../session/student-session.service';

export const errorInterceptor: HttpInterceptorFn = (request, next) => {
  const sessionService = inject(StudentSessionService);
  const router = inject(Router);

  return next(request).pipe(
    catchError((error) => {
      console.error('[HTTP]', {
        method: request.method,
        url: request.url,
        error
      });

      if (error.status === 401) {
        sessionService.clearSession();
        void router.navigate(['/login']);
      }

      return throwError(() => error);
    })
  );
};
