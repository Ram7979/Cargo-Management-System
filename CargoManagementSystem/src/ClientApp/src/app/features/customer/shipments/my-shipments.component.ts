import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ShipmentService, Shipment } from '../../../core/services/shipment.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-my-shipments',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    FormsModule,
    MatIconModule, 
    MatButtonModule, 
    MatInputModule, 
    MatSelectModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ],
  templateUrl: './my-shipments.component.html',
  styleUrls: ['./my-shipments.component.scss']
})
export class MyShipmentsComponent implements OnInit {
  private shipmentService = inject(ShipmentService);

  shipments: Shipment[] = [];
  filteredShipments: Shipment[] = [];
  isLoading = true;
  searchTerm = '';
  statusFilter = 'all';

  ngOnInit() {
    this.loadShipments();
  }

  loadShipments() {
    this.isLoading = true;
    this.shipmentService.getAll(1, 100)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res: any) => {
          if (res.success && res.data) {
            this.shipments = res.data.items;
            this.applyFilters();
          }
        },
        error: (err: any) => {
          console.error('Failed to load shipments', err);
          // Mock data if API fails
          this.loadMockShipments();
        }
      });
  }

  loadMockShipments() {
    this.shipments = [
      {
        id: '1',
        trackingNumber: 'CMS-2025-0001',
        status: 'In Transit',
        senderName: 'John Doe',
        originAddress: '123 Main St, Mumbai',
        senderCity: 'Mumbai',
        senderCountry: 'India',
        recipientName: 'Alice Smith',
        destinationAddress: '456 Global Ave, Dubai',
        recipientCity: 'Dubai',
        recipientCountry: 'UAE',
        weightKg: 10.5,
        volumeCbm: 0.2,
        quantity: 1,
        cargoType: 'Electronics',
        cargoDescription: 'Smartphones',
        serviceType: 'Express',
        paymentMode: 'Prepaid',
        bolDocumentUrl: '',
        createdAt: new Date().toISOString()
      },
      {
        id: '2',
        trackingNumber: 'CMS-2025-0002',
        status: 'Pending',
        senderName: 'John Doe',
        originAddress: '123 Main St, Mumbai',
        senderCity: 'Mumbai',
        senderCountry: 'India',
        recipientName: 'Bob Johnson',
        destinationAddress: '789 Park St, London',
        recipientCity: 'London',
        recipientCountry: 'UK',
        weightKg: 50,
        volumeCbm: 1.5,
        quantity: 5,
        cargoType: 'Furniture',
        cargoDescription: 'Office Chairs',
        serviceType: 'Standard',
        paymentMode: 'Cash on Delivery',
        bolDocumentUrl: '',
        createdAt: new Date(Date.now() - 86400000).toISOString()
      },
      {
        id: '3',
        trackingNumber: 'CMS-2025-0003',
        status: 'Delivered',
        senderName: 'John Doe',
        originAddress: '123 Main St, Mumbai',
        senderCity: 'Mumbai',
        senderCountry: 'India',
        recipientName: 'Charlie Brown',
        destinationAddress: '101 Pine Rd, Tokyo',
        recipientCity: 'Tokyo',
        recipientCountry: 'Japan',
        weightKg: 2,
        volumeCbm: 0.05,
        quantity: 1,
        cargoType: 'Documents',
        cargoDescription: 'Contract Papers',
        serviceType: 'Same-Day',
        paymentMode: 'Prepaid',
        bolDocumentUrl: '',
        createdAt: new Date(Date.now() - 172800000).toISOString()
      }
    ];
    this.applyFilters();
  }

  applyFilters() {
    this.filteredShipments = this.shipments.filter(s => {
      const matchesSearch = s.trackingNumber.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                           s.recipientName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
                           s.destinationAddress.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesStatus = this.statusFilter === 'all' || s.status.toLowerCase() === this.statusFilter.toLowerCase();
      
      return matchesSearch && matchesStatus;
    });
  }

  getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'in transit': return 'status-blue';
      case 'delivered': return 'status-green';
      case 'pending': return 'status-amber';
      case 'cancelled': return 'status-red';
      default: return '';
    }
  }
}