import { useQuery } from '@tanstack/react-query';
import { dashboardApi } from '@/api/dashboard.api';

export function useDashboardSummary(centerId?: string) {
  return useQuery({
    queryKey: ['dashboard', 'summary', centerId],
    queryFn: () => dashboardApi.getSummary(centerId),
    select: (r) => r.data.data,
    refetchInterval: 30_000,
  });
}
