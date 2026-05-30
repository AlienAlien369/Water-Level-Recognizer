import { Bell, Moon, Sun, LogOut, User } from 'lucide-react';
import { useAuthStore } from '@/store/authStore';
import { useUIStore } from '@/store/uiStore';
import { useLogout } from '@/hooks/useAuth';
import { getRoleLabel } from '@/lib/utils';

export function Header() {
  const { user } = useAuthStore();
  const { theme, toggleTheme } = useUIStore();
  const logout = useLogout();

  return (
    <header className="h-16 bg-background border-b border-border flex items-center justify-between px-6 shrink-0">
      <div>
        <h2 className="text-sm font-medium text-muted-foreground">
          {user?.name && <span>Welcome, <strong>{user.name}</strong></span>}
        </h2>
        {user?.role !== undefined && (
          <span className="text-xs text-muted-foreground">{getRoleLabel(user.role)}</span>
        )}
      </div>

      <div className="flex items-center gap-3">
        <button onClick={toggleTheme} className="p-2 rounded-lg hover:bg-accent transition-colors" title="Toggle theme">
          {theme === 'light' ? <Moon className="w-4 h-4" /> : <Sun className="w-4 h-4" />}
        </button>
        <button className="p-2 rounded-lg hover:bg-accent transition-colors relative" title="Notifications">
          <Bell className="w-4 h-4" />
          <span className="absolute top-1.5 right-1.5 w-2 h-2 bg-red-500 rounded-full"></span>
        </button>
        <div className="flex items-center gap-2 pl-3 border-l border-border">
          <div className="w-8 h-8 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
            <User className="w-4 h-4 text-blue-600" />
          </div>
          <button onClick={logout} className="p-2 rounded-lg hover:bg-accent transition-colors text-muted-foreground hover:text-foreground" title="Logout">
            <LogOut className="w-4 h-4" />
          </button>
        </div>
      </div>
    </header>
  );
}
