import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { filter, map, startWith } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { UserProfile } from '../../../core/models/user.model';

@Component({
  selector: 'app-customer-layout',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    MatIconModule, 
    MatButtonModule, 
    MatBadgeModule, 
    MatMenuModule
  ],
  templateUrl: './customer-layout.component.html',
  styleUrls: ['./customer-layout.component.scss']
})
export class CustomerLayoutComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);

  user$: Observable<UserProfile | null> = this.authService.currentUser$;
  pageTitle = 'Dashboard';
  notificationService = inject(NotificationService);
  isSidebarCollapsed = false;

  ngOnInit() {
    // Update page title based on current route
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      startWith(new NavigationEnd(0, this.router.url, this.router.url)),
      map(() => this.getRouteTitle(this.router.url))
    ).subscribe(title => {
      this.pageTitle = title;
    });

    // Load unread count for badge
    this.notificationService.loadNotifications();
    
    // Poll for notifications every 30 seconds
    setInterval(() => this.notificationService.loadNotifications(), 30000);
  }

  private getRouteTitle(url: string): string {
    if (url.includes('dashboard')) return 'Dashboard';
    if (url.includes('shipments')) return 'My Shipments';
    if (url.includes('track')) return 'Track Package';
    if (url.includes('book-shipment')) return 'Book New Shipment';
    if (url.includes('invoices')) return 'Invoices & Payments';
    if (url.includes('notifications')) return 'Notifications';
    if (url.includes('profile')) return 'My Profile';
    return 'CargoPro';
  }

  toggleSidebar() {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  onTrackSearch(event: any) {
    const trackingNumber = event.target.value;
    if (trackingNumber) {
      this.router.navigate(['/user/track'], { queryParams: { tracking: trackingNumber } });
    }
  }

  logout() {
    this.authService.logout();
  }
}
