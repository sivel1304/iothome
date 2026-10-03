#ifndef DHT_H
#define DHT_H

#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "esp_common.h"
#include "gpio.h"
#include "eagle_soc.h"
#include <string.h>
#include "dht_logic.h"


#define DHT_GPIO        2
#define DHT_GPIO_MUX    PERIPHS_IO_MUX_GPIO2_U
#define DHT_GPIO_FUNC   FUNC_GPIO2
#define DHT_GPIO_MASK   (1 << DHT_GPIO)

static void dht_set_output(void);
static void dht_set_input(void);
static void dht_write(uint8_t level);
static uint8_t dht_read(void);
static int wait_for_level(uint8_t level, uint32_t timeout_us);
bool dht11_read(int *humidity, int *temperature);


#endif