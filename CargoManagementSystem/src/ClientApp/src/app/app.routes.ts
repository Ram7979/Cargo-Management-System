import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { 
    path: 'auth', 
    children: [
      { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
      { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
      { path: 'forgot-password', loadComponent: () => import('./features/auth/forgot-password/forgot-password.component').then(m => m.ForgotPasswordComponent) }
    ]
  },
  {
    path: 'user',
    loadChildren: () => import('./features/customer/customer.routes').then(m => m.CUSTOMER_ROUTES),
    canActivate: [authGuard],
    data: { role: 'Customer' }
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },

      // Users
      { path: 'users', loadComponent: () => import('./features/users/user-list/user-list.component').then(m => m.UserListComponent), data: { roles: ['SuperAdmin'] } },
      { path: 'users/:id', loadComponent: () => import('./features/users/user-detail/user-detail.component').then(m => m.UserDetailComponent), data: { roles: ['SuperAdmin'] } },

      // Customers
      { path: 'customers', loadComponent: () => import('./features/customers/customer-list/customer-list.component').then(m => m.CustomerListComponent), data: { roles: ['SuperAdmin', 'OpsManager', 'Support'] } },
      { path: 'customers/:id', loadComponent: () => import('./features/customers/customer-detail/customer-detail.component').then(m => m.CustomerDetailComponent), data: { roles: ['SuperAdmin', 'OpsManager', 'Support'] } },

      // Shipments
      { path: 'shipments', loadComponent: () => import('./features/shipments/shipment-list/shipment-list.component').then(m => m.ShipmentListComponent) },
      { path: 'shipments/new', loadComponent: () => import('./features/shipments/shipment-create/shipment-create.component').then(m => m.ShipmentCreateComponent), data: { roles: ['SuperAdmin', 'OpsManager', 'Dispatcher'] } },
      { path: 'shipments/:id', loadComponent: () => import('./features/shipments/shipment-detail/shipment-detail.component').then(m => m.ShipmentDetailComponent) },

      // Fleet
      {
        path: 'fleet',
        children: [
          { path: '', loadComponent: () => import('./features/fleet/vehicle-list/vehicle-list.component').then(m => m.VehicleListComponent) },
          { path: 'new', loadComponent: () => import('./features/fleet/vehicle-create/vehicle-create.component').then(m => m.VehicleCreateComponent) },
          { path: 'drivers', loadComponent: () => import('./features/fleet/driver-list/driver-list.component').then(m => m.DriverListComponent) },
          { path: 'assignments', loadComponent: () => import('./features/fleet/shipment-assignment/shipment-assignment.component').then(m => m.ShipmentAssignmentComponent) },
          { path: ':id', loadComponent: () => import('./features/fleet/vehicle-detail/vehicle-detail.component').then(m => m.VehicleDetailComponent) }
        ],
        data: { roles: ['SuperAdmin', 'OpsManager', 'Dispatcher', 'FleetManager'] }
      },

      // ← NEW TOP-LEVEL DRIVERS PAGE
      {
        path: 'drivers',
        loadComponent: () => import('./features/drivers/drivers-page.component').then(m => m.DriversPageComponent),
        data: { roles: ['SuperAdmin', 'OpsManager', 'Dispatcher', 'FleetManager'] }
      },

      // Warehouse — ← ADD default redirect so /warehouse goes to /warehouse/inventory
      {
        path: 'warehouse',
        children: [
          { path: '', redirectTo: 'inventory', pathMatch: 'full' },   // ← THIS FIX
          { path: 'inventory', loadComponent: () => import('./features/warehouse/inventory-list/inventory-list.component').then(m => m.InventoryListComponent) },
          { path: 'receive', loadComponent: () => import('./features/warehouse/receive-cargo/receive-cargo.component').then(m => m.ReceiveCargoComponent) },
          { path: 'release', loadComponent: () => import('./features/warehouse/release-cargo/release-cargo.component').then(m => m.ReleaseCargoComponent) }
        ],
        data: { roles: ['SuperAdmin', 'OpsManager', 'WarehouseManager'] }
      },

      // Billing — ← ADD /billing/new route
      {
        path: 'billing',
        children: [
          { path: '', loadComponent: () => import('./features/billing/invoice-list/invoice-list.component').then(m => m.InvoiceListComponent) },
          { path: 'new', loadComponent: () => import('./features/billing/invoice-create/invoice-create.component').then(m => m.InvoiceCreateComponent) },
          { path: ':id', loadComponent: () => import('./features/billing/invoice-detail/invoice-detail.component').then(m => m.InvoiceDetailComponent) }
        ],
        data: { roles: ['SuperAdmin', 'FinanceOfficer', 'OpsManager'] }
      },

      // Reports
      { path: 'reports', loadComponent: () => import('./features/reports/shipment-report/shipment-report.component').then(m => m.ShipmentReportComponent), data: { roles: ['SuperAdmin', 'OpsManager', 'FinanceOfficer'] } },

      // Profile
      { path: 'profile', loadComponent: () => import('./features/profile/profile.component').then(m => m.ProfileComponent) }
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];
