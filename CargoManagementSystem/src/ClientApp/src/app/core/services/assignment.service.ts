import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { map } from 'rxjs/operators';

export interface Assignment {
  id: string;
  shipmentId: string;
  shipmentTrackingNumber: string;
  vehicleId: string;
  vehiclePlate: string;
  driverId: string;
  driverName: string;
  assignedAt: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class AssignmentService extends BaseApiService<Assignment> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/assignments`);
  }

  // Get pending shipments from the shipments endpoint (status=Pending)
  getPendingShipments(): Observable<ApiResponse<any[]>> {
    return this.http.get<any>(`${environment.apiUrl}/shipments?status=Pending&pageSize=100`).pipe(
      map(res => {
        const items = res?.data?.items || res?.data || [];
        return { success: true, data: items, message: '', errors: null };
      })
    );
  }

  createAssignment(data: CreateAssignmentRequest): Observable<ApiResponse<Assignment>> {
    return this.create(data);
  }

  assign(data: CreateAssignmentRequest): Observable<ApiResponse<Assignment>> {
    return this.createAssignment(data);
  }
}

export interface CreateAssignmentRequest {
  shipmentId: string;
  vehicleId: string;
  driverId: string;
}
