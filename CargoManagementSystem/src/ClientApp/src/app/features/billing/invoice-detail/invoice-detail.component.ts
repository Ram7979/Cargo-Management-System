import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatTableModule } from '@angular/material/table';
import { BillingService, Invoice } from '../../../core/services/billing.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-invoice-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatIconModule, MatDividerModule, MatTableModule, StatusChipComponent],
  templateUrl: './invoice-detail.component.html',
  styleUrls: ['./invoice-detail.component.scss']
})
export class InvoiceDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private billingService = inject(BillingService);
  private notification = inject(NotificationService);

  invoice: Invoice | null = null;
  isLoading = true;
  displayedColumns: string[] = ['description', 'quantity', 'unitPrice', 'total'];

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadInvoice(id);
    }
  }

  loadInvoice(id: string) {
    this.billingService.getById(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.invoice = res.data;
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load invoice details');
      }
    });
  }

  downloadPdf() {
    if (!this.invoice) return;
    this.billingService.getInvoicePdf(this.invoice.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `Invoice_${this.invoice?.invoiceNumber}.pdf`;
        link.click();
      },
      error: () => this.notification.error('Failed to download PDF')
    });
  }

  processPayment() {
    // Logic for opening payment dialog would go here
    this.notification.info('Payment processing feature coming soon');
  }

  onPay() {
    this.processPayment();
  }
}
