import { useState } from 'react';
import { motion } from 'framer-motion';
import { Users, Search, ChevronLeft, ChevronRight, Pencil, UserCheck, UserX, ShieldCheck, ShieldOff } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useUsers, useUpdateUserProfile, usePromoteUser, useDemoteUser, useToggleUserActive } from '@/hooks/useUsers';
import { useCenters } from '@/hooks/useCenters';
import { UserRole, type User } from '@/types';
import { useAuthStore } from '@/store/authStore';
import { cn, formatDate } from '@/lib/utils';

const roleBadge = (role: UserRole) => {
  if (role === UserRole.SuperAdmin) return 'bg-purple-100 text-purple-800';
  if (role === UserRole.Admin) return 'bg-blue-100 text-blue-800';
  return 'bg-green-100 text-green-800';
};

const roleLabel = (role: UserRole) => {
  if (role === UserRole.SuperAdmin) return 'Super Admin';
  if (role === UserRole.Admin) return 'Admin';
  return 'Sewadar';
};

export function UsersPage() {
  const { user: currentUser } = useAuthStore();
  const isSuperAdmin = currentUser?.role === UserRole.SuperAdmin;
  const isAdmin = currentUser?.role === UserRole.Admin || isSuperAdmin;

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [filterCenterId, setFilterCenterId] = useState('');
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [promotingUser, setPromotingUser] = useState<User | null>(null);

  const { data, isLoading } = useUsers({ pageNumber: page, pageSize: 20, search, centerId: filterCenterId || undefined });
  const { data: centersData } = useCenters({ pageNumber: 1, pageSize: 100 });
  const centers = centersData?.items ?? [];

  const updateProfile = useUpdateUserProfile();
  const promote = usePromoteUser();
  const demote = useDemoteUser();
  const toggleActive = useToggleUserActive();

  const profileForm = useForm<{ name: string; email?: string }>({});
  const promoteForm = useForm<{ centerId: string }>({});

  const openEdit = (u: User) => {
    setEditingUser(u);
    profileForm.reset({ name: u.name, email: u.email });
  };

  const onProfileSubmit = async (data: { name: string; email?: string }) => {
    if (!editingUser) return;
    await updateProfile.mutateAsync({ id: editingUser.id, data });
    setEditingUser(null);
  };

  const openPromote = (u: User) => {
    setPromotingUser(u);
    promoteForm.reset({ centerId: '' });
  };

  const onPromoteSubmit = async (data: { centerId: string }) => {
    if (!promotingUser) return;
    await promote.mutateAsync({ id: promotingUser.id, centerId: data.centerId });
    setPromotingUser(null);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-start justify-between gap-3">
        <div>
          <h1 className="text-xl md:text-2xl font-bold text-foreground">Users</h1>
          <p className="text-muted-foreground text-sm mt-1">Manage users and their roles</p>
        </div>
      </div>

      <div className="flex flex-col sm:flex-row gap-3">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground" />
          <input
            value={search}
            onChange={e => { setSearch(e.target.value); setPage(1); }}
            placeholder="Search by name or mobile..."
            className="w-full pl-10 pr-4 py-2.5 border border-border rounded-xl bg-background text-foreground placeholder-muted-foreground focus:outline-none focus:ring-2 focus:ring-blue-500 text-sm"
          />
        </div>
        {isAdmin && (
          <select
            value={filterCenterId}
            onChange={e => { setFilterCenterId(e.target.value); setPage(1); }}
            className="px-3 py-2.5 border border-border rounded-xl bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All Centers</option>
            {centers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>
        )}
      </div>

      {/* Desktop table */}
      <div className="hidden md:block bg-card border border-border rounded-xl overflow-hidden shadow-sm">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 border-b border-border">
            <tr>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">User</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Email</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Role</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Center</th>
              <th className="px-4 py-3 text-center font-semibold text-muted-foreground">Status</th>
              <th className="px-4 py-3 text-left font-semibold text-muted-foreground">Last Login</th>
              {isAdmin && <th className="px-4 py-3 text-right font-semibold text-muted-foreground">Actions</th>}
            </tr>
          </thead>
          <tbody className="divide-y divide-border">
            {isLoading
              ? Array.from({ length: 5 }).map((_, i) => (
                  <tr key={i}>{Array.from({ length: isAdmin ? 7 : 6 }).map((_, j) => <td key={j} className="px-4 py-3"><div className="h-4 bg-muted rounded animate-pulse" /></td>)}</tr>
                ))
              : data?.items?.map(u => (
                  <motion.tr key={u.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="hover:bg-muted/30 transition-colors">
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-700 font-bold text-xs flex-shrink-0">
                          {u.name.charAt(0).toUpperCase()}
                        </div>
                        <div>
                          <p className="font-medium text-foreground">{u.name}</p>
                          <p className="text-xs text-muted-foreground">{u.mobileNumber}</p>
                        </div>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">{u.email || '—'}</td>
                    <td className="px-4 py-3 text-center">
                      <span className={cn('text-xs px-2 py-1 rounded-full font-medium', roleBadge(u.role))}>
                        {roleLabel(u.role)}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">{u.centerName || '—'}</td>
                    <td className="px-4 py-3 text-center">
                      <span className={cn('text-xs px-2 py-1 rounded-full font-medium', u.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-700')}>
                        {u.isActive ? 'Active' : 'Inactive'}
                      </span>
                      {u.isLocked && <span className="ml-1 text-xs px-1.5 py-0.5 bg-orange-100 text-orange-700 rounded-full">Locked</span>}
                    </td>
                    <td className="px-4 py-3 text-muted-foreground text-xs">{u.lastLoginAt ? formatDate(u.lastLoginAt) : '—'}</td>
                    {isAdmin && (
                      <td className="px-4 py-3">
                        <div className="flex items-center justify-end gap-1">
                          <button title="Edit profile" onClick={() => openEdit(u)} className="p-1.5 text-muted-foreground hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors">
                            <Pencil className="w-3.5 h-3.5" />
                          </button>
                          {isSuperAdmin && u.role === UserRole.User && (
                            <button title="Promote to Admin" onClick={() => openPromote(u)} className="p-1.5 text-muted-foreground hover:text-purple-600 hover:bg-purple-50 rounded-lg transition-colors">
                              <ShieldCheck className="w-3.5 h-3.5" />
                            </button>
                          )}
                          {isSuperAdmin && u.role === UserRole.Admin && (
                            <button title="Demote to Sewadar" onClick={() => demote.mutate(u.id)} className="p-1.5 text-muted-foreground hover:text-orange-600 hover:bg-orange-50 rounded-lg transition-colors">
                              <ShieldOff className="w-3.5 h-3.5" />
                            </button>
                          )}
                          {u.isActive ? (
                            <button title="Deactivate" onClick={() => toggleActive.mutate({ id: u.id, activate: false })} className="p-1.5 text-muted-foreground hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors">
                              <UserX className="w-3.5 h-3.5" />
                            </button>
                          ) : (
                            <button title="Activate" onClick={() => toggleActive.mutate({ id: u.id, activate: true })} className="p-1.5 text-muted-foreground hover:text-green-600 hover:bg-green-50 rounded-lg transition-colors">
                              <UserCheck className="w-3.5 h-3.5" />
                            </button>
                          )}
                        </div>
                      </td>
                    )}
                  </motion.tr>
                ))}
          </tbody>
        </table>
        {!isLoading && (!data?.items || data.items.length === 0) && (
          <div className="text-center py-16">
            <Users className="w-12 h-12 text-muted-foreground mx-auto mb-3" />
            <p className="text-muted-foreground font-medium">No users found</p>
          </div>
        )}
      </div>

      {/* Mobile card list */}
      <div className="md:hidden space-y-3">
        {isLoading
          ? Array.from({ length: 4 }).map((_, i) => (
              <div key={i} className="h-28 bg-muted rounded-xl animate-pulse" />
            ))
          : data?.items?.map(u => (
              <motion.div
                key={u.id}
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                className="bg-card border border-border rounded-xl p-4 shadow-sm"
              >
                <div className="flex items-start justify-between gap-2">
                  <div className="flex items-start gap-3 min-w-0">
                    <div className="w-10 h-10 rounded-full bg-blue-100 dark:bg-blue-900 flex items-center justify-center text-blue-700 dark:text-blue-300 font-bold text-sm shrink-0">
                      {u.name.charAt(0).toUpperCase()}
                    </div>
                    <div className="min-w-0">
                      <p className="font-semibold text-foreground text-sm">{u.name}</p>
                      <p className="text-xs text-muted-foreground">{u.mobileNumber}</p>
                      {u.email && <p className="text-xs text-muted-foreground truncate">{u.email}</p>}
                    </div>
                  </div>
                  <div className="flex items-center gap-1 shrink-0 flex-wrap justify-end">
                    <span className={cn('text-xs px-2 py-0.5 rounded-full font-medium', roleBadge(u.role))}>
                      {roleLabel(u.role)}
                    </span>
                    <span className={cn('text-xs px-2 py-0.5 rounded-full font-medium', u.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-700')}>
                      {u.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </div>
                </div>

                {(u.centerName || u.lastLoginAt) && (
                  <div className="mt-2.5 pt-2.5 border-t border-border flex flex-wrap gap-x-4 gap-y-1">
                    {u.centerName && <p className="text-xs text-muted-foreground">📍 {u.centerName}</p>}
                    {u.lastLoginAt && <p className="text-xs text-muted-foreground">🕐 {formatDate(u.lastLoginAt)}</p>}
                  </div>
                )}

                {isAdmin && (
                  <div className="mt-2.5 pt-2.5 border-t border-border flex gap-2">
                    <button onClick={() => openEdit(u)} className="flex items-center gap-1.5 px-3 py-1.5 text-xs border border-border rounded-lg hover:bg-accent transition-colors">
                      <Pencil className="w-3 h-3" /> Edit
                    </button>
                    {isSuperAdmin && u.role === UserRole.User && (
                      <button onClick={() => openPromote(u)} className="flex items-center gap-1.5 px-3 py-1.5 text-xs border border-purple-200 text-purple-700 rounded-lg hover:bg-purple-50 transition-colors">
                        <ShieldCheck className="w-3 h-3" /> Promote
                      </button>
                    )}
                    {isSuperAdmin && u.role === UserRole.Admin && (
                      <button onClick={() => demote.mutate(u.id)} className="flex items-center gap-1.5 px-3 py-1.5 text-xs border border-orange-200 text-orange-700 rounded-lg hover:bg-orange-50 transition-colors">
                        <ShieldOff className="w-3 h-3" /> Demote
                      </button>
                    )}
                    {u.isActive ? (
                      <button onClick={() => toggleActive.mutate({ id: u.id, activate: false })} className="flex items-center gap-1.5 px-3 py-1.5 text-xs border border-red-200 text-red-700 rounded-lg hover:bg-red-50 transition-colors ml-auto">
                        <UserX className="w-3 h-3" /> Deactivate
                      </button>
                    ) : (
                      <button onClick={() => toggleActive.mutate({ id: u.id, activate: true })} className="flex items-center gap-1.5 px-3 py-1.5 text-xs border border-green-200 text-green-700 rounded-lg hover:bg-green-50 transition-colors ml-auto">
                        <UserCheck className="w-3 h-3" /> Activate
                      </button>
                    )}
                  </div>
                )}
              </motion.div>
            ))}
        {!isLoading && (!data?.items || data.items.length === 0) && (
          <div className="text-center py-16">
            <Users className="w-12 h-12 text-muted-foreground mx-auto mb-3" />
            <p className="text-muted-foreground font-medium">No users found</p>
          </div>
        )}
      </div>

      {data && data.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <p className="text-sm text-muted-foreground">Showing {data.items.length} of {data.totalCount} users</p>
          <div className="flex gap-2">
            <button disabled={!data.hasPreviousPage} onClick={() => setPage(p => p - 1)} className="p-2 border border-border rounded-lg disabled:opacity-40 hover:bg-muted"><ChevronLeft className="w-4 h-4" /></button>
            <span className="px-3 py-2 text-sm text-muted-foreground">Page {data.pageNumber} of {data.totalPages}</span>
            <button disabled={!data.hasNextPage} onClick={() => setPage(p => p + 1)} className="p-2 border border-border rounded-lg disabled:opacity-40 hover:bg-muted"><ChevronRight className="w-4 h-4" /></button>
          </div>
        </div>
      )}

      {editingUser && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-sm shadow-xl">
            <div className="flex items-center justify-between p-6 border-b border-border">
              <h2 className="text-lg font-semibold">Edit Profile</h2>
              <button onClick={() => setEditingUser(null)} className="text-muted-foreground hover:text-foreground text-xl font-bold">✕</button>
            </div>
            <form onSubmit={profileForm.handleSubmit(onProfileSubmit)} className="p-6 space-y-4">
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Name *</label>
                <input {...profileForm.register('name', { required: true })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              </div>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Email</label>
                <input {...profileForm.register('email')} type="email" className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500" />
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setEditingUser(null)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted">Cancel</button>
                <button type="submit" disabled={updateProfile.isPending} className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 disabled:opacity-50">
                  {updateProfile.isPending ? 'Saving...' : 'Save'}
                </button>
              </div>
            </form>
          </motion.div>
        </div>
      )}

      {promotingUser && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm p-4">
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} className="bg-card border border-border rounded-2xl w-full max-w-sm shadow-xl">
            <div className="flex items-center justify-between p-6 border-b border-border">
              <h2 className="text-lg font-semibold">Promote to Admin</h2>
              <button onClick={() => setPromotingUser(null)} className="text-muted-foreground hover:text-foreground text-xl font-bold">✕</button>
            </div>
            <form onSubmit={promoteForm.handleSubmit(onPromoteSubmit)} className="p-6 space-y-4">
              <p className="text-sm text-muted-foreground">Select the center for <strong>{promotingUser.name}</strong> to administer.</p>
              <div>
                <label className="block text-sm font-medium text-foreground mb-1">Center *</label>
                <select {...promoteForm.register('centerId', { required: true })} className="w-full px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500">
                  <option value="">Select center...</option>
                  {centers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                </select>
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setPromotingUser(null)} className="flex-1 px-4 py-2 border border-border rounded-lg text-sm hover:bg-muted">Cancel</button>
                <button type="submit" disabled={promote.isPending} className="flex-1 px-4 py-2 bg-purple-600 text-white rounded-lg text-sm hover:bg-purple-700 disabled:opacity-50">
                  {promote.isPending ? 'Promoting...' : 'Promote'}
                </button>
              </div>
            </form>
          </motion.div>
        </div>
      )}
    </div>
  );
}
