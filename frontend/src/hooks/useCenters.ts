import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { centersApi } from '@/api/centers.api';
import type { QueryParams } from '@/types';
import toast from 'react-hot-toast';

export function useCenters(params?: QueryParams) {
  return useQuery({
    queryKey: ['centers', params],
    queryFn: () => centersApi.getAll(params),
    select: (r) => r.data.data,
  });
}

export function useCenter(id: string) {
  return useQuery({
    queryKey: ['centers', id],
    queryFn: () => centersApi.getById(id),
    select: (r) => r.data.data,
    enabled: !!id,
  });
}

export function useCreateCenter() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: centersApi.create,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['centers'] });
      toast.success('Center created!');
    },
    onError: () => toast.error('Failed to create center'),
  });
}

export function useUpdateCenter() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Parameters<typeof centersApi.update>[1] }) =>
      centersApi.update(id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['centers'] });
      toast.success('Center updated!');
    },
    onError: () => toast.error('Failed to update center'),
  });
}

export function useDeleteCenter() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: centersApi.delete,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['centers'] });
      toast.success('Center deleted!');
    },
    onError: () => toast.error('Failed to delete center'),
  });
}
