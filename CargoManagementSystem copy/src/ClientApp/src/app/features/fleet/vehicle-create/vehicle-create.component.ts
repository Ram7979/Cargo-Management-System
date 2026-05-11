import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { VehicleService } from '../../../core/services/vehicle.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-vehicle-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, MatFormFieldModule, MatInputModule,
            MatButtonModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule,
            MatIconModule, MatCardModule],
  template: `
    <div class="page-header">
      <h2>Add New Vehicle</h2>
      <button mat-stroked-button routerLink="/fleet"><mat-icon>arrow_back</mat-icon> Back</button>
    </div>
    <mat-card class="form-card">
      <mat-card-content>
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="form-grid">
            <mat-form-field appearance="outline">
              <mat-label>Plate Number *</mat-label>
              <input matInput formControlName="plateNumber">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>GPS Device ID *</mat-label>
              <input matInput formControlName="gpsDeviceId">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Vehicle Type *</mat-label>
              <mat-select formControlName="type">
                <mat-option *ngFor="let t of vehicleTypes" [value]="t">{{ t }}</mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Make *</mat-label>
              <input matInput formControlName="make">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Model *</mat-label>
              <input matInput formControlName="model">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Year *</mat-label>
              <input matInput type="number" formControlName="year">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Fuel Type *</mat-label>
              <mat-select formControlName="fuelType">
                <mat-option *ngFor="let f of fuelTypes" [value]="f">{{ f }}</mat-option>
              </mat-select>
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Capacity (kg) *</mat-label>
              <input matInput type="number" formControlName="capacityKg">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Volume (CBM) *</mat-label>
              <input matInput type="number" formControlName="volumeCbm">
            </mat-form-field>

            <mat-form-field appearance="outline">
              <mat-label>Next Service Date</mat-label>
              <input matInput [matDatepicker]="picker" formControlName="nextServiceDate">
              <mat-datepicker-toggle matSuffix [for]="picker"></mat-datepicker-toggle>
              <mat-datepicker #picker></mat-datepicker>
            </mat-form-field>
          </div>

          <div class="form-actions">
            <button mat-raised-button color="primary" type="submit" [disabled]="isSubmitting || form.invalid">
              {{ isSubmitting ? 'Saving...' : 'Add Vehicle' }}
            </button>
            <button mat-stroked-button type="button" routerLink="/fleet">Cancel</button>
          </div>
        </form>
      </mat-card-content>
    </mat-card>
  `,
  styles: [`
    .form-card { max-width: 800px; margin: 2rem auto; }
    .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
    .form-actions { margin-top: 2rem; display: flex; gap: 1rem; justify-content: flex-end; }
    @media (max-width: 600px) { .form-grid { grid-template-columns: 1fr; } }
  `]
})
export class VehicleCreateComponent {
  private fb = inject(FormBuilder);
  private vehicleService = inject(VehicleService);
  private notification = inject(NotificationService);
  private router = inject(Router);

  vehicleTypes = ['Truck', 'Van', 'Motorcycle', 'Car'];
  fuelTypes = ['Diesel', 'Petrol', 'Electric', 'CNG'];
  isSubmitting = false;

  form = this.fb.group({
    plateNumber:    ['', Validators.required],
    gpsDeviceId:    ['', Validators.required],
    type:           ['Truck', Validators.required],
    make:           ['', Validators.required],
    model:          ['', Validators.required],
    year:           [new Date().getFullYear(), [Validators.required, Validators.min(2000)]],
    fuelType:       ['Diesel', Validators.required],
    capacityKg:     [0, [Validators.required, Validators.min(1)]],
    volumeCbm:      [0, [Validators.required, Validators.min(0.1)]],
    nextServiceDate:[null as Date | null]
  });

  onSubmit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSubmitting = true;
    const val = this.form.value;
    const payload = {
      ...val,
      year: Number(val.year),
      capacityKg: Number(val.capacityKg),
      volumeCbm: Number(val.volumeCbm),
      nextServiceDate: val.nextServiceDate ? new Date(val.nextServiceDate!).toISOString() : null
    };
    this.vehicleService.create(payload).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('Vehicle added successfully');
          this.router.navigate(['/fleet']);
        } else {
          this.notification.error(res.errors?.join(', ') || res.message || 'Failed to add vehicle');
        }
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.notification.error(err.error?.errors?.join(', ') || err.error?.message || 'Failed to add vehicle');
        this.isSubmitting = false;
      }
    });
  }
}
