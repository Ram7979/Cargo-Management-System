import { Component, Inject, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-shipment-status-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule, MatInputModule, MatSelectModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Update Shipment Status</h2>
    <mat-dialog-content>
      <form [formGroup]="statusForm">
        <mat-form-field appearance="outline" class="w-100">
          <mat-label>New Status</mat-label>
          <mat-select formControlName="status">
            <mat-option *ngFor="let s of statuses" [value]="s">{{ s }}</mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="outline" class="w-100">
          <mat-label>Notes</mat-label>
          <textarea matInput formControlName="notes" rows="3"></textarea>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-flat-button color="primary" [disabled]="statusForm.invalid" (click)="onConfirm()">Update Status</button>
    </mat-dialog-actions>
  `,
  styles: [`.w-100 { width: 100%; }`]
})
export class ShipmentStatusDialogComponent {
  private fb = inject(FormBuilder);
  
  statuses = ['Pending', 'In Transit', 'Out for Delivery', 'Delivered', 'Cancelled', 'On Hold'];

  statusForm: FormGroup = this.fb.group({
    status: ['', Validators.required],
    notes: ['']
  });

  constructor(
    public dialogRef: MatDialogRef<ShipmentStatusDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { shipment: any }
  ) {
    if (data.shipment) {
      this.statusForm.patchValue({ status: data.shipment.status });
    }
  }

  onCancel() { this.dialogRef.close(); }

  onConfirm() {
    if (this.statusForm.valid) {
      this.dialogRef.close(this.statusForm.value);
    }
  }
}
