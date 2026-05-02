import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface Driver {
  id: string;
  firstName: string;
  lastName: string;
  licenseNumber: string;
  phone: string;
  status: string; // Active, OnTrip, Leave
  assignedVehicleId?: string;
  assignedVehiclePlate?: string;
}

@Injectable({ providedIn: 'root' })
export class DriverService extends BaseApiService<Driver> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/drivers`);
  }

  updateStatus(id: string, status: string): Observable<ApiResponse<any>> {
    return this.http.patch<ApiResponse<any>>(`${this.apiUrl}/${id}/status`, { status });
  }

  getAvailable(): Observable<ApiResponse<Driver[]>> {
    return this.http.get<ApiResponse<Driver[]>>(`${this.apiUrl}/available`);
  }
}
