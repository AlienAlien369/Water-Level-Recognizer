import { Bell } from 'lucide-react';
export function NotificationsPage() {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-foreground">Notifications</h1>
      <p className="text-muted-foreground">Real-time alerts and system notifications.</p>
      <div className="flex items-center justify-center h-48 border border-dashed border-border rounded-xl">
        <div className="text-center"><Bell className="w-10 h-10 text-muted-foreground mx-auto mb-2" /><p className="text-muted-foreground">Notifications center</p></div>
      </div>
    </div>
  );
}
