import { useEffect, useState } from 'react'
import { subscribe } from './live'
import type { LatestMeasurement } from './types'

export function useLatest(moduleId: string) {
  const [data, setData] = useState<LatestMeasurement | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let cancelled = false

    const load = async () => {
      try {
        const res = await fetch(`/latest-sensor-reading/${encodeURIComponent(moduleId)}`)
        if (!res.ok) throw new Error(`HTTP ${res.status}`)
        const json = (await res.json()) as LatestMeasurement
        if (!cancelled) {
          setData(json)
          setError(null)
        }
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Failed to load')
      }
    }

    load()

    // `load` is also the reconnect handler: after a dropped connection, catch up on anything missed.
    const unsubscribe = subscribe(m => {
      if (m.moduleId === moduleId) {
        setData(m)
        setError(null)
      }
    }, load)

    return () => {
      cancelled = true
      unsubscribe()
    }
  }, [moduleId])

  return { data, error }
}