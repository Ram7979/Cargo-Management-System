import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { BaseApiService, PagedResult } from './base-api.service';

export interface Shipment {
  id: string;
  trackingNumber: string;
  customerId: string;
  status: string;
  senderName: string;
  originAddress: string;
  senderCity?: string;
  senderCountry?: string;
  recipientName: string;
  destinationAddress: string;
  recipientCity?: string;
  recipientCountry?: string;
  weightKg: number;
  volumeCbm?: number;
  quantity?: number;
  cargoType?: string;
  cargoDescription?: string;
  serviceType: string;
  estimatedDeliveryDate?: string;
  podImageUrl?: string;
  lastKnownLocation?: {
    latitude: number;
    longitude: number;
    recordedAt: string;
  };
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ShipmentService extends BaseApiService<Shipment> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/shipments`);
  }

  getMyShipments(page = 1, pageSize = 20, status?: string): Observable<ApiResponse<PagedResult<Shipment>>> {
    let url = `${this.apiUrl}/my?page=${page}&pageSize=${pageSize}`;
    if (status) url += `&status=${status}`;
    return this.http.get<ApiResponse<PagedResult<Shipment>>>(url);
  }

  track(trackingNumber: string): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/track/${trackingNumber}`);
  }

  // Alias for backward compatibility
  getShipmentByTracking(trackingNumber: string): Observable<ApiResponse<any>> {
    return this.track(trackingNumber);
  }

  override getById(id: string): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }

  // Alias for backward compatibility
  getShipmentById(id: string): Observable<ApiResponse<any>> {
    return this.getById(id);
  }

  override create(shipmentData: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(this.apiUrl, shipmentData);
  }

  // Alias for backward compatibility
  createShipment(shipmentData: any): Observable<ApiResponse<any>> {
    return this.create(shipmentData);
  }

  updateStatus(id: string, status: string, notes?: string, extraArgs?: any): Observable<ApiResponse<any>> {
    return this.http.patch<ApiResponse<any>>(`${this.apiUrl}/${id}/status`, { status, notes, ...extraArgs });
  }

  cancel(id: string, reason: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`, { body: { reason } });
  }

  getPrintLabelInfo(id: string): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/${id}/document/bol`);
  }

  getStatusHistory(shipmentId: string): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/${shipmentId}/status-history`);
  }
}
