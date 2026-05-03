import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { BillingService, Invoice } from '../../../core/services/billing.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    MatIconModule, 
    MatButtonModule, 
    MatTableModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.scss']
})
export class InvoicesComponent implements OnInit {
  private billingService = inject(BillingService);
  private snackBar = inject(MatSnackBar);

  invoices: Invoice[] = [];
  isLoading = true;
  isProcessingPayment = false;

  displayedColumns: string[] = ['invoiceNumber', 'shipment', 'amount', 'dueDate', 'status', 'actions'];

  ngOnInit() {
    this.loadInvoices();
  }

  loadInvoices() {
    this.isLoading = true;
    this.billingService.getAll(1, 50)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res: any) => {
          if (res.success && res.data) {
            this.invoices = res.data.items;
          }
        },
        error: (err: any) => {
          console.error('Failed to load invoices', err);
          this.loadMockInvoices();
        }
      });
  }

  loadMockInvoices() {
    this.invoices = [
      {
        id: '1',
        invoiceNumber: 'INV-2025-001',
        customerId: 'cust-1',
        customerName: 'John Doe',
        shipmentId: 'ship-1',
        shipmentTrackingNumber: 'CMS-2025-0001',
        totalAmount: 1500.00,
        status: 'Unpaid',
        dueDate: new Date(Date.now() + 86400000 * 7).toISOString(),
        createdAt: new Date().toISOString()
      },
      {
        id: '2',
        invoiceNumber: 'INV-2025-002',
        customerId: 'cust-1',
        customerName: 'John Doe',
        shipmentId: 'ship-2',
        shipmentTrackingNumber: 'CMS-2025-0002',
        totalAmount: 450.50,
        status: 'Paid',
        dueDate: new Date(Date.now() - 86400000 * 2).toISOString(),
        createdAt: new Date(Date.now() - 86400000 * 5).toISOString()
      },
      {
        id: '3',
        invoiceNumber: 'INV-2025-003',
        customerId: 'cust-1',
        customerName: 'John Doe',
        shipmentId: 'ship-3',
        shipmentTrackingNumber: 'CMS-2025-0003',
        totalAmount: 2100.00,
        status: 'Overdue',
        dueDate: new Date(Date.now() - 86400000 * 10).toISOString(),
        createdAt: new Date(Date.now() - 86400000 * 15).toISOString()
      }
    ];
  }

  payInvoice(invoice: Invoice) {
    this.isProcessingPayment = true;
    
    // Simulate payment processing
    setTimeout(() => {
      invoice.status = 'Paid';
      this.isProcessingPayment = false;
      this.snackBar.open(`Payment for ${invoice.invoiceNumber} successful!`, 'Close', { duration: 5000 });
    }, 2000);
  }

  downloadInvoice(invoice: Invoice) {
    this.snackBar.open(`Preparing PDF for ${invoice.invoiceNumber}...`, 'Close', { duration: 2000 });
    // In a real app, this would call getInvoicePdfUrl
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'paid': return 'status-green';
      case 'unpaid': return 'status-amber';
      case 'overdue': return 'status-red';
      default: return '';
    }
  }
}