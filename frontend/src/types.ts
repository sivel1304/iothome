export type ReadingValue = {
  id: number
  modulePayloadId: number
  type: string          // "temperature", "humidity", ...
  value: number
}

export type SensorReading = {
  id: number
  moduleId: string
  interval: number      // ms between publishes
  timestamp: string
  readings: ReadingValue[]
}

export interface SensorModule {
  id: string;
  name?: string;
  sensor?: string;
  address?: string;
  interval: string;
  /** 0–100 */
  battery?: number;
  voltage?: number;
  daysLeft: number;
  rssi?: number;
  firmware?: string;
  uptime?: string;
  online?: boolean;
  /** Epoch ms of the last packet. */
  updatedAt?: number;
  readings: SensorReading[];
}