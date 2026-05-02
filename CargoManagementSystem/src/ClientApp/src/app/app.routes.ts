import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { 
    path: 'auth', 
    canActivate: [guestGuard],
    children: [
      { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
      { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
      { path: 'forgot-password', loadComponent: () => import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) }
    ]
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
      { 
        path: 'users', 
        loadComponent: () => import('./features/users/user-list/user-list.component').then(m => m.UserListComponent), 
        data: { roles: ['SuperAdmin'] } 
      },
      { 
        path: 'customers', 
        loadComponent: () => import('./features/customers/customer-list/customer-list.component').then(m => m.CustomerListComponent), 
        data: { roles: ['SuperAdmin', 'OpsManager', 'Support'] } 
      },
      { 
        path: 'customers/:id', 
        loadComponent: () => import('./features/customers/customer-detail/customer-detail.component').then(m => m.CustomerDetailComponent), 
        data: { roles: ['SuperAdmin', 'OpsManager', 'Support'] } 
      },
      { 
        path: 'shipments', 
        loadComponent: () => import('./features/shipments/shipment-list/shipment-list.component').then(m => m.ShipmentListComponent) 
      },
      { 
        path: 'shipments/new', 
        loadComponent: () => import('./features/shipments/shipment-create/shipment-create.component').then(m => m.ShipmentCreateComponent),
        data: { roles: ['SuperAdmin', 'OpsManager', 'Dispatcher'] }
      },
      { 
        path: 'shipments/:id', 
        loadComponent: () => import('./features/shipments/shipment-detail/shipment-detail.component').then(m => m.ShipmentDetailComponent) 
      },
      { 
        path: 'fleet', 
        children: [
          { path: '', loadComponent: () => import('./features/fleet/vehicle-list/vehicle-list.component').then(m => m.VehicleListComponent) },
          { path: 'drivers', loadComponent: () => import('./features/fleet/driver-list/driver-list.component').then(m => m.DriverListComponent) },
          { path: 'assignments', loadComponent: () => import('./features/fleet/shipment-assignment/shipment-assignment.component').then(m => m.ShipmentAssignmentComponent) },
          { path: ':id', loadComponent: () => import('./features/fleet/vehicle-detail/vehicle-detail.component').then(m => m.VehicleDetailComponent) }
        ],
        data: { roles: ['SuperAdmin', 'OpsManager', 'Dispatcher', 'FleetManager'] } 
      },
      { 
        path: 'warehouse', 
        children: [
          { path: 'inventory', loadComponent: () => import('./features/warehouse/inventory-list/inventory-list.component').then(m => m.InventoryListComponent) },
          { path: 'receive', loadComponent: () => import('./features/warehouse/receive-cargo/receive-cargo.component').then(m => m.ReceiveCargoComponent) },
          { path: 'release', loadComponent: () => import('./features/warehouse/release-cargo/release-cargo.component').then(m => m.ReleaseCargoComponent) }
        ],
        data: { roles: ['SuperAdmin', 'OpsManager', 'WarehouseManager'] } 
      },
      { 
        path: 'billing', 
        children: [
          { path: '', loadComponent: () => import('./features/billing/invoice-list/invoice-list.component').then(m => m.InvoiceListComponent) },
          { path: ':id', loadComponent: () => import('./features/billing/invoice-detail/invoice-detail.component').then(m => m.InvoiceDetailComponent) }
        ],
        data: { roles: ['SuperAdmin', 'FinanceOfficer', 'OpsManager'] } 
      },
      { 
        path: 'reports', 
        loadComponent: () => import('./features/reports/shipment-report/shipment-report.component').then(m => m.ShipmentReportComponent), 
        data: { roles: ['SuperAdmin', 'OpsManager', 'FinanceOfficer'] } 
      }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
