#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "freertos/semphr.h"
#include "esp_common.h"
#include <string.h>
#include "dht.h"
#include "secrets.h"

#include "MQTTESP8266.h"
#include "MQTTClient.h"

#define MQTT_HOST "test.mosquitto.org"
#define MQTT_PORT 1883

static xSemaphoreHandle wifi_alive;

uint32 user_rf_cal_sector_set(void)
{
    flash_size_map size_map = system_get_flash_size_map();
    uint32 rf_cal_sec = 0;

    switch (size_map)
    {
    case FLASH_SIZE_4M_MAP_256_256:
        rf_cal_sec = 128 - 5;
        break;
    case FLASH_SIZE_8M_MAP_512_512:
        rf_cal_sec = 256 - 5;
        break;
    case FLASH_SIZE_16M_MAP_512_512:
    case FLASH_SIZE_16M_MAP_1024_1024:
        rf_cal_sec = 512 - 5;
        break;
    case FLASH_SIZE_32M_MAP_512_512:
    case FLASH_SIZE_32M_MAP_1024_1024:
        rf_cal_sec = 1024 - 5;
        break;
    default:
        rf_cal_sec = 0;
        break;
    }

    return rf_cal_sec;
}

void ICACHE_FLASH_ATTR
user_set_station_config(void)
{
    char ssid[32] = WIFI_SSID;
    char password[64] = WIFI_PASS;
    struct station_config stationConf;

    memset(&stationConf, 0, sizeof(struct station_config));
    stationConf.bssid_set = 0;

    memcpy(&stationConf.ssid, ssid, 32);
    memcpy(&stationConf.password, password, 64);

    wifi_station_set_config(&stationConf);
}

static void ICACHE_FLASH_ATTR
topic_received(MessageData *md)
{
    MQTTMessage *message = md->message;
    printf("MQTT message received: %.*s\n", (int)message->payloadlen, (char *)message->payload);
}

static void ICACHE_FLASH_ATTR
mqtt_task(void *pvParameters)
{
    struct Network network;
    MQTTClient client = DefaultClient;
    unsigned char mqtt_buf[128];
    unsigned char mqtt_readbuf[128];
    MQTTPacket_connectData data = MQTTPacket_connectData_initializer;
    int ret;

    NewNetwork(&network);

    while (1)
    {
        // Wait until wifi is up
        xSemaphoreTake(wifi_alive, portMAX_DELAY);

        printf("Connecting to MQTT broker %s:%d ... ", MQTT_HOST, MQTT_PORT);
        ret = ConnectNetwork(&network, MQTT_HOST, MQTT_PORT);
        if (ret == 0)
        {
            printf("ok.\n");
            NewMQTTClient(&client, &network, 5000, mqtt_buf, sizeof(mqtt_buf), mqtt_readbuf, sizeof(mqtt_readbuf));

            data.willFlag = 0;
            data.MQTTVersion = 3;
            data.clientID.cstring = "esp8266-iothome";
            data.username.cstring = MQTT_USER;
            data.password.cstring = MQTT_PASS;
            data.keepAliveInterval = 10;
            data.cleansession = 1;

            printf("Sending MQTT connect ... ");
            ret = MQTTConnect(&client, &data);
            if (ret == SUCCESS)
            {
                printf("ok.\n");

                uint32_t last_publish_time = system_get_time() - 5000000; // force immediate first publish

                while (1)
                {
                    ret = MQTTYield(&client, 1000);
                    if (ret == DISCONNECTED)
                    {
                        printf("MQTT disconnected\n");
                        break;
                    }

                    if ((system_get_time() - last_publish_time) > 5000000) // 5 seconds, in microseconds
                    {
                        int humidity = 0, temperature = 0;
                        char temp_str[16];
                        char hum_str[16];

                        if (dht11_read(&humidity, &temperature))
                        {
                            snprintf(temp_str, sizeof(temp_str), "%d", temperature);
                            snprintf(hum_str, sizeof(hum_str), "%d", humidity);
                        }
                        else
                        {
                            strcpy(temp_str, "N/A");
                            strcpy(hum_str, "N/A");
                        }

                        MQTTMessage tempMessage = {
                            .qos = QOS1,
                            .retained = 0,
                            .dup = 0,
                            .payload = temp_str,
                            .payloadlen = strlen(temp_str),
                        };

                        MQTTMessage humMessage = {
                            .qos = QOS1,
                            .retained = 0,
                            .dup = 0,
                            .payload = hum_str,
                            .payloadlen = strlen(hum_str),
                        };

                        MQTTPublish(&client, "viggo-home/sensor1/temp", &tempMessage);
                        MQTTPublish(&client, "viggo-home/sensor1/humidity", &humMessage);

                        last_publish_time = system_get_time();
                    }
                }
            }
            else
            {
                printf("failed.\n");
            }
            DisconnectNetwork(&network);
        }
        else
        {
            printf("failed.\n");
        }
        vTaskDelay(1000 / portTICK_RATE_MS);
    }
}

void wifi_event_handler(System_Event_t *event)
{
    if (event == NULL)
    {
        return;
    }

    switch (event->event_id)
    {
    case EVENT_STAMODE_CONNECTED:
        printf("Connected to AP, waiting for IP...\n");
        break;

    case EVENT_STAMODE_GOT_IP:
        printf("Got IP: " IPSTR "\n", IP2STR(&event->event_info.got_ip.ip));
        xSemaphoreGive(wifi_alive);
        break;

    case EVENT_STAMODE_DISCONNECTED:
        printf("Disconnected from AP, reason: %d\n", event->event_info.disconnected.reason);
        wifi_station_connect(); // attempt to reconnect
        break;

    default:
        break;
    }
}

void user_init(void)
{
    uart_div_modify(0, UART_CLK_FREQ / 115200);
    printf("SDK version: %s\n", system_get_sdk_version());

    vSemaphoreCreateBinary(wifi_alive);
    xSemaphoreTake(wifi_alive, 0);

    wifi_set_opmode(STATION_MODE);
    user_set_station_config();
    wifi_set_event_handler_cb(wifi_event_handler);

    wifi_station_connect();

    xTaskCreate(mqtt_task, "mqtt_task", 1024, NULL, 2, NULL);
    // xTaskCreate(dht_task, "dht_task", 1024, NULL, 2, NULL);
}
