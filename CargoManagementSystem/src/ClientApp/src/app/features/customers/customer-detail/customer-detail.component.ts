import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { CustomerService, Customer } from '../../../core/services/customer.service';
import { NotificationService } from '../../../core/services/notification.service';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip.component';

@Component({
  selector: 'app-customer-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatTableModule, MatTabsModule, MatIconModule, MatButtonModule, StatusChipComponent],
  templateUrl: './customer-detail.component.html',
  styleUrls: ['./customer-detail.component.scss']
})
export class CustomerDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private customerService = inject(CustomerService);
  private notification = inject(NotificationService);

  customer: Customer | null = null;
  shipments: any[] = [];
  isLoading = true;
  shipmentColumns = ['trackingNumber', 'status', 'date', 'amount'];

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadCustomer(id);
      this.loadShipments(id);
    }
  }

  loadCustomer(id: string) {
    this.customerService.getById(id).subscribe(res => {
      if (res.success) {
        this.customer = res.data;
      }
      this.isLoading = false;
    });
  }

  loadShipments(id: string) {
    this.customerService.getShipments(id).subscribe(res => {
      if (res.success && res.data) {
        this.shipments = res.data.items || [];
      }
    });
  }
}
