import api from './axiosInstance';
import type { ApiResponse, Motor, MotorLog, PaginatedResult, QueryParams } from '@/types';

export const motorsApi = {
  getAll: (params?: QueryParams & { locationId?: string }) =>
    api.get<ApiResponse<PaginatedResult<Motor>>>('/motors', { params }),

  getById: (id: string) =>
    api.get<ApiResponse<Motor>>(`/motors/${id}`),

  create: (data: { locationId: string; motorNumber: string; description?: string; waterCapacityLiters: number }) =>
    api.post<ApiResponse<Motor>>('/motors', data),

  update: (id: string, data: { motorNumber: string; description?: string; waterCapacityLiters: number }) =>
    api.put<ApiResponse<Motor>>(`/motors/${id}`, data),

  open: (id: string, notes?: string) =>
    api.post<ApiResponse<MotorLog>>(`/motors/${id}/open`, { notes }),

  close: (id: string, notes?: string) =>
    api.post<ApiResponse<MotorLog>>(`/motors/${id}/close`, { notes }),

  getLogs: (id: string, params?: QueryParams) =>
    api.get<ApiResponse<PaginatedResult<MotorLog>>>(`/motors/${id}/logs`, { params }),

  delete: (id: string) =>
    api.delete<ApiResponse>(`/motors/${id}`),
};
