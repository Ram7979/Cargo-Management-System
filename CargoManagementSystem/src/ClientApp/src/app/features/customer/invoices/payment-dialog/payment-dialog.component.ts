import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { InvoiceService } from '../../../../core/services/invoice.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RevenueEventService } from '../../../../core/services/revenue-event.service';


@Component({
  selector: 'app-payment-dialog',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    MatDialogModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule, 
    MatIconModule,
    MatProgressSpinnerModule
  ],
  template: `
    <h2 mat-dialog-title>Secure Payment</h2>
    <mat-dialog-content>
      <div class="payment-header">
        <p class="invoice-number">Invoice #{{data.invoiceNumber}}</p>
        <p class="amount">Total: <strong>{{data.amount | currency}}</strong></p>
      </div>

      <form [formGroup]="paymentForm" class="payment-form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Cardholder Name</mat-label>
          <input matInput formControlName="cardholderName" placeholder="e.g. John Doe">
          <mat-icon matSuffix>person</mat-icon>
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Card Number</mat-label>
          <input matInput formControlName="cardNumber" placeholder="0000 0000 0000 0000" maxlength="19">
          <mat-icon matSuffix>credit_card</mat-icon>
        </mat-form-field>

        <div class="row">
          <mat-form-field appearance="outline">
            <mat-label>Expiry Date</mat-label>
            <input matInput formControlName="expiryDate" placeholder="MM/YY" maxlength="5">
          </mat-form-field>

          <mat-form-field appearance="outline">
            <mat-label>CVV</mat-label>
            <input matInput type="password" formControlName="cvv" placeholder="123" maxlength="4">
          </mat-form-field>
        </div>
      </form>
      
      <div *ngIf="isProcessing" class="processing-overlay">
        <mat-progress-spinner mode="indeterminate" diameter="40"></mat-progress-spinner>
        <p>Processing Payment...</p>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close [disabled]="isProcessing">Cancel</button>
      <button mat-raised-button color="primary" (click)="onPay()" [disabled]="paymentForm.invalid || isProcessing">
        Confirm Payment
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    h2 { margin: 0; color: #F8FAFC; }
    .payment-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; padding: 16px; background: rgba(59, 130, 246, 0.1); border-radius: 8px; border: 1px solid rgba(59, 130, 246, 0.2); }
    .invoice-number { color: #94A3B8; margin: 0; }
    .amount { color: #F8FAFC; margin: 0; font-size: 1.1rem; strong { color: #3B82F6; } }
    
    .payment-form { display: flex; flex-direction: column; position: relative; }
    .full-width { width: 100%; }
    .row { display: flex; gap: 16px; }
    .row mat-form-field { flex: 1; }
    
    .processing-overlay {
      position: absolute; top: 0; left: 0; right: 0; bottom: 0;
      background: rgba(15, 23, 42, 0.8); backdrop-filter: blur(4px);
      display: flex; flex-direction: column; align-items: center; justify-content: center;
      border-radius: 4px; z-index: 10;
      p { margin-top: 16px; color: #3B82F6; font-weight: 500; }
    }
    
    ::ng-deep .payment-form input.mat-mdc-input-element {
      color: #000000 !important;
    }
    
    ::ng-deep .payment-form input.mat-mdc-input-element::placeholder {
      color: #64748B !important;
    }
  `]
})
export class PaymentDialogComponent {
  paymentForm: FormGroup;
  isProcessing = false;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<PaymentDialogComponent>,
    private invoiceService: InvoiceService,
    private snackBar: MatSnackBar,
    private revenueEvents: RevenueEventService,
    @Inject(MAT_DIALOG_DATA) public data: { invoiceId: string, invoiceNumber: string, amount: number }
  ) {
    this.paymentForm = this.fb.group({
      cardholderName: ['', Validators.required],
      cardNumber: ['', Validators.required],
      expiryDate: ['', Validators.required],
      cvv: ['', Validators.required]
    });
  }

  onPay() {
    if (this.paymentForm.valid) {
      this.isProcessing = true;
      this.invoiceService.payInvoice(this.data.invoiceId, this.data.amount, 'Card').subscribe({
        next: (res) => {
          this.isProcessing = false;
          // Notify dashboard to refresh revenue immediately
          this.revenueEvents.notifyPaymentSuccess();
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.isProcessing = false;
          const msg = err?.error?.message || err?.error?.errors?.join(', ') || err?.message || 'Payment processing failed. Please try again.';
          this.snackBar.open(msg, 'Close', { duration: 7000 });
        }
      });
    }
  }
}
