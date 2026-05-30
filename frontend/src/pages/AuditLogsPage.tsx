import { Shield } from 'lucide-react';
export function AuditLogsPage() {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-foreground">Audit Logs</h1>
      <p className="text-muted-foreground">Complete audit trail of all system activities.</p>
      <div className="flex items-center justify-center h-48 border border-dashed border-border rounded-xl">
        <div className="text-center"><Shield className="w-10 h-10 text-muted-foreground mx-auto mb-2" /><p className="text-muted-foreground">Audit logs interface</p></div>
      </div>
    </div>
  );
}
