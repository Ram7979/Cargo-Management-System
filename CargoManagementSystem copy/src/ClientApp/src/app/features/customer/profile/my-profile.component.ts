import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../../core/services/auth.service';
import { UserProfile } from '../../../core/models/user.model';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule],
  template: `
    <div class="profile-container" *ngIf="user">
      <div class="profile-header">
        <div class="avatar-large">
          {{ user.firstName[0] }}{{ user.lastName[0] }}
        </div>
        <div class="header-text">
          <h2>{{ user.firstName }} {{ user.lastName }}</h2>
          <p>{{ user.email }}</p>
        </div>
        <button mat-flat-button color="primary">Edit Profile</button>
      </div>

      <div class="profile-grid">
        <mat-card class="info-card">
          <mat-card-header>
            <mat-icon mat-card-avatar>person_outline</mat-icon>
            <mat-card-title>Account Information</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="info-row">
              <span class="label">First Name:</span>
              <span class="value">{{user.firstName}}</span>
            </div>
            <div class="info-row">
              <span class="label">Last Name:</span>
              <span class="value">{{user.lastName}}</span>
            </div>
            <div class="info-row">
              <span class="label">Email Address:</span>
              <span class="value">{{user.email}}</span>
            </div>
            <div class="info-row">
              <span class="label">User Role:</span>
              <span class="value">Customer</span>
            </div>
          </mat-card-content>
        </mat-card>

        <mat-card class="info-card">
          <mat-card-header>
            <mat-icon mat-card-avatar>security</mat-icon>
            <mat-card-title>Security</mat-card-title>
          </mat-card-header>
          <mat-card-content>
            <div class="info-row">
              <span class="label">Password:</span>
              <span class="value">••••••••••••</span>
              <button mat-button color="primary">Change</button>
            </div>
            <div class="info-row">
              <span class="label">Two-Factor Auth:</span>
              <span class="value">Disabled</span>
              <button mat-button color="primary">Enable</button>
            </div>
          </mat-card-content>
        </mat-card>
      </div>
    </div>
  `,
  styles: [`
    .profile-container { max-width: 1000px; margin: 0 auto; display: flex; flex-direction: column; gap: 40px; }
    .profile-header {
      display: flex; align-items: center; gap: 32px;
      padding: 40px; background: #161B22; border-radius: 24px; border: 1px solid rgba(255,255,255,0.1);
    }
    .avatar-large {
      width: 100px; height: 100px; border-radius: 50%;
      background: linear-gradient(135deg, #3B82F6, #8B5CF6);
      display: flex; align-items: center; justify-content: center;
      font-size: 2.5rem; font-weight: 700; color: white;
    }
    .header-text { flex: 1; h2 { margin-bottom: 4px; } p { color: #94A3B8; } }

    .profile-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
    .info-card { background: #161B22 !important; border: 1px solid rgba(255,255,255,0.1) !important; border-radius: 16px !important; color: #E2E8F0 !important; }
    .info-row { display: flex; align-items: center; padding: 16px 0; border-bottom: 1px solid rgba(255,255,255,0.05); }
    .info-row:last-child { border-bottom: none; }
    .label { width: 150px; color: #94A3B8; font-size: 0.9rem; }
    .value { flex: 1; font-weight: 500; }
    
    mat-card-title { font-size: 1.1rem !important; font-weight: 600 !important; color: #F8FAFC !important; }
    mat-icon[mat-card-avatar] { color: #3B82F6; }
  `]
})
export class MyProfileComponent implements OnInit {
  private authService = inject(AuthService);
  user: UserProfile | null = null;

  ngOnInit() {
    this.authService.currentUser$.subscribe(user => this.user = user);
  }
}