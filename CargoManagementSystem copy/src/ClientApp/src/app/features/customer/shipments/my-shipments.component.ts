import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterModule } from '@angular/router';
import { ShipmentService } from '../../../core/services/shipment.service';

@Component({
  selector: 'app-my-shipments',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatChipsModule, MatProgressSpinnerModule, RouterModule],
  template: `
    <div class="shipments-container">
      <div class="header-actions">
        <h2>My Shipments</h2>
        <button mat-flat-button color="primary" routerLink="/user/book-shipment">
          <mat-icon>add</mat-icon> Book New Shipment
        </button>
      </div>

      <div class="loading-overlay" *ngIf="loading">
        <mat-progress-spinner mode="indeterminate" diameter="40"></mat-progress-spinner>
      </div>

      <div class="error-msg" *ngIf="error">
        {{ error }}
        <button mat-button (click)="loadShipments()">Retry</button>
      </div>

      <div class="table-card" *ngIf="!loading && !error">
        <table mat-table [dataSource]="shipments" class="full-width-table">
          <ng-container matColumnDef="trackingNumber">
            <th mat-header-cell *matHeaderCellDef> Tracking No </th>
            <td mat-cell *matCellDef="let s"> <span class="tracking-code">{{s.trackingNumber}}</span> </td>
          </ng-container>

          <ng-container matColumnDef="route">
            <th mat-header-cell *matHeaderCellDef> Route </th>
            <td mat-cell *matCellDef="let s"> {{s.senderCity}} → {{s.recipientCity}} </td>
          </ng-container>

          <ng-container matColumnDef="date">
            <th mat-header-cell *matHeaderCellDef> Created Date </th>
            <td mat-cell *matCellDef="let s"> {{s.createdAt | date}} </td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef> Status </th>
            <td mat-cell *matCellDef="let s">
              <mat-chip-set>
                <mat-chip [ngClass]="s.status.toLowerCase().replace(' ', '-')">{{s.status}}</mat-chip>
              </mat-chip-set>
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef> </th>
            <td mat-cell *matCellDef="let s">
              <button mat-icon-button [routerLink]="['/user/track']" [queryParams]="{tracking: s.trackingNumber}">
                <mat-icon>visibility</mat-icon>
              </button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
        </table>
        
        <div class="empty-state" *ngIf="shipments.length === 0">
          <mat-icon>inventory_2</mat-icon>
          <p>No shipments found.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .shipments-container { display: flex; flex-direction: column; gap: 24px; position: relative; min-height: 400px; }
    .header-actions { display: flex; justify-content: space-between; align-items: center; }
    .table-card { background: #161B22; border-radius: 16px; border: 1px solid rgba(255,255,255,0.1); overflow: hidden; }
    .full-width-table { width: 100%; background: transparent; }
    .tracking-code { font-family: 'JetBrains Mono', monospace; color: #3B82F6; font-weight: 600; }
    
    .loading-overlay { display: flex; justify-content: center; padding: 40px; }
    .error-msg { color: #F87171; text-align: center; padding: 20px; }
    .empty-state { padding: 80px; text-align: center; color: #64748B; mat-icon { font-size: 48px; width: 48px; height: 48px; margin-bottom: 16px; } }

    ::ng-deep .mat-mdc-chip { 
      &.in-transit { --mdc-chip-label-text-color: #3B82F6; background: rgba(59, 130, 246, 0.1); }
      &.delivered { --mdc-chip-label-text-color: #22C55E; background: rgba(34, 197, 94, 0.1); }
      &.pending { --mdc-chip-label-text-color: #F59E0B; background: rgba(245, 158, 11, 0.1); }
      &.approved { --mdc-chip-label-text-color: #22C55E; background: rgba(34, 197, 94, 0.1); }
      &.rejected { --mdc-chip-label-text-color: #F87171; background: rgba(248, 113, 113, 0.1); }
    }

    ::ng-deep .mat-mdc-header-cell { color: #94A3B8 !important; border-bottom-color: rgba(255,255,255,0.1) !important; }
    ::ng-deep .mat-mdc-cell { color: #E2E8F0 !important; border-bottom-color: rgba(255,255,255,0.05) !important; }
  `]
})
export class MyShipmentsComponent implements OnInit {
  private shipmentService = inject(ShipmentService);
  
  displayedColumns: string[] = ['trackingNumber', 'route', 'date', 'status', 'actions'];
  shipments: any[] = [];
  loading = true;
  error = '';

  ngOnInit() {
    this.loadShipments();
  }

  loadShipments() {
    this.loading = true;
    this.error = '';
    this.shipmentService.getMyShipments().subscribe({
      next: (res) => {
        this.shipments = Array.isArray(res.data) ? res.data : (res.data as any)?.items || [];
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to fetch shipments. Please try again.';
        this.loading = false;
      }
    });
  }
}