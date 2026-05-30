import { NavLink } from 'react-router-dom';
import { motion, AnimatePresence } from 'framer-motion';
import {
  LayoutDashboard, Building2, MapPin, Zap, Users,
  ClipboardList, BarChart3, Bell, Shield, Settings,
  Droplets, ChevronLeft, ChevronRight, X
} from 'lucide-react';
import { useUIStore } from '@/store/uiStore';
import { useAuthStore } from '@/store/authStore';
import { UserRole } from '@/types';
import { cn } from '@/lib/utils';

const navItems = [
  { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard', roles: [UserRole.SuperAdmin, UserRole.Admin, UserRole.User] },
  { to: '/centers', icon: Building2, label: 'Centers', roles: [UserRole.SuperAdmin] },
  { to: '/locations', icon: MapPin, label: 'Locations', roles: [UserRole.SuperAdmin, UserRole.Admin] },
  { to: '/motors', icon: Zap, label: 'Motors', roles: [UserRole.SuperAdmin, UserRole.Admin, UserRole.User] },
  { to: '/users', icon: Users, label: 'Users', roles: [UserRole.SuperAdmin, UserRole.Admin] },
  { to: '/assignments', icon: ClipboardList, label: 'Assignments', roles: [UserRole.SuperAdmin, UserRole.Admin] },
  { to: '/reports', icon: BarChart3, label: 'Reports', roles: [UserRole.SuperAdmin, UserRole.Admin] },
  { to: '/notifications', icon: Bell, label: 'Notifications', roles: [UserRole.SuperAdmin, UserRole.Admin, UserRole.User] },
  { to: '/audit-logs', icon: Shield, label: 'Audit Logs', roles: [UserRole.SuperAdmin, UserRole.Admin] },
  { to: '/settings', icon: Settings, label: 'Settings', roles: [UserRole.SuperAdmin, UserRole.Admin, UserRole.User] },
];

export function Sidebar() {
  const { sidebarOpen, toggleSidebar, mobileSidebarOpen, closeMobileSidebar } = useUIStore();
  const { user } = useAuthStore();
  const role = user?.role ?? UserRole.User;
  const visibleItems = navItems.filter(item => item.roles.includes(role));

  const NavContent = ({ isMobile }: { isMobile: boolean }) => (
    <>
      {/* Logo */}
      <div className="flex items-center justify-between px-4 py-5 border-b border-gray-700 shrink-0">
        <div className="flex items-center gap-3">
          <div className="flex items-center justify-center w-9 h-9 bg-blue-500 rounded-lg shrink-0">
            <Droplets className="w-5 h-5 text-white" />
          </div>
          {(isMobile || sidebarOpen) && (
            <span className="font-bold text-sm whitespace-nowrap">
              WaterLevel<span className="text-blue-400">Recognizer</span>
            </span>
          )}
        </div>
        {isMobile && (
          <button onClick={closeMobileSidebar} className="p-1.5 rounded-lg hover:bg-gray-700 transition-colors ml-2">
            <X className="w-4 h-4 text-gray-400" />
          </button>
        )}
      </div>

      {/* Nav */}
      <nav className="flex-1 py-4 space-y-1 px-2 overflow-y-auto">
        {visibleItems.map(({ to, icon: Icon, label }) => (
          <NavLink
            key={to}
            to={to}
            onClick={isMobile ? closeMobileSidebar : undefined}
            className={({ isActive }) =>
              cn(
                'flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-colors',
                isActive
                  ? 'bg-blue-600 text-white'
                  : 'text-gray-400 hover:bg-gray-800 hover:text-white'
              )
            }
          >
            <Icon className="w-5 h-5 shrink-0" />
            {(isMobile || sidebarOpen) && (
              <span className="whitespace-nowrap">{label}</span>
            )}
          </NavLink>
        ))}
      </nav>

      {/* Desktop toggle button */}
      {!isMobile && (
        <button
          onClick={toggleSidebar}
          className="flex items-center justify-center p-4 border-t border-gray-700 hover:bg-gray-800 transition-colors shrink-0"
        >
          {sidebarOpen ? <ChevronLeft className="w-4 h-4" /> : <ChevronRight className="w-4 h-4" />}
        </button>
      )}
    </>
  );

  return (
    <>
      {/* Desktop sidebar */}
      <motion.aside
        animate={{ width: sidebarOpen ? 256 : 64 }}
        transition={{ duration: 0.2 }}
        className="hidden md:flex fixed left-0 top-0 h-full bg-gray-900 text-white z-30 flex-col shadow-xl overflow-hidden"
      >
        <NavContent isMobile={false} />
      </motion.aside>

      {/* Mobile sidebar — full slide-out drawer */}
      <AnimatePresence>
        {mobileSidebarOpen && (
          <motion.aside
            initial={{ x: -280 }}
            animate={{ x: 0 }}
            exit={{ x: -280 }}
            transition={{ type: 'spring', damping: 30, stiffness: 300 }}
            className="md:hidden fixed left-0 top-0 h-full w-64 bg-gray-900 text-white z-30 flex flex-col shadow-2xl"
          >
            <NavContent isMobile={true} />
          </motion.aside>
        )}
      </AnimatePresence>
    </>
  );
}
