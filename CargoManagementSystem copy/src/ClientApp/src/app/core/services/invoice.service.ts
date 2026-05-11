import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/invoices`;

  getMyInvoices(page = 1, pageSize = 20, status?: string): Observable<any> {
    let url = `${this.apiUrl}/my?page=${page}&pageSize=${pageSize}`;
    if (status) url += `&status=${status}`;
    return this.http.get<any>(url);
  }

  getInvoicePdf(invoiceId: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${invoiceId}/pdf`);
  }

  downloadPdfBlob(url: string): Observable<Blob> {
    return this.http.get(url, { responseType: 'blob' });
  }

  payInvoice(invoiceId: string, amount: number, method: string = 'Card'): Observable<any> {
    const payload = {
      invoiceId,
      amount,
      method,
      referenceNumber: `CARD-${Math.floor(Math.random() * 1000000)}`,
      paymentDate: new Date().toISOString(),
      forceRecord: true
    };
    return this.http.post<any>(`${environment.apiUrl}/payments`, payload);
  }
}
