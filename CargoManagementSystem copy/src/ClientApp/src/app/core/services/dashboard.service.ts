import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface DashboardSummary {
  totalShipments: number;
  activeShipments: number;
  deliveredToday: number;
  totalRevenue: number;
  revenueGrowth: number;
  shipmentsByStatus: { status: string, count: number }[];
  revenueTrend: { date: string, value: number, amount?: number }[];
  cargoTypes: { type: string, cargoType?: string, count: number }[];
  recentShipments: any[];
  totalActiveShipments: number;
  warehouseUtilization: number;
  fleetAvailability: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/dashboard`;

  getSummary(fromDate?: string, toDate?: string): Observable<ApiResponse<DashboardSummary>> {
    let url = `${this.apiUrl}/summary`;
    if (fromDate && toDate) {
      url += `?fromDate=${fromDate}&toDate=${toDate}`;
    }
    return this.http.get<ApiResponse<DashboardSummary>>(url);
  }

  getKpis(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/kpis`);
  }

  getTrendData(groupBy = 'day'): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/trend-data?groupBy=${groupBy}`);
  }

  getRecentShipments(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/shipments`);
  }

  exportReport(reportType: string, fromDate?: string, toDate?: string): Observable<Blob> {
    const payload = {
      reportType: reportType,
      format: 'csv',
      parameters: {
        fromDate: fromDate,
        toDate: toDate
      }
    };
    return this.http.post(`${environment.apiUrl}/reports/export`, payload, { responseType: 'blob' });
  }
}
