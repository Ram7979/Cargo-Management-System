import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { ShipmentService } from '../../../core/services/shipment.service';
import { NotificationService } from '../../../core/services/notification.service';
import { Shipment } from '../../../core/services/shipment.service';

export interface WarehouseItem {
  id: string;
  name: string;
  city: string;
  country: string;
  address: string;
  availableBins: number;
  totalBins: number;
  capacityKg: number;
}

export interface BinItem {
  id: string;
  binCode: string;
  zone: string;
  level: string;
  isOccupied: boolean;
  isActive: boolean;
  capacityKg: number;
}

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
    MatIconModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatDividerModule,
    MatTooltipModule
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
  warehouses: WarehouseItem[] = [];
  bins: BinItem[] = [];
  isSearching = false;
  isSubmitting = false;
  isLoadingWarehouses = true;
  isLoadingBins = false;
  receiptResult: any = null; // Holds the generated receipt after success

  constructor() {
    this.receiveForm = this.fb.group({
      trackingNumber: ['', [Validators.required]],
      warehouseId: ['', [Validators.required]],
      binId: [''],
      remarks: [''],
      hasDamageReport: [false],
      damageNotes: ['']
    });

    // When warehouse changes, load its bins
    this.receiveForm.get('warehouseId')?.valueChanges.subscribe((warehouseId: string) => {
      if (warehouseId) {
        this.loadBins(warehouseId);
      } else {
        this.bins = [];
        this.receiveForm.patchValue({ binId: '' });
      }
    });
  }

  ngOnInit() {
    this.loadWarehouses();
  }

  loadWarehouses() {
    this.isLoadingWarehouses = true;
    this.warehouseService.getAll().subscribe({
      next: (res: any) => {
        let items: any[] = [];

        if (res.success !== false) {
          const data = res?.data;
          if (Array.isArray(data)) {
            items = data;
          } else if (data?.items && Array.isArray(data.items)) {
            items = data.items;
          } else if (Array.isArray(res)) {
            items = res;
          }
        }

        this.warehouses = items.map((w: any) => ({
          id: w.id,
          name: w.name || 'Unnamed Warehouse',
          city: w.city || '',
          country: w.country || '',
          address: w.address || '',
          availableBins: w.availableBins ?? 0,
          totalBins: w.totalBins ?? 0,
          capacityKg: w.capacityKg ?? 0
        }));

        this.isLoadingWarehouses = false;

        if (this.warehouses.length === 0) {
          this.notification.warn('No warehouses configured. Please create a warehouse first.');
        }
      },
      error: (err: any) => {
        console.error('[Warehouse] Failed to load warehouses:', err);
        this.isLoadingWarehouses = false;
        this.notification.error('Failed to load warehouses. Please check the backend service.');
      }
    });
  }

  loadBins(warehouseId: string) {
    this.isLoadingBins = true;
    this.bins = [];
    this.receiveForm.patchValue({ binId: '' });

    this.warehouseService.getBins(warehouseId).subscribe({
      next: (res: any) => {
        let items: any[] = [];
        if (res?.success !== false) {
          const data = res?.data;
          if (Array.isArray(data)) {
            items = data;
          } else if (data?.items && Array.isArray(data.items)) {
            items = data.items;
          }
        }

        // Only show available (not occupied, active) bins
        this.bins = items
          .filter((b: any) => !b.isOccupied && b.isActive)
          .map((b: any) => ({
            id: b.id,
            binCode: b.binCode || b.code || '',
            zone: b.zone || '',
            level: b.level || '',
            isOccupied: b.isOccupied || false,
            isActive: b.isActive !== false,
            capacityKg: b.capacityKg ?? 0
          }));

        this.isLoadingBins = false;

        if (this.bins.length === 0) {
          this.notification.info('No available bins in this warehouse. A bin will be auto-assigned.');
        }
      },
      error: () => {
        this.isLoadingBins = false;
        this.notification.warn('Could not load bins. A bin will be auto-assigned on receipt.');
      }
    });
  }

  searchShipment() {
    const trackingNo = this.receiveForm.get('trackingNumber')?.value?.trim();
    if (!trackingNo) {
      this.notification.warn('Please enter a tracking number.');
      return;
    }

    this.isSearching = true;
    this.foundShipment = null;
    this.receiptResult = null;

    this.shipmentService.track(trackingNo).subscribe({
      next: (res: any) => {
        if (res.success && res.data) {
          // Track endpoint returns { data: { shipment: {...}, timeline: [...] } }
          const raw = res.data;
          const shipment = raw.shipment || raw;
          this.foundShipment = shipment;
          this.notification.success(`Shipment ${shipment.trackingNumber || trackingNo} found!`);
        } else {
          this.foundShipment = null;
          this.notification.error(res.message || 'Shipment not found');
        }
        this.isSearching = false;
      },
      error: (err: any) => {
        this.isSearching = false;
        const msg = err.error?.message || err.message || 'Error searching shipment';
        this.notification.error(msg);
      }
    });
  }

  onSubmit() {
    if (this.receiveForm.invalid || !this.foundShipment) {
      this.notification.warn('Please complete all required fields and search for a shipment first.');
      return;
    }

    this.isSubmitting = true;
    const formVal = this.receiveForm.value;

    const request: any = {
      trackingNumber: formVal.trackingNumber?.trim(),
      warehouseId: formVal.warehouseId,
      hasDamageReport: formVal.hasDamageReport || false,
      damageNotes: formVal.hasDamageReport ? (formVal.damageNotes || null) : null,
      remarks: formVal.remarks || null
    };

    // Only send binId if one is selected (backend auto-assigns if null)
    if (formVal.binId) {
      request.binId = formVal.binId;
    }

    this.warehouseService.receive(request).subscribe({
      next: (res: any) => {
        if (res.success !== false && res.data) {
          this.receiptResult = res.data;
          this.notification.success('✅ Cargo received and binned successfully!');
        } else {
          const errors = res.errors?.join(', ') || res.message || 'Failed to receive cargo';
          this.notification.error(errors);
        }
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.isSubmitting = false;
        console.error('[ReceiveCargo] Error:', err);
        const body = err.error;
        let msg = 'Failed to receive cargo';
        if (body) {
          if (Array.isArray(body.errors) && body.errors.length > 0) {
            msg = body.errors.join(', ');
          } else if (body.message) {
            msg = body.message;
          } else if (typeof body === 'string') {
            msg = body;
          }
        }
        this.notification.error(msg);
      }
    });
  }

  getSelectedWarehouseName(): string {
    const whId = this.receiveForm.get('warehouseId')?.value;
    const wh = this.warehouses.find(w => w.id === whId);
    return wh ? `${wh.name} — ${wh.city}` : '';
  }

  resetForm() {
    this.receiveForm.reset({ hasDamageReport: false, binId: '', warehouseId: '', remarks: '' });
    this.foundShipment = null;
    this.receiptResult = null;
    this.bins = [];
  }
}
