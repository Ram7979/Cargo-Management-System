import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    const role = authService.getUserRole();
    // Redirect Customer users to their own dashboard
    if (role === 'Customer') {
      router.navigate(['/user/dashboard']);
    } else {
      router.navigate(['/dashboard']);
    }
    return false;
  }

  return true;
};
