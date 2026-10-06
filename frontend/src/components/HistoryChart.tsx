import { useId } from 'react'
import { AreaChart, Area, ReferenceDot, XAxis, YAxis, CartesianGrid, ResponsiveContainer } from 'recharts'
import type { HistoryRow } from '../types'
import type { SeriesStats } from '../stats'

type Props = {
  rows: HistoryRow[]
  type: string
  stats: SeriesStats | null
  loading: boolean
  error: string | null
  minSpan?: number
}

export default function HistoryChart({ rows, type, stats, loading, error, minSpan = 1 }: Props) {
  const points = rows.filter(r => typeof r[type] === 'number')
  const last = points[points.length - 1]

  let domain: [number | string, number | string] = ['auto', 'auto']
  if (stats) {
    const span = Math.max(stats.max - stats.min, minSpan)
    const mid = (stats.min + stats.max) / 2
    domain = [mid - span / 2, mid + span / 2]
  }
  // Every chart on the page shares one SVG id namespace, so each needs its own gradient id.
  const gradientId = `fill-${useId().replace(/:/g, '')}`

  return (
    <div className="rounded-xl bg-surface">
      <div className="mt-4 h-18 pointer-events-none select-none">
        {loading && <p className="text-sm text-muted">Loading…</p>}
        {error && <p className="text-sm text-red-700">Could not load history ({error})</p>}
        {!loading && !error && points.length === 0 && (
          <p className="text-sm text-muted">No data in this range yet.</p>
        )}

        {points.length > 0 && (
          <ResponsiveContainer width="100%" height="100%">
            <AreaChart data={points}>
              <defs>
                <linearGradient id={gradientId} x1="0" y1="0" x2="0" y2="1">
                  <stop offset="0%" style={{ stopColor: 'var(--color-accent)', stopOpacity: 0.4 }} />
                  <stop offset="100%" style={{ stopColor: 'var(--color-accent)', stopOpacity: 0 }} />
                </linearGradient>
              </defs>

              <CartesianGrid stroke="var(--color-neutral-900)" strokeWidth={0.8} vertical={false} />
              <XAxis
                dataKey="timestamp"
                type="number"
                scale="time"
                domain={['dataMin', 'dataMax']}
                hide
              />
              <YAxis domain={domain} hide />

              <Area
                type="monotone"
                dataKey={type}
                stroke="var(--color-accent)"
                strokeWidth={1.5}
                fill={`url(#${gradientId})`}
                dot={false}
                isAnimationActive={false}
              />
              {last && (
                <>
                  <ReferenceDot
                    x={last.timestamp}
                    y={last[type]}
                    r={9}
                    fill="var(--color-accent)"
                    fillOpacity={0.25}
                    stroke="none"
                    ifOverflow="visible"
                  />
                  <ReferenceDot
                    x={last.timestamp}
                    y={last[type]}
                    r={4}
                    fill="var(--color-accent)"
                    stroke="var(--color-background)"
                    strokeWidth={2}
                    ifOverflow="visible"
                  />
                </>
              )}
            </AreaChart>
          </ResponsiveContainer>
        )}
      </div>
    </div>
  )
}