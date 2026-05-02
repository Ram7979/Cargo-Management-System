import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { WarehouseService, ReleaseCargoRequest } from '../../../core/services/warehouse.service';
import { ShipmentService, Shipment } from '../../../core/services/shipment.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-release-cargo',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatIconModule],
  templateUrl: './release-cargo.component.html',
  styleUrls: ['./release-cargo.component.scss']
})
export class ReleaseCargoComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private warehouseService = inject(WarehouseService);
  private shipmentService = inject(ShipmentService);
  private notification = inject(NotificationService);

  releaseForm: FormGroup;
  foundShipment: Shipment | null = null;
  isSearching = false;
  isSubmitting = false;

  constructor() {
    this.releaseForm = this.fb.group({
      trackingNumber: ['', Validators.required],
      releasedTo: ['', Validators.required],
      receiverId: ['', Validators.required],
      remarks: ['']
    });
  }

  ngOnInit() {
    const trackingNo = this.route.snapshot.queryParamMap.get('trackingNumber');
    if (trackingNo) {
      this.releaseForm.patchValue({ trackingNumber: trackingNo });
      this.searchShipment();
    }
  }

  searchShipment() {
    const trackingNo = this.releaseForm.get('trackingNumber')?.value;
    if (!trackingNo) return;

    this.isSearching = true;
    this.shipmentService.track(trackingNo).subscribe({
      next: (res) => {
        if (res.success) {
          this.foundShipment = res.data;
          this.notification.success('Cargo identified in inventory');
        } else {
          this.foundShipment = null;
          this.notification.error('Shipment not found in warehouse');
        }
        this.isSearching = false;
      },
      error: () => {
        this.isSearching = false;
        this.notification.error('Error fetching inventory details');
      }
    });
  }

  onSubmit() {
    if (this.releaseForm.invalid || !this.foundShipment) return;

    this.isSubmitting = true;
    const request: ReleaseCargoRequest = {
      shipmentId: this.foundShipment.id,
      releaseTo: this.releaseForm.get('releasedTo')?.value,
      releasedBy: this.releaseForm.get('receiverId')?.value
    };

    this.warehouseService.release(request).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Cargo released successfully');
          this.resetForm();
        }
        this.isSubmitting = false;
      },
      error: (err) => {
        this.isSubmitting = false;
        this.notification.error(err.message || 'Release failed');
      }
    });
  }

  resetForm() {
    this.releaseForm.reset();
    this.foundShipment = null;
  }
}
