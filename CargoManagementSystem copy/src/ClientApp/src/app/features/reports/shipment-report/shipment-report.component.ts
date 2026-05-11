import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ReportService } from '../../../core/services/report.service';
import { ShipmentService } from '../../../core/services/shipment.service';
import { DriverService } from '../../../core/services/driver.service';
import { forkJoin, of, Subject, Subscription, timer } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';

/* ─────────────────────────────────────────────────────────────────────────── */
export type RecordType = 'Shipment' | 'Driver';

export interface UnifiedRecord {
  _type:       RecordType;
  id:          string;           // trackingNumber or driverId
  name:        string;           // customerName or driverName
  status:      string;
  detail:      string;           // origin→dest or license
  cargoType?:  string;
  weight?:     string;
  driver?:     string;
  vehicle?:    string;
  revenue?:    string;
  date?:       string;
  raw:         any;
}

const STATUS_CLASS: Record<string, string> = {
  Pending:             'badge-pending',
  InTransit:           'badge-transit',
  OutForDelivery:      'badge-ofd',
  AtWarehouse:         'badge-warehouse',
  Delivered:           'badge-delivered',
  Cancelled:           'badge-cancelled',
  FailedDelivery:      'badge-failed',
  Active:              'badge-delivered',
  Inactive:            'badge-cancelled',
  OnLeave:             'badge-pending',
};

/* ─────────────────────────────────────────────────────────────────────────── */
@Component({
  selector: 'app-shipment-report',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatButtonModule, MatIconModule,
    MatInputModule, MatFormFieldModule,
    MatProgressSpinnerModule, MatChipsModule, MatTooltipModule
  ],
  templateUrl: './shipment-report.component.html',
  styleUrls: ['./shipment-report.component.scss']
})
export class ShipmentReportComponent implements OnInit, OnDestroy {
  private reportService  = inject(ReportService);
  private shipmentService = inject(ShipmentService);
  private driverService  = inject(DriverService);
  private cdr            = inject(ChangeDetectorRef);

  /* ── State ────────────────────────────────────────────────────────────── */
  isLoading    = true;
  hasError     = false;
  lastUpdated: Date | null = null;

  /* ── Data ─────────────────────────────────────────────────────────────── */
  allRecords:      UnifiedRecord[] = [];
  filteredRecords: UnifiedRecord[] = [];
  pagedRecords:    UnifiedRecord[] = [];

  /* ── Search ───────────────────────────────────────────────────────────── */
  searchQuery  = '';
  activeType: 'All' | RecordType = 'All';
  private search$ = new Subject<string>();

  /* ── Pagination ───────────────────────────────────────────────────────── */
  pageSize    = 15;
  currentPage = 1;
  totalPages  = 1;

  /* ── Summary counters ─────────────────────────────────────────────────── */
  summary = { total: 0, shipments: 0, drivers: 0, delivered: 0, inTransit: 0, pending: 0 };

  /* ── Polling ──────────────────────────────────────────────────────────── */
  private pollSub?:   Subscription;
  private searchSub?: Subscription;
  private readonly POLL_MS = 30_000;

  /* ═══════════════════════════════════════════════════════════════════════ */
  ngOnInit() {
    this.startSearchPipe();
    this.startPolling();
  }

  ngOnDestroy() {
    this.pollSub?.unsubscribe();
    this.searchSub?.unsubscribe();
  }

  /* ── Search reactive pipe ─────────────────────────────────────────────── */
  private startSearchPipe() {
    this.searchSub = this.search$.pipe(
      debounceTime(250),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 1;
      this.applyFilter();
    });
  }

  onSearchChange(val: string) {
    this.searchQuery = val;
    this.search$.next(val);
  }

  setTypeFilter(t: 'All' | RecordType) {
    this.activeType  = t;
    this.currentPage = 1;
    this.applyFilter();
  }

  /* ── Polling loop ─────────────────────────────────────────────────────── */
  private startPolling() {
    this.pollSub = timer(0, this.POLL_MS)
      .pipe(switchMap(() => this.fetchAll()))
      .subscribe({
        next: (res: any[]) => {
          const [shipmentsRes, driversRes] = res;
          this.buildUnified(shipmentsRes, driversRes);
          this.hasError    = false;
          this.isLoading   = false;
          this.lastUpdated = new Date();
          this.cdr.detectChanges();
        },
        error: () => {
          this.hasError  = true;
          this.isLoading = false;
          this.cdr.detectChanges();
        }
      });
  }

  private fetchAll() {
    const safe = (obs: any) => obs.pipe(catchError(() => of(null)));
    return forkJoin([
      // ✅ Same endpoint as Shipments page — live Shipment Service DB, not stale read model
      safe(this.shipmentService.getAll(1, 200)),
      // Driver list
      safe(this.driverService.getAll()),
    ]);
  }

  reload() {
    this.isLoading = true;
    this.hasError  = false;
    this.pollSub?.unsubscribe();
    this.startPolling();
  }

  /* ── Build unified list ───────────────────────────────────────────────── */
  private buildUnified(shipmentsRes: any, driversRes: any) {
    const records: UnifiedRecord[] = [];

    /* ── Shipments ─────────────────────────────────────────────────────────
     * ShipmentService.getAll() normalizes into ApiResponse<PagedResult<T>>
     * where .data.items = Shipment[]  (same shape as dashboard/shipments page)
     * ─────────────────────────────────────────────────────────────────────── */
    const ships: any[] = (() => {
      if (!shipmentsRes) return [];
      const d = shipmentsRes.data;
      // Normalized by BaseApiService → always .data.items
      if (d && Array.isArray(d.items)) return d.items;
      // Direct array fallback
      if (Array.isArray(d)) return d;
      // Raw response array fallback
      if (Array.isArray(shipmentsRes)) return shipmentsRes;
      return [];
    })();

    for (const s of ships) {
      // Origin/destination — Shipment Service uses these fields
      const origin = s.originAddress || s.senderCity || '?';
      const dest   = s.destinationAddress || s.recipientCity || s.recipientName || '?';
      records.push({
        _type:     'Shipment',
        id:        s.trackingNumber ?? s.id ?? '—',
        name:      s.senderName ?? s.customerName ?? '—',
        status:    s.status     ?? '—',
        detail:    `${origin} → ${dest}`,
        cargoType: s.cargoType  ?? s.cargoDescription,
        weight:    s.weightKg   ? `${s.weightKg} kg` : undefined,
        driver:    s.driverName,               // only present in report DTO
        vehicle:   s.plateNumber,              // only present in report DTO
        revenue:   s.invoiceAmount ? `$${Number(s.invoiceAmount).toFixed(2)}` : undefined,
        date:      s.createdAt,
        raw:       s,
      });
    }

    /* Drivers */
    const driverRaw = driversRes?.data ?? [];
    const drivers: any[] = Array.isArray(driverRaw) ? driverRaw
      : Array.isArray(driverRaw.items) ? driverRaw.items : [];

    for (const d of drivers) {
      const fullName = [d.firstName, d.lastName].filter(Boolean).join(' ') || d.employeeId || '—';
      records.push({
        _type:  'Driver',
        id:     d.employeeId ?? d.id ?? '—',
        name:   fullName,
        status: d.status ?? '—',
        detail: `License: ${d.licenseNumber ?? '—'}`,
        driver: d.phone,
        vehicle: d.currentVehiclePlate,
        date:   d.dateJoined,
        raw:    d,
      });
    }

    this.allRecords = records;

    /* Summary counters (no status filtering — just total counts) */
    this.summary = {
      total:     records.length,
      shipments: ships.length,
      drivers:   records.filter(r => r._type === 'Driver').length,
      delivered: 0,
      inTransit: 0,
      pending:   0,
    };

    this.applyFilter();
  }

  /* ── Filter + search ──────────────────────────────────────────────────── */
  applyFilter() {
    const q = this.searchQuery.trim().toLowerCase();

    this.filteredRecords = this.allRecords.filter(r => {
      if (this.activeType !== 'All' && r._type !== this.activeType) return false;
      if (!q) return true;
      return (
        r.id.toLowerCase().includes(q)        ||
        r.name.toLowerCase().includes(q)      ||
        r.status.toLowerCase().includes(q)    ||
        r.detail.toLowerCase().includes(q)    ||
        (r.cargoType ?? '').toLowerCase().includes(q) ||
        (r.driver    ?? '').toLowerCase().includes(q) ||
        (r.vehicle   ?? '').toLowerCase().includes(q)
      );
    });

    this.totalPages = Math.max(1, Math.ceil(this.filteredRecords.length / this.pageSize));
    if (this.currentPage > this.totalPages) this.currentPage = this.totalPages;
    this.slicePage();
  }

  private slicePage() {
    const start = (this.currentPage - 1) * this.pageSize;
    this.pagedRecords = this.filteredRecords.slice(start, start + this.pageSize);
  }

  prevPage() { if (this.currentPage > 1) { this.currentPage--; this.slicePage(); } }
  nextPage() { if (this.currentPage < this.totalPages) { this.currentPage++; this.slicePage(); } }

  /* ── Helpers ──────────────────────────────────────────────────────────── */
  badgeClass(status: string): string { return STATUS_CLASS[status] ?? 'badge-default'; }

  exportCsv() {
    if (this.filteredRecords.length === 0) return;
    const rows = this.filteredRecords.map(r => ({
      Type: r._type, ID: r.id, Name: r.name, Status: r.status,
      Detail: r.detail, CargoType: r.cargoType ?? '', Weight: r.weight ?? '',
      Driver: r.driver ?? '', Vehicle: r.vehicle ?? '', Revenue: r.revenue ?? '', Date: r.date ?? ''
    }));
    this.reportService.exportToCsv(rows, `cms_report_${Date.now()}`);
  }

  get pageNumbers(): number[] {
    const total  = this.totalPages;
    const cur    = this.currentPage;
    const result = new Set<number>();
    [1, 2, cur - 1, cur, cur + 1, total - 1, total].forEach(n => {
      if (n >= 1 && n <= total) result.add(n);
    });
    return [...result].sort((a, b) => a - b);
  }
  goToPage(n: number) { this.currentPage = n; this.slicePage(); }

  trackById = (_: number, r: UnifiedRecord) => r.id + r._type;
}
