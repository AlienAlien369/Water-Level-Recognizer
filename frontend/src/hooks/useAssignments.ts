import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { assignmentsApi } from '@/api/assignments.api';
import type { QueryParams } from '@/types';
import toast from 'react-hot-toast';

export function useAssignments(params?: QueryParams & { centerId?: string; userId?: string }) {
  return useQuery({
    queryKey: ['assignments', params],
    queryFn: () => assignmentsApi.getAll(params),
    select: (r) => r.data.data,
  });
}

export function useAssignment(id: string) {
  return useQuery({
    queryKey: ['assignments', id],
    queryFn: () => assignmentsApi.getById(id),
    select: (r) => r.data.data,
    enabled: !!id,
  });
}

export function useCreateAssignment() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: assignmentsApi.create,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['assignments'] });
      toast.success('Assignment created!');
    },
    onError: () => toast.error('Failed to create assignment'),
  });
}

export function useRevokeAssignment() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: assignmentsApi.revoke,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['assignments'] });
      toast.success('Assignment revoked!');
    },
    onError: () => toast.error('Failed to revoke assignment'),
  });
}

export function useDeleteAssignment() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: assignmentsApi.delete,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['assignments'] });
      toast.success('Assignment deleted!');
    },
    onError: () => toast.error('Failed to delete assignment'),
  });
}
