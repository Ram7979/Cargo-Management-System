import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthResponse, UserProfile } from '../models/user.model';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = `${environment.apiUrl}/auth`;

  // State
  private currentUserSubject = new BehaviorSubject<UserProfile | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  // Keep signals for template compatibility and performance
  private currentUserSignal = signal<UserProfile | null>(null);
  public currentUser = computed(() => this.currentUserSignal());

  constructor() {
    this.loadSession();
  }

  private loadSession() {
    const userStr = sessionStorage.getItem('user');
    if (userStr && userStr !== 'undefined' && userStr !== 'null') {
      try {
        const user = JSON.parse(userStr);
        this.currentUserSubject.next(user);
        this.currentUserSignal.set(user);
      } catch (e) {
        this.logout();
      }
    }
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap(response => {
        this.setSession(response);
      }),
      catchError(this.handleError)
    );
  }

  // Alias for compatibility
  refreshTokens(): Observable<AuthResponse> {
    return this.refresh();
  }

  refresh(): Observable<AuthResponse> {
    const refreshToken = this.getRefreshToken();
    return this.http.post<AuthResponse>(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap(response => {
        this.setSession(response);
      })
    );
  }

  private setSession(authResult: AuthResponse) {
    sessionStorage.setItem('accessToken', authResult.accessToken);
    sessionStorage.setItem('refreshToken', authResult.refreshToken);
    sessionStorage.setItem('user', JSON.stringify(authResult.user));
    this.currentUserSubject.next(authResult.user);
    this.currentUserSignal.set(authResult.user);
  }

  isLoggedIn(): boolean {
    return !!sessionStorage.getItem('accessToken');
  }

  // Alias for compatibility
  isAuthenticated(): boolean {
    return this.isLoggedIn();
  }

  getUserRole(): string {
    return this.currentUserSubject.value?.role || '';
  }

  hasRole(role: string): boolean {
    const userRole = this.getUserRole();
    if (userRole === 'SuperAdmin') return true;
    return userRole === role;
  }

  isStrictRole(role: string): boolean {
    return this.getUserRole() === role;
  }

  logout() {
    sessionStorage.removeItem('accessToken');
    sessionStorage.removeItem('refreshToken');
    sessionStorage.removeItem('user');
    this.currentUserSubject.next(null);
    this.currentUserSignal.set(null);
    this.router.navigate(['/auth/login']);
  }

  forgotPassword(email: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/forgot-password`, { email }).pipe(
      catchError(this.handleError)
    );
  }

  resetPassword(data: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/reset-password`, data).pipe(
      catchError(this.handleError)
    );
  }

  register(dto: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, dto).pipe(
      catchError(this.handleError)
    );
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem('accessToken');
  }

  getRefreshToken(): string | null {
    return sessionStorage.getItem('refreshToken');
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unexpected error occurred.';
    
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Cannot connect. Check your connection.`;
    } else {
      if (error.status === 401) {
        errorMessage = 'Invalid email or password.';
      } else if (error.status === 403) {
        errorMessage = error.error?.message || 'Access denied.';
      } else if (error.status === 500) {
        errorMessage = 'Server error, please try again.';
      } else {
        errorMessage = error.error?.message || 'Something went wrong. Please try again later.';
      }
    }
    
    return throwError(() => errorMessage);
  }
}
