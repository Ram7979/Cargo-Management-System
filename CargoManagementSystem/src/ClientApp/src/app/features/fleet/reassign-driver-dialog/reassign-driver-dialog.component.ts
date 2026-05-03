import { Component, OnInit, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatRadioModule } from '@angular/material/radio';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { DriverService, Driver } from '../../../core/services/driver.service';

export interface ReassignDriverDialogData {
  vehicleId: string;
  vehiclePlate: string;
  currentDriverId?: string;
}

@Component({
  selector: 'app-reassign-driver-dialog',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatDialogModule, MatButtonModule,
    MatIconModule, MatRadioModule, MatProgressSpinnerModule,
    MatDividerModule, MatInputModule, MatFormFieldModule
  ],
  template: `
    <h2 mat-dialog-title class="dialog-title">
      <mat-icon class="title-icon">swap_horiz</mat-icon>
      Assign Driver to {{ data.vehiclePlate }}
    </h2>

    <mat-dialog-content class="dialog-content">
      <div class="loading-state" *ngIf="isLoading">
        <mat-spinner diameter="36"></mat-spinner>
        <span>Loading available drivers...</span>
      </div>

      <div class="empty-state" *ngIf="!isLoading && availableDrivers.length === 0">
        <mat-icon class="empty-icon">person_off</mat-icon>
        <p>No available drivers found</p>
        <span class="hint">All drivers are currently assigned or off-duty.</span>
      </div>

      <div class="driver-list" *ngIf="!isLoading && availableDrivers.length > 0">
        <mat-radio-group [(ngModel)]="selectedDriverId" class="driver-radio-group">
          <div class="driver-option" *ngFor="let driver of availableDrivers"
               [class.selected]="selectedDriverId === driver.id"
               (click)="selectedDriverId = driver.id">
            <mat-radio-button [value]="driver.id" color="primary">
              <div class="driver-info">
                <div class="driver-avatar">
                  {{ getInitials(driver) }}
                </div>
                <div class="driver-meta">
                  <div class="driver-name">{{ getDriverName(driver) }}</div>
                  <div class="driver-details-row">
                    <span class="detail-chip">
                      <mat-icon>badge</mat-icon> {{ driver.employeeId }}
                    </span>
                    <span class="detail-chip" *ngIf="driver.licenseNumber">
                      <mat-icon>credit_card</mat-icon> {{ driver.licenseNumber }}
                    </span>
                    <span class="detail-chip status-chip available">
                      <mat-icon>check_circle</mat-icon> {{ driver.status }}
                    </span>
                  </div>
                </div>
              </div>
            </mat-radio-button>
          </div>
        </mat-radio-group>
      </div>
    </mat-dialog-content>

    <mat-dialog-actions align="end" class="dialog-actions">
      <button mat-stroked-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" 
              [disabled]="!selectedDriverId || isAssigning"
              (click)="onAssign()">
        <mat-spinner diameter="18" *ngIf="isAssigning" class="btn-spinner"></mat-spinner>
        <mat-icon *ngIf="!isAssigning">person_add</mat-icon>
        {{ isAssigning ? 'Assigning...' : 'Assign Driver' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    .dialog-title {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 1.15rem;
      margin: 0;
      padding: 20px 24px 0;
    }
    .title-icon { color: #3182ce; }

    .dialog-content {
      min-width: 480px;
      max-height: 420px;
      padding: 16px 24px;
    }

    .loading-state, .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 40px 0;
      gap: 12px;
      color: #718096;
    }
    .empty-icon { font-size: 48px; width: 48px; height: 48px; color: #cbd5e0; }

    .driver-radio-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .driver-option {
      border: 2px solid #e2e8f0;
      border-radius: 12px;
      padding: 12px 16px;
      cursor: pointer;
      transition: all 0.2s ease;

      &:hover { border-color: #90cdf4; background: #f7fafc; }
      &.selected { border-color: #3182ce; background: #ebf8ff; }
    }

    .driver-info {
      display: flex;
      align-items: center;
      gap: 12px;
    }

    .driver-avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 700;
      font-size: 0.85rem;
      flex-shrink: 0;
    }

    .driver-meta {
      display: flex;
      flex-direction: column;
      gap: 4px;
    }

    .driver-name {
      font-weight: 600;
      font-size: 0.95rem;
      color: #2d3748;
    }

    .driver-details-row {
      display: flex;
      gap: 8px;
      flex-wrap: wrap;
    }

    .detail-chip {
      display: inline-flex;
      align-items: center;
      gap: 2px;
      font-size: 0.72rem;
      color: #718096;
      background: #f7fafc;
      padding: 2px 6px;
      border-radius: 4px;

      mat-icon {
        font-size: 12px;
        width: 12px;
        height: 12px;
      }
    }

    .status-chip.available {
      background: #f0fff4;
      color: #22543d;
    }

    .dialog-actions {
      padding: 12px 24px 20px;
      gap: 8px;
    }

    .btn-spinner { display: inline-block; margin-right: 8px; }
  `]
})
export class ReassignDriverDialogComponent implements OnInit {
  private driverService = inject(DriverService);

  availableDrivers: Driver[] = [];
  selectedDriverId: string = '';
  isLoading = true;
  isAssigning = false;

  constructor(
    public dialogRef: MatDialogRef<ReassignDriverDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ReassignDriverDialogData
  ) {}

  ngOnInit() {
    this.loadAvailableDrivers();
  }

  loadAvailableDrivers() {
    this.isLoading = true;
    this.driverService.getAvailable().subscribe({
      next: (res) => {
        const drivers = res?.data || [];
        // Filter out the currently assigned driver
        this.availableDrivers = Array.isArray(drivers) 
          ? drivers.filter(d => d.id !== this.data.currentDriverId)
          : [];
        this.isLoading = false;
      },
      error: () => {
        // Fallback: fetch all drivers instead
        this.driverService.getAll(1, 50).subscribe({
          next: (res) => {
            const items = res?.data?.items || [];
            this.availableDrivers = items.filter(
              (d: Driver) => d.status === 'Available' && d.id !== this.data.currentDriverId
            );
            this.isLoading = false;
          },
          error: () => {
            this.availableDrivers = [];
            this.isLoading = false;
          }
        });
      }
    });
  }

  getDriverName(driver: Driver): string {
    if (driver.firstName || driver.lastName) {
      return `${driver.firstName || ''} ${driver.lastName || ''}`.trim();
    }
    return driver.employeeId || 'Unknown Driver';
  }

  getInitials(driver: Driver): string {
    const name = this.getDriverName(driver);
    const parts = name.split(' ');
    return parts.map(p => p[0]?.toUpperCase() || '').join('').substring(0, 2) || '?';
  }

  onAssign() {
    if (!this.selectedDriverId) return;
    const selectedDriver = this.availableDrivers.find(d => d.id === this.selectedDriverId);
    this.dialogRef.close({
      driverId: this.selectedDriverId,
      driverName: selectedDriver ? this.getDriverName(selectedDriver) : 'Driver'
    });
  }
}
