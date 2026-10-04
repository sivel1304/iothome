export type DhtReading = {
  sensorId: string
  temperature: number
  humidity: number
  timestamp: string
}

export type MeasurementKind = 'temperature' | 'humidity' | 'soil' | 'pressure';

export interface Measurement {
  key: string;
  kind: MeasurementKind;
  label: string;
  unit: string;
  decimals: number;
  /** Water readings render in aqua, everything else in green. */
  water?: boolean;
  /** Minimum visible span on the y-axis, so flat signals don't look noisy. */
  minSpan?: number;
  /** Last 3 h at 5-min resolution (36 points), oldest first. */
  history?: number[];
  /** Last 24 h at 15-min resolution (96 points). */
  history24h?: number[];
  /** Last 7 d at 2-h resolution (84 points). */
  history7d?: number[];
}

export interface SensorModule {
  id: string;
  name: string;
  sensor: string;
  address: string;
  interval: string;
  /** 0–100 */
  battery: number;
  voltage: number;
  daysLeft: number;
  rssi: number;
  firmware: string;
  uptime: string;
  online: boolean;
  /** Epoch ms of the last packet. */
  updatedAt: number;
  measurements: Measurement[];
}