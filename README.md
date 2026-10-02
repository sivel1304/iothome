# iothome

Code for a home IoT sensor dashboard. ESP8266 sensor nodes POST readings as JSON to a .NET API, and a React frontend (planned) displays them.

```text
ESP8266 + DHT11  --HTTP POST-->  .NET API  -->  React dashboard
```

## Project structure

| Folder | Description | Status |
| --- | --- | --- |
| [iothome-esp8266](iothome-esp8266/) | Firmware for an ESP8266 (NodeMCU v2) temperature and humidity sensor | Working |
| .NET backend | Receives readings on `POST /api/readings`, stores them, and serves them to the dashboard | Planned |
| React frontend | Dashboard that shows sensor data | Planned |

## ESP8266 sensor

Built with PlatformIO using the ESP8266 RTOS SDK. It reads a DHT11 sensor on GPIO2 (D4 on the NodeMCU) every 5 seconds and sends it to the API:

```http
POST /api/readings
Content-Type: application/json

{"sensorId":"sensor1","temperature":22,"humidity":45}
```

### Setup

1. Copy `iothome-esp8266/src/secrets.example.h` to `iothome-esp8266/src/secrets.h` and fill in your WiFi credentials and the API address (`API_HOST`, `API_PORT`). `secrets.h` is ignored by git.
2. Open `iothome-esp8266` in VS Code with the PlatformIO extension.
3. Build and upload:

   ```sh
   pio run -t upload
   ```

The serial monitor (`pio device monitor`, 115200 baud) prints each request and the HTTP status code it got back.
