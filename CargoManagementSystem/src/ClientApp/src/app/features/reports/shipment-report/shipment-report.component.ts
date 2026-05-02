import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ReportService, ReportFilter } from '../../../core/services/report.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-shipment-report',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatDatepickerModule, 
    MatNativeDateModule,
    MatSelectModule, 
    MatButtonModule, 
    MatIconModule,
    MatTableModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './shipment-report.component.html',
  styleUrls: ['./shipment-report.component.scss']
})
export class ShipmentReportComponent implements OnInit {
  private fb = inject(FormBuilder);
  private reportService = inject(ReportService);
  private notification = inject(NotificationService);

  filterForm: FormGroup;
  reportData: any[] = [];
  isLoading = false;
  displayedColumns: string[] = ['trackingNumber', 'status', 'sender', 'recipient', 'weight', 'date'];

  statusOptions = ['All', 'Pending', 'In Transit', 'Delivered', 'Cancelled'];

  constructor() {
    this.filterForm = this.fb.group({
      fromDate: [null],
      toDate: [null],
      status: ['All']
    });
  }

  ngOnInit() {
    // Don't auto-generate — wait for user to select date range
  }

  generateReport() {
    const { fromDate, toDate, status } = this.filterForm.value;

    if (!fromDate || !toDate) {
      this.notification.warn('Please select a valid date range');
      return;
    }

    this.isLoading = true;
    
    // Format dates to YYYY-MM-DD for backend
    const formatDate = (date: Date) => {
      const d = new Date(date);
      return d.toISOString().split('T')[0];
    };

    const filters: ReportFilter = {
      fromDate: formatDate(fromDate),
      toDate: formatDate(toDate),
      status: status === 'All' ? undefined : status
    };

    console.log('Generating report with filters:', filters);

    this.reportService.getShipmentReport(filters).subscribe({
      next: (res) => {
        if (res.success) {
          this.reportData = Array.isArray(res.data) ? res.data : [];
          if (this.reportData.length === 0) {
            this.notification.info('No data found for the selected range');
          }
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.notification.error(err.message || 'Failed to generate report');
      }
    });
  }

  exportData() {
    if (this.reportData.length === 0) return;
    this.reportService.exportToCsv(this.reportData, `shipment_report_${new Date().getTime()}`);
  }
}
