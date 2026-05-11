import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseCrudService } from './base-crud.service';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { User } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService extends BaseCrudService<User> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/users`);
  }

  override update(id: string | number, data: any): Observable<ApiResponse<any>> {
    return this.http.patch<ApiResponse<any>>(`${this.apiUrl}/${id}`, data);
  }

  updateRoles(userId: string, roles: string[]): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${userId}/roles`, { roles });
  }

  getAuditLogs(userId: string, page = 1, pageSize = 20): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/${userId}/audit-logs?page=${page}&pageSize=${pageSize}`);
  }
}
