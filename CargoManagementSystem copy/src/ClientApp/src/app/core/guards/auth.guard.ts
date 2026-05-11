import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    const userRole = authService.getUserRole();

    // If the user is a Customer and trying to access admin routes, redirect to user dashboard
    if (userRole === 'Customer') {
      const adminPaths = ['/dashboard', '/shipments', '/fleet', '/drivers', '/warehouse', '/billing', '/reports', '/users', '/customers'];
      const isAdminRoute = adminPaths.some(p => state.url === p || state.url.startsWith(p + '/') || state.url.startsWith(p + '?'));
      if (isAdminRoute) {
        router.navigate(['/user/dashboard']);
        return false;
      }
    }

    const requiredRole = route.data['role'] as string;
    if (requiredRole) {
      if (userRole !== requiredRole && userRole !== 'Admin' && userRole !== 'SuperAdmin') {
        router.navigate(['/user/dashboard']);
        return false;
      }
    }

    // Check for roles array (used in admin child routes)
    const requiredRoles = route.data['roles'] as string[];
    if (requiredRoles && requiredRoles.length > 0) {
      if (!requiredRoles.includes(userRole) && userRole !== 'SuperAdmin') {
        router.navigate(['/user/dashboard']);
        return false;
      }
    }

    return true;
  }

  router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
  return false;
};
