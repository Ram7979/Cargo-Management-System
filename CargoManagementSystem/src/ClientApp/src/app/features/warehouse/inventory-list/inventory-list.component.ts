import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterModule } from '@angular/router';
import { WarehouseService } from '../../../core/services/warehouse.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-inventory-list',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, RouterModule, StatusChipComponent],
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
        if (res.success) {
          this.dataSource = res.data;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load warehouse inventory');
      }
    });
  }
}
