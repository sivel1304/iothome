#include <unity.h>
#include <string.h>
#include "dht_logic.h"

void setUp(void) {}
void tearDown(void) {}

void test_checksum_accepts_valid_frame(void)
{
    uint8_t data[5] = {55, 0, 22, 0, 77};   // 55+0+22+0 = 77
    TEST_ASSERT_TRUE(dht11_checksum_ok(data));
}

void test_checksum_rejects_corrupted_frame(void)
{
    uint8_t data[5] = {55, 0, 22, 0, 78};
    TEST_ASSERT_FALSE(dht11_checksum_ok(data));
}

void test_range_accepts_typical_values(void)
{
    TEST_ASSERT_TRUE(dht11_values_in_range(55, 22));
}

void test_range_rejects_out_of_range_values(void)
{
    TEST_ASSERT_FALSE(dht11_values_in_range(19, 22));   // humidity too low
    TEST_ASSERT_FALSE(dht11_values_in_range(91, 22));   // humidity too high
    TEST_ASSERT_FALSE(dht11_values_in_range(55, -1));   // temp too low
    TEST_ASSERT_FALSE(dht11_values_in_range(55, 51));   // temp too high
}

void test_range_rejects_all_zeros(void)
{
    // the bug you hit earlier: a bad decode passes the checksum (0 == 0)
    uint8_t zeros[5] = {0, 0, 0, 0, 0};
    TEST_ASSERT_TRUE(dht11_checksum_ok(zeros));          // checksum alone is fooled
    TEST_ASSERT_FALSE(dht11_values_in_range(0, 0));      // the range check catches it
}

void test_payload_format(void)
{
    char buf[64];
    dht11_format_payload(buf, sizeof(buf), 25, 55, 5000);
    TEST_ASSERT_EQUAL_STRING("{\"readings\":{\"temperature\":25,\"humidity\":55},\"interval\":5000}", buf);
}

int main(void)
{
    UNITY_BEGIN();
    RUN_TEST(test_checksum_accepts_valid_frame);
    RUN_TEST(test_checksum_rejects_corrupted_frame);
    RUN_TEST(test_range_accepts_typical_values);
    RUN_TEST(test_range_rejects_out_of_range_values);
    RUN_TEST(test_range_rejects_all_zeros);
    RUN_TEST(test_payload_format);
    return UNITY_END();
}