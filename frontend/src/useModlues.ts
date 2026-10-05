import { useCallback, useEffect, useRef, useState } from 'react'
import { subscribe } from './live'
import type { Module } from './types'

export function useModules() {
  const [modules, setModules] = useState<Module[] | null>(null)
  const [error, setError] = useState<string | null>(null)
  const known = useRef(new Set<string>())

  const load = useCallback(async () => {
    try {
      const res = await fetch('/modules')
      if (!res.ok) throw new Error(`HTTP ${res.status}`)
      const json = (await res.json()) as Module[]
      known.current = new Set(json.map(m => m.id))
      setModules(json)
      setError(null)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load')
    }
  }, [])

  useEffect(() => {
    load()
    return subscribe(m => {
      if (!known.current.has(m.moduleId)) load()   // a module we haven't seen yet
    }, load)
  }, [load])

  return { modules, error }
}