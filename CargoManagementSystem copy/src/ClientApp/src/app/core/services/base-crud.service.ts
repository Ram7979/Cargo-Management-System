import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export class BaseCrudService<T> {
  constructor(protected http: HttpClient, protected apiUrl: string) {}

  getAll(page = 1, pageSize = 10, filters?: Record<string, any>): Observable<ApiResponse<PagedResult<T>>> {
    let queryParams = `?page=${page}&pageSize=${pageSize}`;
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== null && filters[key] !== '') {
          queryParams += `&${key}=${encodeURIComponent(filters[key])}`;
        }
      });
    }
    return this.http.get<ApiResponse<PagedResult<T>>>(`${this.apiUrl}${queryParams}`);
  }

  getById(id: string | number): Observable<ApiResponse<T>> {
    return this.http.get<ApiResponse<T>>(`${this.apiUrl}/${id}`);
  }

  create(data: any): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(this.apiUrl, data);
  }

  update(id: string | number, data: any): Observable<ApiResponse<T>> {
    return this.http.put<ApiResponse<T>>(`${this.apiUrl}/${id}`, data);
  }

  delete(id: string | number): Observable<ApiResponse<void>> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${id}`);
  }
}
