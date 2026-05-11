import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../../core/services/auth.service';
import { ShipmentService } from '../../../core/services/shipment.service';
import { InvoiceService } from '../../../core/services/invoice.service';
import { NotificationService } from '../../../core/services/notification.service';
import { RevenueEventService } from '../../../core/services/revenue-event.service';
import { Observable, of, forkJoin, catchError, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { UserProfile } from '../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrls: ['./customer-dashboard.component.scss']
})
export class CustomerDashboardComponent implements OnInit, OnDestroy {
  private authService = inject(AuthService);
  private shipmentService = inject(ShipmentService);
  private invoiceService = inject(InvoiceService);
  private notificationService = inject(NotificationService);
  private revenueEvents = inject(RevenueEventService);
  private http = inject(HttpClient);

  user: UserProfile | null = null;
  summary$: Observable<any> = of(null);
  loading = true;
  error = '';
  private pollingInterval: any;
  private paymentSub?: Subscription;

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => this.user = user);
    this.loadDashboardData();
    // Poll every 30 seconds for real-time updates
    this.pollingInterval = setInterval(() => {
      this.loadDashboardData(false); // background refresh
    }, 30000);
    // Instantly refresh when any payment succeeds
    this.paymentSub = this.revenueEvents.paymentSuccess$.subscribe(() => {
      this.loadDashboardData(false);
    });
  }

  ngOnDestroy() {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
    }
    this.paymentSub?.unsubscribe();
  }

  loadDashboardData(showLoading = true) {
    if (showLoading) this.loading = true;
    this.error = '';

    forkJoin({
      shipmentsRes: this.shipmentService.getMyShipments(1, 100),
      invoicesRes: this.invoiceService.getMyInvoices(1, 100),
      notificationsRes: this.notificationService.getMyNotifications(1, 5)
    }).pipe(
      map(({ shipmentsRes, invoicesRes, notificationsRes }) => {
        // Parse Shipments
        const shipments = Array.isArray(shipmentsRes.data) ? shipmentsRes.data : (shipmentsRes.data as any)?.items || [];
        
        // Parse Invoices
        const invoices = Array.isArray(invoicesRes.data) ? invoicesRes.data : (invoicesRes.data as any)?.items || [];
        
        // Parse Notifications
        const notifications = notificationsRes?.items || notificationsRes?.data?.items || (Array.isArray(notificationsRes?.data) ? notificationsRes.data : null) || (Array.isArray(notificationsRes) ? notificationsRes : []);

        const currentMonth = new Date().getMonth();
        const currentYear = new Date().getFullYear();

        const activeShipments = shipments.filter((s: any) => s.status?.toLowerCase() === 'intransit' || s.status?.toLowerCase() === 'in transit').length;
        
        const deliveredThisMonth = shipments.filter((s: any) => {
          if (s.status?.toLowerCase() !== 'delivered') return false;
          const d = new Date(s.createdAt || s.updatedAt);
          return d.getMonth() === currentMonth && d.getFullYear() === currentYear;
        }).length;

        const pendingInvoices = invoices.filter((i: any) => i.outstandingBalance > 0);
        const pendingAmount = pendingInvoices.reduce((sum: number, i: any) => sum + (i.outstandingBalance || 0), 0);

        const sortedShipments = [...shipments].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        
        return {
          activeShipments: activeShipments,
          deliveredThisMonth: deliveredThisMonth,
          totalShipments: shipments.length,
          pendingInvoicesCount: pendingInvoices.length,
          pendingInvoicesAmount: pendingAmount,
          recentShipments: sortedShipments.slice(0, 5),
          recentNotifications: notifications.slice(0, 5)
        };
      }),
      catchError(err => {
        this.error = 'Failed to load dashboard data. Please try again later.';
        console.error('[Dashboard Error]', err);
        return of(null);
      })
    ).subscribe(data => {
      this.summary$ = of(data);
      this.loading = false;
    });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'in transit': return 'status-blue';
      case 'delivered': return 'status-green';
      case 'pending': return 'status-amber';
      case 'cancelled': return 'status-red';
      default: return '';
    }
  }

  getNotificationIcon(note: any): string {
    const t = (note.subject || note.title || note.eventType || '').toLowerCase();
    if (t.includes('shipment') || t.includes('book')) return 'local_shipping';
    if (t.includes('payment') || t.includes('invoice') || t.includes('received')) return 'payment';
    if (t.includes('deliver')) return 'check_circle';
    if (t.includes('alert') || t.includes('fail')) return 'warning';
    return 'notifications';
  }
}