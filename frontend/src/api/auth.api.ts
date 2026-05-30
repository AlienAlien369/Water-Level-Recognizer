import api from './axiosInstance';
import type { ApiResponse, AuthResponse } from '@/types';

export const authApi = {
  register: (data: { name: string; mobileNumber: string; password: string; email?: string; centerId?: string }) =>
    api.post<ApiResponse<AuthResponse>>('/auth/register', data),

  login: (data: { mobileNumber: string; password: string }) =>
    api.post<ApiResponse<AuthResponse>>('/auth/login', data),

  refreshToken: (refreshToken: string) =>
    api.post<ApiResponse<AuthResponse>>('/auth/refresh-token', { refreshToken }),
};
