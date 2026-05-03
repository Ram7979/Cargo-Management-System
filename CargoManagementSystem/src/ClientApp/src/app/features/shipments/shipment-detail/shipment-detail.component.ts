import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { ShipmentStatusDialogComponent } from '../shipment-status-dialog/shipment-status-dialog.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { ShipmentService, Shipment } from '../../../core/services/shipment.service';
import { NotificationService } from '../../../core/services/notification.service';
import { environment } from '../../../../environments/environment';
import * as L from 'leaflet';

@Component({
  selector: 'app-shipment-detail',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatIconModule, MatButtonModule, MatChipsModule, MatProgressSpinnerModule, MatDividerModule, MatDialogModule],
  templateUrl: './shipment-detail.component.html',
  styleUrls: ['./shipment-detail.component.scss']
})
export class ShipmentDetailComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private shipmentService = inject(ShipmentService);
  private notification = inject(NotificationService);
  private dialog = inject(MatDialog);

  shipment: Shipment | null = null;
  isLoading = true;
  isUpdatingStatus = false;
  isPrinting = false;
  map: L.Map | undefined;
  marker: L.Marker | undefined;
  private refreshInterval: any;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadShipment(id);
      // Poll for location updates every 30 seconds for live tracking simulation
      this.refreshInterval = setInterval(() => this.pollLocation(id), 30000);
    }
  }

  ngOnDestroy() {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
  }

  loadShipment(id: string) {
    this.isLoading = true;
    this.shipmentService.getById(id).subscribe({
      next: (res) => {
        if (res.success) {
          this.shipment = res.data;
          if (this.shipment) {
            setTimeout(() => this.initMap(), 100);
          }
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.notification.error('Failed to load shipment details');
      }
    });
  }

  pollLocation(id: string) {
    this.shipmentService.getById(id).subscribe(res => {
      if (res.success && res.data && res.data.lastKnownLocation) {
        this.shipment = res.data;
        this.updateMapLocation(res.data.lastKnownLocation.latitude, res.data.lastKnownLocation.longitude);
      }
    });
  }

  initMap() {
    if (!this.shipment) return;
    
    const lat = this.shipment.lastKnownLocation?.latitude || 51.505;
    const lng = this.shipment.lastKnownLocation?.longitude || -0.09;

    this.map = L.map('map', {
      zoomControl: true,
      dragging: true
    }).setView([lat, lng], 13);
    
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap'
    }).addTo(this.map);
    
    const customIcon = L.icon({
      iconUrl: 'assets/marker-icon.png',
      shadowUrl: 'assets/marker-shadow.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41],
      popupAnchor: [1, -34],
    });

    this.marker = L.marker([lat, lng], { icon: customIcon }).addTo(this.map)
      .bindPopup(`<b>${this.shipment.trackingNumber}</b><br>${this.shipment.status}`)
      .openPopup();
  }

  openStatusDialog() {
    if (!this.shipment) return;
    const dialogRef = this.dialog.open(ShipmentStatusDialogComponent, {
      data: { shipment: this.shipment },
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.isUpdatingStatus = true;
        this.shipmentService.updateStatus(this.shipment!.id, result.status, result.notes).subscribe({
          next: (res) => {
            if (res.success) {
              this.notification.success('Shipment status updated successfully');
              this.loadShipment(this.shipment!.id);
            }
            this.isUpdatingStatus = false;
          },
          error: (err) => {
            this.isUpdatingStatus = false;
            const msg = err.error?.message || err.error?.errors?.join(', ') || 'Failed to update status';
            this.notification.error(msg);
          }
        });
      }
    });
  }

  private downloadBlob(blob: Blob, fileName: string) {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  printLabel() {
    if (!this.shipment) return;
    this.isPrinting = true;
    this.shipmentService.getPrintLabelInfo(this.shipment.id).subscribe({
      next: (res) => {
        if (res.success && res.data?.bolUrl) {
          // Construct full URL and download
          const fullUrl = res.data.bolUrl.startsWith('http') 
            ? res.data.bolUrl 
            : `${environment.apiUrl}/${res.data.bolUrl}`;
          
          const link = document.createElement('a');
          link.href = fullUrl;
          link.target = '_blank';
          link.download = `Label_${this.shipment!.trackingNumber}.pdf`;
          link.click();
          this.isPrinting = false;
          this.notification.success('Label opened in new tab');
        } else {
          this.notification.warning('PDF info missing. Generating fallback...');
          this.downloadFallbackDocument('bol');
        }
      },
      error: (err) => {
        this.isPrinting = false;
        this.notification.warning('PDF Generation failed. Generating text fallback...');
        this.downloadFallbackDocument('bol');
      }
    });
  }

  openDoc(type: 'bol' | 'pod') {
    if (!this.shipment) return;
    // Attempt direct URL first for "normal save/view" behavior
    const url = `${environment.apiUrl}/shipments/${this.shipment.id}/document/${type}`;
    const win = window.open(url, '_blank');
    
    // If blocked or fails, we provide the fallback
    if (!win) {
      this.notification.warning('Pop-up blocked or failed. Generating fallback document...');
      this.downloadFallbackDocument(type);
    }
  }

  private downloadFallbackDocument(type: string) {
    if (!this.shipment) return;
    const content = `
      CARGO MANAGEMENT SYSTEM - ${type.toUpperCase()} FALLBACK
      --------------------------------------------------
      Tracking Number: ${this.shipment.trackingNumber}
      Status: ${this.shipment.status}
      Sender: ${this.shipment.senderName} (${this.shipment.originAddress}, ${this.shipment.senderCity})
      Recipient: ${this.shipment.recipientName} (${this.shipment.destinationAddress}, ${this.shipment.recipientCity})
      Cargo: ${this.shipment.cargoType} - ${this.shipment.cargoDescription}
      Weight: ${this.shipment.weightKg} KG
      Date: ${new Date().toLocaleString()}
      --------------------------------------------------
    `;
    const blob = new Blob([content], { type: 'text/plain' });
    const blobUrl = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = blobUrl;
    link.download = `${type.toUpperCase()}_${this.shipment.trackingNumber}_Fallback.txt`;
    link.click();
    window.URL.revokeObjectURL(blobUrl);
  }

  updateMapLocation(lat: number, lng: number) {
    if (this.map && this.marker) {
      const newPos = new L.LatLng(lat, lng);
      this.marker.setLatLng(newPos);
      this.map.panTo(newPos);
      this.marker.getPopup()?.setContent(`<b>${this.shipment?.trackingNumber}</b><br>${this.shipment?.status}<br><small>Updated: ${new Date().toLocaleTimeString()}</small>`);
    }
  }

  getStatusClass(status: string): string {
    return status.toLowerCase().replace(' ', '-');
  }
}
