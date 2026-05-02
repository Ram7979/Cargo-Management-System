import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { VehicleService, Vehicle } from '../../../core/services/vehicle.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-vehicle-detail',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, MatDividerModule, MatProgressBarModule],
  templateUrl: './vehicle-detail.component.html',
  styleUrls: ['./vehicle-detail.component.scss']
})
export class VehicleDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private vehicleService = inject(VehicleService);
  private notification = inject(NotificationService);

  vehicle: Vehicle | null = null;
  isLoading = true;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadVehicle(id);
    }
  }

  loadVehicle(id: string) {
    this.isLoading = true;
    this.vehicleService.getById(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.vehicle = res.data;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load vehicle details');
      }
    });
  }

  updateStatus(status: string) {
    if (!this.vehicle) return;
    this.vehicleService.updateStatus(this.vehicle.id, status).subscribe(res => {
      if (res.success) {
        this.notification.success('Vehicle status updated');
        this.loadVehicle(this.vehicle!.id);
      }
    });
  }
}
