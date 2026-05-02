import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface GpsCoordinate {
  latitude: number;
  longitude: number;
  recordedAt: string;
}

export interface Shipment {
  id: string;
  trackingNumber: string;
  status: string;
  senderName: string;
  originAddress: string;
  senderCity: string;
  senderCountry: string;
  recipientName: string;
  destinationAddress: string;
  recipientCity: string;
  recipientCountry: string;
  weightKg: number;
  volumeCbm: number;
  quantity: number;
  cargoType: string;
  cargoDescription: string;
  serviceType: string;
  paymentMode: string;
  bolDocumentUrl: string;
  podImageUrl?: string;
  lastKnownLocation?: GpsCoordinate;
  estimatedDeliveryDate?: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ShipmentService extends BaseApiService<Shipment> {
  constructor() {
    const http = inject(HttpClient);
    super(http, `${environment.apiUrl}/shipments`);
  }

  track(trackingNumber: string): Observable<ApiResponse<Shipment>> {
    return this.http.get<ApiResponse<Shipment>>(`${this.apiUrl}/track/${trackingNumber}`);
  }

  updateStatus(id: string, status: string, notes?: string): Observable<ApiResponse<any>> {
    return this.http.patch<ApiResponse<any>>(`${this.apiUrl}/${id}/status`, { status, notes });
  }

  printLabel(id: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${id}/document/bol`, {
      responseType: 'blob'
    });
  }

  cancel(id: string, reason: string): Observable<ApiResponse<any>> {
    // Backend CancelShipment uses DELETE with body { cancellationReason }
    return this.http.request<ApiResponse<any>>('DELETE', `${this.apiUrl}/${id}`, {
      body: { cancellationReason: reason },
      headers: { 'Content-Type': 'application/json' }
    });
  }
}
