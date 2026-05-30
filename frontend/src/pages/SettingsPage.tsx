import { Settings } from 'lucide-react';
export function SettingsPage() {
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold text-foreground">Settings</h1>
      <p className="text-muted-foreground">Configure platform settings and preferences.</p>
      <div className="flex items-center justify-center h-48 border border-dashed border-border rounded-xl">
        <div className="text-center"><Settings className="w-10 h-10 text-muted-foreground mx-auto mb-2" /><p className="text-muted-foreground">Settings interface</p></div>
      </div>
    </div>
  );
}
