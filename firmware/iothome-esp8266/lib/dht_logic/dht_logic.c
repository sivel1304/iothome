#include "dht_logic.h"
#include <stdio.h>

bool dht11_checksum_ok(const uint8_t data[5])
{
    return (uint8_t)(data[0] + data[1] + data[2] + data[3]) == data[4];
}

bool dht11_values_in_range(int humidity, int temperature)
{
    return humidity >= 20 && humidity <= 90 &&
           temperature >= 0 && temperature <= 50;
}

int dht11_format_payload(char *buf, size_t size, int temperature, int humidity, int interval)
{
    return snprintf(buf, size, "{\"readings\":{\"temperature\":%d,\"humidity\":%d},\"interval\":%d}", temperature, humidity, interval);
}