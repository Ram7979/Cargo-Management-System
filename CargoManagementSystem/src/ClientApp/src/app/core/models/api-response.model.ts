export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message: string;
  errors: string[] | null;
  page?: number;
  currentPage?: number;
  pageSize?: number;
  totalCount?: number;
  totalResults?: number;
  totalElements?: number;
  totalPages?: number;
  items?: any[];
  content?: any[];
}
