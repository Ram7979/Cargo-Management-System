import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MatSnackBar } from '@angular/material/snack-bar';

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: 'Info' | 'Success' | 'Warning' | 'Error';
  isRead: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private http = inject(HttpClient);
  private snackBar = inject(MatSnackBar);
  private apiUrl = `${environment.apiUrl}/notifications`;

  notifications = signal<Notification[]>([]);
  unreadCount = signal<number>(0);

  loadNotifications(): void {
    this.http.get<any>(`${this.apiUrl}/my?page=1&pageSize=10`).subscribe({
      next: (res) => {
        if (res && res.data) {
          const items = Array.isArray(res.data) ? res.data : (res.data.items || res.data.data || []);
          this.notifications.set(items);
          // Backend may use 'status' (string: 'Unread'/'Read') or 'isRead' (boolean)
          this.unreadCount.set(items.filter((n: any) => n.status === 'Unread' || n.isRead === false).length);
        } else if (Array.isArray(res)) {
          this.notifications.set(res);
          this.unreadCount.set(res.filter((n: any) => n.status === 'Unread' || n.isRead === false).length);
        }
      },
      error: () => {
        // Silently fail for polling
      }
    });
  }

  success(message: string): void {
    this.snackBar.open(message, 'Close', { 
      duration: 5000, 
      panelClass: ['success-snackbar'],
      horizontalPosition: 'right',
      verticalPosition: 'top'
    });
  }

  error(message: string): void {
    this.snackBar.open(message, 'Close', { 
      duration: 7000, 
      panelClass: ['error-snackbar'],
      horizontalPosition: 'right',
      verticalPosition: 'top'
    });
  }

  info(message: string): void {
    this.snackBar.open(message, 'Close', { 
      duration: 5000,
      horizontalPosition: 'right',
      verticalPosition: 'top'
    });
  }

  warning(message: string): void {
    this.snackBar.open(message, 'Close', { 
      duration: 6000, 
      panelClass: ['warning-snackbar'],
      horizontalPosition: 'right',
      verticalPosition: 'top'
    });
  }

  warn(message: string): void {
    this.warning(message);
  }

  getMyNotifications(page = 1, pageSize = 20): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/my?page=${page}&pageSize=${pageSize}`);
  }

  createNotification(payload: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, payload).pipe(
      tap(() => this.loadNotifications())
    );
  }

  markAsRead(id: string): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/read`, {}).pipe(
      tap(() => this.loadNotifications())
    );
  }

  markAllAsRead(): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/read-all`, {}).pipe(
      tap(() => this.loadNotifications())
    );
  }
}

