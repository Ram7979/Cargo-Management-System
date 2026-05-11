import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { AssignmentService, CreateAssignmentRequest } from '../../../core/services/assignment.service';
import { VehicleService, Vehicle } from '../../../core/services/vehicle.service';
import { DriverService, Driver } from '../../../core/services/driver.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-shipment-assignment',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatTableModule, MatButtonModule, MatIconModule, MatSelectModule, MatFormFieldModule, FormsModule],
  templateUrl: './shipment-assignment.component.html',
  styleUrls: ['./shipment-assignment.component.scss']
})
export class ShipmentAssignmentComponent implements OnInit {
  private assignmentService = inject(AssignmentService);
  private vehicleService = inject(VehicleService);
  private driverService = inject(DriverService);
  private notification = inject(NotificationService);

  pendingShipments: any[] = [];
  availableVehicles: Vehicle[] = [];
  availableDrivers: Driver[] = [];
  
  selectedShipment: any = null;
  selectedVehicleId = '';
  selectedDriverId = '';
  isSubmitting = false;

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.assignmentService.getPendingShipments().subscribe(res => {
      if (res.success) this.pendingShipments = res.data;
    });

    this.vehicleService.getAll().subscribe(res => {
      if (res.success) this.availableVehicles = res.data.items.filter((v: any) => v.status === 'Available');
    });

    this.driverService.getAll().subscribe(res => {
      if (res.success) this.availableDrivers = res.data.items.filter((d: any) => d.status === 'Available');
    });
  }

  selectShipment(shipment: any) {
    this.selectedShipment = shipment;
  }

  onDispatch() {
    if (!this.selectedShipment || !this.selectedVehicleId || !this.selectedDriverId) return;

    this.isSubmitting = true;
    const request: CreateAssignmentRequest = {
      shipmentId: this.selectedShipment.id,
      vehicleId: this.selectedVehicleId,
      driverId: this.selectedDriverId
    };

    this.assignmentService.assign(request).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('Shipment successfully dispatched');
          this.resetSelection();
          this.loadData();
        }
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.notification.error(err.message || 'Dispatch failed');
        this.isSubmitting = false;
      }
    });
  }

  resetSelection() {
    this.selectedShipment = null;
    this.selectedVehicleId = '';
    this.selectedDriverId = '';
  }
}
