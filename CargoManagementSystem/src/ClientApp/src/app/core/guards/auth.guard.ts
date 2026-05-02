import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    // Check if route requires specific role
    const requiredRoles = route.data['roles'] as Array<string>;
    if (requiredRoles) {
      const hasRole = requiredRoles.some(role => authService.hasRole(role));
      if (!hasRole) {
        router.navigate(['/unauthorized']);
        return false;
      }
    }
    return true;
  }

  // Not logged in, redirect to login page with the return url
  router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
