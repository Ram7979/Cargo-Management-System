import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, MatListModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule],
  template: `
    <div class="notifications-container">
      <div class="header">
        <h2>Notifications</h2>
        <button mat-button color="primary" (click)="markAllRead()" [disabled]="notifications.length === 0">Mark all as read</button>
      </div>

      <div class="loading-overlay" *ngIf="loading">
        <mat-progress-spinner mode="indeterminate" diameter="40"></mat-progress-spinner>
      </div>

      <div class="notifications-list" *ngIf="!loading">
        <div class="notification-card" *ngFor="let n of notifications" [class.unread]="n.status === 'Unread'" (click)="markRead(n)">
          <div class="icon-box" [ngClass]="n.eventType">
            <mat-icon>{{getIcon(n.eventType)}}</mat-icon>
          </div>
          <div class="content">
            <div class="title">{{n.subject || n.title || n.eventType}}</div>
            <div class="message">{{n.body || n.message}}</div>
            <div class="time" *ngIf="n.eventType !== 'PaymentReceived'">{{n.createdAt | date:'medium'}}</div>
          </div>
          <div class="actions" *ngIf="n.status === 'Unread'">
            <div class="unread-dot"></div>
          </div>
        </div>

        <div class="empty-state" *ngIf="notifications.length === 0">
          <mat-icon>notifications_off</mat-icon>
          <p>You're all caught up!</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .notifications-container { max-width: 800px; margin: 0 auto; display: flex; flex-direction: column; gap: 24px; min-height: 400px; }
    .header { display: flex; justify-content: space-between; align-items: center; }
    
    .loading-overlay { display: flex; justify-content: center; padding: 40px; }
    .notifications-list { display: flex; flex-direction: column; gap: 12px; }
    .notification-card {
      display: flex; gap: 20px; padding: 20px;
      background: #161B22; border-radius: 16px; border: 1px solid rgba(255,255,255,0.05);
      transition: all 0.2s ease; cursor: pointer;
    }
    .notification-card:hover { background: #1C2128; border-color: rgba(255,255,255,0.1); }
    .notification-card.unread { border-left: 4px solid #3B82F6; }

    .icon-box {
      width: 48px; height: 48px; border-radius: 12px;
      display: flex; align-items: center; justify-content: center;
      background: rgba(148, 163, 184, 0.1); color: #94A3B8;
      &.ShipmentUpdate, &.WarehouseArrival { background: rgba(59, 130, 246, 0.1); color: #3B82F6; }
      &.Invoice, &.PaymentReceived { background: rgba(34, 197, 94, 0.1); color: #22C55E; }
    }

    .content { flex: 1; }
    .title { font-weight: 600; font-size: 1.1rem; color: #F8FAFC; margin-bottom: 4px; }
    .message { color: #94A3B8; font-size: 0.9rem; line-height: 1.4; }
    .time { color: #64748B; font-size: 0.8rem; margin-top: 8px; }
    
    .unread-dot { width: 8px; height: 8px; border-radius: 50%; background: #3B82F6; box-shadow: 0 0 8px #3B82F6; }
    .empty-state { padding: 80px; text-align: center; color: #64748B; mat-icon { font-size: 48px; width: 48px; height: 48px; margin-bottom: 16px; } }
  `]
})
export class NotificationsComponent implements OnInit {
  private notificationService = inject(NotificationService);

  notifications: any[] = [];
  loading = true;

  ngOnInit() {
    this.loadNotifications();
  }

  loadNotifications() {
    this.loading = true;
    this.notificationService.getMyNotifications().subscribe({
      next: (res) => {
        this.notifications = Array.isArray(res.data) ? res.data : (res.data?.items || res.data?.data || []);
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  getIcon(type: string): string {
    if (type?.includes('Shipment')) return 'local_shipping';
    if (type?.includes('Invoice')) return 'receipt';
    if (type?.includes('Warehouse')) return 'inventory_2';
    return 'notifications';
  }

  markRead(n: any) {
    if (n.status === 'Unread') {
      this.notificationService.markAsRead(n.id).subscribe(() => {
        n.status = 'Read';
      });
    }
  }

  markAllRead() {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.forEach(n => n.status = 'Read');
    });
  }
}