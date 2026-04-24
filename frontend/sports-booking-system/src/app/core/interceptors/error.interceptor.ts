import { HttpInterceptorFn,HttpErrorResponse } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { inject } from '@angular/core'
import { AuthService } from '../services/auth';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messages = inject(MessageService);
  const auth = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        auth.logout();
        return throwError(() => error);
      }

      if (error.status === 429) {
        messages.add({ severity: 'warn', summary: 'Slow down', detail: 'Too many requests. Please wait a moment.' });
        return throwError(() => error);
      }

      const detail =
        error.error?.title ||
        error.error?.message ||
        'An unexpected error occurred.';

      messages.add({ severity: 'error', summary: `Error ${error.status}`, detail });

      return throwError(() => error);
    })
  );
  
  return next(req);
};
