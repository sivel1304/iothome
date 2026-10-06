import { LineChart, Line, XAxis, YAxis, Tooltip, CartesianGrid, ResponsiveContainer } from 'recharts'

export default function HistoryChart() {

  return (
    <div className="rounded-xl bg-surface">
      <div className="mt-4 h-20">
        {/* {loading && <p className="text-sm text-muted">Loading…</p>}
        {error && <p className="text-sm text-muted">Could not load history ({error})</p>}
        {!loading && !error && points.length === 0 && (
          <p className="text-sm text-muted">No data in this range yet.</p>
        )} */}

        {(
          <ResponsiveContainer width="100%" height="100%">
            <LineChart /* data={points} */>
              <CartesianGrid stroke="var(--color-border)" vertical={false} />
              <XAxis
                dataKey="t"
                type="number"
                scale="time"
                domain={['dataMin', 'dataMax']}
                /* tickFormatter={fmtTick} */
                stroke="var(--color-muted)"
                fontSize={12}
              />
              <YAxis
                domain={['auto', 'auto']}
                tickFormatter={v => `${v}`}
                stroke="var(--color-muted)"
                fontSize={12}
                width={40}
              />
              <Tooltip
                labelFormatter={t => new Date(Number(t)).toLocaleString()}
/*                 formatter={v => [fmtValue(Number(v)), meta?.label ?? type]}
 */                contentStyle={{
                  background: 'var(--color-surface)',
                  border: '1px solid var(--color-border)',
                  color: 'var(--color-foreground)',
                }}
              />
              <Line
                type="monotone"
                dataKey="value"
                stroke="var(--color-accent)"
                strokeWidth={2}
                dot={false}
                isAnimationActive={false}
              />
            </LineChart>
          </ResponsiveContainer>
        )}
      </div>
    </div>
  )
}