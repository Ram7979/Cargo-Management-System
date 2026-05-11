import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable } from 'rxjs';

export interface ReportFilter {
  status?:          string;
  originCity?:      string;
  destinationCity?: string;
  pageSize?:        number;
  page?:            number;
}

@Injectable({ providedIn: 'root' })
export class ReportService extends BaseApiService<any> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/reports`);
  }

  /**
   * Fetch the full shipment report.
   * No date filter — returns all records (large page).
   * Backend: GET /api/v1/reports/shipments
   */
  getShipmentReport(filters: ReportFilter = {}): Observable<any> {
    const params: any = {
      page:     filters.page     ?? 1,
      pageSize: filters.pageSize ?? 200,
    };
    if (filters.status          && filters.status !== 'All') params['status']      = filters.status;
    if (filters.originCity)       params['origin']      = filters.originCity;
    if (filters.destinationCity)  params['destination'] = filters.destinationCity;

    return this.http.get<any>(`${this.apiUrl}/shipments`, { params });
  }

  exportToCsv(data: any[], fileName: string) {
    if (!data || data.length === 0) return;
    const replacer = (_: any, value: any) => (value === null ? '' : value);
    const header = Object.keys(data[0]);
    const rows = data.map(row =>
      header.map(field => JSON.stringify(row[field], replacer)).join(',')
    );
    rows.unshift(header.join(','));
    const blob = new Blob([rows.join('\r\n')], { type: 'text/csv' });
    const url  = window.URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = `${fileName}.csv`;
    a.click();
    window.URL.revokeObjectURL(url);
  }
}
