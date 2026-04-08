import { HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(
    catchError((error) => {
      console.error('[HTTP]', {
        method: request.method,
        url: request.url,
        error
      });

      return throwError(() => error);
    })
  );

