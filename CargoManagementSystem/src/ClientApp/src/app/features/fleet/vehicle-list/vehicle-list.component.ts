import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { VehicleService, Vehicle } from '../../../core/services/vehicle.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatIconModule, MatChipsModule, MatProgressBarModule, MatProgressSpinnerModule, StatusChipComponent],
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.scss']
})
export class VehicleListComponent implements OnInit {
  private vehicleService = inject(VehicleService);
  private notification = inject(NotificationService);

  vehicles: Vehicle[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadVehicles();
  }

  loadVehicles() {
    this.isLoading = true;
    this.vehicleService.getAll().subscribe({
      next: (res) => {
        if (res.success) {
          this.vehicles = res.data.items;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load fleet data');
      }
    });
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Available': return 'primary';
      case 'On Trip': return 'accent';
      case 'Maintenance': return 'warn';
      default: return 'primary';
    }
  }

  getVehicleIcon(type: string | undefined): string {
    switch ((type || '').toLowerCase()) {
      case 'truck': return 'local_shipping';
      case 'van': return 'airport_shuttle';
      case 'bike': return 'motorcycle';
      case 'motorcycle': return 'motorcycle';
      default: return 'directions_car';
    }
  }
}
