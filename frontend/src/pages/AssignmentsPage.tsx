import { useState } from 'react';
import { motion } from 'framer-motion';
import { Link, Plus, Search, ChevronLeft, ChevronRight, Trash2, XCircle } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useAssignments, useCreateAssignment, useRevokeAssignment, useDeleteAssignment } from '@/hooks/useAssignments';
import { useUsers } from '@/hooks/useUsers';
import { useCenters } from '@/hooks/useCenters';
import { useLocations } from '@/hooks/useLocations';
import { useMotors } from '@/hooks/useMotors';
import { AssignmentStatus, UserRole, type Assignment } from '@/types';
import { useAuthStore } from '@/store/authStore';
import { cn, formatDate } from '@/lib/utils';

const statusBadge = (status: AssignmentStatus) => {
  if (status === AssignmentStatus.Active) return 'bg-green-100 text-green-800';
  if (status === AssignmentStatus.Revoked) return 'bg-red-100 text-red-700';
  return 'bg-gray-100 text-gray-600';
};

const statusLabel = (status: AssignmentStatus) => {
  if (status === AssignmentStatus.Active) return 'Active';
  if (status === AssignmentStatus.Revoked) return 'Revoked';
  return 'Inactive';
};

interface AssignmentFormData {
  userId: string;
  centerId: string;
  locationId?: string;
  motorId?: string;
  notes?: string;
}

export function AssignmentsPage() {
  const { user } = useAuthStore();
  const canManage = user?.role === UserRole.Admin || user?.role === UserRole.SuperAdmin;

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [filterCenterId, setFilterCenterId] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [deletingAssignment, setDeletingAssignment] = useState<Assignment | null>(null);

  const { data, isLoading } = useAssignments({ pageNumber: page, pageSize: 20, search, centerId: filterCenterId || undefined });
  const { data: centersData } = useCenters({ pageNumber: 1, pageSize: 100 });
  const centers = centersData?.items ?? [];

  const createAssignment = useCreateAssignment();
  const revokeAssignment = useRevokeAssignment();
  const deleteAssignment = useDeleteAssignment();

  const { register, handleSubmit, reset, watch, formState: { errors } } = useForm<AssignmentFormData>();
  const watchedCenterId = watch('centerId');

  const { data: usersData } = useUsers({ pageNumber: 1, pageSize: 100 });
  const { data: locationsData } = useLocations({ pageNumber: 1, pageSize: 100, centerId: watchedCenterId || undefined });
  const { data: motorsData } = useMotors({ pageNumber: 1, pageSize: 100, locationId: watch('locationId') || undefined });

  const onSubmit = async (data: AssignmentFormData) => {
    await createAssignment.mutateAsync({
      userId: data.userId,
      centerId: data.centerId,
      locationId: data.locationId || undefined,
      motorId: data.motorId || undefined,
      notes: data.notes,
    });
    setShowModal(false);
    reset({});
  };

  const confirmDelete = async () => {
    if (!deletingAssignment) return;
    await deleteAssignment.mutateAsync(deletingAssignment.id);
    setDeletingAssignment(null);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Assignments</h1>
          <p className="text-muted-foreground text-sm mt-1">Manage motor and location assignments</p>
        </div>
        {canManage && (
          <button onClick={() => { reset({}); setShowModal(true); }} className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 transition-colors">
            <Plus className="w-4 h-4" /> Add Assignment
          </button>
        )}
      </div>

      <div className="flex gap-3">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <input
            value={search}
            onChange={e => { setSearch(e.target.value); setPage(1); }}
            placeholder="Search by user name or mobile..."
            className="w-full pl-10 pr-4 py-2.5 border border-border rounded-xl bg-background text-foreground placeholder-muted-foreground focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
          />
        </div>
        <select
          value={filterCenterId}
          onChange={e => { setFilterCenterId(e.target.value); setPage(1); }}
          className="px-3 py-2.5 border border-border rounded-xl bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option value="">All Centers</option>
          {centers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
      </div>

      <div className="bg-card border border-border rounded-xl overflow-hidden shadow-sm">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 border-b border-border">
            <tr>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">User</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Center</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Location</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Motor</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Status</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Assigned At</th>
              {canManage && <th className="px-4 py-3 text-right font-semibold text-muted-foreground">Actions</th>}
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {isLoading
              ? Array.from({ length: 5 }).map((_, i) => (
                  <tr key={i}>{Array.from({ length: canManage ? 7 : 6 }).map((_, j) => <td key={j} className="px-4 py-3"><div className="h-4 bg-muted rounded animate-pulse" /></td>)}</tr>
                ))
              : data?.items?.map(a => (
                  <motion.tr key={a.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="hover:bg-muted/30 transition-colors">
                    <td className="px-4 py-3">
                      <div>
                        <p className="font-medium text-foreground">{a.userName}</p>
                        <p className="text-xs text-muted-foreground">{a.userMobile}</p>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">{a.centerName}</td>
                    <td className="px-4 py-3 text-muted-foreground">{a.locationName || '—'}</td>
                    <td className="px-4 py-3 text-muted-foreground">{a.motorNumber ? `#${a.motorNumber}` : '—'}</td>
                    <td className="px-4 py-3 text-center">
                      <span className={cn('text-xs px-2 py-1 rounded-full font-medium', statusBadge(a.status))}>
                        {statusLabel(a.status)}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground text-xs">{formatDate(a.assignedAt)}</td>
                    {canManage && (
                      <td className="px-4 py-3">
                        <div className="flex items-center justify-end gap-1">
                          {a.status === AssignmentStatus.Active && (
                            <button
                              title="Revoke assignment"
                              onClick={() => revokeAssignment.mutate(a.id)}
                              className="p-1.5 text-muted-foreground hover:text-orange-600 hover:bg-orange-50 rounded-lg transition-colors"
                            >
                              <XCircle className="w-3.5 h-3.5" />
                            </button>
                          )}
                          <button
                            title="Delete assignment"
                            onClick={() => setDeletingAssignment(a)}
                            className="p-1.5 text-muted-foreground hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                          </button>
                        </div>
                      </td>
                    )}
                  </motion.tr>
                ))}
          </tbody>
        </table>
        {!isLoading && (!data?.items || data.items.length === 0) && (
          <div className="text-center py-16">
            <Link className="w-12 h-12 text-muted-foreground mx-auto mb-3" />
            <p className="text-muted-foreground font-medium">No assignments found</p>
          </div>
        )}
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-muted-foreground">Showing {data.items.length} of {data.totalCount} assignments</p>
          <div className="flex gap-2">
            <button disabled={!data.hasPreviousPage} onClick={() => setPage(p => p - 1)} className="p-2 border border-border rounded-lg disabled:opacity-40 hover:bg-muted"><ChevronLeft className="w-4 h-4" /></button>
            <span className="px-3 py-2 text-sm text-muted-foreground">Page {data.pageNumber} of {data.totalPages}</span>
            <button disabled={!data.hasNextPage} onClick={() => setPage(p => p + 1)} className="p-2 border border-border rounded-lg disabled:opacity-40 hover:bg-muted"><ChevronRight className="w-4 h-4" /></button>
          </div>
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-md shadow-xl">
            <div className="flex items-center justify-between p-6 border-b border-border">
              <h2 className="text-lg font-semibold">Add Assignment</h2>
              <button onClick={() => setShowModal(false)} className="text-muted-foreground hover:text-foreground text-xl font-bold">✕</button>
            </div>
            <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">User *</label>
                <select {...register('userId', { required: 'User is required' })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                  <option value="">Select user...</option>
                  {usersData?.items?.map(u => <option key={u.id} value={u.id}>{u.name} ({u.mobileNumber})</option>)}
                </select>
                {errors.userId && <p className="text-red-500 text-xs mt-1">{errors.userId.message}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Center *</label>
                <select {...register('centerId', { required: 'Center is required' })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                  <option value="">Select center...</option>
                  {centers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
                {errors.centerId && <p className="text-red-500 text-xs mt-1">{errors.centerId.message}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Location (optional)</label>
                <select {...register('locationId')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" disabled={!watchedCenterId}>
                  <option value="">Select location...</option>
                  {locationsData?.items?.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Motor (optional)</label>
                <select {...register('motorId')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" disabled={!watch('locationId')}>
                  <option value="">Select motor...</option>
                  {motorsData?.items?.map(m => <option key={m.id} value={m.id}>#{m.motorNumber}</option>)}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Notes</label>
                <textarea {...register('notes')} rows={2} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Optional notes" />
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setShowModal(false)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted">Cancel</button>
                <button type="submit" disabled={createAssignment.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 disabled:opacity-50">
                  {createAssignment.isPending ? 'Creating...' : 'Create'}
                </button>
              </div>
            </form>
          </motion.div>
        </div>
      )}

      {deletingAssignment && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-sm shadow-xl p-6">
            <h2 className="text-lg font-semibold mb-2">Delete Assignment</h2>
            <p className="text-muted-foreground text-sm mb-6">Delete the assignment for <strong>{deletingAssignment.userName}</strong>?</p>
            <div className="flex gap-3">
              <button onClick={() => setDeletingAssignment(null)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted">Cancel</button>
              <button onClick={confirmDelete} disabled={deleteAssignment.isPending} className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg text-sm hover:bg-red-700 disabled:opacity-50">
                {deleteAssignment.isPending ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </motion.div>
        </div>
      )}
    </div>
  );
}
