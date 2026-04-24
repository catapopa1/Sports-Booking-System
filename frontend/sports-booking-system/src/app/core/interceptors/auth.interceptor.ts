import { HttpInterceptorFn } from '@angular/common/http';
import { AuthService } from '../services/auth';
import { inject } from '@angular/core';


export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).token();
  
  if (!token)
    return next(req);

  return next(req.clone({
    setHeaders: { Authorization: `Bearer ${token}}` }
  }))
};
