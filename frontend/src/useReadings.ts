import { useEffect, useState } from 'react'
import type { SensorModule } from './types'
import type { DhtReading } from './types'

const toDate = (ts: string) => new Date(ts.endsWith('Z') ? ts : ts + 'Z')

export function useReadings(limit = 50, intervalMs = 5000) {
  const [readings, setReadings] = useState<DhtReading[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const load = async () => {
      try {
        const res = await fetch(`/dht11?limit=${limit}`)
        if (!res.ok) throw new Error(`HTTP ${res.status}`)
        const data: DhtReading[] = await res.json()
        setReadings(data.reverse()) 
        setError(null)
      } catch (e) {
        setError(e instanceof Error ? e.message : 'Failed to load')
      }
    }

    load()
    const id = setInterval(load, intervalMs)
    return () => clearInterval(id)
  }, [limit, intervalMs])

  return { readings, error, toDate }
}

