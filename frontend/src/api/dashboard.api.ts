import api from './axiosInstance';
import type { ApiResponse, DashboardSummary } from '@/types';

export const dashboardApi = {
  getSummary: (centerId?: string) =>
    api.get<ApiResponse<DashboardSummary>>('/dashboard/summary', { params: { centerId } }),
};
