import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ShipmentService, Shipment } from '../../../core/services/shipment.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-shipment-tracking',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    FormsModule,
    MatIconModule, 
    MatButtonModule, 
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './shipment-tracking.component.html',
  styleUrls: ['./shipment-tracking.component.scss']
})
export class ShipmentTrackingComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private shipmentService = inject(ShipmentService);

  trackingId = '';
  shipment: Shipment | null = null;
  isLoading = false;
  error = '';

  trackingHistory = [
    { status: 'Delivered', location: 'Tokyo, Japan', date: new Date(Date.now() - 86400000 * 1), description: 'Package has been delivered to the recipient.', icon: 'check_circle', completed: true },
    { status: 'Out for Delivery', location: 'Tokyo, Japan', date: new Date(Date.now() - 86400000 * 1.2), description: 'Package is with the local courier for delivery.', icon: 'delivery_dining', completed: true },
    { status: 'In Transit', location: 'Tokyo Hub', date: new Date(Date.now() - 86400000 * 2), description: 'Arrived at the destination sorting facility.', icon: 'hub', completed: true },
    { status: 'Shipped', location: 'Mumbai International Airport', date: new Date(Date.now() - 86400000 * 3), description: 'Package has left the origin country.', icon: 'flight_takeoff', completed: true },
    { status: 'Processed', location: 'Mumbai Hub', date: new Date(Date.now() - 86400000 * 3.5), description: 'Package has been processed and is ready for shipping.', icon: 'inventory', completed: true },
    { status: 'Picked Up', location: 'Mumbai, India', date: new Date(Date.now() - 86400000 * 4), description: 'Package picked up by our courier.', icon: 'hail', completed: true }
  ];

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['tracking']) {
        this.trackingId = params['tracking'];
        this.trackShipment();
      }
    });
  }

  trackShipment() {
    if (!this.trackingId) return;

    this.isLoading = true;
    this.error = '';
    this.shipment = null;

    this.shipmentService.track(this.trackingId)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res: any) => {
          if (res.success && res.data) {
            this.shipment = res.data;
            this.generateTrackingHistory(res.data);
          } else {
            this.error = 'Shipment not found. Please check the tracking ID and try again.';
          }
        },
        error: (err: any) => {
          console.error('Tracking failed', err);
          // Fallback for demo if it looks like a valid ID
          if (this.trackingId.startsWith('CMS-')) {
            this.loadMockTracking();
          } else {
            this.error = 'An error occurred while fetching tracking info.';
          }
        }
      });
  }

  loadMockTracking() {
    this.shipment = {
      id: '1',
      trackingNumber: this.trackingId,
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
      createdAt: new Date(Date.now() - 86400000 * 5).toISOString()
    };
    this.generateTrackingHistory(this.shipment!);
  }

  generateTrackingHistory(shipment: Shipment) {
    // In a real app, this would come from the API
    // For now, we'll generate some realistic steps based on status
    const allSteps = [
      { status: 'Delivered', location: shipment.recipientCity + ', ' + shipment.recipientCountry, icon: 'check_circle', description: 'Package has been delivered to the recipient.' },
      { status: 'Out for Delivery', location: shipment.recipientCity, icon: 'delivery_dining', description: 'Package is with the local courier for delivery.' },
      { status: 'In Transit', location: 'International Sorting Center', icon: 'hub', description: 'Arrived at the sorting facility.' },
      { status: 'Shipped', location: shipment.senderCity + ' Airport', icon: 'flight_takeoff', description: 'Package has left the origin country.' },
      { status: 'Picked Up', location: shipment.senderCity, icon: 'hail', description: 'Package picked up by our courier.' }
    ];

    let currentFound = false;
    this.trackingHistory = allSteps.map((step, index) => {
      const isCurrent = step.status.toLowerCase() === shipment.status.toLowerCase();
      if (isCurrent) currentFound = true;

      return {
        ...step,
        date: new Date(Date.now() - (index * 86400000 * 0.5)),
        completed: currentFound || isCurrent
      };
    }).reverse();
  }
}