import api from './axiosInstance';
import type { ApiResponse, User, PaginatedResult, QueryParams } from '@/types';

export const usersApi = {
  getAll: (params?: QueryParams & { centerId?: string }) =>
    api.get<ApiResponse<PaginatedResult<User>>>('/users', { params }),

  getById: (id: string) =>
    api.get<ApiResponse<User>>(`/users/${id}`),

  updateProfile: (id: string, data: { name: string; email?: string; profileImageUrl?: string }) =>
    api.put<ApiResponse<User>>(`/users/${id}/profile`, data),

  promote: (id: string, centerId: string) =>
    api.put<ApiResponse<User>>(`/users/${id}/promote`, null, { params: { centerId } }),

  demote: (id: string) =>
    api.put<ApiResponse<User>>(`/users/${id}/demote`),

  activate: (id: string) =>
    api.put<ApiResponse>(`/users/${id}/activate`),

  deactivate: (id: string) =>
    api.put<ApiResponse>(`/users/${id}/deactivate`),
};
