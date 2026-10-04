#ifndef DHT_LOGIC_H
#define DHT_LOGIC_H

#include <stdint.h>
#include <stdbool.h>
#include <stddef.h>

bool dht11_checksum_ok(const uint8_t data[5]);
bool dht11_values_in_range(int humidity, int temperature);
int  dht11_format_payload(char *buf, size_t size, int temperature, int humidity, int interval);

#endif