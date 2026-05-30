import { useState } from 'react';
import { motion } from 'framer-motion';
import { History, Power, PowerOff, ChevronLeft, ChevronRight, Calendar, Clock, Search, Filter } from 'lucide-react';
import { useMotorHistory } from '@/hooks/useMotors';
import { useLocations } from '@/hooks/useLocations';
import { formatDate, formatDuration, cn } from '@/lib/utils';

type DateFilter = 'today' | 'yesterday' | '7days' | 'custom';

const DATE_FILTERS: { key: DateFilter; label: string }[] = [
  { key: 'today', label: 'Today' },
  { key: 'yesterday', label: 'Yesterday' },
  { key: '7days', label: 'Past 7 Days' },
  { key: 'custom', label: 'Custom' },
];

export function HistoryPage() {
  const [dateFilter, setDateFilter] = useState<DateFilter>('today');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [locationId, setLocationId] = useState('');
  const [motorSearch, setMotorSearch] = useState('');
  const [minHoursInput, setMinHoursInput] = useState('');
  const [appliedMinHours, setAppliedMinHours] = useState<number | undefined>(undefined);
  const [page, setPage] = useState(1);

  const { data: locationsData } = useLocations({ pageNumber: 1, pageSize: 100 });
  const locations = locationsData?.items ?? [];

  const { data, isLoading } = useMotorHistory({
    pageNumber: page,
    pageSize: 20,
    dateFilter,
    startDate: dateFilter === 'custom' && startDate ? startDate : undefined,
    endDate: dateFilter === 'custom' && endDate ? endDate : undefined,
    locationId: locationId || undefined,
    motorSearch: motorSearch || undefined,
    minDurationHours: appliedMinHours,
  });

  const resetPage = () => setPage(1);

  const applyMinHours = () => {
    const v = parseFloat(minHoursInput);
    setAppliedMinHours(isNaN(v) || v <= 0 ? undefined : v);
    resetPage();
  };

  return (
    <div className="space-y-5">
      <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }}>
        <h1 className="text-xl md:text-2xl font-bold text-foreground">Motor History</h1>
        <p className="text-muted-foreground text-sm mt-1">Complete log of all motor sessions (open → close)</p>
      </motion.div>

      {/* Date Filters */}
      <div className="flex flex-wrap gap-2">
        {DATE_FILTERS.map(f => (
          <button
            key={f.key}
            onClick={() => { setDateFilter(f.key); resetPage(); }}
            className={cn(
              'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
              dateFilter === f.key
                ? 'bg-blue-600 text-white shadow-sm'
                : 'bg-card border border-border text-foreground hover:bg-accent'
            )}
          >
            {f.key === 'custom' && <Calendar className="w-3.5 h-3.5 inline mr-1.5" />}
            {f.label}
          </button>
        ))}
      </div>

      {/* Custom date pickers */}
      {dateFilter === 'custom' && (
        <motion.div
          initial={{ opacity: 0, height: 0 }}
          animate={{ opacity: 1, height: 'auto' }}
          className="flex flex-wrap gap-4 p-4 bg-card border border-border rounded-xl"
        >
          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium text-muted-foreground">From</label>
            <input type="date" value={startDate} onChange={e => { setStartDate(e.target.value); resetPage(); }}
              className="px-3 py-2 border border-border rounded-lg text-sm bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>
          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium text-muted-foreground">To</label>
            <input type="date" value={endDate} onChange={e => { setEndDate(e.target.value); resetPage(); }}
              className="px-3 py-2 border border-border rounded-lg text-sm bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500" />
          </div>
        </motion.div>
      )}

      {/* Advanced Filters */}
      <div className="flex flex-wrap gap-3 items-end p-4 bg-card border border-border rounded-xl">
        <Filter className="w-4 h-4 text-muted-foreground self-center mt-4" />

        {/* Motor search */}
        <div className="flex flex-col gap-1 flex-1 min-w-[140px]">
          <label className="text-xs font-medium text-muted-foreground">Motor Number</label>
          <div className="relative">
            <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-muted-foreground" />
            <input
              value={motorSearch}
              onChange={e => { setMotorSearch(e.target.value); resetPage(); }}
              placeholder="Search motor #..."
              className="w-full pl-8 pr-3 py-2 border border-border rounded-lg text-sm bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </div>

        {/* Location filter */}
        <div className="flex flex-col gap-1 min-w-[160px]">
          <label className="text-xs font-medium text-muted-foreground">Location</label>
          <select
            value={locationId}
            onChange={e => { setLocationId(e.target.value); resetPage(); }}
            className="px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All Locations</option>
            {locations.map(l => <option key={l.id} value={l.id}>{l.name}</option>)}
          </select>
        </div>

        {/* Min duration hours */}
        <div className="flex flex-col gap-1">
          <label className="text-xs font-medium text-muted-foreground">Duration more than (hrs)</label>
          <div className="flex gap-1.5">
            <input
              type="number" min="0" step="0.5"
              value={minHoursInput}
              onChange={e => setMinHoursInput(e.target.value)}
              onKeyDown={e => e.key === 'Enter' && applyMinHours()}
              placeholder="e.g. 20"
              className="w-24 px-3 py-2 border border-border rounded-lg bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <button onClick={applyMinHours} className="px-3 py-2 bg-blue-600 text-white rounded-lg text-sm hover:bg-blue-700 transition-colors">Apply</button>
            {appliedMinHours && (
              <button onClick={() => { setMinHoursInput(''); setAppliedMinHours(undefined); resetPage(); }} className="px-3 py-2 border border-border rounded-lg text-sm hover:bg-accent">✕</button>
            )}
          </div>
        </div>
      </div>

      {/* Active filter chips */}
      {(locationId || motorSearch || appliedMinHours) && (
        <div className="flex flex-wrap gap-1.5">
          {motorSearch && (
            <span className="inline-flex items-center gap-1 text-xs bg-blue-100 text-blue-700 dark:bg-blue-950 dark:text-blue-300 px-2 py-1 rounded-full">
              🔍 Motor: {motorSearch}
              <button onClick={() => { setMotorSearch(''); resetPage(); }}>✕</button>
            </span>
          )}
          {locationId && (
            <span className="inline-flex items-center gap-1 text-xs bg-purple-100 text-purple-700 dark:bg-purple-950 dark:text-purple-300 px-2 py-1 rounded-full">
              📍 {locations.find(l => l.id === locationId)?.name}
              <button onClick={() => { setLocationId(''); resetPage(); }}>✕</button>
            </span>
          )}
          {appliedMinHours && (
            <span className="inline-flex items-center gap-1 text-xs bg-orange-100 text-orange-700 dark:bg-orange-950 dark:text-orange-300 px-2 py-1 rounded-full">
              ⏱ &gt;{appliedMinHours}h duration
              <button onClick={() => { setMinHoursInput(''); setAppliedMinHours(undefined); resetPage(); }}>✕</button>
            </span>
          )}
        </div>
      )}

      {/* Table */}
      <div className="bg-card border border-border rounded-xl shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full min-w-[750px] text-sm">
            <thead>
              <tr className="border-b border-border bg-muted/50">
                <th className="text-left px-4 py-3 font-semibold text-foreground">Motor #</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Location / Center</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Opened At</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Closed At</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Duration</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Operated By</th>
              </tr>
            </thead>
            <tbody>
              {isLoading ? (
                Array.from({ length: 8 }).map((_, i) => (
                  <tr key={i} className="border-b border-border">
                    {Array.from({ length: 6 }).map((_, j) => (
                      <td key={j} className="px-4 py-3"><div className="h-4 bg-muted rounded animate-pulse" /></td>
                    ))}
                  </tr>
                ))
              ) : data?.items?.length === 0 ? (
                <tr>
                  <td colSpan={6} className="px-4 py-16 text-center">
                    <History className="w-10 h-10 text-muted-foreground mx-auto mb-2" />
                    <p className="text-muted-foreground font-medium">No history found</p>
                    <p className="text-muted-foreground text-xs mt-1">Try adjusting the filters</p>
                  </td>
                </tr>
              ) : (
                data?.items?.map((session, idx) => (
                  <motion.tr
                    key={session.sessionId}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: idx * 0.02 }}
                    className="border-b border-border hover:bg-accent/50 transition-colors"
                  >
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        <div className={cn('w-2 h-2 rounded-full shrink-0', session.isRunning ? 'bg-green-500 animate-pulse' : 'bg-gray-300')} />
                        <span className="font-semibold text-foreground">#{session.motorNumber}</span>
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <p className="text-foreground text-sm">{session.locationName}</p>
                      <p className="text-muted-foreground text-xs">{session.centerName}</p>
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-1.5 text-green-600 dark:text-green-400">
                        <Power className="w-3.5 h-3.5 shrink-0" />
                        <span className="whitespace-nowrap text-sm">{formatDate(session.openTime)}</span>
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      {session.isRunning ? (
                        <span className="inline-flex items-center gap-1 text-xs font-medium bg-green-50 text-green-700 dark:bg-green-950 dark:text-green-400 px-2 py-1 rounded-full">
                          <Clock className="w-3 h-3" /> Running
                        </span>
                      ) : (
                        <div className="flex items-center gap-1.5 text-red-600 dark:text-red-400">
                          <PowerOff className="w-3.5 h-3.5 shrink-0" />
                          <span className="whitespace-nowrap text-sm">{session.closeTime ? formatDate(session.closeTime) : '—'}</span>
                        </div>
                      )}
                    </td>
                    <td className="px-4 py-3 text-muted-foreground font-medium">
                      {session.isRunning ? '—' : session.durationMinutes != null ? formatDuration(session.durationMinutes) : '—'}
                    </td>
                    <td className="px-4 py-3 text-foreground">{session.openedByUserName}</td>
                  </motion.tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {data && data.totalPages > 1 && (
          <div className="flex items-center justify-between px-4 py-3 border-t border-border">
            <p className="text-sm text-muted-foreground">
              {data.totalCount} session{data.totalCount !== 1 ? 's' : ''}
            </p>
            <div className="flex items-center gap-2">
              <button disabled={!data.hasPreviousPage} onClick={() => setPage(p => p - 1)}
                className="p-1.5 rounded-lg border border-border disabled:opacity-40 hover:bg-accent transition-colors">
                <ChevronLeft className="w-4 h-4" />
              </button>
              <span className="text-sm text-muted-foreground px-1">{data.pageNumber} / {data.totalPages}</span>
              <button disabled={!data.hasNextPage} onClick={() => setPage(p => p + 1)}
                className="p-1.5 rounded-lg border border-border disabled:opacity-40 hover:bg-accent transition-colors">
                <ChevronRight className="w-4 h-4" />
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
