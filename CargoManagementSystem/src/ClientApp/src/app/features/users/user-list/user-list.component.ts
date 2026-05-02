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
import { UserService } from '../../../core/services/user.service';
import { User } from '../../../core/models/user.model';
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule,
    MatTableModule, 
    MatPaginatorModule, 
    MatButtonModule, 
    MatIconModule, 
    MatChipsModule, 
    MatInputModule, 
    MatFormFieldModule, 
    FormsModule,
    MatDialogModule
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

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getAll(this.pageIndex + 1, this.pageSize).subscribe({
      next: (res: any) => {
        if (res.success) {
          // Handle both paginated and direct array responses
          const data = res.data;
          if (data?.items && Array.isArray(data.items)) {
            this.dataSource = data.items;
            this.totalItems = data.totalCount || data.items.length;
          } else if (Array.isArray(data)) {
            this.dataSource = data;
            this.totalItems = data.length;
          } else {
            this.dataSource = [];
            this.totalItems = 0;
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
    this.loadUsers();
  }

  toggleUserStatus(user: User) {
    const action = user.isActive ? 'deactivate' : 'activate';
    if (confirm(`Are you sure you want to ${action} this user?`)) {
      this.userService.delete(user.userId).subscribe(res => {
        if (res.success) {
          this.notification.success(`User ${action}d successfully`);
          this.loadUsers();
        }
      });
    }
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
