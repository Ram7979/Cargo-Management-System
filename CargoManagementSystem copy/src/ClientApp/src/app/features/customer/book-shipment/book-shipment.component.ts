import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { ShipmentService } from '../../../core/services/shipment.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-book-shipment',
  standalone: true,
  imports: [
    CommonModule, ReactiveFormsModule, MatInputModule, MatButtonModule, 
    MatSelectModule, MatStepperModule, MatIconModule, MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  template: `
    <div class="booking-container">
      <h2>Book New Shipment</h2>
      
      <mat-stepper orientation="horizontal" #stepper class="dark-stepper">
        <!-- Step 1: Sender & Recipient -->
        <mat-step [stepControl]="routeForm">
          <form [formGroup]="routeForm">
            <ng-template matStepLabel>Route</ng-template>
            <div class="step-content">
              <h3>Sender Details</h3>
              <div class="form-grid">
                <mat-form-field appearance="outline">
                  <mat-label>Sender Name</mat-label>
                  <input matInput formControlName="senderName">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Sender Contact</mat-label>
                  <input matInput formControlName="senderContact">
                </mat-form-field>
                <mat-form-field appearance="outline" class="span-2">
                  <mat-label>Origin Address</mat-label>
                  <input matInput formControlName="originAddress">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>City</mat-label>
                  <input matInput formControlName="senderCity">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Zip Code</mat-label>
                  <input matInput formControlName="senderZip">
                </mat-form-field>
                <mat-form-field appearance="outline" class="span-2">
                  <mat-label>Country</mat-label>
                  <input matInput formControlName="senderCountry">
                </mat-form-field>
              </div>

              <h3>Recipient Details</h3>
              <div class="form-grid">
                <mat-form-field appearance="outline">
                  <mat-label>Recipient Name</mat-label>
                  <input matInput formControlName="recipientName">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Recipient Contact</mat-label>
                  <input matInput formControlName="recipientContact">
                </mat-form-field>
                <mat-form-field appearance="outline" class="span-2">
                  <mat-label>Destination Address</mat-label>
                  <input matInput formControlName="destinationAddress">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>City</mat-label>
                  <input matInput formControlName="recipientCity">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Zip Code</mat-label>
                  <input matInput formControlName="recipientZip">
                </mat-form-field>
                <mat-form-field appearance="outline" class="span-2">
                  <mat-label>Country</mat-label>
                  <input matInput formControlName="recipientCountry">
                </mat-form-field>
              </div>
            </div>
            <div class="actions">
              <button mat-flat-button color="primary" matStepperNext>Next</button>
            </div>
          </form>
        </mat-step>

        <!-- Step 2: Cargo Details -->
        <mat-step [stepControl]="cargoForm">
          <form [formGroup]="cargoForm">
            <ng-template matStepLabel>Cargo</ng-template>
            <div class="step-content">
              <h3>Cargo Information</h3>
              <div class="form-grid">
                <mat-form-field appearance="outline">
                  <mat-label>Cargo Type</mat-label>
                  <mat-select formControlName="cargoType">
                    <mat-option value="Standard">Standard</mat-option>
                    <mat-option value="Fragile">Fragile</mat-option>
                    <mat-option value="Perishable">Perishable</mat-option>
                    <mat-option value="Hazardous">Hazardous</mat-option>
                  </mat-select>
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Service Type</mat-label>
                  <mat-select formControlName="serviceType">
                    <mat-option value="Standard">Standard</mat-option>
                    <mat-option value="Express">Express</mat-option>
                    <mat-option value="Same-Day">Same-Day</mat-option>
                    <mat-option value="International">International</mat-option>
                  </mat-select>
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Weight (kg)</mat-label>
                  <input matInput type="number" formControlName="weightKg">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Volume (CBM)</mat-label>
                  <input matInput type="number" formControlName="volumeCbm">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Quantity</mat-label>
                  <input matInput type="number" formControlName="quantity">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Declared Value ($)</mat-label>
                  <input matInput type="number" formControlName="declaredValue">
                </mat-form-field>
                <mat-form-field appearance="outline">
                  <mat-label>Payment Mode</mat-label>
                  <mat-select formControlName="paymentMode">
                    <mat-option value="Prepaid">Prepaid</mat-option>
                    <mat-option value="Collect">Collect</mat-option>
                    <mat-option value="ThirdParty">Third Party</mat-option>
                  </mat-select>
                </mat-form-field>
                <mat-form-field appearance="outline" class="span-2">
                  <mat-label>Description</mat-label>
                  <textarea matInput formControlName="cargoDescription" rows="3"></textarea>
                </mat-form-field>
              </div>
            </div>
            <div class="actions">
              <button mat-button matStepperPrevious>Back</button>
              <button mat-flat-button color="primary" matStepperNext>Next</button>
            </div>
          </form>
        </mat-step>

        <!-- Step 3: Review & Confirm -->
        <mat-step>
          <ng-template matStepLabel>Confirm</ng-template>
          <div class="summary-card">
            <h3>Shipment Summary</h3>
            <div class="summary-row">
              <span>Route:</span>
              <span>{{routeForm.value.senderCity}} → {{routeForm.value.recipientCity}}, {{routeForm.value.recipientCountry}}</span>
            </div>
            <div class="summary-row">
              <span>Service:</span>
              <span>{{cargoForm.value.serviceType}}</span>
            </div>
            <div class="summary-row">
              <span>Cargo:</span>
              <span>{{cargoForm.value.cargoType}} — {{cargoForm.value.cargoDescription}}</span>
            </div>
            <div class="summary-row">
              <span>Weight:</span>
              <span>{{cargoForm.value.weightKg}} kg</span>
            </div>
            <div class="summary-row">
              <span>Quantity:</span>
              <span>{{cargoForm.value.quantity}} pcs</span>
            </div>
            <div class="summary-row">
              <span>Payment:</span>
              <span>{{cargoForm.value.paymentMode}}</span>
            </div>
            <div class="price-estimate" *ngIf="cargoForm.value.declaredValue">
              <span class="label">Declared Value:</span>
              <span class="price">\${{cargoForm.value.declaredValue | number:'1.2-2'}}</span>
            </div>
          </div>
          <div class="actions">
            <button mat-button matStepperPrevious [disabled]="isSubmitting">Back</button>
            <button mat-flat-button color="accent" (click)="confirmBooking()" [disabled]="isSubmitting">
              <mat-progress-spinner *ngIf="isSubmitting" diameter="20" mode="indeterminate"></mat-progress-spinner>
              <span *ngIf="!isSubmitting">Confirm & Book</span>
            </button>
          </div>
        </mat-step>
      </mat-stepper>
    </div>
  `,
  styles: [`
    .booking-container { max-width: 900px; margin: 0 auto; }
    .dark-stepper { background: #161B22; border-radius: 16px; border: 1px solid rgba(255,255,255,0.1); padding: 24px; }
    .step-content { margin: 24px 0; }
    h3 { color: #3B82F6; margin-bottom: 20px; font-size: 1.1rem; }
    .form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .span-2 { grid-column: span 2; }
    mat-form-field { width: 100%; }
    .actions { display: flex; justify-content: flex-end; gap: 16px; margin-top: 24px; }
    
    .summary-card { background: rgba(255,255,255,0.03); padding: 32px; border-radius: 12px; margin: 32px 0; }
    .summary-row { display: flex; justify-content: space-between; margin-bottom: 16px; color: #94A3B8; }
    .price-estimate { display: flex; justify-content: space-between; align-items: center; margin-top: 24px; padding-top: 24px; border-top: 1px solid rgba(255,255,255,0.1); }
    .price { font-size: 2rem; font-weight: 700; color: #22C55E; }
    
    ::ng-deep .mat-step-header { color: #E2E8F0 !important; }
    ::ng-deep .mat-step-label { color: #94A3B8 !important; }
    ::ng-deep .mat-step-label-selected { color: #3B82F6 !important; font-weight: 700 !important; }
    
    /* Fix Input Text Colors for Dark Theme */
    .booking-container ::ng-deep input.mat-mdc-input-element, 
    .booking-container ::ng-deep textarea.mat-mdc-input-element,
    .booking-container ::ng-deep .mat-mdc-select-value-text { 
      color: #ffffff !important; 
    }
    
    .booking-container ::ng-deep input.mat-mdc-input-element::placeholder, 
    .booking-container ::ng-deep textarea.mat-mdc-input-element::placeholder { 
      color: #A0AEC0 !important; 
    }

    .booking-container ::ng-deep .mdc-text-field--outlined .mdc-notched-outline {
      border-color: rgba(255, 255, 255, 0.2) !important;
    }
    .booking-container ::ng-deep .mdc-text-field--outlined.mdc-text-field--focused .mdc-notched-outline {
      border-color: #3B82F6 !important;
    }
    
    .booking-container ::ng-deep .mat-mdc-form-field-focus-overlay {
      background-color: rgba(59, 130, 246, 0.1) !important;
    }
  `]
})
export class BookShipmentComponent {
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private shipmentService = inject(ShipmentService);
  private notificationService = inject(NotificationService);

  isSubmitting = false;

  routeForm = this.fb.group({
    senderName: ['', Validators.required],
    senderContact: ['', Validators.required],
    originAddress: ['', Validators.required],
    senderCity: ['', Validators.required],
    senderCountry: ['India', Validators.required],
    senderZip: ['400001', Validators.required],
    recipientName: ['', Validators.required],
    recipientContact: ['', Validators.required],
    destinationAddress: ['', Validators.required],
    recipientCity: ['', Validators.required],
    recipientCountry: ['', Validators.required],
    recipientZip: ['', Validators.required]
  });

  cargoForm = this.fb.group({
    weightKg: [1, [Validators.required, Validators.min(0.1)]],
    volumeCbm: [0.1, [Validators.required, Validators.min(0.01)]],
    quantity: [1, [Validators.required, Validators.min(1)]],
    cargoType: ['Standard', Validators.required],
    declaredValue: [100, [Validators.required, Validators.min(0)]],
    cargoDescription: ['', Validators.required],
    serviceType: ['Standard', Validators.required],
    paymentMode: ['Prepaid', Validators.required]
  });

  confirmBooking() {
    if (this.routeForm.invalid || this.cargoForm.invalid) {
      this.routeForm.markAllAsTouched();
      this.cargoForm.markAllAsTouched();
      this.snackBar.open('Please fill all required fields.', 'Close', { duration: 3000 });
      return;
    }

    this.isSubmitting = true;
    const request = {
      ...this.routeForm.value,
      ...this.cargoForm.value,
      weightKg: Number(this.cargoForm.value.weightKg),
      volumeCbm: Number(this.cargoForm.value.volumeCbm),
      quantity: Number(this.cargoForm.value.quantity),
      declaredValue: Number(this.cargoForm.value.declaredValue)
    };

    this.shipmentService.createShipment(request).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.snackBar.open('Shipment booked successfully!', 'Close', { duration: 5000 });
          this.notificationService.loadNotifications();
          this.router.navigate(['/user/shipments']);
        } else {
          const errors = res.errors?.join(', ') || res.message || 'Failed to book shipment';
          this.snackBar.open(errors, 'Close', { duration: 5000 });
          this.isSubmitting = false;
        }
      },
      error: (err: any) => {
        const errBody = err.error;
        const msg = errBody?.errors?.join(', ') || errBody?.message || err.message || 'Failed to book shipment. Please try again.';
        this.snackBar.open(msg, 'Close', { duration: 5000 });
        this.isSubmitting = false;
      }
    });
  }
}