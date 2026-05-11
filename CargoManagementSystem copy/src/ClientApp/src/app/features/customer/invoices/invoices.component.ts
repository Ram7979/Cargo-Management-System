import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InvoiceService } from '../../../core/services/invoice.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { PaymentDialogComponent } from './payment-dialog/payment-dialog.component';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule, MatDialogModule, MatSnackBarModule],
  template: `
    <div class="invoices-container">
      <h2>Invoices & Payments</h2>
      
      <div class="loading-overlay" *ngIf="loading">
        <mat-progress-spinner mode="indeterminate" diameter="40"></mat-progress-spinner>
      </div>

      <div class="error-msg" *ngIf="error">
        {{ error }}
        <button mat-button (click)="loadInvoices()">Retry</button>
      </div>

      <div class="table-card" *ngIf="!loading && !error">
        <table mat-table [dataSource]="invoices" class="full-width-table">
          <ng-container matColumnDef="invoiceNo">
            <th mat-header-cell *matHeaderCellDef> Invoice No </th>
            <td mat-cell *matCellDef="let inv"> {{inv.invoiceNumber}} </td>
          </ng-container>

          <ng-container matColumnDef="shipment">
            <th mat-header-cell *matHeaderCellDef> Shipment </th>
            <td mat-cell *matCellDef="let inv"> {{inv.trackingNumber || 'N/A'}} </td>
          </ng-container>

          <ng-container matColumnDef="amount">
            <th mat-header-cell *matHeaderCellDef> Total Amount </th>
            <td mat-cell *matCellDef="let inv"> {{inv.totalAmount | currency}} </td>
          </ng-container>

          <ng-container matColumnDef="balance">
            <th mat-header-cell *matHeaderCellDef> Outstanding </th>
            <td mat-cell *matCellDef="let inv"> {{inv.outstandingBalance | currency}} </td>
          </ng-container>

          <ng-container matColumnDef="status">
            <th mat-header-cell *matHeaderCellDef> Status </th>
            <td mat-cell *matCellDef="let inv">
              <span class="status-dot" [ngClass]="inv.status.toLowerCase()"></span>
              {{inv.status}}
            </td>
          </ng-container>

          <ng-container matColumnDef="actions">
            <th mat-header-cell *matHeaderCellDef> </th>
            <td mat-cell *matCellDef="let inv">
              <button mat-stroked-button *ngIf="inv.outstandingBalance > 0 && inv.status !== 'Paid'" color="primary" (click)="openPaymentDialog(inv)">Pay Now</button>
              <button mat-icon-button (click)="downloadPdf(inv.id)"><mat-icon>file_download</mat-icon></button>
            </td>
          </ng-container>

          <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>
          <tr mat-row *matRowDef="let row; columns: displayedColumns;"></tr>
        </table>

        <div class="empty-state" *ngIf="invoices.length === 0">
          <mat-icon>receipt_long</mat-icon>
          <p>No invoices found.</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .invoices-container { display: flex; flex-direction: column; gap: 24px; min-height: 400px; position: relative; }
    .table-card { background: #161B22; border-radius: 16px; border: 1px solid rgba(255,255,255,0.1); overflow: hidden; }
    .full-width-table { width: 100%; background: transparent; }
    
    .loading-overlay { display: flex; justify-content: center; padding: 40px; }
    .error-msg { color: #F87171; text-align: center; padding: 20px; }
    .empty-state { padding: 80px; text-align: center; color: #64748B; mat-icon { font-size: 48px; width: 48px; height: 48px; margin-bottom: 16px; } }

    .status-dot { display: inline-block; width: 8px; height: 8px; border-radius: 50%; margin-right: 8px; }
    .status-dot.paid { background: #22C55E; box-shadow: 0 0 8px #22C55E; }
    .status-dot.issued { background: #3B82F6; box-shadow: 0 0 8px #3B82F6; }
    .status-dot.pending { background: #F59E0B; box-shadow: 0 0 8px #F59E0B; }
    .status-dot.overdue { background: #EF4444; box-shadow: 0 0 8px #EF4444; }

    ::ng-deep .mat-mdc-header-cell { color: #94A3B8 !important; border-bottom-color: rgba(255,255,255,0.1) !important; }
    ::ng-deep .mat-mdc-cell { color: #E2E8F0 !important; border-bottom-color: rgba(255,255,255,0.05) !important; }
  `]
})
export class InvoicesComponent implements OnInit {
  private invoiceService = inject(InvoiceService);
  private authService = inject(AuthService);
  private notificationService = inject(NotificationService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  displayedColumns = ['invoiceNo', 'shipment', 'amount', 'balance', 'status', 'actions'];
  invoices: any[] = [];
  loading = true;
  error = '';

  ngOnInit() {
    this.loadInvoices();
  }

  loadInvoices() {
    this.loading = true;
    this.error = '';
    this.invoiceService.getMyInvoices().subscribe({
      next: (res) => {
        this.invoices = Array.isArray(res.data) ? res.data : (res.data as any)?.items || [];
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to fetch invoices. Please try again.';
        this.loading = false;
      }
    });
  }

  downloadPdf(id: string) {
    this.invoiceService.getInvoicePdf(id).subscribe({
      next: (res) => {
        if (res.pdfUrl) {
          // Because the token must be passed, we use HttpClient to fetch the blob.
          const url = res.pdfUrl.startsWith('http') ? res.pdfUrl : `http://localhost:5000${res.pdfUrl}`;
          this.invoiceService.downloadPdfBlob(url).subscribe({
            next: (blob: Blob) => {
              const objUrl = window.URL.createObjectURL(blob);
              const link = document.createElement('a');
              link.href = objUrl;
              link.download = `Invoice_${id}.pdf`;
              document.body.appendChild(link);
              link.click();
              document.body.removeChild(link);
              window.URL.revokeObjectURL(objUrl);
            },
            error: () => alert('Failed to download invoice PDF.')
          });
        }
      }
    });
  }

  openPaymentDialog(invoice: any) {
    const dialogRef = this.dialog.open(PaymentDialogComponent, {
      width: '450px',
      disableClose: true,
      data: { 
        invoiceId: invoice.id, 
        invoiceNumber: invoice.invoiceNumber, 
        amount: invoice.outstandingBalance 
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        // Reload invoices from API to reflect permanent change
        this.loadInvoices();

        // Generate exact timestamp notification
        const userId = this.authService.currentUser()?.id;
        if (userId) {
          const now = new Date();
          const dateFormatter = new Intl.DateTimeFormat('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
          const timeFormatter = new Intl.DateTimeFormat('en-US', { hour: '2-digit', minute: '2-digit', hour12: true });
          const formattedDate = `${dateFormatter.format(now)}, ${timeFormatter.format(now)}`;

          const payload = {
            recipientId: userId,
            channel: 'Push',
            recipient: userId,
            subject: 'Payment Successful',
            body: `Payment completed successfully — ${formattedDate}`,
            eventType: 'PaymentReceived'
          };
          this.notificationService.createNotification(payload).subscribe({
            error: (err) => console.error('Notification Error:', err)
          });
        }
        
        // Show success message
        this.snackBar.open('Payment Successful! Invoice updated.', 'Close', {
          duration: 5000,
          panelClass: ['success-snackbar'],
          horizontalPosition: 'right',
          verticalPosition: 'top'
        });
      }
    });
  }
}