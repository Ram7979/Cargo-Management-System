import {
  Component, OnInit, OnDestroy, inject, ChangeDetectorRef, NgZone
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { StatusChipComponent } from '../../shared/components/status-chip/status-chip.component';
import { BaseChartDirective, provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';
import { DashboardService } from '../../core/services/dashboard.service';
import { ShipmentService } from '../../core/services/shipment.service';
import { RevenueEventService } from '../../core/services/revenue-event.service';
import { forkJoin, merge, of, Subject, Subscription, timer } from 'rxjs';
import { catchError, debounceTime, switchMap } from 'rxjs/operators';

/* ─────────────────────────────────────────────────────────────────────────── */
const STATUS_COLORS: Record<string, string> = {
  Pending:             '#F6AD55',
  InTransit:           '#4299E1',
  OutForDelivery:      '#9F7AEA',
  AtWarehouse:         '#FC8181',
  Delivered:           '#48BB78',
  Cancelled:           '#A0AEC0',
  FailedDelivery:      '#F56565',
  Assigned:            '#63B3ED',
  PickedUp:            '#B794F4',
  ReturnedToWarehouse: '#FEB2B2',
};

const CARGO_COLORS = [
  '#4299E1', '#48BB78', '#F6AD55', '#F56565',
  '#9F7AEA', '#A0AEC0', '#ED64A6', '#38B2AC',
];

/** Generate an array of date strings for the last N days (yyyy-MM-dd). */
function lastNDays(n: number): string[] {
  const result: string[] = [];
  for (let i = n - 1; i >= 0; i--) {
    const d = new Date();
    d.setDate(d.getDate() - i);
    result.push(d.toISOString().split('T')[0]);
  }
  return result;
}

/* ─────────────────────────────────────────────────────────────────────────── */

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, MatCardModule, MatIconModule,
    MatProgressSpinnerModule, MatProgressBarModule,
    MatButtonModule, MatTooltipModule,
    RouterModule, StatusChipComponent, BaseChartDirective
  ],
  providers: [provideCharts(withDefaultRegisterables())],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private dashboardService  = inject(DashboardService);
  private shipmentService   = inject(ShipmentService);
  private revenueEvents     = inject(RevenueEventService);
  private cdr               = inject(ChangeDetectorRef);
  private zone              = inject(NgZone);

  /* ── UI state ─────────────────────────────────────────────────────────── */
  isLoading    = true;
  hasError     = false;
  lastUpdated: Date | null = null;

  /* ── KPIs ─────────────────────────────────────────────────────────────── */
  kpi = {
    totalRevenue:         0,
    totalCollected:       0,
    activeShipments:      0,
    warehouseUtilization: 0,
    fleetAvailability:    0,
    pendingInvoices:      0,
    deliveredToday:       0,
    totalShipments:       0,
    failedDeliveries:     0,
    inTransitCount:       0,
    pendingPickups:       0,
  };

  /* ── Status pills ─────────────────────────────────────────────────────── */
  statusBreakdown: { status: string; count: number; color: string }[] = [];
  totalStatusCount = 0;

  /* ── Recent shipments ─────────────────────────────────────────────────── */
  recentShipments: any[] = [];

  /* ── Chart state flags ─────────────────────────────────────────────────── */
  hasBarSeries     = false;
  hasDoughnutSeries = false;

  /* ── Chart data ───────────────────────────────────────────────────────── */
  // Revenue line chart is ALWAYS shown — fallback to zero-fill for 30 days
  lineChartData:  ChartData<'line'>     = this.buildZeroLineChart();
  barChartData:   ChartData<'bar'>      = { labels: [], datasets: [] };
  doughnutData:   ChartData<'doughnut'> = { labels: [], datasets: [] };

  /* ── Stable chart options ─────────────────────────────────────────────── */
  lineChartOptions: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    animation: { duration: 500 },
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: ctx => ` $${(ctx.raw as number).toLocaleString('en-US', { minimumFractionDigits: 2 })}`
        }
      }
    },
    scales: {
      x: { grid: { display: false }, ticks: { color: '#718096', maxTicksLimit: 8, maxRotation: 30 } },
      y: {
        grid: { color: 'rgba(0,0,0,0.05)' }, beginAtZero: true,
        ticks: { color: '#718096', callback: v => `$${Number(v).toLocaleString()}` }
      }
    }
  };

  barChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    maintainAspectRatio: false,
    animation: { duration: 500 },
    plugins: { legend: { display: false } },
    scales: {
      x: { grid: { display: false }, ticks: { color: '#718096' } },
      y: { grid: { color: 'rgba(0,0,0,0.05)' }, beginAtZero: true, ticks: { color: '#718096', stepSize: 1 } }
    }
  };

  doughnutOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    maintainAspectRatio: false,
    animation: { duration: 500 },
    cutout: '60%',
    plugins: {
      legend: { position: 'bottom', labels: { color: '#4a5568', font: { size: 11 }, padding: 10 } },
      tooltip: {
        callbacks: {
          label: ctx => {
            const total = (ctx.dataset.data as number[]).reduce((a, b) => a + (b as number), 0);
            const pct = total ? Math.round((ctx.raw as number) / total * 100) : 0;
            return ` ${ctx.label}: ${ctx.raw} (${pct}%)`;
          }
        }
      }
    }
  };

  /* ── Polling + event triggers ─────────────────────────────────────────── */
  private pollSub?: Subscription;
  private readonly POLL_MS = 30_000;
  private manualRefresh$ = new Subject<void>();

  ngOnInit()    { this.startPolling(); }
  ngOnDestroy() { this.pollSub?.unsubscribe(); }

  /* ── Public: manual reload ─────────────────────────────────────────────── */
  reload() {
    this.isLoading = true;
    this.hasError  = false;
    this.lineChartData = this.buildZeroLineChart();
    this.cdr.detectChanges();
    this.manualRefresh$.next();
  }

  /* ── Polling loop ─────────────────────────────────────────────────────── */
  private startPolling() {
    // Merge: 30s timer + manual refresh button + payment success event
    const trigger$ = merge(
      timer(0, this.POLL_MS),
      this.manualRefresh$,
      this.revenueEvents.paymentSuccess$
    ).pipe(debounceTime(200));

    this.pollSub = trigger$
      .pipe(switchMap(() => this.fetchAll()))
      .subscribe({
        next: (res: any[]) => {
          const [kpisRes, trendRes, shipmentsRes, summaryRes] = res;
          this.zone.run(() => {
            this.applyAll(kpisRes, trendRes, shipmentsRes, summaryRes);
            this.hasError    = false;
            this.isLoading   = false;
            this.lastUpdated = new Date();
            this.cdr.detectChanges();
          });
        },
        error: () => {
          this.hasError  = true;
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
  }

  private fetchAll() {
    const safe = (obs: any) => obs.pipe(catchError(() => of({ success: false, data: null })));
    return forkJoin([
      safe(this.dashboardService.getKpis()),
      safe(this.dashboardService.getTrendData('day')),
      safe(this.shipmentService.getAll(1, 100)),
      safe(this.dashboardService.getSummary()),
    ]);
  }

  /* ── Master apply ──────────────────────────────────────────────────────── */
  private applyAll(kpisRes: any, trendRes: any, shipmentsRes: any, summaryRes: any) {
    const k = kpisRes?.data  ?? {};
    const t = trendRes?.data ?? {};
    const s = summaryRes?.data ?? {};

    /* Shipments — single source of truth */
    const rawShipments: any[] = (() => {
      const d = shipmentsRes?.data;
      if (!d) return [];
      if (Array.isArray(d)) return d;
      if (Array.isArray(d.items)) return d.items;
      return [];
    })();

    this.recentShipments = rawShipments.slice(0, 8);

    /* Status + cargo counts from shipments list */
    const statusMap: Record<string, number> = {};
    const cargoMap:  Record<string, number> = {};
    for (const ship of rawShipments) {
      const st = ship.status ?? 'Unknown';
      statusMap[st] = (statusMap[st] ?? 0) + 1;
      const ct = ship.cargoType ?? 'Standard';
      cargoMap[ct] = (cargoMap[ct] ?? 0) + 1;
    }

    this.totalStatusCount = rawShipments.length;
    this.statusBreakdown  = Object.entries(statusMap)
      .map(([status, count]) => ({ status, count, color: STATUS_COLORS[status] ?? '#A0AEC0' }))
      .sort((a, b) => b.count - a.count);

    /* KPIs */
    const activeStatuses = new Set([
      'Pending','Assigned','PickedUp','InTransit',
      'AtWarehouse','OutForDelivery','FailedDelivery','ReturnedToWarehouse'
    ]);
    const activeCount  = rawShipments.filter(sh => activeStatuses.has(sh.status)).length;
    const inTransit    = statusMap['InTransit']      ?? 0;
    const pendingCount = statusMap['Pending']         ?? 0;
    const failed       = statusMap['FailedDelivery']  ?? 0;
    const delivered    = statusMap['Delivered']       ?? 0;

    const totalRevenue   = k.totalRevenue ?? s.revenueThisMonth ?? 0;

    this.kpi = {
      totalRevenue,
      totalCollected:       totalRevenue,   // same field from billing
      activeShipments:      activeCount || (k.activeShipments ?? s.totalActiveShipments ?? 0),
      warehouseUtilization: k.warehouseUtilization ?? 0,
      fleetAvailability:    k.fleetAvailability    ?? 0,
      pendingInvoices:      s.pendingInvoicesCount  ?? 0,
      deliveredToday:       s.deliveredToday        ?? delivered,
      totalShipments:       rawShipments.length     || (s.totalActiveShipments ?? 0),
      failedDeliveries:     failed,
      inTransitCount:       inTransit,
      pendingPickups:       pendingCount,
    };

    /* ── Revenue Trend line chart ───────────────────────────────────────── */
    // Build a 30-day date spine, zero-fill gaps, overlay API data
    const spine = lastNDays(30);
    const revMap: Record<string, number> = {};
    for (const pt of (Array.isArray(t.revenueTrend) ? t.revenueTrend : [])) {
      revMap[pt.date] = (revMap[pt.date] ?? 0) + (pt.value ?? 0);
    }
    const revValues = spine.map(d => revMap[d] ?? 0);
    const hasRevenue = revValues.some(v => v > 0);

    this.lineChartData = {
      labels: [...spine],
      datasets: [{
        data:                [...revValues],
        label:               'Revenue (₹)',
        tension:             0.4,
        borderColor:         hasRevenue ? '#48BB78' : '#A0AEC0',
        borderWidth:         2.5,
        fill:                true,
        backgroundColor:     hasRevenue
          ? 'rgba(72,187,120,0.12)'
          : 'rgba(160,174,192,0.08)',
        pointBackgroundColor: hasRevenue ? '#48BB78' : 'transparent',
        pointRadius:          hasRevenue ? 3 : 0,
        pointHoverRadius:     5,
      }]
    };

    /* ── Bar chart — Shipments by Status ───────────────────────────────── */
    const statusEntries = [...this.statusBreakdown];
    this.barChartData = {
      labels: [...statusEntries.map(e => e.status)],
      datasets: [{
        data:            [...statusEntries.map(e => e.count)],
        label:           'Shipments',
        backgroundColor: [...statusEntries.map(e => e.color)],
        borderRadius:    6,
        borderSkipped:   false,
      }]
    };
    this.hasBarSeries = statusEntries.length > 0;

    /* ── Doughnut chart — Cargo Distribution ───────────────────────────── */
    let cargoEntries = Object.entries(cargoMap)
      .map(([type, count]) => ({ type, count }))
      .sort((a, b) => b.count - a.count);

    // Fallback to trend API
    if (cargoEntries.length === 0 && Array.isArray(t.cargoTypeBreakdown)) {
      const agg: Record<string, number> = {};
      for (const c of t.cargoTypeBreakdown) {
        const key = c.cargoType ?? 'Unknown';
        agg[key] = (agg[key] ?? 0) + (c.count ?? 0);
      }
      cargoEntries = Object.entries(agg)
        .map(([type, count]) => ({ type, count }))
        .sort((a, b) => b.count - a.count);
    }

    this.doughnutData = {
      labels: [...cargoEntries.map(c => c.type)],
      datasets: [{
        data:            [...cargoEntries.map(c => c.count)],
        backgroundColor: [...cargoEntries.map((_, i) => CARGO_COLORS[i % CARGO_COLORS.length])],
        borderWidth:     2,
        borderColor:     '#fff',
        hoverOffset:     8,
      }]
    };
    this.hasDoughnutSeries = cargoEntries.length > 0;
  }

  /* ── Zero-fill 30-day chart (shown before data loads) ─────────────────── */
  private buildZeroLineChart(): ChartData<'line'> {
    const spine = lastNDays(30);
    return {
      labels: [...spine],
      datasets: [{
        data:                [...spine.map(() => 0)],
        label:               'Revenue (₹)',
        tension:             0.4,
        borderColor:         '#A0AEC0',
        borderWidth:         1.5,
        fill:                true,
        backgroundColor:     'rgba(160,174,192,0.06)',
        pointRadius:         0,
        pointHoverRadius:    4,
      }]
    };
  }

  /* ── Helpers ──────────────────────────────────────────────────────────── */
  formatCurrency(v: number): string {
    return new Intl.NumberFormat('en-IN', { style: 'currency', currency: 'INR' }).format(v ?? 0);
  }
  formatPercent(v: number): string { return `${Math.round(v ?? 0)}%`; }
  statusPercent(count: number): number {
    return this.totalStatusCount > 0 ? Math.round(count / this.totalStatusCount * 100) : 0;
  }

  exportSummary() {
    this.dashboardService.exportReport('summary').subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a   = document.createElement('a');
        a.href    = url;
        a.download = `cms-dashboard-${new Date().toISOString().split('T')[0]}.csv`;
        a.click();
        window.URL.revokeObjectURL(url);
      }
    });
  }

  trackByStatus   = (_: number, i: any) => i.status;
  trackByShipment = (_: number, i: any) => i.id;
}
