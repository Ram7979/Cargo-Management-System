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

    return this.http.get<ApiResponse<any>>(`${this.apiUrl}${queryParams}`).pipe(
      map(res => {
        console.log(`API Raw Response [${this.apiUrl}]:`, res);
        
        let items: T[] = [];
        let totalCount = 0;

        // Global Mapping Logic: Try extracting from known patterns
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

        // Fallback for totalCount
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
        console.error(`API Error in ${this.apiUrl}:`, err);
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
    return this.http.get<ApiResponse<T>>(`${this.apiUrl}/${id}`).pipe(
      map(res => {
        if (res.success) res.data = this.safeObject(res.data);
        return res;
      })
    );
  }

  create(data: any): Observable<ApiResponse<T>> {
    return this.http.post<ApiResponse<T>>(this.apiUrl, data);
  }

  update(id: string | number, data: any): Observable<ApiResponse<T>> {
    return this.http.put<ApiResponse<T>>(`${this.apiUrl}/${id}`, data);
  }

  delete(id: string | number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`);
  }
}
