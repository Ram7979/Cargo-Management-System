import { Component, EventEmitter, Output, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatDividerModule } from '@angular/material/divider';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [
    CommonModule, 
    MatToolbarModule, 
    MatIconModule, 
    MatButtonModule, 
    MatMenuModule, 
    MatBadgeModule, 
    MatDividerModule,
    FormsModule
  ],
  templateUrl: './topbar.component.html',
  styleUrls: ['./topbar.component.scss']
})
export class TopbarComponent implements OnInit {
  @Output() toggleSidebar = new EventEmitter<void>();
  
  authService = inject(AuthService);
  notificationService = inject(NotificationService);
  private router = inject(Router);
  
  searchQuery = '';

  ngOnInit() {
    this.notificationService.loadNotifications();
  }

  onSearch() {
    if (this.searchQuery.trim()) {
      this.router.navigate(['/shipments'], { queryParams: { trackingNumber: this.searchQuery } });
      this.searchQuery = '';
    }
  }

  onLogout() {
    this.authService.logout();
  }

  getNoteIcon(type: string): string {
    switch (type) {
      case 'Success': return 'check_circle';
      case 'Warning': return 'warning';
      case 'Error': return 'error';
      default: return 'info';
    }
  }
}
