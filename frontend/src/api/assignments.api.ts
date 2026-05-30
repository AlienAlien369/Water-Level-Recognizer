import api from './axiosInstance';
import type { ApiResponse, Assignment, PaginatedResult, QueryParams } from '@/types';

export const assignmentsApi = {
  getAll: (params?: QueryParams & { centerId?: string; userId?: string }) =>
    api.get<ApiResponse<PaginatedResult<Assignment>>>('/assignments', { params }),

  getById: (id: string) =>
    api.get<ApiResponse<Assignment>>(`/assignments/${id}`),

  create: (data: { userId: string; centerId: string; locationId?: string; motorId?: string; notes?: string }) =>
    api.post<ApiResponse<Assignment>>('/assignments', data),

  revoke: (id: string) =>
    api.put<ApiResponse>(`/assignments/${id}/revoke`),

  delete: (id: string) =>
    api.delete<ApiResponse>(`/assignments/${id}`),
};
