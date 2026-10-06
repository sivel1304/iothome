import { useEffect, useState } from 'react'
import { subscribe } from './live'
import { toDate } from './utils'
import type { HistoryMeasurement, HistoryRow, ReadingValue } from './types'

const toRow = (timestamp: string, readings: ReadingValue[]): HistoryRow => ({
  timestamp: toDate(timestamp).getTime(),
  ...Object.fromEntries(readings.map(r => [r.type, r.value])),
})

export function useHistory(moduleId: string, hours = 3) {
  const [rows, setRows] = useState<HistoryRow[]>([])
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    let cancelled = false

    const load = async () => {
      try {
        const res = await fetch(
          `/modules/${encodeURIComponent(moduleId)}/history?hours=${hours}`
        )
        if (!res.ok) throw new Error(`HTTP ${res.status}`)
        const json = (await res.json()) as HistoryMeasurement[]
        if (!cancelled) {
          setRows(json.map(m => toRow(m.timestamp, m.readings)))
          setError(null)
        }
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Failed to load')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    setLoading(true)
    load()

    const unsubscribe = subscribe(m => {
      if (m.moduleId !== moduleId) return
      const cutoff = Date.now() - hours * 3_600_000
      setRows(prev => [...prev.filter(r => r.timestamp >= cutoff), toRow(m.timestamp, m.readings)])
    }, load)

    return () => {
      cancelled = true
      unsubscribe()
    }
  }, [moduleId, hours])

  return { rows, loading, error }
}