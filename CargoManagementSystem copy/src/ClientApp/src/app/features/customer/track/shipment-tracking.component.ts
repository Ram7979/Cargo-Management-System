import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ShipmentService } from '../../../core/services/shipment.service';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-shipment-tracking',
  standalone: true,
  imports: [CommonModule, FormsModule, MatInputModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  template: `
    <div class="tracking-container">
      <h2>Track Package</h2>
      
      <div class="search-box">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Tracking Number</mat-label>
          <input matInput [(ngModel)]="trackingNumber" placeholder="e.g. CMS-2026-000011" (keyup.enter)="track()">
          <mat-icon matSuffix>search</mat-icon>
        </mat-form-field>
        <button mat-flat-button color="primary" class="track-btn" [disabled]="loading && !isSilentLoad" (click)="track()">
          <span *ngIf="!(loading && !isSilentLoad)">Track</span>
          <mat-spinner *ngIf="loading && !isSilentLoad" diameter="24" color="accent"></mat-spinner>
        </button>
      </div>

      <div class="error-msg" *ngIf="error">
        <mat-icon>error_outline</mat-icon>
        <span>{{ error }}</span>
      </div>

      <div class="tracking-result" *ngIf="trackingData">
        <div class="result-header">
          <div class="main-info">
            <span class="label">Tracking Number</span>
            <span class="value">{{trackingData.trackingNumber}}</span>
          </div>
          <div class="status-info">
            <span class="label">Current Status</span>
            <span class="status-badge" [ngClass]="getBadgeClass(trackingData.status)">
              <mat-icon inline>{{ getIconForStatus(trackingData.status) }}</mat-icon>
              {{ formatStatus(trackingData.status) }}
            </span>
          </div>
        </div>

        <div class="timeline-container">
          <div class="timeline-item" *ngFor="let event of trackingData.statusHistory; let first = first; let last = last" [class.latest]="first">
            <div class="timeline-icon">
              <mat-icon>{{ getIconForStatus(event.newStatus) }}</mat-icon>
              <div class="timeline-line" *ngIf="!last"></div>
            </div>
            <div class="timeline-content">
              <div class="timeline-header">
                <span class="event-status">{{ formatStatus(event.newStatus) }}</span>
                <span class="event-date">{{ event.timestamp | date:'MMM d, y, h:mm a' }}</span>
              </div>
              <div class="event-location" *ngIf="event.location">
                <mat-icon inline>place</mat-icon> 
                {{ event.location.latitude }}, {{ event.location.longitude }}
              </div>
              <div class="event-notes" *ngIf="event.notes">{{ event.notes }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .tracking-container { max-width: 800px; margin: 0 auto; display: flex; flex-direction: column; gap: 32px; padding: 16px 0; }
    h2 { font-size: 1.8rem; font-weight: 800; color: #F8FAFC; margin: 0; }
    
    .search-box { display: flex; gap: 16px; align-items: flex-start; background: #161B22; padding: 24px; border-radius: 16px; border: 1px solid rgba(255,255,255,0.08); box-shadow: 0 10px 30px rgba(0,0,0,0.2); }
    .full-width { flex: 1; }
    .track-btn { height: 56px; padding: 0 32px; font-size: 1rem; font-weight: 600; border-radius: 8px; }
    
    .error-msg { display: flex; align-items: center; gap: 12px; color: #F87171; background: rgba(248, 113, 113, 0.1); padding: 16px 20px; border-radius: 12px; border: 1px solid rgba(248, 113, 113, 0.2); font-weight: 500; }

    .tracking-result { background: #161B22; border-radius: 16px; border: 1px solid rgba(255,255,255,0.08); padding: 32px; box-shadow: 0 10px 40px rgba(0,0,0,0.2); }
    
    .result-header { display: flex; justify-content: space-between; align-items: flex-end; margin-bottom: 40px; padding-bottom: 24px; border-bottom: 1px solid rgba(255,255,255,0.08); flex-wrap: wrap; gap: 20px; }
    .label { display: block; font-size: 0.85rem; color: #94A3B8; margin-bottom: 8px; font-weight: 500; text-transform: uppercase; letter-spacing: 0.5px; }
    .value { font-size: 1.75rem; font-weight: 700; font-family: 'JetBrains Mono', monospace; color: #3B82F6; letter-spacing: -0.5px; }
    
    .status-badge { display: inline-flex; align-items: center; gap: 6px; padding: 8px 20px; border-radius: 50px; font-weight: 700; font-size: 1.1rem; }
    .status-badge mat-icon { font-size: 20px; width: 20px; height: 20px; }
    .status-badge.badge-pending { background: rgba(245, 158, 11, 0.15); color: #F59E0B; border: 1px solid rgba(245, 158, 11, 0.2); }
    .status-badge.badge-transit { background: rgba(59, 130, 246, 0.15); color: #3B82F6; border: 1px solid rgba(59, 130, 246, 0.2); }
    .status-badge.badge-delivered { background: rgba(34, 197, 94, 0.15); color: #22C55E; border: 1px solid rgba(34, 197, 94, 0.2); }
    .status-badge.badge-failed { background: rgba(248, 113, 113, 0.15); color: #F87171; border: 1px solid rgba(248, 113, 113, 0.2); }
    .status-badge.badge-default { background: rgba(148, 163, 184, 0.15); color: #94A3B8; border: 1px solid rgba(148, 163, 184, 0.2); }

    .timeline-container { padding: 10px 10px 10px 20px; display: flex; flex-direction: column; }
    .timeline-item { display: flex; gap: 24px; position: relative; min-height: 90px; }
    
    .timeline-icon { display: flex; flex-direction: column; align-items: center; width: 44px; }
    .timeline-icon mat-icon { width: 44px; height: 44px; font-size: 22px; display: flex; align-items: center; justify-content: center; border-radius: 50%; background: #21262D; color: #64748B; border: 2px solid #30363D; z-index: 2; transition: all 0.3s ease; }
    
    .timeline-item.latest .timeline-icon mat-icon { background: rgba(59, 130, 246, 0.1); color: #3B82F6; border-color: #3B82F6; box-shadow: 0 0 20px rgba(59, 130, 246, 0.3); transform: scale(1.1); }
    
    .timeline-line { flex: 1; width: 2px; background: #30363D; margin-top: 4px; margin-bottom: 4px; border-radius: 2px; }
    .timeline-item.latest .timeline-line { background: linear-gradient(to bottom, #3B82F6, #30363D); }
    
    .timeline-content { flex: 1; padding-bottom: 32px; padding-top: 8px; }
    .timeline-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 8px; flex-wrap: wrap; gap: 8px; }
    
    .event-status { font-size: 1.15rem; font-weight: 700; color: #94A3B8; transition: color 0.3s; }
    .timeline-item.latest .event-status { color: #F8FAFC; }
    
    .event-date { font-size: 0.9rem; color: #64748B; font-weight: 500; font-family: 'JetBrains Mono', monospace; }
    .timeline-item.latest .event-date { color: #94A3B8; }
    
    .event-location { display: flex; align-items: center; gap: 6px; font-size: 0.9rem; color: #64748B; margin-bottom: 8px; }
    .event-location mat-icon { font-size: 16px; width: 16px; height: 16px; }
    
    .event-notes { font-size: 0.95rem; color: #94A3B8; background: rgba(255, 255, 255, 0.02); padding: 12px 16px; border-radius: 8px; border-left: 3px solid #30363D; margin-top: 8px; line-height: 1.5; }
    .timeline-item.latest .event-notes { border-left-color: #3B82F6; background: rgba(59, 130, 246, 0.05); color: #E2E8F0; }

    /* Fix Input Text Colors for Dark Theme */
    .tracking-container ::ng-deep input.mat-mdc-input-element { color: #ffffff !important; font-size: 1.1rem; }
    .tracking-container ::ng-deep input.mat-mdc-input-element::placeholder { color: #64748B !important; }
    .tracking-container ::ng-deep .mdc-text-field--outlined .mdc-notched-outline { border-color: rgba(255, 255, 255, 0.15) !important; border-width: 1.5px; }
    .tracking-container ::ng-deep .mdc-text-field--outlined.mdc-text-field--focused .mdc-notched-outline { border-color: #3B82F6 !important; border-width: 2px; }
    
    @media(max-width: 600px) {
      .timeline-header { flex-direction: column; }
      .search-box { flex-direction: column; align-items: stretch; }
      .track-btn { width: 100%; }
    }
  `]
})
export class ShipmentTrackingComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private shipmentService = inject(ShipmentService);

  trackingNumber = '';
  trackingData: any = null;
  loading = false;
  isSilentLoad = false;
  error = '';
  private pollSub?: Subscription;

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['tracking']) {
        this.trackingNumber = params['tracking'];
        this.track();
      }
    });
  }

  ngOnDestroy() {
    this.stopPolling();
  }

  track() {
    if (!this.trackingNumber) return;
    this.loading = true;
    this.isSilentLoad = false;
    this.error = '';
    this.trackingData = null;
    this.fetchData();
    this.startPolling();
  }

  startPolling() {
    this.stopPolling();
    // Poll every 15 seconds to ensure real-time tracking
    this.pollSub = timer(15000, 15000).subscribe(() => {
      if (this.trackingNumber && !this.loading) {
        this.isSilentLoad = true;
        this.loading = true;
        this.fetchData();
      }
    });
  }

  stopPolling() {
    if (this.pollSub) {
      this.pollSub.unsubscribe();
      this.pollSub = undefined;
    }
  }

  fetchData() {
    this.shipmentService.getShipmentByTracking(this.trackingNumber).subscribe({
      next: (res: any) => {
        if (res.success && res.data) {
          const shipment = res.data.shipment || res.data;
          let timeline = res.data.timeline || res.data.statusHistory || [];
          
          // Map to uniform objects and parse dates
          let mappedTimeline = timeline.map((t: any) => ({
            newStatus: t.toStatus || t.newStatus || t.status || t.eventName,
            timestamp: new Date(t.changedAt || t.timestamp || t.createdAt || t.date || new Date()),
            location: t.location,
            notes: t.notes || t.description
          }));

          // 1. Sort by timestamp descending (latest first)
          mappedTimeline.sort((a: any, b: any) => b.timestamp.getTime() - a.timestamp.getTime());

          // 2. Remove duplicate consecutive entries
          const uniqueTimeline = [];
          let lastStatus = '';
          for (const event of mappedTimeline) {
             if (event.newStatus !== lastStatus) {
               uniqueTimeline.push(event);
               lastStatus = event.newStatus;
             } else {
               // If it's a duplicate status, keep the one with notes if possible
               if (event.notes && !uniqueTimeline[uniqueTimeline.length - 1].notes) {
                  uniqueTimeline[uniqueTimeline.length - 1].notes = event.notes;
               }
             }
          }

          this.trackingData = {
            ...shipment,
            statusHistory: uniqueTimeline
          };
        } else {
          this.error = 'No tracking data available for this number.';
          this.stopPolling();
        }
        this.loading = false;
      },
      error: (err: any) => {
        // Only show error visually if it's the first load
        if (!this.trackingData) {
          this.error = 'Failed to fetch tracking details. Please try again.';
        }
        this.loading = false;
        this.stopPolling();
      }
    });
  }

  formatStatus(status: string): string {
    if (!status) return 'Unknown';
    return status.replace(/([A-Z])/g, ' $1').trim();
  }

  getIconForStatus(status: string): string {
    const s = (status || '').toLowerCase();
    if (s.includes('pending') || s.includes('hold')) return 'hourglass_empty';
    if (s.includes('transit')) return 'local_shipping';
    if (s.includes('delivery')) return 'directions_car';
    if (s.includes('delivered')) return 'check_circle';
    if (s.includes('warehouse')) return 'store';
    if (s.includes('cancel') || s.includes('fail')) return 'cancel';
    if (s.includes('assign')) return 'assignment_ind';
    if (s.includes('pick')) return 'inventory_2';
    return 'radio_button_checked';
  }

  getBadgeClass(status: string): string {
    const s = (status || '').toLowerCase();
    if (s.includes('pending')) return 'badge-pending';
    if (s.includes('transit') || s.includes('delivery')) return 'badge-transit';
    if (s.includes('delivered')) return 'badge-delivered';
    if (s.includes('fail') || s.includes('cancel')) return 'badge-failed';
    return 'badge-default';
  }
}