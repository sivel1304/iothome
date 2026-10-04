#include "dht.h"

static void dht_set_output(void)
{
    gpio_output_set(0, 0, DHT_GPIO_MASK, 0); // enable as output
}

static void dht_set_input(void)
{
    gpio_output_set(0, 0, 0, DHT_GPIO_MASK); // disable output = act as input
}

static void dht_write(uint8_t level)
{
    if (level)
    {
        gpio_output_set(DHT_GPIO_MASK, 0, 0, 0);
    }
    else
    {
        gpio_output_set(0, DHT_GPIO_MASK, 0, 0);
    }
}

static uint8_t dht_read(void)
{
    return (gpio_input_get() & DHT_GPIO_MASK) ? 1 : 0;
}

static int wait_for_level(uint8_t level, uint32_t timeout_us)
{
    uint32_t start = system_get_time();
    while (dht_read() != level)
    {
        if ((system_get_time() - start) > timeout_us)
        {
            return -1;
        }
    }
    return (int)(system_get_time() - start);
}

bool dht11_read(int *humidity, int *temperature)
{
    uint8_t data[5] = {0, 0, 0, 0, 0};

    PIN_FUNC_SELECT(DHT_GPIO_MUX, DHT_GPIO_FUNC);

    dht_set_output();
    dht_write(1);
    os_delay_us(50);

    dht_write(0);
    os_delay_us(20000);
    dht_write(1);
    os_delay_us(30);

    dht_set_input();

    if (wait_for_level(0, 100) < 0)
    {
        printf("DHT11: no response (timeout waiting for low)\n");
        return false;
    }
    if (wait_for_level(1, 100) < 0)
    {
        printf("DHT11: no response (timeout waiting for high)\n");
        return false;
    }
    if (wait_for_level(0, 100) < 0)
    {
        printf("DHT11: bad response (timeout waiting for data start)\n");
        return false;
    }

    for (int i = 0; i < 40; i++)
    {
        if (wait_for_level(1, 100) < 0)
        {
            printf("DHT11: timeout during bit %d (rising edge)\n", i);
            return false;
        }

        // Measure how long the line stays high — short = 0, long = 1
        int duration = wait_for_level(0, 100);
        if (duration < 0)
        {
            printf("DHT11: timeout during bit %d (falling edge)\n", i);
            return false;
        }

        uint8_t bit = (duration > 40) ? 1 : 0;
        data[i / 8] <<= 1;
        data[i / 8] |= bit;
    }

    if (!dht11_checksum_ok(data))
    {
        uint8_t sum = (uint8_t)(data[0] + data[1] + data[2] + data[3]);
        printf("DHT11: checksum failed (computed %d, expected %d)\n", sum, data[4]);
        printf("DHT11 raw: %d %d %d %d %d\n", data[0], data[1], data[2], data[3], data[4]);
        return false;
    }

    *humidity = data[0];    // DHT11 has no decimal component, data[1] is always 0
    *temperature = data[2]; // data[3] is always 0 on DHT11

    if (!dht11_values_in_range(*humidity, *temperature))
    {
        printf("DHT11: values out of range\n");
        return false;
    }

    return true;
}