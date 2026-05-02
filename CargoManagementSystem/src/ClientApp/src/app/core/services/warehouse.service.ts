import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';

export interface ReceiveCargoRequest {
  trackingNumber: string;
  warehouseId: string;
  binId?: string;
  hasDamageReport?: boolean;
  damageNotes?: string;
  remarks?: string;
}

export interface ReleaseCargoRequest {
  shipmentId: string;
  releaseTo: string;
  releasedBy: string;
}

@Injectable({ providedIn: 'root' })
export class WarehouseService extends BaseApiService<any> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/warehouse`);
  }

  receive(request: ReceiveCargoRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/receive`, request);
  }

  release(request: ReleaseCargoRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/release`, request);
  }

  getBinLocations(): Observable<ApiResponse<string[]>> {
    return this.http.get<ApiResponse<string[]>>(`${this.apiUrl}/bins`);
  }

  getInventory(warehouseId: string): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/${warehouseId}/inventory`);
  }

  getAllInventory(): Observable<ApiResponse<any[]>> {
    // Use the receipts endpoint which lists all cargo receipts across warehouses
    return this.http.get<any>(`${this.apiUrl}/receipts?pageSize=100`).pipe(
      map((res: any) => {
        const items = res?.data?.items || res?.data || [];
        return { success: true, data: items, message: '', errors: null } as ApiResponse<any[]>;
      })
    );
  }

  getShipmentsInWarehouse(): Observable<ApiResponse<any[]>> {
    return this.getAllInventory();
  }
}
