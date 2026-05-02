import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface Vehicle {
  id: string;
  plateNumber: string;
  type: string;           // actual backend field name
  vehicleType?: string;   // alias - may be same as type
  make?: string;
  model?: string;
  year?: number;
  fuelType?: string;
  capacityKg: number;
  volumeCbm?: number;
  status: string;
  currentDriverId?: string;
  currentDriverName?: string;
  currentLoadKg?: number;
  lastMaintenanceDate?: string;
  nextServiceDate?: string;
  currentLatitude?: number;
  currentLongitude?: number;
  lastLocation?: string;
  gpsDeviceId?: string;
  gpsTrackingStatus?: string;
}

@Injectable({ providedIn: 'root' })
export class VehicleService extends BaseApiService<Vehicle> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/vehicles`);
  }

  getLiveLocations(): Observable<ApiResponse<Vehicle[]>> {
    return this.http.get<ApiResponse<Vehicle[]>>(`${this.apiUrl}/live-locations`);
  }

  updateStatus(id: string, status: string): Observable<ApiResponse<any>> {
    return this.http.patch<ApiResponse<any>>(`${this.apiUrl}/${id}/status`, { status });
  }
}
