import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatListModule, MatIconModule],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent {
  authService = inject(AuthService);

  menuItems = [
    { path: '/dashboard', icon: 'dashboard', label: 'Dashboard', roles: [] },
    { path: '/shipments', icon: 'local_shipping', label: 'Shipments', roles: [] },
    { path: '/fleet', icon: 'directions_car', label: 'Fleet Management', roles: ['SuperAdmin', 'OpsManager', 'Dispatcher', 'FleetManager'] },
    { path: '/warehouse', icon: 'warehouse', label: 'Warehouse', roles: ['SuperAdmin', 'OpsManager', 'WarehouseManager', 'WarehouseOperator'] },
    { path: '/customers', icon: 'people', label: 'Customers', roles: ['SuperAdmin', 'OpsManager', 'Support'] },
    { path: '/billing', icon: 'receipt', label: 'Billing & Payments', roles: ['SuperAdmin', 'FinanceOfficer', 'OpsManager'] },
    { path: '/reports', icon: 'bar_chart', label: 'Reports', roles: ['SuperAdmin', 'OpsManager', 'FinanceOfficer'] },
    { path: '/users', icon: 'admin_panel_settings', label: 'User Management', roles: ['SuperAdmin'] }
  ];

  get visibleMenuItems() {
    return this.menuItems.filter(item => 
      item.roles.length === 0 || item.roles.some(role => this.authService.hasRole(role))
    );
  }
}
