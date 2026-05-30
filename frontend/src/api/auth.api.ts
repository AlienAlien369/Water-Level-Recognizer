import api from './axiosInstance';
import type { ApiResponse, AuthResponse } from '@/types';

export const authApi = {
  sendOtp: (mobileNumber: string) =>
    api.post<ApiResponse<boolean>>('/auth/send-otp', { mobileNumber }),

  register: (data: { name: string; mobileNumber: string; otpCode: string; email?: string }) =>
    api.post<ApiResponse<AuthResponse>>('/auth/register', data),

  login: (data: { mobileNumber: string; otpCode: string }) =>
    api.post<ApiResponse<AuthResponse>>('/auth/login', data),

  refreshToken: (refreshToken: string) =>
    api.post<ApiResponse<AuthResponse>>('/auth/refresh-token', { refreshToken }),
};
