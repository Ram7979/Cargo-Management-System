import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { CustomerService } from '../../../core/services/customer.service';
import { NotificationService } from '../../../core/services/notification.service';
import { CustomerEditDialogComponent } from '../customer-edit-dialog/customer-edit-dialog.component';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatDialogModule],
  templateUrl: './customer-list.component.html'
})
export class CustomerListComponent implements OnInit {
  private customerService = inject(CustomerService);
  private notification = inject(NotificationService);
  private dialog = inject(MatDialog);

  displayedColumns: string[] = ['customerCode', 'companyName', 'email', 'phone', 'actions'];
  dataSource: any[] = [];
  totalItems = 0;
  pageSize = 10;
  pageIndex = 0;

  ngOnInit() { this.loadData(); }

  loadData() {
    this.customerService.getAll(this.pageIndex + 1, this.pageSize).subscribe(res => {
      if (res.success) {
        this.dataSource = res.data.items;
        this.totalItems = res.data.totalCount;
      }
    });
  }

  onAddCustomer() {
    const dialogRef = this.dialog.open(CustomerEditDialogComponent, {
      width: '500px',
      data: {}
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.customerService.create(result).subscribe(res => {
          if (res.success) {
            this.notification.success('Customer added successfully');
            this.loadData();
          }
        });
      }
    });
  }

  onPageChange(e: PageEvent) {
    this.pageIndex = e.pageIndex;
    this.pageSize = e.pageSize;
    this.loadData();
  }
}
