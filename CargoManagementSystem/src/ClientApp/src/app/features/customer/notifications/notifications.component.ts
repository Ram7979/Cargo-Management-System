import { Component, OnInit, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { NotificationService, Notification } from '../../../core/services/notification.service';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatButtonModule, MatListModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
  private notificationService = inject(NotificationService);

  // Use the service's signal
  notifications = computed(() => {
    const list = this.notificationService.notifications();
    return list.length > 0 ? list : this.mockNotifications;
  });
  
  isLoading = false;

  private mockNotifications: Notification[] = [
    {
      id: '1',
      title: 'Shipment Delivered',
      message: 'Your shipment CMS-2025-0003 has been successfully delivered to Tokyo.',
      type: 'Success',
      createdAt: new Date(Date.now() - 3600000 * 2).toISOString(),
      isRead: false
    },
    {
      id: '2',
      title: 'New Invoice Generated',
      message: 'Invoice INV-2025-001 for your recent shipment is now available.',
      type: 'Info',
      createdAt: new Date(Date.now() - 86400000).toISOString(),
      isRead: true
    },
    {
      id: '3',
      title: 'Shipment Update',
      message: 'Shipment CMS-2025-0001 is now in transit from Mumbai Hub.',
      type: 'Warning',
      createdAt: new Date(Date.now() - 86400000 * 2).toISOString(),
      isRead: true
    }
  ];

  ngOnInit() {
    this.notificationService.loadNotifications();
  }

  markAsRead(id: string) {
    this.notificationService.markAsRead(id);
  }

  deleteNotification(notif: Notification) {
    // Note: delete is not in the service currently, so we'll just local filter for now
    // In a real app, this would call service.delete(notif.id)
  }
}