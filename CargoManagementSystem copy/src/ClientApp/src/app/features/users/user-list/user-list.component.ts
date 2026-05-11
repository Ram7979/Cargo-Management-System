import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/user.model';
import { NotificationService } from '../../../core/services/notification.service';
import { UserCreateDialogComponent } from '../user-create-dialog/user-create-dialog.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule, RouterModule, MatTableModule, MatPaginatorModule,
    MatButtonModule, MatIconModule, MatChipsModule, MatInputModule,
    MatFormFieldModule, FormsModule, MatDialogModule
  ],
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit {
  private userService = inject(UserService);
  private notification = inject(NotificationService);
  private dialog = inject(MatDialog);

  displayedColumns: string[] = ['name', 'email', 'roles', 'status', 'actions'];
  dataSource: User[] = [];
  totalItems = 0;
  pageSize = 10;
  pageIndex = 0;
  searchQuery = '';

  ngOnInit() { this.loadUsers(); }

  loadUsers() {
    this.userService.getAll(this.pageIndex + 1, this.pageSize).subscribe({
      next: (res: any) => {
        if (res.success) {
          const data = res.data;
          if (data?.items && Array.isArray(data.items)) {
            this.dataSource = data.items;
            this.totalItems = data.totalCount || data.items.length;
          } else if (Array.isArray(data)) {
            this.dataSource = data;
            this.totalItems = data.length;
          } else {
            this.dataSource = [];
          }
        }
      },
      error: () => this.notification.error('Failed to load users')
    });
  }

  onPageChange(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadUsers();
  }

  onSearch() {
    this.pageIndex = 0;
    console.log(`[User Management] Searching for: ${this.searchQuery}`);
    this.loadUsers();
  }

  openAddUser() {
    console.log('[User Management] Opening Add User dialog');
    const ref = this.dialog.open(UserCreateDialogComponent, { width: '480px' });
    ref.afterClosed().subscribe(result => {
      if (result) {
        this.notification.success('User list refreshed');
        this.loadUsers();
      }
    });
  }

  getUserId(user: User): string {
    const id = user.id || (user as any).userId;
    if (!id) console.warn('[User Management] Could not find ID for user:', user);
    return id || '';
  }

  toggleUserStatus(user: User, event?: Event) {
    if (event) {
      event.stopPropagation();
    }
    const id = this.getUserId(user);
    if (!id) { this.notification.error('Cannot identify user'); return; }
    
    const newStatus = !user.isActive;
    const action = newStatus ? 'activate' : 'deactivate';
    
    if (!confirm(`Are you sure you want to ${action} this user?`)) return;
    
    console.log(`[User Management] ${action}ing user ${id}`);
    
    // Using update (which is PATCH in UserService) to toggle status
    this.userService.update(id, { isActive: newStatus }).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success(`User ${action}d successfully`);
          this.loadUsers();
        } else {
          this.notification.error(res.message || `Failed to ${action} user`);
        }
      },
      error: (err: any) => {
        console.error(`[User Management] ${action} error:`, err);
        this.notification.error(
          err.error?.errors?.join(', ') || err.error?.message || `Failed to ${action} user`
        );
      }
    });
  }

  getRoleColor(role: string): string {
    switch (role) {
      case 'SuperAdmin': return 'warn';
      case 'OpsManager': return 'primary';
      case 'Dispatcher': return 'accent';
      default: return 'primary';
    }
  }
}
