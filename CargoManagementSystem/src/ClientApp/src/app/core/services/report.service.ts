import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface ReportFilter {
  fromDate?: string;
  toDate?: string;
  status?: string;
  originCity?: string;
  destinationCity?: string;
}

@Injectable({ providedIn: 'root' })
export class ReportService extends BaseApiService<any> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/reports`);
  }

  getShipmentReport(filters: ReportFilter): Observable<ApiResponse<any[]>> {
    const params = { ...filters };
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/shipments`, { params: params as any });
  }

  exportToCsv(data: any[], fileName: string) {
    if (!data || data.length === 0) return;
    
    const replacer = (key: any, value: any) => value === null ? '' : value;
    const header = Object.keys(data[0]);
    let csv = data.map(row => header.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','));
    csv.unshift(header.join(','));
    let csvArray = csv.join('\r\n');

    const blob = new Blob([csvArray], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.setAttribute('style', 'display:none');
    a.href = url;
    a.download = `${fileName}.csv`;
    document.body.appendChild(a);
    a.click();
    window.URL.revokeObjectURL(url);
  }
}
