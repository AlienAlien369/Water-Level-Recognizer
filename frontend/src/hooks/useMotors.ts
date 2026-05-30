import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { motorsApi } from '@/api/motors.api';
import type { QueryParams } from '@/types';
import toast from 'react-hot-toast';

export function useMotors(params?: QueryParams & { locationId?: string; minRunningHours?: number }) {
  return useQuery({
    queryKey: ['motors', params],
    queryFn: () => motorsApi.getAll(params),
    select: (r) => r.data.data,
    refetchInterval: 15_000,
  });
}

export function useMotor(id: string) {
  return useQuery({
    queryKey: ['motors', id],
    queryFn: () => motorsApi.getById(id),
    select: (r) => r.data.data,
  });
}

export function useCreateMotor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: motorsApi.create,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['motors'] });
      toast.success('Motor created!');
    },
    onError: () => toast.error('Failed to create motor'),
  });
}

export function useUpdateMotor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: { motorNumber: string; description?: string; waterCapacityLiters: number } }) =>
      motorsApi.update(id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['motors'] });
      toast.success('Motor updated!');
    },
    onError: () => toast.error('Failed to update motor'),
  });
}

export function useDeleteMotor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: motorsApi.delete,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['motors'] });
      toast.success('Motor deleted!');
    },
    onError: () => toast.error('Failed to delete motor'),
  });
}

export function useOpenMotor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, notes }: { id: string; notes?: string }) => motorsApi.open(id, notes),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['motors'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
      toast.success('Motor opened successfully!');
    },
  });
}

export function useCloseMotor() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, notes }: { id: string; notes?: string }) => motorsApi.close(id, notes),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['motors'] });
      qc.invalidateQueries({ queryKey: ['dashboard'] });
      toast.success('Motor closed successfully!');
    },
  });
}

export function useMotorHistory(params?: {
  pageNumber?: number;
  pageSize?: number;
  dateFilter?: string;
  startDate?: string;
  endDate?: string;
  motorId?: string;
  centerId?: string;
  locationId?: string;
  motorSearch?: string;
  minDurationHours?: number;
}) {
  return useQuery({
    queryKey: ['motor-history', params],
    queryFn: () => motorsApi.getHistory(params),
    select: (r) => r.data.data,
  });
}
