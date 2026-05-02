import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { MatSnackBar } from '@angular/material/snack-bar';

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: 'Info' | 'Success' | 'Warning' | 'Error';
  isRead: boolean;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private http = inject(HttpClient);
  private snackBar = inject(MatSnackBar);
  private apiUrl = `${environment.apiUrl}/notifications`;

  private notificationsSignal = signal<Notification[]>([]);
  public notifications = computed(() => this.notificationsSignal());
  public unreadCount = computed(() => this.notificationsSignal().filter(n => !n.isRead).length);

  loadNotifications() {
    this.http.get<ApiResponse<Notification[]>>(this.apiUrl).subscribe(res => {
      if (res.success && Array.isArray(res.data)) {
        this.notificationsSignal.set(res.data);
      } else {
        this.notificationsSignal.set([]);
      }
    });
  }

  markAsRead(id: string) {
    this.http.patch<ApiResponse<any>>(`${this.apiUrl}/${id}/read`, {}).subscribe(res => {
      if (res.success) {
        this.notificationsSignal.update(notes => 
          notes.map(n => n.id === id ? { ...n, isRead: true } : n)
        );
      }
    });
  }

  success(message: string) {
    this.snackBar.open(message, 'Close', { duration: 3000, panelClass: ['success-snackbar'] });
  }

  error(message: string) {
    this.snackBar.open(message, 'Close', { duration: 5000, panelClass: ['error-snackbar'] });
  }

  info(message: string) {
    this.snackBar.open(message, 'Close', { duration: 3000 });
  }

  warn(message: string) {
    this.snackBar.open(message, 'Close', { duration: 4000, panelClass: ['warn-snackbar'] });
  }
}
