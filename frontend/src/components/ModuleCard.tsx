/*
import { batteryTextColor, fmt, seriesColor, timeAgo, trend } from '../lib/format';
import { AreaChart } from './AreaChart';
import { MeasurementIcon } from './MeasurementIcon'; */
import { ArrowUpRight } from '@phosphor-icons/react';
/* import type { SensorModule, SensorReading } from '../types';
 */import { BatteryIcon } from './BatteryIcon';
import { usePolling } from '../usePolling'
import type { LatestMeasurement, Module } from '../types';


/* interface Props {
  readings: SensorModule;
  now: number;
  onOpen: (id: string) => void;
} */

export function ModuleCard({ module }: { module: Module }) {
  /* const battText = batteryTextColor(m.battery); */
  const { data: latestMeas, error } = usePolling<LatestMeasurement>(
    `/latest-reading/${encodeURIComponent(module.id)}`
  )


  return (
    <article
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        /* if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault();
          onOpen(m.id);
        } */
      }}
      className="flex h-full min-w-0 cursor-pointer flex-col gap-5 rounded-lg bg-surface p-5 shadow-edge transition hover:shadow-lift active:scale-[.995]"
    >
      <header className="flex items-start justify-between gap-3">
        <div className="flex min-w-0 flex-col gap-0.5">
          <div className="flex items-center gap-2">
            <span className="h-1.5 w-1.5 flex-none rounded-full bg-accent shadow-[0_0_8px_var(--color-accent)]" /* : 'bg-neutral-600 '}`*/ />
            <h2 className="m-0 text-[15px] font-medium">{module.name}</h2>
          </div>
          <p className="m-0 text-xs text-neutral-500">
            {/* {m.sensor} · {m.address} · every {m.interval} */}
            {module.sensorType} · every {module.intervalMs} ms
          </p>
        </div>
        <div className="flex flex-none items-center gap-1.5 text-xs tabular-nums">
          <BatteryIcon percent={100} />
          <span>{100}%</span>
        </div>
      </header>
    

      <section className="flex flex-col gap-2">
        <div className="flex items-center justify-between gap-3">
          <div className="flex items-center gap-1.5 text-xs text-neutral-400">
            {/* <MeasurementIcon kind={x.kind} size={14} color={color} />
                {x.label} */}
                {latestMeas?.readings.map(reading => (
                    <p key={reading.type}>
                {reading.type}: {reading.value}
              </p>
            ))}

          
          </div>
          <div className="text-[11px] tabular-nums text-neutral-600">
            {/* {fmt(x, Math.min(...h))}–{fmt(x, Math.max(...h))} {x.unit} */}
          </div>
        </div>
        <div className="flex items-baseline gap-1.5">
          {/* <span className="text-[32px] font-medium leading-none tracking-[-0.02em] tabular-nums">{fmt(x, h[h.length - 1])}</span>
              <span className="text-sm text-neutral-500">{x.unit}</span> */}
          {/*               <span className="ml-auto text-[11px] tabular-nums text-neutral-500">{trend(x)} in 3 h</span>
 */}            </div>
        {/* <AreaChart values={h} color={color} minSpan={x.minSpan} height={56} /> */}
        <div className="flex justify-between text-[10px] text-neutral-700">
          <span>−3 h</span>
          <span>now</span>
        </div>
      </section>

      <footer className="mt-auto flex items-center justify-between gap-3 text-[11px] tabular-nums text-neutral-500">
        {/* <span>
          Updated {timeAgo(m.updatedAt, now)} · <span style={{ color: battText }}>~{m.daysLeft} days left</span>
        </span>
        <span className="flex items-center gap-0.5 text-neutral-400">
          Details <ArrowUpRight size={12} />
        </span> */}

        <span>
          Updated xx · <span>~xx days left</span>
        </span>
        <span className="flex items-center gap-0.5 text-neutral-400">
          Details <ArrowUpRight size={12} />
        </span>
      </footer>
    </article>
  );
}
