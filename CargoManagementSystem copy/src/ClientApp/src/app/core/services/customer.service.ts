import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseApiService } from './base-api.service';
import { Observable, of } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface Customer {
  id: string;
  customerId?: string;
  customerCode: string;
  firstName?: string;
  lastName?: string;
  companyName: string;
  contactPerson: string;
  email: string;
  phone: string;
  address: string;
  city: string;
  country: string;
  customerType: string;
  creditLimit: number;
  paymentTerms: string;
  isActive: boolean;
  createdAt: string;
}

export interface KycDocument {
  id: string;
  documentType: string;
  blobReference: string;
  status: string;
  uploadedAt: string;
}

@Injectable({ providedIn: 'root' })
export class CustomerService extends BaseApiService<Customer> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/customers`);
  }

  getShipments(customerId: string, page = 1, pageSize = 10): Observable<any> {
    if (!customerId || customerId === 'undefined') return of({ success: false, message: 'Invalid ID', data: { items: [], totalCount: 0 }, errors: null });
    return this.http.get<any>(`${this.apiUrl}/${customerId}/shipments?page=${page}&pageSize=${pageSize}`);
  }

  getDocuments(customerId: string): Observable<ApiResponse<KycDocument[]>> {
    if (!customerId || customerId === 'undefined') return of({ success: false, message: 'Invalid ID', data: [], errors: null });
    return this.http.get<ApiResponse<KycDocument[]>>(`${this.apiUrl}/${customerId}/documents`);
  }

  uploadDocument(customerId: string, doc: { documentType: string, blobReference: string }): Observable<ApiResponse<KycDocument>> {
    if (!customerId || customerId === 'undefined') return of({ success: false, message: 'Invalid customer ID', data: null as any, errors: null });
    return this.http.post<ApiResponse<KycDocument>>(`${this.apiUrl}/${customerId}/documents`, doc);
  }

  deleteDocument(customerId: string, documentId: string): Observable<ApiResponse<any>> {
    if (!customerId || !documentId || customerId === 'undefined') return of({ success: false, message: 'Invalid ID', data: null, errors: null });
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${customerId}/documents/${documentId}`);
  }
}
