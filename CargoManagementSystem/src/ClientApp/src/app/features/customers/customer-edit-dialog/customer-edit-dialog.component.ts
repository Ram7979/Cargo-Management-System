import { Component, Inject, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { Customer } from '../../../core/services/customer.service';

@Component({
  selector: 'app-customer-edit-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule],
  template: `
    <h2 mat-dialog-title>{{ data.customer ? 'Edit Customer' : 'Add New Customer' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="customerForm" class="customer-form">
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Full Name *</mat-label>
          <input matInput formControlName="fullName" placeholder="John Doe">
          <mat-error *ngIf="customerForm.get('fullName')?.hasError('required')">Full name is required</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Company Name</mat-label>
          <input matInput formControlName="companyName">
        </mat-form-field>
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Contact Person</mat-label>
          <input matInput formControlName="contactPerson">
        </mat-form-field>
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Email *</mat-label>
          <input matInput formControlName="email" type="email">
          <mat-error *ngIf="customerForm.get('email')?.hasError('required')">Email is required</mat-error>
          <mat-error *ngIf="customerForm.get('email')?.hasError('email')">Invalid email</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Phone *</mat-label>
          <input matInput formControlName="phone">
          <mat-error *ngIf="customerForm.get('phone')?.hasError('required')">Phone is required</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Address *</mat-label>
          <input matInput formControlName="address">
          <mat-error *ngIf="customerForm.get('address')?.hasError('required')">Address is required</mat-error>
        </mat-form-field>
        <div class="form-row">
          <mat-form-field appearance="outline">
            <mat-label>City *</mat-label>
            <input matInput formControlName="city">
            <mat-error *ngIf="customerForm.get('city')?.hasError('required')">Required</mat-error>
          </mat-form-field>
          <mat-form-field appearance="outline">
            <mat-label>Country *</mat-label>
            <input matInput formControlName="country">
            <mat-error *ngIf="customerForm.get('country')?.hasError('required')">Required</mat-error>
          </mat-form-field>
        </div>
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Customer Type *</mat-label>
          <mat-select formControlName="type">
            <mat-option value="Individual">Individual</mat-option>
            <mat-option value="Corporate">Corporate</mat-option>
          </mat-select>
          <mat-error *ngIf="customerForm.get('type')?.hasError('required')">Type is required</mat-error>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-flat-button color="primary" [disabled]="customerForm.invalid" (click)="onConfirm()">Save</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .customer-form { display: flex; flex-direction: column; gap: 8px; padding-top: 10px; min-width: 400px; }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .w-100 { width: 100%; }
  `]
})
export class CustomerEditDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  customerForm: FormGroup;

  constructor(
    public dialogRef: MatDialogRef<CustomerEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { customer?: Customer }
  ) {
    this.customerForm = this.fb.group({
      fullName: [data.customer ? `${data.customer.firstName || ''} ${data.customer.lastName || ''}`.trim() : '', Validators.required],
      companyName: [data.customer?.companyName || ''],
      contactPerson: [data.customer?.contactPerson || ''],
      email: [data.customer?.email || '', [Validators.required, Validators.email]],
      phone: [data.customer?.phone || '', Validators.required],
      address: [data.customer?.address || '', Validators.required],
      city: [data.customer?.city || '', Validators.required],
      country: [data.customer?.country || '', Validators.required],
      type: [data.customer?.customerType || 'Individual', Validators.required]
    });
  }

  ngOnInit() {}

  onCancel() {
    this.dialogRef.close();
  }

  onConfirm() {
    if (this.customerForm.valid) {
      this.dialogRef.close(this.customerForm.value);
    }
  }
}
