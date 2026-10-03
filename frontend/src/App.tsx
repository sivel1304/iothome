import { LineChart, Line, XAxis, YAxis, Tooltip, Legend, ResponsiveContainer } from 'recharts'
import { useReadings } from './useReadings'

export default function App() {
  const { readings, error, toDate } = useReadings()

  const data = readings.map(r => ({
    time: toDate(r.timestamp).toLocaleTimeString(),
    temperature: r.temperature,
    humidity: r.humidity,
  }))

  const latest = readings.at(-1)

  return (
    <div style={{ maxWidth: 900, margin: '2rem auto', padding: '0 1rem' }}>
      <h1>IoT Home</h1>
      {error && <p style={{ color: 'crimson' }}>API error: {error}</p>}

      {latest && (
        <p>
          Latest ({latest.sensorId}): {latest.temperature}°C, {latest.humidity}%
        </p>
      )}

      <div style={{ height: 320 }}>
        <ResponsiveContainer>
          <LineChart data={data}>
            <XAxis dataKey="time" />
            <YAxis />
            <Tooltip />
            <Legend />
            <Line type="monotone" dataKey="temperature" stroke="#e4572e" dot={false} />
            <Line type="monotone" dataKey="humidity" stroke="#2e86e4" dot={false} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  )
}