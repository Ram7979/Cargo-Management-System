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

  /**
   * Load bins for a specific warehouse.
   * Backend route: GET /api/v1/warehouse/{warehouseId}/bins
   */
  getBins(warehouseId: string, onlyAvailable = false): Observable<ApiResponse<any[]>> {
    let url = `${this.apiUrl}/${warehouseId}/bins`;
    if (onlyAvailable) {
      url += '?available=true';
    }
    return this.http.get<any>(url).pipe(
      map((res: any) => {
        let items: any[] = [];
        if (res?.success !== false) {
          const data = res?.data;
          if (Array.isArray(data)) {
            items = data;
          } else if (data?.items && Array.isArray(data.items)) {
            items = data.items;
          }
        }
        return { success: true, data: items, message: '', errors: null } as ApiResponse<any[]>;
      })
    );
  }

  getInventory(warehouseId: string): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/${warehouseId}/inventory`);
  }

  getAllInventory(): Observable<ApiResponse<any[]>> {
    // Receipts endpoint returns double-wrapped: ApiResponse<PagedResponse<CargoReceiptDto>>
    // Items are at res.data.data (PagedResponse inherits Data from ApiResponse)
    return this.http.get<any>(`${this.apiUrl}/receipts?pageSize=100`).pipe(
      map((res: any) => {
        let items: any[] = [];
        const outer = res?.data;
        if (outer) {
          // Double-wrapped: res.data.data (PagedResponse.Data = items array)
          if (Array.isArray(outer.data)) {
            items = outer.data;
          }
          // Standard paged: res.data.items
          else if (Array.isArray(outer.items)) {
            items = outer.items;
          }
          // Direct array
          else if (Array.isArray(outer)) {
            items = outer;
          }
        }
        return { success: true, data: items, message: '', errors: null } as ApiResponse<any[]>;
      })
    );
  }

  getShipmentsInWarehouse(): Observable<ApiResponse<any[]>> {
    return this.getAllInventory();
  }
}
