import api from './axiosInstance';
import type { ApiResponse, Location, PaginatedResult, QueryParams } from '@/types';

export const locationsApi = {
  getAll: (params?: QueryParams & { centerId?: string }) =>
    api.get<ApiResponse<PaginatedResult<Location>>>('/locations', { params }),

  getById: (id: string) =>
    api.get<ApiResponse<Location>>(`/locations/${id}`),

  create: (data: { centerId: string; name: string; description?: string; floor?: string; zone?: string }) =>
    api.post<ApiResponse<Location>>('/locations', data),

  update: (id: string, data: { name: string; description?: string; floor?: string; zone?: string }) =>
    api.put<ApiResponse<Location>>(`/locations/${id}`, data),

  delete: (id: string) =>
    api.delete<ApiResponse>(`/locations/${id}`),
};
