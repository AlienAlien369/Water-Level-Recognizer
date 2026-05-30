import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { locationsApi } from '@/api/locations.api';
import type { QueryParams } from '@/types';
import toast from 'react-hot-toast';

export function useLocations(params?: QueryParams & { centerId?: string }) {
  return useQuery({
    queryKey: ['locations', params],
    queryFn: () => locationsApi.getAll(params),
    select: (r) => r.data.data,
  });
}

export function useLocation(id: string) {
  return useQuery({
    queryKey: ['locations', id],
    queryFn: () => locationsApi.getById(id),
    select: (r) => r.data.data,
    enabled: !!id,
  });
}

export function useCreateLocation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: locationsApi.create,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['locations'] });
      toast.success('Location created!');
    },
    onError: () => toast.error('Failed to create location'),
  });
}

export function useUpdateLocation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: Parameters<typeof locationsApi.update>[1] }) =>
      locationsApi.update(id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['locations'] });
      toast.success('Location updated!');
    },
    onError: () => toast.error('Failed to update location'),
  });
}

export function useDeleteLocation() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: locationsApi.delete,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['locations'] });
      toast.success('Location deleted!');
    },
    onError: () => toast.error('Failed to delete location'),
  });
}
