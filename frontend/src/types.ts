export type Module = {
  id: string
  name: string | null
  sensorType: string | null
  intervalMs: number
  lastSeen: string
}

export type ReadingValue = {
  type: string      // "temperature", "humidity", "battery_voltage", "battery_level"
  value: number
}

export type LatestMeasurement = {
  moduleId: string
  timestamp: string
  readings: ReadingValue[]
}

export type HistoryMeasurement = {
  timestamp: string
  readings: ReadingValue[]
}

// One row per measurement: { timestamp: 1760000000000, temperature: 22, humidity: 47 }
export type HistoryRow = { timestamp: number; [type: string]: number }