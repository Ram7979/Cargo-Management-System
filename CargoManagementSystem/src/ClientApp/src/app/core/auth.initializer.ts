import { AuthService } from './services/auth.service';

export function authInitializer(authService: AuthService) {
  return () => {
    const refreshToken = localStorage.getItem('refreshToken');
    const accessToken = localStorage.getItem('accessToken');

    // If we have a refresh token but no access token (or it's about to be checked), try refreshing
    if (refreshToken && !accessToken) {
      return new Promise((resolve) => {
        authService.refreshTokens().subscribe({
          next: () => resolve(true),
          error: () => resolve(false)
        });
      });
    }
    
    return Promise.resolve(true);
  };
}
