import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Zap, Power, PowerOff, Search, Plus, RefreshCw, Pencil, Trash2 } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useMotors, useOpenMotor, useCloseMotor, useCreateMotor, useUpdateMotor, useDeleteMotor } from '@/hooks/useMotors';
import { useLocations } from '@/hooks/useLocations';
import { MotorState, MotorStatus, UserRole, type Motor } from '@/types';
import { cn, getMotorStatusColor, getMotorStatusLabel, formatDate, formatDuration } from '@/lib/utils';
import { useAuthStore } from '@/store/authStore';

interface MotorFormData {
  locationId: string;
  motorNumber: string;
  description?: string;
  waterCapacityLiters: number;
}

export function MotorsPage() {
  const [search, setSearch] = useState('');
  const [locationFilter, setLocationFilter] = useState('');
  const [minHoursInput, setMinHoursInput] = useState('');
  const [appliedMinHours, setAppliedMinHours] = useState<number | undefined>(undefined);
  const [page, setPage] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [editingMotor, setEditingMotor] = useState<Motor | null>(null);
  const [deletingMotor, setDeletingMotor] = useState<Motor | null>(null);
  const { user } = useAuthStore();

  const { data, isLoading, refetch } = useMotors({
    pageNumber: page, pageSize: 20, search,
    locationId: locationFilter || undefined,
    minRunningHours: appliedMinHours,
  });
  const { data: locationsData } = useLocations({ pageNumber: 1, pageSize: 100 });
  const locations = locationsData?.items ?? [];

  const openMotor = useOpenMotor();
  const closeMotor = useCloseMotor();
  const createMotor = useCreateMotor();
  const updateMotor = useUpdateMotor();
  const deleteMotor = useDeleteMotor();

  const canManage = user?.role === UserRole.Admin || user?.role === UserRole.SuperAdmin;

  const { register, handleSubmit, reset, formState: { errors }, setValue } = useForm<MotorFormData>();

  useEffect(() => {
    if (locations.length === 1) setValue('locationId', locations[0].id);
  }, [locations, setValue]);

  const applyHoursFilter = () => {
    const v = parseFloat(minHoursInput);
    setAppliedMinHours(isNaN(v) || v <= 0 ? undefined : v);
    setPage(1);
  };

  const clearHoursFilter = () => {
    setMinHoursInput('');
    setAppliedMinHours(undefined);
    setPage(1);
  };

  const openCreate = () => {
    setEditingMotor(null);
    reset({ waterCapacityLiters: 0 });
    setShowModal(true);
  };

  const openEdit = (m: Motor) => {
    setEditingMotor(m);
    reset({ locationId: m.locationId, motorNumber: m.motorNumber, description: m.description, waterCapacityLiters: m.waterCapacityLiters });
    setShowModal(true);
  };

  const onSubmit = async (data: MotorFormData) => {
    if (editingMotor) {
      await updateMotor.mutateAsync({ id: editingMotor.id, data: { motorNumber: data.motorNumber, description: data.description, waterCapacityLiters: Number(data.waterCapacityLiters) } });
    } else {
      await createMotor.mutateAsync({ locationId: data.locationId, motorNumber: data.motorNumber, description: data.description, waterCapacityLiters: Number(data.waterCapacityLiters) });
    }
    setShowModal(false);
    reset({});
  };

  const confirmDelete = async () => {
    if (!deletingMotor) return;
    await deleteMotor.mutateAsync(deletingMotor.id);
    setDeletingMotor(null);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between gap-3">
        <div>
          <h1 className="text-xl md:text-2xl font-bold text-foreground">Motors</h1>
          <p className="text-muted-foreground text-sm mt-1">Manage and monitor all water motors</p>
        </div>
        <div className="flex gap-2 shrink-0">
          <button onClick={() => refetch()} className="flex items-center gap-1.5 px-3 py-2 border border-border rounded-lg text-sm hover:bg-accent transition-colors">
            <RefreshCw className="w-4 h-4" /> <span className="hidden sm:inline">Refresh</span>
          </button>
          {canManage && (
            <button onClick={openCreate} className="flex items-center gap-1.5 px-3 md:px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 transition-colors">
              <Plus className="w-4 h-4" /> <span className="hidden sm:inline">Add Motor</span><span className="sm:hidden">Add</span>
            </button>
          )}
        </div>
      </div>

      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
        <input
          value={search}
          onChange={e => { setSearch(e.target.value); setPage(1); }}
          placeholder="Search motors by number, location..."
          className="w-full pl-10 pr-4 py-2.5 border border-border rounded-xl bg-background text-foreground placeholder-muted-foreground focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
        />
      </div>

      {/* Filters row */}
      <div className="flex flex-wrap gap-3 items-end">
        {/* Location filter */}
        <div className="flex flex-col gap-1 min-w-[160px]">
          <label className="text-xs font-medium text-muted-foreground">Location</label>
          <select
            value={locationFilter}
            onChange={e => { setLocationFilter(e.target.value); setPage(1); }}
            className="px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All Locations</option>
            {locations.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}
          </select>
        </div>

        {/* Min running hours filter */}
        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium text-muted-foreground">Run more than (hours)</label>
          <div className="flex gap-1.5">
            <input
              type="number"
              min="0"
              step="0.5"
              value={minHoursInput}
              onChange={e => setMinHoursInput(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && applyHoursFilter()}
              placeholder="e.g. 20"
              className="w-28 px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <button
              onClick={applyHoursFilter}
              className="px-3 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 transition-colors"
            >Apply</button>
            {appliedMinHours && (
              <button onClick={clearHoursFilter} className="px-3 py-2 border border-border rounded-lg text-sm hover:bg-accent transition-colors">✕</button>
            )}
          </div>
        </div>

        {/* Active filter chips */}
        {(locationFilter || appliedMinHours) && (
          <div className="flex flex-wrap gap-1.5 items-center self-end pb-0.5">
            {locationFilter && (
              <span className="inline-flex items-center gap-1 text-xs bg-blue-100 text-blue-700 dark:bg-blue-950 dark:text-blue-300 px-2 py-1 rounded-full">
                📍 {locations.find(l => l.id === locationFilter)?.name}
                <button onClick={() => { setLocationFilter(''); setPage(1); }} className="ml-0.5 hover:text-blue-900">✕</button>
              </span>
            )}
            {appliedMinHours && (
              <span className="inline-flex items-center gap-1 text-xs bg-orange-100 text-orange-700 dark:bg-orange-950 dark:text-orange-300 px-2 py-1 rounded-full">
                ⏱ &gt;{appliedMinHours}h run time
                <button onClick={clearHoursFilter} className="ml-0.5 hover:text-orange-900">✕</button>
              </span>
            )}
          </div>
        )}
      </div>

      {isLoading ? (
        <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
          {Array.from({ length: 6 }).map((_, i) => (
            <div key={i} className="h-48 bg-muted rounded-xl animate-pulse" />
          ))}
        </div>
      ) : (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-4">
            {data?.items?.map(motor => (
              <motion.div
                key={motor.id}
                initial={{ opacity: 0, scale: 0.95 }}
                animate={{ opacity: 1, scale: 1 }}
                className="bg-card border border-border rounded-xl p-5 shadow-sm hover:shadow-md transition-all"
              >
                <div className="flex items-start justify-between mb-3">
                  <div className="flex items-center gap-2">
                    <div className={cn(
                      'w-3 h-3 rounded-full',
                      motor.currentState === MotorState.On ? 'bg-green-500 animate-pulse' : 'bg-gray-300'
                    )} />
                    <span className="font-bold text-foreground">#{motor.motorNumber}</span>
                  </div>
                  <div className="flex items-center gap-1">
                    <span className={cn('text-xs px-2 py-0.5 rounded-full font-medium', getMotorStatusColor(motor.status))}>
                      {getMotorStatusLabel(motor.status)}
                    </span>
                    {canManage && (
                      <>
                        <button onClick={() => openEdit(motor)} className="p-1 text-muted-foreground hover:text-blue-600 hover:bg-blue-50 rounded transition-colors">
                          <Pencil className="w-3.5 h-3.5" />
                        </button>
                        <button onClick={() => setDeletingMotor(motor)} className="p-1 text-muted-foreground hover:text-red-600 hover:bg-red-50 rounded transition-colors">
                          <Trash2 className="w-3.5 h-3.5" />
                        </button>
                      </>
                    )}
                  </div>
                </div>

                <div className="space-y-1 mb-4 text-sm text-muted-foreground">
                  <p>Location: {motor.locationName} - {motor.centerName}</p>
                  <p>Capacity: {motor.waterCapacityLiters}L</p>
                  {motor.assignedSewadaarName && <p>Assigned: {motor.assignedSewadaarName}</p>}
                  {motor.lastOpenedAt && <p>Last opened: {formatDate(motor.lastOpenedAt)}</p>}
                  <p>Total run: {formatDuration(motor.totalRunningMinutes)}</p>
                </div>

                <div className="flex gap-2">
                  <button
                    disabled={motor.currentState === MotorState.On || openMotor.isPending || motor.status === MotorStatus.Fault}
                    onClick={() => openMotor.mutate({ id: motor.id })}
                    className="flex-1 flex items-center justify-center gap-1.5 py-2 bg-green-50 hover:bg-green-100 text-green-700 disabled:opacity-40 disabled:cursor-not-allowed rounded-lg text-sm font-medium transition-colors"
                  >
                    <Power className="w-3.5 h-3.5" /> Open
                  </button>
                  <button
                    disabled={motor.currentState === MotorState.Off || closeMotor.isPending}
                    onClick={() => closeMotor.mutate({ id: motor.id })}
                    className="flex-1 flex items-center justify-center gap-1.5 py-2 bg-red-50 hover:bg-red-100 text-red-700 disabled:opacity-40 disabled:cursor-not-allowed rounded-lg text-sm font-medium transition-colors"
                  >
                    <PowerOff className="w-3.5 h-3.5" /> Close
                  </button>
                </div>
              </motion.div>
            ))}
          </div>

          {(!data?.items || data.items.length === 0) && (
            <div className="text-center py-16">
              <Zap className="w-12 h-12 text-muted-foreground mx-auto mb-3" />
              <p className="text-muted-foreground font-medium">No motors found</p>
              <p className="text-muted-foreground text-sm">Try adjusting your search or add new motors.</p>
            </div>
          )}

          {data && data.totalPages > 1 && (
            <div className="flex justify-center gap-2">
              <button disabled={!data.hasPreviousPage} onClick={() => setPage(p => p - 1)} className="px-4 py-2 border border-border rounded-lg text-sm disabled:opacity-40">Previous</button>
              <span className="px-4 py-2 text-sm text-muted-foreground">Page {data.pageNumber} of {data.totalPages}</span>
              <button disabled={!data.hasNextPage} onClick={() => setPage(p => p + 1)} className="px-4 py-2 border border-border rounded-lg text-sm disabled:opacity-40">Next</button>
            </div>
          )}
        </>
      )}

      {showModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-md shadow-xl">
            <div className="flex items-center justify-between p-6 border-b border-border">
              <h2 className="text-lg font-semibold">{editingMotor ? 'Edit Motor' : 'Add Motor'}</h2>
              <button onClick={() => setShowModal(false)} className="text-muted-foreground hover:text-foreground text-xl font-bold">✕</button>
            </div>
            <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-4">
              {!editingMotor && (
                <div>
                  <label className="block text-sm font-medium text-foreground mb-1">Location *</label>
                  <select {...register('locationId', { required: 'Location is required' })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                    <option value="">Select location...</option>
                    {locations.map(l => <option key={l.id} value={l.id}>{l.name} ({l.centerName})</option>)}
                  </select>
                  {errors.locationId && <p className="text-red-500 text-xs mt-1">{errors.locationId.message}</p>}
                </div>
              )}
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Motor Number *</label>
                <input {...register('motorNumber', { required: 'Motor number is required' })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="e.g. M-001" />
                {errors.motorNumber && <p className="text-red-500 text-xs mt-1">{errors.motorNumber.message}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Description</label>
                <textarea {...register('description')} rows={2} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="Brief description" />
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Water Capacity (Liters) *</label>
                <input {...register('waterCapacityLiters', { required: 'Capacity is required', min: { value: 1, message: 'Must be > 0' } })} type="number" step="0.01" className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" placeholder="e.g. 1000" />
                {errors.waterCapacityLiters && <p className="text-red-500 text-xs mt-1">{errors.waterCapacityLiters.message}</p>}
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setShowModal(false)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted">Cancel</button>
                <button type="submit" disabled={createMotor.isPending || updateMotor.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 disabled:opacity-50">
                  {createMotor.isPending || updateMotor.isPending ? 'Saving...' : editingMotor ? 'Update' : 'Create'}
                </button>
              </div>
            </form>
          </motion.div>
        </div>
      )}

      {deletingMotor && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-sm shadow-xl p-6">
            <h2 className="text-lg font-semibold mb-2">Delete Motor</h2>
            <p className="text-muted-foreground text-sm mb-6">Delete motor <strong>#{deletingMotor.motorNumber}</strong>? This cannot be undone.</p>
            <div className="flex gap-3">
              <button onClick={() => setDeletingMotor(null)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted">Cancel</button>
              <button onClick={confirmDelete} disabled={deleteMotor.isPending} className="flex-1 px-4 py-2 bg-red-600 text-white rounded-lg text-sm hover:bg-red-700 disabled:opacity-50">
                {deleteMotor.isPending ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </motion.div>
        </div>
      )}
    </div>
  );
}
