import api from './axiosInstance';
import type { ApiResponse, Center, PaginatedResult, QueryParams } from '@/types';

export const centersApi = {
  getAll: (params?: QueryParams) =>
    api.get<ApiResponse<PaginatedResult<Center>>>('/centers', { params }),

  getById: (id: string) =>
    api.get<ApiResponse<Center>>(`/centers/${id}`),

  create: (data: Omit<Center, 'id' | 'locationCount' | 'motorCount' | 'createdAt'>) =>
    api.post<ApiResponse<Center>>('/centers', data),

  update: (id: string, data: Partial<Center>) =>
    api.put<ApiResponse<Center>>(`/centers/${id}`, data),

  delete: (id: string) =>
    api.delete<ApiResponse>(`/centers/${id}`),
};
