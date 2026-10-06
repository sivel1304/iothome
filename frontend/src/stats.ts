import type { HistoryRow } from './types'

export type SeriesStats = {
  min: number
  max: number
  first: number
  last: number
  delta: number
}

export function seriesStats(rows: HistoryRow[], type: string): SeriesStats | null {
  let min = Infinity
  let max = -Infinity
  let first: number | null = null
  let last = 0

  for (const r of rows) {
    const v = r[type]
    if (typeof v !== 'number') continue
    if (first === null) first = v
    last = v
    if (v < min) min = v
    if (v > max) max = v
  }

  if (first === null) return null
  return { min, max, first, last, delta: last - first }
}