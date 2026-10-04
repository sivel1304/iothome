export function BatteryIcon({ percent }: { percent: number }) {
  return (
    <div className="flex items-center" aria-hidden>
      <div className="box-border h-[11px] w-[22px] rounded-[3px] p-[2px] shadow-[inset_0_0_0_1px_var(--color-neutral-600)]">
        <div className="h-full rounded-[1px] var(--color-accent)" style={{ width: `${percent}%` }} />
      </div>
      <div className="h-[5px] w-[2px] rounded-r-[1px] bg-neutral-600" />
    </div>
  );
}

export function BatterySegments({ percent, segments = 20 }: { percent: number; segments?: number }) {
  const filled = Math.round((percent / 100) * segments);
  return (
    <div className="flex gap-[2px]" role="meter" aria-valuenow={percent} aria-valuemin={0} aria-valuemax={100} aria-label="Battery">
      {Array.from({ length: segments }, (_, i) => (
        <div key={i} className="h-[5px] flex-1 rounded-[1px]" style={{ background: i < filled ? 'var(--color-accent)' : 'var(--color-neutral-900)' }} />
      ))}
    </div>
  );
}
