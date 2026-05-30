import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { usersApi } from '@/api/users.api';
import type { QueryParams } from '@/types';
import toast from 'react-hot-toast';

export function useUsers(params?: QueryParams & { centerId?: string }) {
  return useQuery({
    queryKey: ['users', params],
    queryFn: () => usersApi.getAll(params),
    select: (r) => r.data.data,
  });
}

export function useUser(id: string) {
  return useQuery({
    queryKey: ['users', id],
    queryFn: () => usersApi.getById(id),
    select: (r) => r.data.data,
    enabled: !!id,
  });
}

export function useUpdateUserProfile() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: { name: string; email?: string; profileImageUrl?: string } }) =>
      usersApi.updateProfile(id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['users'] });
      toast.success('Profile updated!');
    },
    onError: () => toast.error('Failed to update profile'),
  });
}

export function usePromoteUser() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, centerId }: { id: string; centerId: string }) => usersApi.promote(id, centerId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['users'] });
      toast.success('User promoted to Admin!');
    },
    onError: () => toast.error('Failed to promote user'),
  });
}

export function useDemoteUser() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => usersApi.demote(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['users'] });
      toast.success('User demoted!');
    },
    onError: () => toast.error('Failed to demote user'),
  });
}

export function useToggleUserActive() {
  const qc = useQueryClient();
  return useMutation({
    mutationFn: ({ id, activate }: { id: string; activate: boolean }) =>
      activate ? usersApi.activate(id) : usersApi.deactivate(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['users'] });
      toast.success('User status updated!');
    },
    onError: () => toast.error('Failed to update user status'),
  });
}
