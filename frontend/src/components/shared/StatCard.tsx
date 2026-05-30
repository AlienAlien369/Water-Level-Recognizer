import { motion } from 'framer-motion';
import { cn } from '@/lib/utils';
import type { LucideIcon } from 'lucide-react';

interface StatCardProps {
  title: string;
  value: string | number;
  icon: LucideIcon;
  trend?: string;
  trendUp?: boolean;
  color?: 'blue' | 'green' | 'red' | 'yellow' | 'purple';
  className?: string;
}

const colorMap = {
  blue: 'bg-blue-50 dark:bg-blue-950 text-blue-600',
  green: 'bg-green-50 dark:bg-green-950 text-green-600',
  red: 'bg-red-50 dark:bg-red-950 text-red-600',
  yellow: 'bg-yellow-50 dark:bg-yellow-950 text-yellow-600',
  purple: 'bg-purple-50 dark:bg-purple-950 text-purple-600',
};

export function StatCard({ title, value, icon: Icon, trend, trendUp, color = 'blue', className }: StatCardProps) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className={cn('bg-card border border-border rounded-xl p-6 shadow-sm hover:shadow-md transition-shadow', className)}
    >
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm font-medium text-muted-foreground">{title}</p>
          <p className="text-3xl font-bold text-foreground mt-1">{value}</p>
          {trend && (
            <p className={cn('text-xs mt-1', trendUp ? 'text-green-600' : 'text-red-600')}>
              {trendUp ? 'up' : 'down'} {trend}
            </p>
          )}
        </div>
        <div className={cn('p-3 rounded-xl', colorMap[color])}>
          <Icon className="w-6 h-6" />
        </div>
      </div>
    </motion.div>
  );
}
