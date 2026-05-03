import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatMenuModule } from '@angular/material/menu';
import { DriverService, Driver } from '../../core/services/driver.service';
import { UserService } from '../../core/services/user.service';
import { NotificationService } from '../../core/services/notification.service';
import { StatusChipComponent } from '../../shared/components/status-chip/status-chip.component';
import { AssignmentService } from '../../core/services/assignment.service';
import { VehicleService } from '../../core/services/vehicle.service';
import { forkJoin, of } from 'rxjs';
import { map } from 'rxjs/operators';

// ─── Add/Edit Driver Dialog ───────────────────────────────────────────────────
@Component({
  selector: 'app-driver-form-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
            MatInputModule, MatButtonModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule],
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit Driver' : 'Register Driver' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Linked User (Identity) *</mat-label>
          <mat-select formControlName="userId">
            <mat-option *ngFor="let u of users" [value]="u.id || u.userId">
              {{ u.firstName }} {{ u.lastName }} — {{ u.email }}
            </mat-option>
          </mat-select>
          <mat-hint>Select the user account this driver is linked to</mat-hint>
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Employee ID *</mat-label>
          <input matInput formControlName="employeeId" placeholder="e.g. EMP-001">
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>License Number *</mat-label>
          <input matInput formControlName="licenseNumber">
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>License Expiry *</mat-label>
          <input matInput [matDatepicker]="expPicker" formControlName="licenseExpiry">
          <mat-datepicker-toggle matSuffix [for]="expPicker"></mat-datepicker-toggle>
          <mat-datepicker #expPicker></mat-datepicker>
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Phone *</mat-label>
          <input matInput formControlName="phone">
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Date Joined</mat-label>
          <input matInput [matDatepicker]="joinPicker" formControlName="dateJoined">
          <mat-datepicker-toggle matSuffix [for]="joinPicker"></mat-datepicker-toggle>
          <mat-datepicker #joinPicker></mat-datepicker>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-flat-button color="primary" [disabled]="isSubmitting || form.invalid" (click)="onSubmit()">
        {{ isSubmitting ? 'Saving...' : (data ? 'Update Driver' : 'Register Driver') }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full-width { width: 100%; } .dialog-form { display: flex; flex-direction: column; gap: 6px; padding: 8px 0; }`]
})
export class DriverFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private driverService = inject(DriverService);
  private userService = inject(UserService);
  private notification = inject(NotificationService);
  private dialogRef = inject(MatDialogRef<DriverFormDialogComponent>);
  data = inject(MAT_DIALOG_DATA, { optional: true });

  users: any[] = [];
  isSubmitting = false;

  form = this.fb.group({
    userId:        ['', Validators.required],
    employeeId:    ['', Validators.required],
    licenseNumber: ['', Validators.required],
    licenseExpiry: [null as Date | null, Validators.required],
    phone:         ['', Validators.required],
    dateJoined:    [null as Date | null]
  });

  ngOnInit() {
    // Load users to populate dropdown
    this.userService.getAll(1, 200).subscribe((res: any) => {
      if (res.success) {
        const items = res.data?.items || res.data || [];
        this.users = items;
      }
    });
  }

  onSubmit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSubmitting = true;
    const v = this.form.value;
    const payload = {
      userId:        v.userId,
      employeeId:    v.employeeId,
      licenseNumber: v.licenseNumber,
      licenseExpiry: v.licenseExpiry ? new Date(v.licenseExpiry!).toISOString() : null,
      phone:         v.phone,
      dateJoined:    v.dateJoined ? new Date(v.dateJoined!).toISOString() : null
    };
    // POST /api/v1/drivers
    this.driverService.create(payload).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('Driver registered successfully');
          this.dialogRef.close(true);
        } else {
          this.notification.error(res.errors?.join(', ') || res.message || 'Failed to register driver');
        }
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.notification.error(err.error?.errors?.join(', ') || err.error?.message || 'Failed to register driver');
        this.isSubmitting = false;
      }
    });
  }
}

// ─── Main Drivers Page ────────────────────────────────────────────────────────
@Component({
  selector: 'app-drivers-page',
  standalone: true,
  imports: [
    CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatChipsModule,
    MatDialogModule, MatPaginatorModule, MatMenuModule, MatProgressSpinnerModule,
    StatusChipComponent
  ],
  template: `
    <div class="page-header">
      <h2>Drivers</h2>
      <button mat-raised-button color="primary" (click)="openAddDriver()">
        <mat-icon>person_add</mat-icon> Register Driver
      </button>
    </div>

    <div class="card-container">
      <div class="loading-shade" *ngIf="isLoading"><mat-spinner diameter="40"></mat-spinner></div>

      <table mat-table [dataSource]="dataSource" class="w-100">
        <ng-container matColumnDef="name">
          <th mat-header-cell *matHeaderCellDef>Driver</th>
          <td mat-cell *matCellDef="let d">
            <div class="driver-cell">
              <div class="avatar-bg">{{ d.firstName?.[0] }}{{ d.lastName?.[0] }}</div>
              <div>
                <div>{{ d.firstName }} {{ d.lastName }}</div>
                <small class="muted">{{ d.employeeId }}</small>
              </div>
            </div>
          </td>
        </ng-container>
        <ng-container matColumnDef="license">
          <th mat-header-cell *matHeaderCellDef>License #</th>
          <td mat-cell *matCellDef="let d">{{ d.licenseNumber }}</td>
        </ng-container>
        <ng-container matColumnDef="phone">
          <th mat-header-cell *matHeaderCellDef>Phone</th>
          <td mat-cell *matCellDef="let d">{{ d.phone || d.phoneNumber }}</td>
        </ng-container>
        <ng-container matColumnDef="vehicle">
          <th mat-header-cell *matHeaderCellDef>Assigned Vehicle</th>
          <td mat-cell *matCellDef="let d">
            <span *ngIf="d.currentVehiclePlate" class="plate-tag">{{ d.currentVehiclePlate }}</span>
            <span *ngIf="!d.currentVehiclePlate" class="muted">Unassigned</span>
          </td>
        </ng-container>
        <ng-container matColumnDef="status">
          <th mat-header-cell *matHeaderCellDef>Status</th>
          <td mat-cell *matCellDef="let d"><app-status-chip [status]="d.status"></app-status-chip></td>
        </ng-container>
        <ng-container matColumnDef="actions">
          <th mat-header-cell *matHeaderCellDef></th>
          <td mat-cell *matCellDef="let d">
            <button mat-icon-button [matMenuTriggerFor]="menu"><mat-icon>more_vert</mat-icon></button>
            <mat-menu #menu="matMenu">
              <button mat-menu-item (click)="setStatus(d, 'Available')">
                <mat-icon>check_circle</mat-icon> Set Available
              </button>
              <button mat-menu-item (click)="setStatus(d, 'OffDuty')">
                <mat-icon>event_busy</mat-icon> Set Off Duty
              </button>
            </mat-menu>
          </td>
        </ng-container>
        <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
        <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
      </table>

      <div class="empty-state" *ngIf="!isLoading && dataSource.length === 0">
        <mat-icon>badge</mat-icon>
        <h3>No drivers registered</h3>
        <p>Click "Register Driver" to add the first driver.</p>
      </div>

      <mat-paginator [length]="totalItems" [pageSize]="pageSize"
                     [pageSizeOptions]="[10, 20, 50]" (page)="onPageChange($event)">
      </mat-paginator>
    </div>
  `
})
export class DriversPageComponent implements OnInit {
  private driverService = inject(DriverService);
  private assignmentService = inject(AssignmentService);
  private vehicleService = inject(VehicleService);
  private notification = inject(NotificationService);
  private dialog = inject(MatDialog);

  displayedColumns = ['name', 'license', 'phone', 'vehicle', 'status', 'actions'];
  dataSource: Driver[] = [];
  totalItems = 0;
  pageSize = 10;
  pageIndex = 0;
  isLoading = false;

  ngOnInit() { this.loadDrivers(); }

  loadDrivers() {
    this.isLoading = true;
    this.driverService.getAll(this.pageIndex + 1, this.pageSize).subscribe({
      next: (res: any) => {
        if (res.success) {
          const data = res.data;
          this.dataSource = data?.items || (Array.isArray(data) ? data : []);
          this.totalItems = data?.totalCount || this.dataSource.length;
          this.loadVehiclesForDrivers();
        } else {
          this.isLoading = false;
        }
      },
      error: () => { this.isLoading = false; this.notification.error('Failed to load drivers'); }
    });
  }

  loadVehiclesForDrivers() {
    this.assignmentService.getAll(1, 100).subscribe({
      next: (assignRes) => {
        const assignments = assignRes?.data?.items || [];
        const activeAssignments = assignments.filter((a: any) =>
          a.status === 'Active' || a.status === 'InProgress' || a.status === 'Assigned'
        );

        const vehicleRequests = this.dataSource.map(driver => {
          const assignment = activeAssignments.find((a: any) => a.driverId === driver.id);
          if (assignment && assignment.vehicleId) {
            return this.vehicleService.getById(assignment.vehicleId).pipe(
              map(vehicleRes => {
                if (vehicleRes.success && vehicleRes.data) {
                  driver.currentVehiclePlate = vehicleRes.data.plateNumber;
                }
                return driver;
              })
            );
          }
          return of(driver);
        });

        if (vehicleRequests.length > 0) {
          forkJoin(vehicleRequests).subscribe({
            next: () => {
              this.dataSource = [...this.dataSource];
              this.isLoading = false;
            },
            error: () => { this.isLoading = false; }
          });
        } else {
          this.isLoading = false;
        }
      },
      error: () => { this.isLoading = false; }
    });
  }

  openAddDriver() {
    const ref = this.dialog.open(DriverFormDialogComponent, { width: '520px', disableClose: false });
    ref.afterClosed().subscribe(result => { if (result) this.loadDrivers(); });
  }

  setStatus(driver: Driver, status: string) {
    this.driverService.updateStatus(driver.id, status).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success(`Driver status set to ${status}`);
          this.loadDrivers();
        } else {
          this.notification.error(res.message || 'Status update failed');
        }
      },
      error: (err: any) => this.notification.error(err.error?.message || 'Status update failed')
    });
  }

  onPageChange(e: PageEvent) {
    this.pageIndex = e.pageIndex;
    this.pageSize = e.pageSize;
    this.loadDrivers();
  }
}
