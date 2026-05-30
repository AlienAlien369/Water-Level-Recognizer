import { useState } from 'react';
import { motion } from 'framer-motion';
import { MapPin, Plus, Pencil, Trash2, Search, ChevronLeft, ChevronRight } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useLocations, useCreateLocation, useUpdateLocation, useDeleteLocation } from '@/hooks/useLocations';
import { useCenters } from '@/hooks/useCenters';
import { UserRole, type Location } from '@/types';
import { useAuthStore } from '@/store/authStore';
import { cn } from '@/lib/utils';

interface LocationFormData {
  centerId: string;
  name: string;
  description?: string;
  floor?: string;
  zone?: string;
}

export function LocationsPage() {
  const { user } = useAuthStore();
  const canManage = user?.role === UserRole.Admin || user?.role === UserRole.SuperAdmin;

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [filterCenterId, setFilterCenterId] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editingLocation, setEditingLocation] = useState<Location | null>(null);
  const [deletingLocation, setDeletingLocation] = useState<Location | null>(null);

  const { data, isLoading } = useLocations({ pageNumber: page, pageSize: 20, search, centerId: filterCenterId || undefined });
  const { data: centersData } = useCenters({ pageNumber: 1, pageSize: 100 });
  const centers = centersData?.items ?? [];

  const createLocation = useCreateLocation();
  const updateLocation = useUpdateLocation();
  const deleteLocation = useDeleteLocation();

  const { register, handleSubmit, reset, formState: { errors } } = useForm<LocationFormData>();

  const openCreate = () => {
    setEditingLocation(null);
    reset({ centerId: filterCenterId || '' });
    setShowModal(true);
  };

  const openEdit = (loc: Location) => {
    setEditingLocation(loc);
    reset({ centerId: loc.centerId, name: loc.name, description: loc.description, floor: loc.floor, zone: loc.zone });
    setShowModal(true);
  };

  const onSubmit = async (data: LocationFormData) => {
    if (editingLocation) {
      await updateLocation.mutateAsync({ id: editingLocation.id, data: { name: data.name, description: data.description, floor: data.floor, zone: data.zone } });
    } else {
      await createLocation.mutateAsync(data);
    }
    setShowModal(false);
    reset({});
  };

  const confirmDelete = async () => {
    if (!deletingLocation) return;
    await deleteLocation.mutateAsync(deletingLocation.id);
    setDeletingLocation(null);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-foreground">Locations</h1>
          <p className="text-muted-foreground text-sm mt-1">Manage all locations within centers</p>
        </div>
        {canManage && (
          <button onClick={openCreate} className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 transition-colors">
            <Plus className="w-4 h-4" /> Add Location
          </button>
        )}
      </div>

      <div className="flex gap-3">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <input
            value={search}
            onChange={e => { setSearch(e.target.value); setPage(1); }}
            placeholder="Search locations..."
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
        <div className="overflow-x-auto">
        <table className="w-full min-w-[640px] text-sm">
          <thead className="bg-muted/50 border-b border-border">
            <tr>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Name</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Center</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Floor / Zone</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Motors</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Active</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Status</th>
              {canManage && <th className="px-4 py-3 text-right font-semibold text-muted-foreground">Actions</th>}
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {isLoading
              ? Array.from({ length: 5 }).map((_, i) => (
                  <tr key={i}>
                    {Array.from({ length: canManage ? 7 : 6 }).map((_, j) => (
                      <td key={j} className="px-4 py-3"><div className="h-4 bg-muted rounded animate-pulse" /></td>
                    ))}
                  </tr>
                ))
              : data?.items?.map(loc => (
                  <motion.tr key={loc.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="hover:bg-muted/30 transition-colors">
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <MapPin className="w-4 h-4 text-purple-500 flex-shrink-0" />
                        <div>
                          <p className="font-medium text-foreground">{loc.name}</p>
                          {loc.description && <p className="text-xs text-muted-foreground truncate max-w-[160px]">{loc.description}</p>}
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">{loc.centerName}</td>
                    <td className="px-4 py-3 text-muted-foreground">{[loc.floor, loc.zone].filter(Boolean).join(' / ') || '—'}</td>
                    <td className="px-4 py-3 text-center">
                      <span className="inline-flex items-center justify-center w-7 h-7 bg-blue-50 text-blue-700 rounded-full text-xs font-bold">{loc.motorCount}</span>
                    </td>
                    <td className="px-4 py-3 text-center">
                      <span className="inline-flex items-center justify-center w-7 h-7 bg-green-50 text-green-700 rounded-full text-xs font-bold">{loc.activeMotorCount}</span>
                    </td>
                    <td className="px-4 py-3 text-center">
                      <span className={cn('text-xs px-2 py-1 rounded-full font-medium', loc.isActive ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-600')}>
                        {loc.isActive ? 'Active' : 'Inactive'}
                      </span>
                    </td>
                    {canManage && (
                      <td className="px-4 py-3">
                        <div className="flex items-center justify-end gap-1">
                          <button onClick={() => openEdit(loc)} className="p-1.5 text-muted-foreground hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                            <Pencil className="w-4 h-4" />
                          </button>
                          <button onClick={() => setDeletingLocation(loc)} className="p-1.5 text-muted-foreground hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      </td>
                    )}
                  </motion.tr>
                ))}
          </tbody>
        </table>
        </div>
        {!isLoading && (!data?.items || data.items.length === 0) && (
          <div className="text-center py-16">
            <MapPin className="w-12 h-12 text-muted-foreground mx-auto mb-3" />
            <p className="text-muted-foreground font-medium">No locations found</p>
          </div>
        )}
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-muted-foreground">Showing {data.items.length} of {data.totalCount} locations</p>
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
              <h2 className="text-lg font-semibold">{editingLocation ? 'Edit Location' : 'Add Location'}</h2>
              <button onClick={() => setShowModal(false)} className="text-muted-foreground hover:text-foreground text-xl font-bold">✕</button>
            </div>
            <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-4">
              {!editingLocation && (
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Center *</label>
                  <select {...register('centerId', { required: 'Center is required' })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                    <option value="">Select center...</option>
                    {centers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                  </select>
                  {errors.centerId && <p className="text-red-500 text-xs mt-1">{errors.centerId.message}</p>}
                </div>
              )}
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Name *</label>
                <input {...register('name', { required: 'Name is required' })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Location name" />
                {errors.name && <p className="text-red-500 text-xs mt-1">{errors.name.message}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Description</label>
                <textarea {...register('description')} rows={2} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Brief description" />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Floor</label>
                  <input {...register('floor')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="e.g. Ground" />
                </div>
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Zone</label>
                  <input {...register('zone')} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="e.g. North Wing" />
                </div>
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setShowModal(false)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted transition-colors">Cancel</button>
                <button type="submit" disabled={createLocation.isPending || updateLocation.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 disabled:opacity-50 transition-colors">
                  {createLocation.isPending || updateLocation.isPending ? 'Saving...' : editingLocation ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </motion.div>
        </div>
      )}

      {deletingLocation && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-sm shadow-xl p-6">
            <h2 className="text-lg font-semibold mb-2">Delete Location</h2>
            <p className="text-muted-foreground text-sm mb-6">Are you sure you want to delete <strong>{deletingLocation.name}</strong>?</p>
            <div className="flex gap-3">
              <button onClick={() => setDeletingLocation(null)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted">Cancel</button>
              <button onClick={confirmDelete} disabled={deleteLocation.isPending} className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg text-sm hover:bg-red-700 disabled:opacity-50">
                {deleteLocation.isPending ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </motion.div>
        </div>
      )}
    </div>
  );
}
