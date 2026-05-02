import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BillingService, Invoice } from '../../../core/services/billing.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-invoice-list',
  standalone: true,
  imports: [CommonModule, RouterModule, MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatChipsModule, MatTooltipModule, MatProgressSpinnerModule, StatusChipComponent],
  templateUrl: './invoice-list.component.html',
  styleUrls: ['./invoice-list.component.scss']
})
export class InvoiceListComponent implements OnInit {
  private billingService = inject(BillingService);
  private notification = inject(NotificationService);

  displayedColumns: string[] = ['invoiceNumber', 'customer', 'shipment', 'amount', 'status', 'dueDate', 'actions'];
  dataSource: Invoice[] = [];
  totalItems = 0;
  pageSize = 10;
  pageIndex = 0;
  isLoading = false;

  ngOnInit() { this.loadData(); }

  loadData() {
    this.isLoading = true;
    this.billingService.getAll(this.pageIndex + 1, this.pageSize).subscribe({
      next: (res) => {
        if (res.success) {
          this.dataSource = res.data.items;
          this.totalItems = res.data.totalCount;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load invoices');
      }
    });
  }

  onPageChange(e: PageEvent) {
    this.pageIndex = e.pageIndex;
    this.pageSize = e.pageSize;
    this.loadData();
  }

  downloadPdf(invoice: Invoice) {
    this.billingService.getPdfUrl(invoice.id).subscribe((res: any) => {
      if (res.success && res.data.pdfUrl) {
        window.open(res.data.pdfUrl, '_blank');
      } else {
        this.notification.error('PDF not available for this invoice');
      }
    });
  }

  getStatusClass(status: string): string {
    return status.toLowerCase().replace(' ', '-');
  }
}
