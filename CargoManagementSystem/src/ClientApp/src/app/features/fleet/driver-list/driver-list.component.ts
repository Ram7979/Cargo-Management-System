import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { DriverService, Driver } from '../../../core/services/driver.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-driver-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatChipsModule, MatMenuModule, StatusChipComponent],
  templateUrl: './driver-list.component.html',
  styleUrls: ['./driver-list.component.scss']
})
export class DriverListComponent implements OnInit {
  private driverService = inject(DriverService);
  private notification = inject(NotificationService);

  displayedColumns: string[] = ['name', 'license', 'phone', 'vehicle', 'status', 'actions'];
  dataSource: Driver[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadDrivers();
  }

  loadDrivers() {
    this.isLoading = true;
    this.driverService.getAll().subscribe({
      next: (res) => {
        if (res.success) {
          this.dataSource = res.data.items;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load drivers');
      }
    });
  }

  toggleStatus(driver: Driver) {
    const newStatus = driver.status === 'Active' ? 'On Leave' : 'Active';
    this.driverService.updateStatus(driver.id, newStatus).subscribe(res => {
      if (res.success) {
        this.notification.success(`Driver status updated to ${newStatus}`);
        this.loadDrivers();
      }
    });
  }
}
