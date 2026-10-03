# iothome

Home IoT sensor dashboard. An ESP8266 sensor node publishes temperature and humidity readings over MQTT, a .NET API subscribes to them and stores them in SQLite, and a React dashboard charts them.

```text
ESP8266 + DHT11  --MQTT-->  test.mosquitto.org  --MQTT-->  .NET API (SQLite)  <--HTTP--  React dashboard
                 viggo-home/dht11                         GET /dht11
```

## Project structure

| Folder | Description |
| --- | --- |
| [firmware/iothome-esp8266](firmware/iothome-esp8266/) | ESP8266 (NodeMCU v2) firmware that reads a DHT11 and publishes over MQTT |
| [backend/IotHomeAPI](backend/IotHomeAPI/) | ASP.NET Core API: MQTT listener, SQLite storage (EF Core), `GET /dht11` |
| [backend/IotHomeAPI.Tests](backend/IotHomeAPI.Tests/) | xUnit tests for payload parsing and reading validation |
| [frontend](frontend/) | React + TypeScript + Vite dashboard (Recharts) |

## Firmware

Built with PlatformIO on the ESP8266 RTOS SDK. Every 5 seconds it reads the DHT11 on GPIO2 (D4 on the NodeMCU) and publishes to `viggo-home/dht11` on `test.mosquitto.org:1883`:

```json
{"temperature":22,"humidity":45}
```

If the sensor read fails it publishes `{"error":"failed to read DHT11"}` instead.

### Setup

1. Copy `firmware/iothome-esp8266/src/secrets.example.h` to `src/secrets.h` and fill in your WiFi credentials (and MQTT credentials if your broker needs them). `secrets.h` is ignored by git.
2. Open `firmware/iothome-esp8266` in VS Code with the PlatformIO extension.
3. Build and upload, then watch the serial output (115200 baud):

   ```sh
   pio run -e nodemcuv2 -t upload
   pio device monitor
   ```

Unit tests for the hardware-independent logic (`lib/dht_logic`) run on the host:

```sh
pio test -e native
```

## Backend

Requires the .NET 10 SDK.

The `MqttListenerService` subscribes to `viggo-home/#`, parses each payload, rejects readings outside the DHT11 range (0–50 °C, 20–90 % RH), and saves valid ones to `iothome.db`. The topic is stored as the sensor ID.

```sh
cd backend/IotHomeAPI
dotnet ef database update   # creates iothome.db (first run only)
dotnet run                  # http://localhost:5195
```

| Endpoint | Description |
| --- | --- |
| `GET /dht11` | All stored readings, newest first |

Run the tests:

```sh
dotnet test backend/IotHomeAPI.Tests
```

## Frontend

Requires Node.js.

```sh
cd frontend
npm install
npm run dev
```

The Vite dev server proxies `/dht11` to the backend on `http://localhost:5195`, so start the backend first. The dashboard polls for new readings every 5 seconds.

## CI

[.github/workflows/ci.yml](.github/workflows/ci.yml) runs the .NET tests and the PlatformIO native tests on every push to `main` and on pull requests.

## Note

`test.mosquitto.org` is a public broker, so anyone can read or publish to the `viggo-home/` topics. Use a private broker for anything beyond testing.
