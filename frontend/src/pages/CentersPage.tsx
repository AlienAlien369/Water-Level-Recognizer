import { useState } from 'react';
import { motion } from 'framer-motion';
import { Building2, Plus, Pencil, Trash2, Search, ChevronLeft, ChevronRight } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useCenters, useCreateCenter, useUpdateCenter, useDeleteCenter } from '@/hooks/useCenters';
import { UserRole, type Center } from '@/types';
import { useAuthStore } from '@/store/authStore';
import { cn } from '@/lib/utils';

interface CenterFormData {
  name: string;
  description?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  contactPhone?: string;
  contactEmail?: string;
}

export function CentersPage() {
  const { user } = useAuthStore();
  const isSuperAdmin = user?.role === UserRole.SuperAdmin;

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingCenter, setEditingCenter] = useState<Center | null>(null);
  const [deletingCenter, setDeletingCenter] = useState<Center | null>(null);

  const { data, isLoading } = useCenters({ pageNumber: page, pageSize: 20, search });
  const createCenter = useCreateCenter();
  const updateCenter = useUpdateCenter();
  const deleteCenter = useDeleteCenter();

  const { register, handleSubmit, reset, formState: { errors } } = useForm<CenterFormData>();

  const openCreate = () => {
    setEditingCenter(null);
    reset({});
    setShowModal(true);
  };

  const openEdit = (center: Center) => {
    setEditingCenter(center);
    reset({
      name: center.name,
      description: center.description,
      address: center.address,
      city: center.city,
      state: center.state,
      country: center.country,
      contactPhone: center.contactPhone,
      contactEmail: center.contactEmail,
    });
    setShowModal(true);
  };

  const onSubmit = async (data: CenterFormData) => {
    if (editingCenter) {
      await updateCenter.mutateAsync({ id: editingCenter.id, data });
    } else {
      await createCenter.mutateAsync(data as any);
    }
    setShowModal(false);
    reset({});
  };

  const confirmDelete = async () => {
    if (!deletingCenter) return;
    await deleteCenter.mutateAsync(deletingCenter.id);
    setDeletingCenter(null);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Centers</h1>
          <p className="text-muted-foreground text-sm mt-1">Manage all operational centers</p>
        </div>
        {isSuperAdmin && (
          <button onClick={openCreate} className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 transition-colors">
            <Plus className="w-4 h-4" /> Add Center
          </button>
        )}
      </div>

      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <input
          value={search}
          onChange={e => { setSearch(e.target.value); setPage(1); }}
          placeholder="Search centers by name or city..."
          className="w-full pl-10 pr-4 py-2.5 border border-border rounded-xl bg-background text-foreground placeholder-muted-foreground focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
        />
      </div>

      <div className="bg-card border border-border rounded-xl overflow-hidden shadow-sm">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 border-b border-border">
            <tr>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Name</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Location</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Locations</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Motors</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Status</th>
              {isSuperAdmin && <th className="px-4 py-3 text-right font-semibold text-muted-foreground">Actions</th>}
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {isLoading
              ? Array.from({ length: 5 }).map((_, i) => (
                  <tr key={i}>
                    {Array.from({ length: isSuperAdmin ? 6 : 5 }).map((_, j) => (
                      <td key={j} className="px-4 py-3">
                        <div className="h-4 bg-muted rounded animate-pulse" />
                      </td>
                    ))}
                  </tr>
                ))
              : data?.items?.map(center => (
                  <motion.tr
                    key={center.id}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    className="hover:bg-muted/30 transition-colors"
                  >
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <Building2 className="w-4 h-4 text-blue-500 flex-shrink-0" />
                        <div>
                          <p className="font-medium text-foreground">{center.name}</p>
                          {center.description && <p className="text-xs text-muted-foreground truncate max-w-[200px]">{center.description}</p>}
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">
                      {[center.city, center.state, center.country].filter(Boolean).join(', ') || '—'}
                    </td>
                    <td className="px-4 py-3 text-center">
                      <span className="inline-flex items-center justify-center w-8 h-8 bg-blue-50 text-blue-700 rounded-full text-xs font-bold">
                        {center.locationCount}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-center">
                      <span className="inline-flex items-center justify-center w-8 h-8 bg-green-50 text-green-700 rounded-full text-xs font-bold">
                        {center.motorCount}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-center">
                      <span className={cn('text-xs px-2 py-1 rounded-full font-medium', center.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-600')}>
                        {center.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    {isSuperAdmin && (
                      <td className="px-4 py-3">
                        <div className="flex items-center justify-end gap-1">
                          <button onClick={() => openEdit(center)} className="p-1.5 text-muted-foreground hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                            <Pencil className="w-4 h-4" />
                          </button>
                          <button onClick={() => setDeletingCenter(center)} className="p-1.5 text-muted-foreground hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                            <Trash2 className="w-4 h-4" />
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
            <Building2 className="w-12 h-12 text-muted-foreground mx-auto mb-3" />
            <p className="text-muted-foreground font-medium">No centers found</p>
            <p className="text-muted-foreground text-sm">Try adjusting your search or add a new center.</p>
          </div>
        )}
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-muted-foreground">Showing {data.items.length} of {data.totalCount} centers</p>
          <div className="flex gap-2">
            <button disabled={!data.hasPreviousPage} onClick={() => setPage(p => p - 1)} className="p-2 border border-border rounded-lg disabled:opacity-40 hover:bg-muted transition-colors">
              <ChevronLeft className="w-4 h-4" />
            </button>
            <span className="px-3 py-2 text-sm text-muted-foreground">Page {data.pageNumber} of {data.totalPages}</span>
            <button disabled={!data.hasNextPage} onClick={() => setPage(p => p + 1)} className="p-2 border border-border rounded-lg disabled:opacity-40 hover:bg-muted transition-colors">
              <ChevronRight className="w-4 h-4" />
            </button>
          </div>
        </div>
      )}

      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div
            initial={{ opacity: 0, scale: 0.95 }}
            animate={{ opacity: 1, scale: 1 }}
            className="bg-card border border-border rounded-2xl w-full max-w-lg shadow-xl"
          >
            <div className="flex items-center justify-between p-6 border-b border-border">
              <h2 className="text-lg font-semibold">{editingCenter ? 'Edit Center' : 'Add Center'}</h2>
              <button onClick={() => setShowModal(false)} className="text-muted-foreground hover:text-foreground text-xl font-bold">✕</button>
            </div>
            <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Name *</label>
                <input
                  {...register('name', { required: 'Name is required' })}
                  className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Center name"
                />
                {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name.message}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Description</label>
                <textarea
                  {...register('description')}
                  rows={2}
                  className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                  placeholder="Brief description"
                />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Address</label>
                  <input {...register('address')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Street address" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">City</label>
                  <input {...register('city')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="City" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">State</label>
                  <input {...register('state')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="State" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Country</label>
                  <input {...register('country')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Country" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Contact Phone</label>
                  <input {...register('contactPhone')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="+91 9999999999" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Contact Email</label>
                  <input {...register('contactEmail')} type="email" className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="contact@center.org" />
                </div>
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setShowModal(false)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted transition-colors">Cancel</button>
                <button
                  type="submit"
                  disabled={createCenter.isPending || updateCenter.isPending}
                  className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 disabled:opacity-50 transition-colors"
                >
                  {createCenter.isPending || updateCenter.isPending ? 'Saving...' : editingCenter ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </motion.div>
        </div>
      )}

      {deletingCenter && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-sm shadow-xl p-6">
            <h2 className="text-lg font-semibold mb-2">Delete Center</h2>
            <p className="text-muted-foreground text-sm mb-6">Are you sure you want to delete <strong>{deletingCenter.name}</strong>? This action cannot be undone.</p>
            <div className="flex gap-3">
              <button onClick={() => setDeletingCenter(null)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted transition-colors">Cancel</button>
              <button onClick={confirmDelete} disabled={deleteCenter.isPending} className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg text-sm hover:bg-red-700 disabled:opacity-50 transition-colors">
                {deleteCenter.isPending ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </motion.div>
        </div>
      )}
    </div>
  );
}
