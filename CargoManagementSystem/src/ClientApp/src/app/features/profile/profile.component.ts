import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
            MatInputModule, MatButtonModule, MatIconModule, MatChipsModule, MatDividerModule],
  templateUrl: './profile.component.html',
  styles: [`
    .profile-container { max-width: 800px; margin: 2rem auto; }
    .avatar-section { display: flex; align-items: center; gap: 2rem; margin-bottom: 2rem; }
    .avatar-xl { width: 100px; height: 100px; border-radius: 50%; background: #4299e1; color: white; display: flex; align-items: center; justify-content: center; font-size: 2.5rem; font-weight: bold; }
    .user-meta h3 { margin: 0; font-size: 1.8rem; }
    .user-meta .email { margin: 0; color: #64748b; margin-bottom: 0.5rem; }
    .roles { display: flex; gap: 0.5rem; }
    .edit-form { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-top: 1.5rem; }
    .full-width { grid-column: span 2; }
    @media (max-width: 600px) { .edit-form { grid-template-columns: 1fr; } .full-width { grid-column: span 1; } }
  `]
})
export class ProfileComponent implements OnInit {
  authService = inject(AuthService);
  private userService = inject(UserService);
  private notification = inject(NotificationService);
  private fb = inject(FormBuilder);

  isSaving = false;

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName:  ['', Validators.required]
  });

  ngOnInit() {
    const user = this.authService.currentUser();
    if (user) this.form.patchValue({ firstName: user.firstName, lastName: user.lastName });
  }

  get initials(): string {
    const u = this.authService.currentUser();
    return u ? `${u.firstName?.[0] || ''}${u.lastName?.[0] || ''}`.toUpperCase() : '?';
  }

  save() {
    if (this.form.invalid) return;
    const user = this.authService.currentUser();
    if (!user) return;
    this.isSaving = true;
    const id = user.id || (user as any).userId;
    this.userService.update(id, this.form.value).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('Profile updated successfully');
          const updated = { ...user, ...this.form.value };
          localStorage.setItem('user', JSON.stringify(updated));
          // In a real app, update the signal in AuthService
        } else {
          this.notification.error(res.message || 'Update failed');
        }
        this.isSaving = false;
      },
      error: (err: any) => {
        this.notification.error(err.error?.message || 'Update failed');
        this.isSaving = false;
      }
    });
  }
}
