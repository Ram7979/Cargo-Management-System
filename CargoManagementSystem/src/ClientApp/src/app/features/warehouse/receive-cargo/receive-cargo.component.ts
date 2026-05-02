import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { ShipmentService } from '../../../core/services/shipment.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Shipment } from '../../../core/services/shipment.service';

@Component({
  selector: 'app-receive-cargo',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule, 
    MatSelectModule, 
    MatIconModule
  ],
  templateUrl: './receive-cargo.component.html',
  styleUrls: ['./receive-cargo.component.scss']
})
export class ReceiveCargoComponent implements OnInit {
  private fb = inject(FormBuilder);
  private warehouseService = inject(WarehouseService);
  private shipmentService = inject(ShipmentService);
  private notification = inject(NotificationService);

  receiveForm: FormGroup;
  foundShipment: Shipment | null = null;
  warehouses: any[] = [];
  bins: string[] = ['BIN-A1', 'BIN-A2', 'BIN-B1', 'BIN-B2', 'BIN-C1', 'COLD-STORAGE-01'];
  isSearching = false;
  isSubmitting = false;

  constructor() {
    this.receiveForm = this.fb.group({
      trackingNumber: ['', [Validators.required]],
      warehouseId: ['', [Validators.required]],
      remarks: [''],
      hasDamageReport: [false],
      damageNotes: ['']
    });
  }

  ngOnInit() {
    this.loadWarehouses();
  }

  loadWarehouses() {
    this.warehouseService.getAll().subscribe({
      next: (res: any) => {
        if (res.success) {
          const data = res.data;
          this.warehouses = Array.isArray(data) ? data : (data?.items || []);
        }
      },
      error: () => {
        // Warehouses may not be set up yet — that's OK
      }
    });
  }

  searchShipment() {
    const trackingNo = this.receiveForm.get('trackingNumber')?.value;
    if (!trackingNo) return;

    this.isSearching = true;
    this.shipmentService.track(trackingNo).subscribe({
      next: (res) => {
        if (res.success) {
          this.foundShipment = res.data;
          this.notification.success('Shipment found!');
        } else {
          this.foundShipment = null;
          this.notification.error('Shipment not found');
        }
        this.isSearching = false;
      },
      error: () => {
        this.isSearching = false;
        this.notification.error('Error searching shipment');
      }
    });
  }

  onSubmit() {
    if (this.receiveForm.invalid || !this.foundShipment) return;

    this.isSubmitting = true;
    const formVal = this.receiveForm.value;

    // Backend expects: trackingNumber, warehouseId, binId (optional), hasDamageReport, damageNotes, remarks
    const request = {
      trackingNumber: formVal.trackingNumber,
      warehouseId: formVal.warehouseId,
      hasDamageReport: formVal.hasDamageReport || false,
      damageNotes: formVal.damageNotes || null,
      remarks: formVal.remarks || null
    };

    this.warehouseService.receive(request as any).subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Cargo received and binned successfully');
          this.resetForm();
        } else {
          const errors = (res as any).errors?.join(', ') || (res as any).message || 'Failed to receive cargo';
          this.notification.error(errors);
        }
        this.isSubmitting = false;
      },
      error: (err) => {
        this.isSubmitting = false;
        const msg = err.error?.errors?.join(', ') || err.error?.message || err.message || 'Failed to receive cargo';
        this.notification.error(msg);
      }
    });
  }

  resetForm() {
    this.receiveForm.reset();
    this.foundShipment = null;
  }
}
