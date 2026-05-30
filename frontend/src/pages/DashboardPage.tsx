import { motion } from 'framer-motion';
import { Building2, MapPin, Zap, Users, Activity, AlertTriangle, Clock, TrendingUp } from 'lucide-react';
import { StatCard } from '@/components/shared/StatCard';
import { MotorStatusChart } from '@/components/charts/MotorStatusChart';
import { useDashboardSummary } from '@/hooks/useDashboard';
import { formatDuration } from '@/lib/utils';

export function DashboardPage() {
  const { data: summary, isLoading } = useDashboardSummary();

  return (
    <div className="space-y-6">
      <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }}>
        <h1 className="text-xl md:text-2xl font-bold text-foreground">Dashboard</h1>
        <p className="text-muted-foreground text-sm mt-1">Real-time overview of all water motor operations</p>
      </motion.div>

      {isLoading ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          {Array.from({ length: 8 }).map((_, i) => (
            <div key={i} className="h-32 bg-muted rounded-xl animate-pulse" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <StatCard title="Total Centers" value={summary?.totalCenters ?? 0} icon={Building2} color="blue" />
          <StatCard title="Total Locations" value={summary?.totalLocations ?? 0} icon={MapPin} color="purple" />
          <StatCard title="Total Motors" value={summary?.totalMotors ?? 0} icon={Zap} color="blue" />
          <StatCard title="Total Users" value={summary?.totalUsers ?? 0} icon={Users} color="green" />
          <StatCard title="Active Motors" value={summary?.activeMotors ?? 0} icon={Activity} color="green" />
          <StatCard title="Running Now" value={summary?.runningMotors ?? 0} icon={TrendingUp} color="blue" />
          <StatCard title="Fault Motors" value={summary?.faultMotors ?? 0} icon={AlertTriangle} color="red" />
          <StatCard
            title="Running Today"
            value={summary ? formatDuration(summary.totalRunningMinutesToday) : '0m'}
            icon={Clock}
            color="yellow"
          />
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.2 }}
          className="bg-card border border-border rounded-xl p-6 shadow-sm"
        >
          <h3 className="font-semibold text-foreground mb-1">Motor Status Distribution</h3>
          <p className="text-xs text-muted-foreground mb-4">Real-time status breakdown</p>
          <MotorStatusChart
            running={summary?.runningMotors ?? 0}
            active={(summary?.activeMotors ?? 0) - (summary?.runningMotors ?? 0)}
            fault={summary?.faultMotors ?? 0}
            inactive={(summary?.totalMotors ?? 0) - (summary?.activeMotors ?? 0) - (summary?.faultMotors ?? 0)}
          />
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          transition={{ delay: 0.3 }}
          className="bg-card border border-border rounded-xl p-6 shadow-sm"
        >
          <h3 className="font-semibold text-foreground mb-1">Quick Actions</h3>
          <p className="text-xs text-muted-foreground mb-4">Common operations</p>
          <div className="grid grid-cols-2 gap-3">
            {[
              { label: 'View All Motors', icon: Zap, href: '/motors', color: 'bg-blue-50 text-blue-700 hover:bg-blue-100' },
              { label: 'Manage Centers', icon: Building2, href: '/centers', color: 'bg-purple-50 text-purple-700 hover:bg-purple-100' },
              { label: 'View Reports', icon: TrendingUp, href: '/reports', color: 'bg-green-50 text-green-700 hover:bg-green-100' },
              { label: 'Audit Logs', icon: Activity, href: '/audit-logs', color: 'bg-yellow-50 text-yellow-700 hover:bg-yellow-100' },
            ].map(({ label, icon: Icon, href, color }) => (
              <a
                key={href}
                href={href}
                className={`flex items-center gap-2 p-3 rounded-lg text-sm font-medium transition-colors ${color}`}
              >
                <Icon className="w-4 h-4" />
                {label}
              </a>
            ))}
          </div>
        </motion.div>
      </div>
    </div>
  );
}
