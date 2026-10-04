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