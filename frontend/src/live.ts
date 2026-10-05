import * as signalR from '@microsoft/signalr'
import type { LatestMeasurement } from './types'

type MeasurementHandler = (m: LatestMeasurement) => void

const measurementHandlers = new Set<MeasurementHandler>()
const reconnectHandlers = new Set<() => void>()

const connection = new signalR.HubConnectionBuilder()
  .withUrl('/hubs/sensors')
  .withAutomaticReconnect()
  .build()

connection.on('measurement', (m: LatestMeasurement) => {
  measurementHandlers.forEach(h => h(m))
})
connection.onreconnected(() => reconnectHandlers.forEach(h => h()))

// Automatic reconnect only covers a connection that was already up, so retry the first start.
async function start() {
  try {
    await connection.start()
  } catch {
    setTimeout(start, 5000)
  }
}
let started = false

export function subscribe(onMeasurement: MeasurementHandler, onReconnect?: () => void) {
  if (!started) {
    started = true
    start()
  }
  measurementHandlers.add(onMeasurement)
  if (onReconnect) reconnectHandlers.add(onReconnect)

  return () => {
    measurementHandlers.delete(onMeasurement)
    if (onReconnect) reconnectHandlers.delete(onReconnect)
  }
}