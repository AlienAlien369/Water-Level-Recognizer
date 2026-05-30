import { useState } from 'react';
import { motion } from 'framer-motion';
import { History, Power, PowerOff, ChevronLeft, ChevronRight, Calendar } from 'lucide-react';
import { useMotorHistory } from '@/hooks/useMotors';
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
  const [page, setPage] = useState(1);

  const { data, isLoading } = useMotorHistory({
    pageNumber: page,
    pageSize: 20,
    dateFilter,
    startDate: dateFilter === 'custom' && startDate ? startDate : undefined,
    endDate: dateFilter === 'custom' && endDate ? endDate : undefined,
  });

  const handleFilterChange = (f: DateFilter) => {
    setDateFilter(f);
    setPage(1);
  };

  return (
    <div className="space-y-6">
      <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }}>
        <h1 className="text-xl md:text-2xl font-bold text-foreground">Motor History</h1>
        <p className="text-muted-foreground text-sm mt-1">Complete log of all motor open/close operations</p>
      </motion.div>

      {/* Date Filters */}
      <div className="flex flex-wrap gap-2">
        {DATE_FILTERS.map(f => (
          <button
            key={f.key}
            onClick={() => handleFilterChange(f.key)}
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
            <input
              type="date"
              value={startDate}
              onChange={e => { setStartDate(e.target.value); setPage(1); }}
              className="px-3 py-2 border border-border rounded-lg text-sm bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
          <div className="flex flex-col gap-1">
            <label className="text-xs font-medium text-muted-foreground">To</label>
            <input
              type="date"
              value={endDate}
              onChange={e => { setEndDate(e.target.value); setPage(1); }}
              className="px-3 py-2 border border-border rounded-lg text-sm bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>
        </motion.div>
      )}

      {/* Table */}
      <div className="bg-card border border-border rounded-xl shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full min-w-[700px] text-sm">
            <thead>
              <tr className="border-b border-border bg-muted/50">
                <th className="text-left px-4 py-3 font-semibold text-foreground">Motor #</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Location</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Center</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Action</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Operated By</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Time</th>
                <th className="text-left px-4 py-3 font-semibold text-foreground">Duration</th>
              </tr>
            </thead>
            <tbody>
              {isLoading ? (
                Array.from({ length: 8 }).map((_, i) => (
                  <tr key={i} className="border-b border-border">
                    {Array.from({ length: 7 }).map((_, j) => (
                      <td key={j} className="px-4 py-3">
                        <div className="h-4 bg-muted rounded animate-pulse" />
                      </td>
                    ))}
                  </tr>
                ))
              ) : data?.items?.length === 0 ? (
                <tr>
                  <td colSpan={7} className="px-4 py-16 text-center">
                    <History className="w-10 h-10 text-muted-foreground mx-auto mb-2" />
                    <p className="text-muted-foreground font-medium">No history found</p>
                    <p className="text-muted-foreground text-xs mt-1">Try selecting a different date range</p>
                  </td>
                </tr>
              ) : (
                data?.items?.map((log, idx) => (
                  <motion.tr
                    key={log.id}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    transition={{ delay: idx * 0.02 }}
                    className="border-b border-border hover:bg-accent/50 transition-colors"
                  >
                    <td className="px-4 py-3 font-semibold text-foreground">#{log.motorNumber}</td>
                    <td className="px-4 py-3 text-muted-foreground">{log.locationName}</td>
                    <td className="px-4 py-3 text-muted-foreground">{log.centerName}</td>
                    <td className="px-4 py-3">
                      <span className={cn(
                        'inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold',
                        log.action === 'Open'
                          ? 'bg-green-50 text-green-700 dark:bg-green-950 dark:text-green-400'
                          : 'bg-red-50 text-red-700 dark:bg-red-950 dark:text-red-400'
                      )}>
                        {log.action === 'Open'
                          ? <Power className="w-3 h-3" />
                          : <PowerOff className="w-3 h-3" />}
                        {log.action}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-foreground">{log.operatedByUserName}</td>
                    <td className="px-4 py-3 text-muted-foreground whitespace-nowrap">{formatDate(log.actionTime)}</td>
                    <td className="px-4 py-3 text-muted-foreground">
                      {log.durationMinutes != null ? formatDuration(log.durationMinutes) : '—'}
                    </td>
                  </motion.tr>
                ))
              )}
            </tbody>
          </table>
        </div>

        {/* Pagination */}
        {data && data.totalPages > 1 && (
          <div className="flex items-center justify-between px-4 py-3 border-t border-border">
            <p className="text-sm text-muted-foreground">
              {data.totalCount} record{data.totalCount !== 1 ? 's' : ''}
            </p>
            <div className="flex items-center gap-2">
              <button
                disabled={!data.hasPreviousPage}
                onClick={() => setPage(p => p - 1)}
                className="p-1.5 rounded-lg border border-border disabled:opacity-40 hover:bg-accent transition-colors"
              >
                <ChevronLeft className="w-4 h-4" />
              </button>
              <span className="text-sm text-muted-foreground px-1">
                {data.pageNumber} / {data.totalPages}
              </span>
              <button
                disabled={!data.hasNextPage}
                onClick={() => setPage(p => p + 1)}
                className="p-1.5 rounded-lg border border-border disabled:opacity-40 hover:bg-accent transition-colors"
              >
                <ChevronRight className="w-4 h-4" />
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
