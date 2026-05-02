import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { AuthResponse, User } from '../models/user.model';
import { ApiResponse } from '../models/api-response.model';
import { tap, catchError } from 'rxjs/operators';
import { throwError, Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  
  private apiUrl = `${environment.apiUrl}/auth`;

  // State
  private currentUserSignal = signal<User | null>(null);
  private accessTokenSignal = signal<string | null>(null);
  private refreshTokenSignal = signal<string | null>(null);

  // Computed
  public currentUser = computed(() => this.currentUserSignal());
  public isAuthenticated = computed(() => !!this.accessTokenSignal());
  public userRoles = computed(() => this.currentUserSignal()?.roles || []);

  constructor() {
    this.loadTokens();
  }

  private loadTokens() {
    const token = localStorage.getItem('accessToken');
    const refresh = localStorage.getItem('refreshToken');
    const userStr = localStorage.getItem('user');

    if (token && refresh && userStr) {
      try {
        if (userStr === 'undefined' || userStr === 'null') {
           this.logout();
           return;
        }
        const user = JSON.parse(userStr);
        this.accessTokenSignal.set(token);
        this.refreshTokenSignal.set(refresh);
        this.currentUserSignal.set(user);
      } catch (e) {
        console.error('AuthService: Failed to parse user session', e);
        this.logout();
      }
    }
  }

  public getAccessToken(): string | null {
    return this.accessTokenSignal();
  }

  public getRefreshToken(): string | null {
    return this.refreshTokenSignal();
  }

  public refreshTokens(): Observable<ApiResponse<AuthResponse>> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) return throwError(() => new Error('No refresh token available'));

    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/refresh`, { refreshToken }).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      }),
      catchError(err => {
        this.logout();
        return throwError(() => err);
      })
    );
  }

  public login(credentials: any): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      })
    );
  }

  public register(userDto: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiUrl}/users`, userDto);
  }

  public logout() {
    const refreshToken = this.getRefreshToken();
    if (refreshToken) {
      this.http.post(`${this.apiUrl}/logout`, { refreshToken }).subscribe();
    }

    this.accessTokenSignal.set(null);
    this.refreshTokenSignal.set(null);
    this.currentUserSignal.set(null);
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    this.router.navigate(['/auth/login']);
  }

  private setSession(authResult: AuthResponse) {
    console.log('AuthService: Setting session with result:', authResult);
    if (!authResult || !authResult.accessToken || !authResult.user) {
      console.error('AuthService: Invalid auth result structure. Missing accessToken or user.', {
        hasToken: !!authResult?.accessToken,
        hasUser: !!authResult?.user,
        result: authResult
      });
      return;
    }

    this.accessTokenSignal.set(authResult.accessToken);
    this.refreshTokenSignal.set(authResult.refreshToken);
    this.currentUserSignal.set(authResult.user);

    localStorage.setItem('accessToken', authResult.accessToken);
    localStorage.setItem('refreshToken', authResult.refreshToken);
    localStorage.setItem('user', JSON.stringify(authResult.user));
  }
  
  public hasRole(role: string): boolean {
    const roles = this.userRoles();
    return roles.includes('SuperAdmin') || roles.includes(role);
  }

  public forgotPassword(email: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/forgot-password`, { email });
  }

  public resetPassword(data: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/reset-password`, data);
  }
}
