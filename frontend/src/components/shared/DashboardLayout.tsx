import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';
import { Header } from './Header';
import { useUIStore } from '@/store/uiStore';
import { useEffect } from 'react';
import { startSignalR, stopSignalR } from '@/lib/signalr';

export function DashboardLayout() {
  const { sidebarOpen, mobileSidebarOpen, closeMobileSidebar, theme } = useUIStore();

  useEffect(() => {
    document.documentElement.classList.toggle('dark', theme === 'dark');
  }, [theme]);

  useEffect(() => {
    startSignalR().catch(console.warn);
    return () => { stopSignalR().catch(console.warn); };
  }, []);

  return (
    <div className="flex h-screen bg-background overflow-hidden">
      {/* Mobile backdrop */}
      {mobileSidebarOpen && (
        <div
          className="fixed inset-0 z-20 bg-black/50 md:hidden"
          onClick={closeMobileSidebar}
        />
      )}

      <Sidebar />

      {/* Main content — on mobile: full width; on desktop: shift by sidebar width */}
      <div className={`flex-1 flex flex-col overflow-hidden transition-all duration-300 ${sidebarOpen ? 'md:ml-64' : 'md:ml-16'}`}>
        <Header />
        <main className="flex-1 overflow-y-auto p-4 md:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
