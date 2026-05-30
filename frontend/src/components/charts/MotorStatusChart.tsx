import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from 'recharts';

interface Props {
  running: number;
  active: number;
  fault: number;
  inactive: number;
}

const COLORS = ['#22c55e', '#3b82f6', '#ef4444', '#6b7280'];

export function MotorStatusChart({ running, active, fault, inactive }: Props) {
  const data = [
    { name: 'Running', value: running },
    { name: 'Active', value: active },
    { name: 'Fault', value: fault },
    { name: 'Inactive', value: inactive },
  ].filter(d => d.value > 0);

  if (data.length === 0) return (
    <div className="flex items-center justify-center h-48 text-muted-foreground text-sm">No motor data</div>
  );

  return (
    <ResponsiveContainer width="100%" height={240}>
      <PieChart>
        <Pie data={data} cx="50%" cy="50%" innerRadius={60} outerRadius={90} paddingAngle={4} dataKey="value">
          {data.map((_, index) => (
            <Cell key={index} fill={COLORS[index % COLORS.length]} />
          ))}
        </Pie>
        <Tooltip formatter={(value) => [`${value} motors`, '']} />
        <Legend />
      </PieChart>
    </ResponsiveContainer>
  );
}
