import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShipmentService, Shipment } from '../../../core/services/shipment.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';
import { ApiResponse } from '../../../core/models/api-response.model';
import { PagedResult } from '../../../core/services/base-api.service';

@Component({
  selector: 'app-shipment-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatChipsModule, StatusChipComponent, MatProgressSpinnerModule, MatTooltipModule],
  templateUrl: './shipment-list.component.html',
  styleUrls: ['./shipment-list.component.scss']
})
export class ShipmentListComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private shipmentService = inject(ShipmentService);
  private notification = inject(NotificationService);

  displayedColumns: string[] = ['trackingNumber', 'status', 'sender', 'recipient', 'date', 'actions'];
  dataSource: Shipment[] = [];
  totalItems = 0;
  pageSize = 10;
  pageIndex = 0;
  trackingFilter: string | null = null;
  isLoading = false;

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.trackingFilter = params['trackingNumber'] || null;
      this.loadShipments();
    });
  }

  loadShipments() {
    this.isLoading = true;
    const filters = this.trackingFilter ? { trackingNumber: this.trackingFilter } : undefined;
    this.shipmentService.getAll(this.pageIndex + 1, this.pageSize, filters).subscribe({
      next: (response: ApiResponse<PagedResult<Shipment>>) => {
        if (response.success && response.data) {
          const data = response.data;
          this.dataSource = Array.isArray(data.items) ? data.items : [];
          this.totalItems = data.totalCount || 0;
        } else {
          this.dataSource = [];
          this.totalItems = 0;
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('ShipmentList: Failed to load shipments', error);
        this.notification.error('Failed to load shipments');
        this.dataSource = [];
        this.totalItems = 0;
        this.isLoading = false;
      }
    });
  }

  onDelete(id: string) {
    if (!confirm('Are you sure you want to cancel this shipment?')) return;
    this.shipmentService.cancel(id, 'Cancelled by admin').subscribe({
      next: (res) => {
        if (res.success) {
          this.notification.success('Shipment cancelled successfully');
          this.loadShipments();
        } else {
          const msg = (res as any).errors?.join(', ') || (res as any).message || 'Cancellation failed';
          this.notification.error(msg);
        }
      },
      error: (err) => {
        const msg = err.error?.errors?.join(', ') || err.error?.message || err.message || 'Cancellation failed';
        this.notification.error(msg);
      }
    });
  }

  onPageChange(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadShipments();
  }

  getStatusColor(status: string): string {
    switch (status) {
      case 'Pending': return 'accent';
      case 'Delivered': return 'primary';
      case 'Cancelled': return 'warn';
      default: return 'primary';
    }
  }
}
