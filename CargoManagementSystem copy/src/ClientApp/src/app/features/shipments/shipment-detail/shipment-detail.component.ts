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
import { ApiResponse } from '../../../core/models/api-response.model';
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
  currentMarker: L.Marker | undefined;
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
      next: (res: ApiResponse<Shipment>) => {
        if (res.success && res.data) {
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
    this.shipmentService.getById(id).subscribe((res: ApiResponse<Shipment>) => {
      if (res.success && res.data && res.data.lastKnownLocation) {
        this.shipment = res.data;
        this.updateMapLocation(res.data.lastKnownLocation.latitude, res.data.lastKnownLocation.longitude);
      }
    });
  }

  async initMap() {
    if (!this.shipment) return;
    
    // Setup map if not already done
    if (!this.map) {
      this.map = L.map('map', { zoomControl: true, dragging: true });
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors'
      }).addTo(this.map);
    } else {
      // Clear existing layers if any
      this.map.eachLayer((layer) => {
        if (!!(layer as any)._url) return; // Keep tile layer
        this.map!.removeLayer(layer);
      });
      this.currentMarker = undefined;
    }

    try {
      // Build address strings
      const originStr = `${this.shipment.originAddress}, ${this.shipment.senderCity || ''}, ${this.shipment.senderCountry || ''}`.replace(/,\s*,/g, ',').trim();
      const destStr = `${this.shipment.destinationAddress}, ${this.shipment.recipientCity || ''}, ${this.shipment.recipientCountry || ''}`.replace(/,\s*,/g, ',').trim();

      // 1. Geocode
      const [originCoord, destCoord] = await Promise.all([
        this.geocode(originStr, { lat: 34.0522, lon: -118.2437 }), // LA default
        this.geocode(destStr, { lat: 40.7128, lon: -74.0060 })    // NY default
      ]);

      const start = [parseFloat(originCoord.lat), parseFloat(originCoord.lon)] as [number, number];
      const end = [parseFloat(destCoord.lat), parseFloat(destCoord.lon)] as [number, number];

      // 2. Custom Icons
      const startIcon = L.divIcon({
        className: 'custom-marker',
        html: `<div style="background-color: #22C55E; width: 32px; height: 32px; border-radius: 50%; border: 3px solid white; box-shadow: 0 4px 10px rgba(0,0,0,0.3); display: flex; align-items: center; justify-content: center; color: white;"><span class="material-icons" style="font-size: 18px;">flight_takeoff</span></div>`,
        iconSize: [32, 32],
        iconAnchor: [16, 16],
        popupAnchor: [0, -16]
      });

      const endIcon = L.divIcon({
        className: 'custom-marker',
        html: `<div style="background-color: #EF4444; width: 32px; height: 32px; border-radius: 50%; border: 3px solid white; box-shadow: 0 4px 10px rgba(0,0,0,0.3); display: flex; align-items: center; justify-content: center; color: white;"><span class="material-icons" style="font-size: 18px;">location_on</span></div>`,
        iconSize: [32, 32],
        iconAnchor: [16, 16],
        popupAnchor: [0, -16]
      });

      // 3. Add Origin & Destination Markers
      L.marker(start, { icon: startIcon }).addTo(this.map)
        .bindPopup(`
          <div style="font-family: inherit; padding: 4px;">
            <div style="font-size: 11px; color: #64748b; text-transform: uppercase; font-weight: 700; margin-bottom: 2px;">Sender</div>
            <div style="font-size: 14px; font-weight: 600; color: #0f172a; margin-bottom: 4px;">${this.shipment.senderName}</div>
            <div style="font-size: 12px; color: #475569; margin-bottom: 8px;">${originStr}</div>
            <div style="background: #f1f5f9; padding: 4px 8px; border-radius: 4px; font-size: 11px; font-weight: 600; color: #3b82f6;">${this.shipment.trackingNumber}</div>
          </div>
        `);
      
      L.marker(end, { icon: endIcon }).addTo(this.map)
        .bindPopup(`
          <div style="font-family: inherit; padding: 4px;">
            <div style="font-size: 11px; color: #64748b; text-transform: uppercase; font-weight: 700; margin-bottom: 2px;">Receiver</div>
            <div style="font-size: 14px; font-weight: 600; color: #0f172a; margin-bottom: 4px;">${this.shipment.recipientName}</div>
            <div style="font-size: 12px; color: #475569; margin-bottom: 8px;">${destStr}</div>
            <div style="background: #f1f5f9; padding: 4px 8px; border-radius: 4px; font-size: 11px; font-weight: 600; color: #3b82f6;">${this.shipment.trackingNumber}</div>
          </div>
        `);

      // 4. Fetch Route (OSRM)
      const routeRes = await fetch(`https://router.project-osrm.org/route/v1/driving/${start[1]},${start[0]};${end[1]},${end[0]}?overview=full&geometries=geojson`);
      const routeData = await routeRes.json();

      if (routeData.code === 'Ok' && routeData.routes && routeData.routes.length > 0) {
        const coords = routeData.routes[0].geometry.coordinates.map((c: any[]) => [c[1], c[0]]);
        L.polyline(coords, { 
          color: '#3B82F6', 
          weight: 4, 
          opacity: 0.8,
          lineCap: 'round',
          lineJoin: 'round',
          dashArray: '10, 10',
          className: 'animated-route'
        }).addTo(this.map);
      }

      // 5. Fit Bounds
      const bounds = L.latLngBounds([start, end]);
      this.map.fitBounds(bounds, { padding: [60, 60], maxZoom: 14 });

      // Add Current Location Truck Marker if different
      if (this.shipment.lastKnownLocation && this.shipment.status !== 'Delivered') {
        const truckIcon = L.divIcon({
          className: 'custom-marker',
          html: `<div style="background-color: #F59E0B; width: 36px; height: 36px; border-radius: 50%; border: 3px solid white; box-shadow: 0 4px 15px rgba(245,158,11,0.5); display: flex; align-items: center; justify-content: center; color: white;"><span class="material-icons" style="font-size: 20px;">local_shipping</span></div>`,
          iconSize: [36, 36],
          iconAnchor: [18, 18],
          popupAnchor: [0, -18]
        });
        const curr = [this.shipment.lastKnownLocation.latitude, this.shipment.lastKnownLocation.longitude] as [number, number];
        this.currentMarker = L.marker(curr, { icon: truckIcon }).addTo(this.map)
          .bindPopup(`
            <div style="font-family: inherit; padding: 4px;">
              <div style="font-size: 11px; color: #d97706; text-transform: uppercase; font-weight: 800; margin-bottom: 4px;">Current Location</div>
              <div style="font-size: 13px; font-weight: 600; color: #0f172a; margin-bottom: 2px;">${this.shipment.status}</div>
              <div style="font-size: 11px; color: #64748b;">${new Date().toLocaleTimeString()}</div>
            </div>
          `);
      }

    } catch (e) {
      console.error('Map loading error:', e);
    }
  }

  async geocode(address: string, fallback: {lat: number, lon: number}): Promise<{lat: string, lon: string}> {
    // If address is too short, return fallback
    if (!address || address.length < 5) return { lat: fallback.lat.toString(), lon: fallback.lon.toString() };
    try {
      const res = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${encodeURIComponent(address)}&limit=1`, {
        headers: { 'Accept': 'application/json' }
      });
      const data = await res.json();
      if (data && data.length > 0) {
        return { lat: data[0].lat, lon: data[0].lon };
      }
    } catch (e) {
      console.warn('Geocoding failed for', address, e);
    }
    return { lat: fallback.lat.toString(), lon: fallback.lon.toString() };
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
        this.shipmentService.updateStatus(this.shipment!.id, result.status, result.notes, result).subscribe({
          next: (res: ApiResponse<any>) => {
            if (res.success) {
              this.notification.success('Shipment status updated successfully');
              
              // Generate real notification
              this.notification.createNotification({
                recipientId: this.shipment!.customerId,
                channel: 'System',
                subject: `Shipment Status: ${result.status}`,
                body: `Your shipment has been updated to: ${result.status}. ${result.notes || ''}`.trim(),
                eventType: 'StatusUpdated'
              }).subscribe();

              this.loadShipment(this.shipment!.id);
            }
            this.isUpdatingStatus = false;
          },
          error: (err: any) => {
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
      next: (res: ApiResponse<any>) => {
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
      error: (err: any) => {
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
    if (this.map && this.currentMarker) {
      const newPos = new L.LatLng(lat, lng);
      this.currentMarker.setLatLng(newPos);
      // Optional: don't pan automatically if user is interacting with map
      // this.map.panTo(newPos);
      this.currentMarker.getPopup()?.setContent(`
        <div style="font-family: inherit; padding: 4px;">
          <div style="font-size: 11px; color: #d97706; text-transform: uppercase; font-weight: 800; margin-bottom: 4px;">Current Location</div>
          <div style="font-size: 13px; font-weight: 600; color: #0f172a; margin-bottom: 2px;">${this.shipment?.status}</div>
          <div style="font-size: 11px; color: #64748b;">${new Date().toLocaleTimeString()}</div>
        </div>
      `);
    }
  }

  getStatusClass(status: string): string {
    return status.toLowerCase().replace(' ', '-');
  }
}
