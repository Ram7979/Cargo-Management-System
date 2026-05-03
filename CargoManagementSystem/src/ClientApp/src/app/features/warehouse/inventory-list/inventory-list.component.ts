import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterModule } from '@angular/router';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-inventory-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatTooltipModule, RouterModule, StatusChipComponent],
  templateUrl: './inventory-list.component.html',
  styleUrls: ['./inventory-list.component.scss']
})
export class InventoryListComponent implements OnInit {
  private warehouseService = inject(WarehouseService);
  private notification = inject(NotificationService);

  displayedColumns: string[] = ['trackingNumber', 'binLocation', 'receivedDate', 'cargoType', 'weight', 'actions'];
  dataSource: any[] = [];
  isLoading = true;

  ngOnInit() {
    this.loadInventory();
  }

  loadInventory() {
    this.isLoading = true;
    this.warehouseService.getShipmentsInWarehouse().subscribe({
      next: (res: any) => {
        let items: any[] = [];

        if (res.success !== false) {
          const data = res.data;
          if (Array.isArray(data)) {
            items = data;
          } else if (data && typeof data === 'object') {
            // Paged response: { items: [...], totalCount, ... }
            if (Array.isArray(data.items)) {
              items = data.items;
            }
            // PagedResponse wrapper: { data: { items: [...] } }
            else if (data.data && Array.isArray(data.data.items)) {
              items = data.data.items;
            }
          }
        }

        // Map CargoReceiptDto fields to the template's expected field names
        this.dataSource = items.map((item: any) => ({
          shipmentId: item.shipmentId || item.id,
          trackingNumber: item.trackingNumber || '—',
          binLocation: item.binCode || item.binLocation || item.binId || '—',
          receivedDate: item.receivedAt || item.receivedDate || null,
          cargoType: item.cargoType || item.cargoDescription || '—',
          weight: item.weightKg || item.weight || 0,
          remarks: item.remarks || '',
          hasDamageReport: item.hasDamageReport || false
        }));

        this.isLoading = false;
      },
      error: (err: any) => {
        this.isLoading = false;
        console.error('[Inventory] Failed to load inventory:', err);
        this.notification.error('Failed to load warehouse inventory');
      }
    });
  }
}
