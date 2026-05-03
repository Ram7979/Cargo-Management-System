import { HttpInterceptorFn, HttpErrorResponse, HttpEvent, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, filter, take } from 'rxjs/operators';
import { throwError, BehaviorSubject, Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

let isRefreshing = false;
const refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

// Paths that should NOT trigger snackbar on 401 (login handles its own errors)
const AUTH_PATHS = ['/auth/login', '/auth/refresh', '/auth/logout', '/auth/forgot-password', '/auth/reset-password'];

export const errorInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn): Observable<HttpEvent<unknown>> => {
  const authService = inject(AuthService);
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthPath = AUTH_PATHS.some(p => req.url.includes(p));

      if (error.status === 401 && !isAuthPath) {
        return handle401Error(req, next, authService);
      }

      // Don't show snackbar for auth paths — let the component handle it
      if (isAuthPath) {
        return throwError(() => error);
      }

      let errorMessage = 'An unknown error occurred!';
      if (error.error instanceof ErrorEvent) {
        errorMessage = `Error: ${error.error.message}`;
      } else if (error.error) {
        // Try to extract structured error from backend ApiResponse
        if (error.error.errors && Array.isArray(error.error.errors) && error.error.errors.length > 0) {
          errorMessage = error.error.errors.join(', ');
        } else if (error.error.message) {
          errorMessage = error.error.message;
        } else if (typeof error.error === 'string') {
          errorMessage = error.error;
        } else {
          errorMessage = `Error ${error.status}: ${error.statusText}`;
        }
      } else {
        errorMessage = `Error ${error.status}: ${error.message}`;
      }

      // Don't show snackbar for 403 on page load (role-based access)
      if (error.status !== 403) {
        snackBar.open(errorMessage, 'Close', {
          duration: 5000,
          horizontalPosition: 'right',
          verticalPosition: 'top',
          panelClass: ['error-snackbar']
        });
      }

      return throwError(() => error);
    })
  );
};

function handle401Error(req: HttpRequest<unknown>, next: HttpHandlerFn, authService: AuthService): Observable<HttpEvent<unknown>> {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    return authService.refreshTokens().pipe(
      switchMap((response) => {
        isRefreshing = false;
        const newToken = response.accessToken || '';
        refreshTokenSubject.next(newToken);
        
        return next(req.clone({
          setHeaders: {
            Authorization: `Bearer ${newToken}`
          }
        }));
      }),
      catchError((err) => {
        isRefreshing = false;
        authService.logout();
        return throwError(() => err);
      })
    );
  } else {
    return refreshTokenSubject.pipe(
      filter(token => token !== null),
      take(1),
      switchMap((token) => {
        return next(req.clone({
          setHeaders: {
            Authorization: `Bearer ${token}`
          }
        }));
      })
    );
  }
}
