import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { ApiResponse } from '../models/api-response.model';

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export abstract class BaseApiService<T> {
  constructor(protected http: HttpClient, protected apiUrl: string) {}

  protected safeArray<U>(arr: U[] | null | undefined): U[] {
    return Array.isArray(arr) ? arr : [];
  }

  protected safeObject<U>(obj: U | null | undefined): U | any {
    return obj || {};
  }

  getAll(page = 1, pageSize = 10, filters?: Record<string, any>): Observable<ApiResponse<PagedResult<T>>> {
    let queryParams = `?page=${page}&pageSize=${pageSize}`;
    if (filters) {
      Object.keys(filters).forEach(key => {
        const val = filters[key];
        if (val !== null && val !== undefined && val !== '') {
          queryParams += `&${key}=${encodeURIComponent(val)}`;
        }
      });
    }

    console.log(`[API Request] GET ${this.apiUrl}${queryParams}`);
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}${queryParams}`).pipe(
      map(res => {
        console.log(`[API Response] ${this.apiUrl}:`, res);
        
        let items: T[] = [];
        let totalCount = 0;

        const data = res?.data || res;
        
        if (Array.isArray(data)) {
          items = data;
        } else if (data?.items && Array.isArray(data.items)) {
          items = data.items;
          totalCount = data.totalCount || data.totalResults || 0;
        } else if (data?.content && Array.isArray(data.content)) {
          items = data.content;
          totalCount = data.totalElements || data.totalCount || 0;
        } else if (data?.data && Array.isArray(data.data)) {
          items = data.data;
        }

        totalCount = totalCount || res?.totalCount || res?.totalResults || items.length;

        const normalizedData: PagedResult<T> = {
          items: this.safeArray(items),
          totalCount: totalCount,
          page: res?.page || res?.currentPage || page,
          pageSize: res?.pageSize || pageSize
        };

        return {
          success: res?.success !== false,
          message: res?.message || '',
          data: normalizedData,
          totalCount: totalCount,
          page: normalizedData.page,
          pageSize: normalizedData.pageSize
        } as ApiResponse<PagedResult<T>>;
      }),
      catchError(err => {
        console.error(`[API Error] ${this.apiUrl}:`, err);
        return of({ 
          success: false, 
          message: 'API connection failed', 
          data: { items: [], totalCount: 0, page, pageSize },
          errors: [err.message]
        } as ApiResponse<PagedResult<T>>);
      })
    );
  }

  getById(id: string | number): Observable<ApiResponse<T>> {
    if (!id || id === 'undefined' || id === 'null') {
      console.warn(`[API Guard] Prevented GET request with invalid ID: ${id}`);
      return of({ success: false, message: 'Invalid ID provided', data: null as any, errors: null } as ApiResponse<T>);
    }
    console.log(`[API Request] GET ${this.apiUrl}/${id}`);
    return this.http.get<ApiResponse<T>>(`${this.apiUrl}/${id}`).pipe(
      map(res => {
        console.log(`[API Response] ${this.apiUrl}/${id}:`, res);
        if (res.success) res.data = this.safeObject(res.data);
        return res;
      }),
      catchError(err => {
        console.error(`[API Error] ${this.apiUrl}/${id}:`, err);
        throw err;
      })
    );
  }

  create(data: any): Observable<ApiResponse<T>> {
    if (!data) return of({ success: false, message: 'No data provided', data: null as any, errors: null } as ApiResponse<T>);
    console.log(`[API Request] POST ${this.apiUrl}`, data);
    return this.http.post<ApiResponse<T>>(this.apiUrl, data).pipe(
      map(res => { console.log(`[API Response] POST ${this.apiUrl}:`, res); return res; }),
      catchError(err => { console.error(`[API Error] POST ${this.apiUrl}:`, err); throw err; })
    );
  }

  update(id: string | number, data: any): Observable<ApiResponse<T>> {
    if (!id || id === 'undefined' || !data) {
      console.warn(`[API Guard] Prevented PUT request with invalid ID or data`);
      return of({ success: false, message: 'Invalid ID or data', data: null as any, errors: null } as ApiResponse<T>);
    }
    console.log(`[API Request] PUT ${this.apiUrl}/${id}`, data);
    return this.http.put<ApiResponse<T>>(`${this.apiUrl}/${id}`, data).pipe(
      map(res => { console.log(`[API Response] PUT ${this.apiUrl}/${id}:`, res); return res; }),
      catchError(err => { console.error(`[API Error] PUT ${this.apiUrl}/${id}:`, err); throw err; })
    );
  }

  patch(id: string | number, data: any): Observable<ApiResponse<any>> {
    if (!id || id === 'undefined' || !data) {
      console.warn(`[API Guard] Prevented PATCH request with invalid ID or data`);
      return of({ success: false, message: 'Invalid ID or data', data: null, errors: null } as ApiResponse<any>);
    }
    console.log(`[API Request] PATCH ${this.apiUrl}/${id}`, data);
    return this.http.patch<ApiResponse<any>>(`${this.apiUrl}/${id}`, data).pipe(
      map(res => { console.log(`[API Response] PATCH ${this.apiUrl}/${id}:`, res); return res; }),
      catchError(err => { console.error(`[API Error] PATCH ${this.apiUrl}/${id}:`, err); throw err; })
    );
  }

  delete(id: string | number): Observable<ApiResponse<any>> {
    if (!id || id === 'undefined') {
      console.warn(`[API Guard] Prevented DELETE request with invalid ID`);
      return of({ success: false, message: 'Invalid ID', data: null, errors: null } as ApiResponse<any>);
    }
    console.log(`[API Request] DELETE ${this.apiUrl}/${id}`);
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`).pipe(
      map(res => { console.log(`[API Response] DELETE ${this.apiUrl}/${id}:`, res); return res; }),
      catchError(err => { console.error(`[API Error] DELETE ${this.apiUrl}/${id}:`, err); throw err; })
    );
  }
}
