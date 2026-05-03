import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UserService } from '../../../core/services/user.service';
import { NotificationService } from '../../../core/services/notification.service';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, MatCardModule,
            MatFormFieldModule, MatInputModule, MatButtonModule, MatChipsModule,
            MatSelectModule, MatIconModule, MatDividerModule, MatProgressSpinnerModule],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.scss'
})
export class UserDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private userService = inject(UserService);
  private notification = inject(NotificationService);
  private fb = inject(FormBuilder);

  user: User | null = null;
  isLoading = true;
  isSaving = false;
  isUpdatingRoles = false;

  allRoles = ['SuperAdmin', 'OpsManager', 'Dispatcher', 'FleetManager',
              'WarehouseManager', 'WarehouseOperator', 'FinanceOfficer', 'Support', 'Customer'];
  selectedRoles: string[] = [];

  profileForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName:  ['', Validators.required]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) this.loadUser(id);
  }

  getUserId(): string {
    return this.user?.id || (this.user as any)?.userId || '';
  }

  loadUser(id: string) {
    this.isLoading = true;
    this.userService.getById(id).subscribe({
      next: (res: any) => {
        if (res.success && res.data) {
          this.user = res.data;
          this.profileForm.patchValue({ firstName: res.data.firstName, lastName: res.data.lastName });
          this.selectedRoles = [...(res.data.roles || [])];
        } else {
          this.notification.error('User not found');
        }
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; this.notification.error('Failed to load user'); }
    });
  }

  saveProfile() {
    if (!this.user || this.profileForm.invalid) return;
    this.isSaving = true;
    this.userService.update(this.getUserId(), this.profileForm.value).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('Profile updated successfully');
          this.loadUser(this.getUserId());
        } else {
          this.notification.error(res.errors?.join(', ') || res.message || 'Update failed');
        }
        this.isSaving = false;
      },
      error: (err: any) => {
        this.notification.error(err.error?.errors?.join(', ') || err.error?.message || 'Update failed');
        this.isSaving = false;
      }
    });
  }

  saveRoles() {
    if (!this.user) return;
    this.isUpdatingRoles = true;
    this.userService.updateRoles(this.getUserId(), this.selectedRoles).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('Roles updated');
          this.loadUser(this.getUserId());
        } else {
          this.notification.error(res.message || 'Role update failed');
        }
        this.isUpdatingRoles = false;
      },
      error: (err: any) => {
        this.notification.error(err.error?.message || 'Role update failed');
        this.isUpdatingRoles = false;
      }
    });
  }

  toggleRole(role: string) {
    const idx = this.selectedRoles.indexOf(role);
    if (idx >= 0) this.selectedRoles.splice(idx, 1);
    else this.selectedRoles.push(role);
  }

  isRoleSelected(role: string): boolean { return this.selectedRoles.includes(role); }
}
