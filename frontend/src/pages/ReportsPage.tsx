import { BarChart3 } from 'lucide-react';
export function ReportsPage() {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-foreground">Reports</h1>
      <p className="text-muted-foreground">Generate and export operational reports (Excel/PDF).</p>
      <div className="flex items-center justify-center h-48 border border-dashed border-border rounded-xl">
        <div className="text-center"><BarChart3 className="w-10 h-10 text-muted-foreground mx-auto mb-2" /><p className="text-muted-foreground">Reports interface</p></div>
      </div>
    </div>
  );
}
