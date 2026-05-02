import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { DashboardService, DashboardSummary } from '../../core/services/dashboard.service';
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from 'ng2-charts';

import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { RouterModule } from '@angular/router';
import { StatusChipComponent } from '../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatGridListModule, MatProgressSpinnerModule, MatProgressBarModule, MatButtonModule, MatChipsModule, RouterModule, StatusChipComponent, BaseChartDirective],
  providers: [provideCharts(withDefaultRegisterables())],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  
  summary: DashboardSummary | null = null;
  isLoading = true;

  // Chart configuration
  public barChartOptions = { responsive: true };
  public barChartLabels: string[] = [];
  public barChartData: any[] = [];
  
  public lineChartOptions = { responsive: true };
  public lineChartLabels: string[] = [];
  public lineChartData: any[] = [];

  public pieChartOptions = { responsive: true };
  public pieChartLabels: string[] = [];
  public pieChartData: any[] = [];

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.isLoading = true;
    this.pendingRequests = 1; // Use single summary endpoint

    // Initialize with defaults
    this.summary = {
      totalRevenue: 0,
      activeShipments: 0,
      warehouseUtilization: 0,
      fleetAvailability: 0,
      shipmentsByStatus: [],
      revenueTrend: [],
      cargoTypes: [],
      recentShipments: [],
      totalActiveShipments: 0,
      totalShipments: 0,
      deliveredToday: 0,
      revenueGrowth: 0
    };

    // Use the summary endpoint which returns all KPIs in one call
    this.dashboardService.getSummary().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          const d = res.data;
          this.summary = {
            totalRevenue: d.totalRevenue || 0,
            activeShipments: d.activeShipments || d.totalActiveShipments || 0,
            totalActiveShipments: d.activeShipments || d.totalActiveShipments || 0,
            totalShipments: d.totalShipments || 0,
            deliveredToday: d.deliveredToday || 0,
            revenueGrowth: d.revenueGrowth || 0,
            warehouseUtilization: d.warehouseUtilization || 0,
            fleetAvailability: d.fleetAvailability || 0,
            shipmentsByStatus: Array.isArray(d.shipmentsByStatus) ? d.shipmentsByStatus : [],
            revenueTrend: Array.isArray(d.revenueTrend) ? d.revenueTrend : [],
            cargoTypes: Array.isArray(d.cargoTypes) ? d.cargoTypes : [],
            recentShipments: Array.isArray(d.recentShipments) ? d.recentShipments : []
          };
          this.initCharts();
        }
        this.checkLoadingState();
      },
      error: () => {
        // Fallback: try individual endpoints
        this.pendingRequests = 2;
        this.dashboardService.getKpis().subscribe({
          next: (res) => {
            if (res.success && res.data) {
              this.summary!.totalRevenue = res.data.totalRevenue || 0;
              this.summary!.activeShipments = res.data.activeShipments || 0;
              this.summary!.totalActiveShipments = res.data.activeShipments || 0;
              this.summary!.warehouseUtilization = res.data.warehouseUtilization || 0;
              this.summary!.fleetAvailability = res.data.fleetAvailability || 0;
            }
            this.checkLoadingState();
          },
          error: () => this.checkLoadingState()
        });

        this.dashboardService.getTrendData('day').subscribe({
          next: (res) => {
            if (res.success && res.data) {
              this.summary!.revenueTrend = res.data.revenueTrend || [];
              this.summary!.shipmentsByStatus = res.data.shipmentsByStatus || [];
              this.summary!.cargoTypes = res.data.cargoTypeBreakdown || res.data.cargoTypes || [];
              this.initCharts();
            }
            this.checkLoadingState();
          },
          error: () => this.checkLoadingState()
        });
      }
    });
  }

  private pendingRequests = 1;
  private checkLoadingState() {
    this.pendingRequests--;
    if (this.pendingRequests <= 0) {
      this.isLoading = false;
    }
  }

  exportSummary() {
    this.dashboardService.exportReport('summary').subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `cms-summary-${new Date().toISOString().split('T')[0]}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Failed to export summary', err)
    });
  }

  initCharts() {
    if (!this.summary) return;

    const statusData = Array.isArray(this.summary.shipmentsByStatus) ? this.summary.shipmentsByStatus : [];
    this.barChartLabels = statusData.map((s: any) => s.status);
    this.barChartData = [{ 
      data: statusData.map((s: any) => s.count), 
      label: 'Shipments',
      backgroundColor: ['#4299e1', '#48bb78', '#f6ad55', '#f56565', '#a0aec0'] 
    }];

    const trendData = Array.isArray(this.summary.revenueTrend) ? this.summary.revenueTrend : [];
    this.lineChartLabels = trendData.map((r: any) => r.date);
    this.lineChartData = [{ 
      data: trendData.map((r: any) => r.value), 
      label: 'Revenue', 
      tension: 0.4,
      borderColor: '#4299e1',
      fill: true,
      backgroundColor: 'rgba(66, 153, 225, 0.1)'
    }];

    const typeData = Array.isArray(this.summary.cargoTypes) ? this.summary.cargoTypes : [];
    this.pieChartLabels = typeData.map((c: any) => c.cargoType || c.type);
    this.pieChartData = [{ 
      data: typeData.map((c: any) => c.count), 
      label: 'Cargo Types',
      backgroundColor: ['#4299e1', '#48bb78', '#f6ad55', '#f56565', '#a0aec0'] 
    }];
  }
}
