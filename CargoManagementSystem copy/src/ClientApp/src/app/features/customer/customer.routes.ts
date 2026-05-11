import { Routes } from '@angular/router';
import { CustomerLayoutComponent } from './layout/customer-layout.component';

export const CUSTOMER_ROUTES: Routes = [
  {
    path: '',
    component: CustomerLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { 
        path: 'dashboard', 
        loadComponent: () => import('./dashboard/customer-dashboard.component').then(m => m.CustomerDashboardComponent) 
      },
      { 
        path: 'shipments', 
        loadComponent: () => import('./shipments/my-shipments.component').then(m => m.MyShipmentsComponent) 
      },
      { 
        path: 'track', 
        loadComponent: () => import('./track/shipment-tracking.component').then(m => m.ShipmentTrackingComponent) 
      },
      { 
        path: 'book-shipment', 
        loadComponent: () => import('./book-shipment/book-shipment.component').then(m => m.BookShipmentComponent) 
      },
      { 
        path: 'invoices', 
        loadComponent: () => import('./invoices/invoices.component').then(m => m.InvoicesComponent) 
      },
      { 
        path: 'notifications', 
        loadComponent: () => import('./notifications/notifications.component').then(m => m.NotificationsComponent) 
      },
      { 
        path: 'profile', 
        loadComponent: () => import('./profile/my-profile.component').then(m => m.MyProfileComponent) 
      },
    ]
  }
];
