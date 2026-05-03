import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DriverService, Driver } from '../../../core/services/driver.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';
import { AssignmentService } from '../../../core/services/assignment.service';
import { VehicleService } from '../../../core/services/vehicle.service';
import { forkJoin, of } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-driver-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatChipsModule, MatMenuModule, StatusChipComponent, MatDialogModule],
  templateUrl: './driver-list.component.html',
  styleUrls: ['./driver-list.component.scss']
})
export class DriverListComponent implements OnInit {
  private driverService = inject(DriverService);
  private assignmentService = inject(AssignmentService);
  private vehicleService = inject(VehicleService);
  private notification = inject(NotificationService);
  private dialog = inject(MatDialog);

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
          this.loadVehiclesForDrivers();
        } else {
          this.isLoading = false;
        }
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load drivers');
      }
    });
  }

  loadVehiclesForDrivers() {
    this.assignmentService.getAll(1, 100).subscribe({
      next: (assignRes) => {
        const assignments = assignRes?.data?.items || [];
        const activeAssignments = assignments.filter((a: any) => 
          a.status === 'Active' || a.status === 'InProgress' || a.status === 'Assigned'
        );

        const vehicleRequests = this.dataSource.map(driver => {
          const assignment = activeAssignments.find((a: any) => a.driverId === driver.id);
          if (assignment && assignment.vehicleId) {
            return this.vehicleService.getById(assignment.vehicleId).pipe(
              map(vehicleRes => {
                if (vehicleRes.success && vehicleRes.data) {
                  driver.currentVehiclePlate = vehicleRes.data.plateNumber;
                }
                return driver;
              })
            );
          }
          return of(driver);
        });

        if (vehicleRequests.length > 0) {
          forkJoin(vehicleRequests).subscribe({
            next: () => {
              // Trigger Material Table re-render by creating a new array reference
              this.dataSource = [...this.dataSource];
              this.isLoading = false;
            },
            error: () => {
              this.isLoading = false;
            }
          });
        } else {
          this.isLoading = false;
        }
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  openRegisterDriver() {
    // Import DriverFormDialogComponent from the drivers-page
    import('../../drivers/drivers-page.component').then(m => {
      const ref = this.dialog.open(m.DriverFormDialogComponent, { width: '520px' });
      ref.afterClosed().subscribe(result => { if (result) this.loadDrivers(); });
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
