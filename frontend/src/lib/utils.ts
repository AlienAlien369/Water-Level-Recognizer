import { type ClassValue, clsx } from 'clsx';
import { twMerge } from 'tailwind-merge';
import { format, formatDistanceToNow } from 'date-fns';
import { MotorStatus, MotorState, UserRole, NotificationType } from '@/types';

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatDate(date: string | Date) {
  return format(new Date(date), 'MMM dd, yyyy HH:mm');
}

export function formatRelativeTime(date: string | Date) {
  return formatDistanceToNow(new Date(date), { addSuffix: true });
}

export function formatDuration(minutes: number) {
  const h = Math.floor(minutes / 60);
  const m = Math.round(minutes % 60);
  if (h > 0) return `${h}h ${m}m`;
  return `${m}m`;
}

export function getMotorStatusColor(status: MotorStatus) {
  switch (status) {
    case MotorStatus.Running: return 'bg-green-100 text-green-800';
    case MotorStatus.Active: return 'bg-blue-100 text-blue-800';
    case MotorStatus.Fault: return 'bg-red-100 text-red-800';
    case MotorStatus.Maintenance: return 'bg-yellow-100 text-yellow-800';
    default: return 'bg-gray-100 text-gray-800';
  }
}

export function getMotorStatusLabel(status: MotorStatus) {
  return MotorStatus[status] || 'Unknown';
}

export function getMotorStateColor(state: MotorState) {
  return state === MotorState.On ? 'text-green-500' : 'text-gray-400';
}

export function getRoleLabel(role: UserRole) {
  switch (role) {
    case UserRole.SuperAdmin: return 'Super Admin';
    case UserRole.Admin: return 'Admin';
    case UserRole.User: return 'Sewadar';
    default: return 'Unknown';
  }
}

export function getRoleBadgeColor(role: UserRole) {
  switch (role) {
    case UserRole.SuperAdmin: return 'bg-purple-100 text-purple-800';
    case UserRole.Admin: return 'bg-blue-100 text-blue-800';
    case UserRole.User: return 'bg-green-100 text-green-800';
    default: return 'bg-gray-100 text-gray-800';
  }
}

export function getNotificationColor(type: NotificationType) {
  switch (type) {
    case NotificationType.Critical: return 'text-red-500';
    case NotificationType.Warning: return 'text-yellow-500';
    case NotificationType.Success: return 'text-green-500';
    default: return 'text-blue-500';
  }
}
