import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { ProtectedRoute } from '@/components/shared/ProtectedRoute';
import { DashboardLayout } from '@/components/shared/DashboardLayout';

import { LoginPage } from '@/pages/LoginPage';
import { RegisterPage } from '@/pages/RegisterPage';
import { DashboardPage } from '@/pages/DashboardPage';
import { CentersPage } from '@/pages/CentersPage';
import { LocationsPage } from '@/pages/LocationsPage';
import { MotorsPage } from '@/pages/MotorsPage';
import { UsersPage } from '@/pages/UsersPage';
import { AssignmentsPage } from '@/pages/AssignmentsPage';
import { ReportsPage } from '@/pages/ReportsPage';
import { NotificationsPage } from '@/pages/NotificationsPage';
import { AuditLogsPage } from '@/pages/AuditLogsPage';
import { SettingsPage } from '@/pages/SettingsPage';
import { HistoryPage } from '@/pages/HistoryPage';
import { NotFoundPage } from '@/pages/NotFoundPage';
import { UserRole } from '@/types';

export default function App() {
  const { isAuthenticated, user } = useAuthStore();

  const defaultHome = isAuthenticated
    ? user?.role === UserRole.User ? '/motors' : '/dashboard'
    : '/login';

  return (
    <Routes>
      {/* Public */}
      <Route path="/login" element={!isAuthenticated ? <LoginPage /> : <Navigate to={defaultHome} replace />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/otp" element={<Navigate to="/login" replace />} />

      {/* Protected */}
      <Route element={<ProtectedRoute />}>
        <Route element={<DashboardLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/centers" element={<CentersPage />} />
          <Route path="/locations" element={<LocationsPage />} />
          <Route path="/motors" element={<MotorsPage />} />
          <Route path="/users" element={<UsersPage />} />
          <Route path="/assignments" element={<AssignmentsPage />} />
          <Route path="/reports" element={<ReportsPage />} />
          <Route path="/notifications" element={<NotificationsPage />} />
          <Route path="/audit-logs" element={<AuditLogsPage />} />
          <Route path="/settings" element={<SettingsPage />} />
          <Route path="/history" element={<HistoryPage />} />
        </Route>
      </Route>

      <Route path="/" element={<Navigate to={defaultHome} replace />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}

