import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable, map } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface Invoice {
  id: string;
  invoiceNumber: string;
  customerId: string;
  customerName: string;
  customerAddress?: string;
  customerEmail?: string;
  shipmentId: string;
  shipmentTrackingNumber: string;
  totalAmount: number;
  status: string;
  dueDate: string;
  createdAt: string;
  serviceType?: string;
  cargoType?: string;
  weightKg?: number;
  insuranceAmount?: number;
  items?: InvoiceItem[];
}

export interface InvoiceItem {
  description: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

export interface PaymentRequest {
  invoiceId: string;
  amount: number;
  paymentMethod: string;
  transactionReference: string;
}

@Injectable({ providedIn: 'root' })
export class BillingService extends BaseApiService<Invoice> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/invoices`);
  }

  override create(payload: any): Observable<ApiResponse<Invoice>> {
    return this.http.post<ApiResponse<Invoice>>(this.apiUrl, payload);
  }

  processPayment(payment: PaymentRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiUrl}/payments`, payment);
  }

  // The backend /pdf endpoint returns JSON { pdfUrl: "..." } not a byte stream
  getInvoicePdfUrl(invoiceId: string): Observable<ApiResponse<{ pdfUrl: string }>> {
    return this.http.get<ApiResponse<{ pdfUrl: string }>>(`${this.apiUrl}/${invoiceId}/pdf`);
  }

  downloadPdfBlob(url: string): Observable<Blob> {
    return this.http.get(url, { responseType: 'blob' });
  }

  getStatistics(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/stats`);
  }
}
