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

  processPayment(payment: PaymentRequest): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiUrl}/payments`, payment);
  }

  getInvoicePdf(invoiceId: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${invoiceId}/pdf`, { responseType: 'blob' });
  }

  getPdfUrl(invoiceId: string): Observable<ApiResponse<{ pdfUrl: string }>> {
    return this.getInvoicePdf(invoiceId).pipe(
      map(blob => {
        const url = window.URL.createObjectURL(blob);
        return { success: true, message: '', data: { pdfUrl: url }, errors: null };
      })
    );
  }

  getStatistics(): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/stats`);
  }
}
