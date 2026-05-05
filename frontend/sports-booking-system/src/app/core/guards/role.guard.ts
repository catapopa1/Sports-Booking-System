import { CanActivateFn,Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core'; 

export const roleGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const required = route.data['role'] as string;

  if (!auth.isLoggedIn())
    return router.createUrlTree(['/login']);

  if(auth.role() !== required)
    return router.createUrlTree(['/']);

  return true;
};
