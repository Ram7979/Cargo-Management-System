import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';
import { UserProfile } from '../../../core/models/user.model';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    MatIconModule, 
    MatButtonModule, 
    MatInputModule,
    MatCardModule,
    MatDividerModule,
    MatSnackBarModule
  ],
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.scss']
})
export class MyProfileComponent implements OnInit {
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  user: UserProfile | null = null;
  profileForm!: FormGroup;
  passwordForm!: FormGroup;
  isSaving = false;

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => {
      this.user = user;
      this.initForms();
    });
  }

  initForms() {
    this.profileForm = this.fb.group({
      firstName: [this.user?.firstName || '', Validators.required],
      lastName: [this.user?.lastName || '', Validators.required],
      email: [this.user?.email || '', [Validators.required, Validators.email]],
      phone: [this.user?.phone || ''],
      company: [this.user?.company || ''],
      address: [this.user?.address || '']
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { 'mismatch': true };
  }

  updateProfile() {
    if (this.profileForm.invalid) return;

    this.isSaving = true;
    // Simulate API call
    setTimeout(() => {
      this.isSaving = false;
      this.snackBar.open('Profile updated successfully!', 'Close', { duration: 3000 });
    }, 1500);
  }

  updatePassword() {
    if (this.passwordForm.invalid) return;

    this.isSaving = true;
    // Simulate API call
    setTimeout(() => {
      this.isSaving = false;
      this.passwordForm.reset();
      this.snackBar.open('Password changed successfully!', 'Close', { duration: 3000 });
    }, 1500);
  }
}