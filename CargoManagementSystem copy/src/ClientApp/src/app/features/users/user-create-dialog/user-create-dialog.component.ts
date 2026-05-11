import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { UserService } from '../../../core/services/user.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-user-create-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
            MatInputModule, MatButtonModule, MatSelectModule, MatIconModule],
  template: `
    <h2 mat-dialog-title>Add New User</h2>
    <mat-dialog-content>
      <form [formGroup]="form" class="dialog-form">
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>First Name *</mat-label>
          <input matInput formControlName="firstName">
          <mat-error *ngIf="form.get('firstName')?.hasError('required')">Required</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Last Name *</mat-label>
          <input matInput formControlName="lastName">
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Email *</mat-label>
          <input matInput type="email" formControlName="email">
          <mat-error *ngIf="form.get('email')?.hasError('email')">Invalid email</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Password *</mat-label>
          <input matInput type="password" formControlName="password">
          <mat-error *ngIf="form.get('password')?.hasError('minlength')">Min 8 characters</mat-error>
        </mat-form-field>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Role *</mat-label>
          <mat-select formControlName="role">
            <mat-option *ngFor="let r of roles" [value]="r">{{ r }}</mat-option>
          </mat-select>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-flat-button color="primary" [disabled]="isSubmitting || form.invalid" (click)="onSubmit()">
        {{ isSubmitting ? 'Creating...' : 'Create User' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full-width { width: 100%; } .dialog-form { display: flex; flex-direction: column; gap: 4px; padding-top: 8px; }`]
})
export class UserCreateDialogComponent {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private notification = inject(NotificationService);
  private dialogRef = inject(MatDialogRef<UserCreateDialogComponent>);

  isSubmitting = false;

  roles = ['SuperAdmin', 'OpsManager', 'Dispatcher', 'FleetManager',
           'WarehouseManager', 'WarehouseOperator', 'FinanceOfficer', 'Support', 'Customer'];

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName:  ['', Validators.required],
    email:     ['', [Validators.required, Validators.email]],
    password:  ['', [Validators.required, Validators.minLength(8)]],
    role:      ['Support', Validators.required]
  });

  onCancel() { this.dialogRef.close(null); }

  onSubmit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSubmitting = true;
    // Backend RegisterUserRequest: { email, password, firstName, lastName, role }
    this.userService.create(this.form.value).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('User created successfully');
          this.dialogRef.close(true);
        } else {
          this.notification.error(res.errors?.join(', ') || res.message || 'Failed to create user');
        }
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.notification.error(err.error?.errors?.join(', ') || err.error?.message || 'Failed to create user');
        this.isSubmitting = false;
      }
    });
  }
}
