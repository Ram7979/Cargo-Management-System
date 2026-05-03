import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AuthService } from '../../../core/services/auth.service';
import { Observable, of } from 'rxjs';
import { UserProfile } from '../../../core/models/user.model';

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatButtonModule],
  templateUrl: './customer-dashboard.component.html',
  styleUrls: ['./customer-dashboard.component.scss']
})
export class CustomerDashboardComponent implements OnInit {
  private authService = inject(AuthService);

  user: UserProfile | null = null;
  summary$: Observable<any> = of(null); // Will be replaced by service call

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => this.user = user);
    
    // Mock data for initial preview
    this.summary$ = of({
      activeShipments: 2,
      deliveredThisMonth: 5,
      totalShipments: 12,
      pendingInvoicesCount: 1,
      pendingInvoicesAmount: 1500.00,
      recentShipments: [
        { trackingNumber: 'CMS-2025-0001', origin: 'Mumbai', destination: 'Dubai', status: 'In Transit', serviceType: 'Express' },
        { trackingNumber: 'CMS-2025-0002', origin: 'London', destination: 'New York', status: 'Pending', serviceType: 'Standard' },
        { trackingNumber: 'CMS-2025-0003', origin: 'Singapore', destination: 'Tokyo', status: 'Delivered', serviceType: 'Same-Day' }
      ],
      recentNotifications: [
        { id: '1', message: 'Your shipment CMS-2025-0001 is in transit.', type: 'ShipmentUpdate', createdAt: new Date() },
        { id: '2', message: 'New invoice INV-2025-001 has been generated.', type: 'Invoice', createdAt: new Date(Date.now() - 86400000) }
      ]
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'in transit': return 'status-blue';
      case 'delivered': return 'status-green';
      case 'pending': return 'status-amber';
      case 'cancelled': return 'status-red';
      default: return '';
    }
  }
}