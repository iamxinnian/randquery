/*
 * Copyright (C) 2015-2018 Alibaba Group Holding Limited
 */

#include "app_main.h"
#include "tcp_net.h"
#include "diffie_hellman.h"
#include "work_core.h"
#include "sensor_task.h"

uint32_t random_t=0;
aos_timer_t g_timer;
aos_timer_t systime_timer;
system_time this_now;
extern char *tcp_send_buffer;
extern char *tcp_recv_buffer;

extern aos_mutex_t mutex_send;
extern aos_mutex_t mutex_recv;

static void random_timer_handler(void)
{
    random_t++;
}

static void systime_calc(void)
{
    this_now.seconds++;
    if (this_now.seconds >= 60)
    {
        this_now.seconds = 00;
        this_now.minutes++;
    }

    if (this_now.minutes >= 60)
    {
        this_now.minutes = 0;
        this_now.hour++;
    }

    if (this_now.hour >= 24)
    {
        this_now.hour = 0;
        this_now.day++;
    }

    if (this_now.month == 1 || this_now.month == 3 || this_now.month == 5 || this_now.month == 7 || this_now.month == 8 || this_now.month == 10 || this_now.month == 12)
    {
        if (this_now.day >= 32)
        {
            this_now.day = 1;
            this_now.month++;
        }
    }
    else if (this_now.month == 2)
    {
        if ((this_now.year % 4 == 0 && this_now.year % 100 != 0) || this_now.year % 400 == 0)
        {
            if (this_now.day >= 30)
            {
                this_now.day = 1;
                this_now.month++;
            }
        }else{
            if (this_now.day >= 29)
            {
                this_now.day = 1;
                this_now.month++;
            }
        }
    }
    else{
        if (this_now.day >= 31)
        {
            this_now.day = 1;
            this_now.month++;
        }
    }

    if (this_now.month >= 13)
    {
        this_now.month = 1;
        this_now.year++;
    }
}

int application_start(int argc, char **argv)
{
    aos_timer_new(&g_timer,random_timer_handler,NULL,10,1);
    aos_timer_new_ext(&systime_timer,systime_calc,NULL,999,1,0);

    tcp_send_buffer=(char *)aos_calloc(2000,1);
    tcp_recv_buffer=(char *)aos_calloc(400,1);

    sal_add_dev(NULL, NULL);
    sal_init();

    aos_set_log_level(AOS_LL_DEBUG);
    IOT_SetLogLevel(IOT_LOG_DEBUG);

    uint8_t ret = aos_mutex_new(&mutex_send);
    if (ret != 0) {
        return;
    }

    ret = aos_mutex_new(&mutex_recv);
    if (ret != 0) {
        return;
    }

    aos_task_new("netmgr", start_netmgr, NULL, 4096);
    aos_task_new("tcpsend", tcp_send_task, NULL, 4096);
    aos_task_new("work_core", work_core_task, NULL, 4096);
    aos_task_new("work_sensor",sensor_local_task,NULL, 4096);
    aos_loop_run();
    return 0;
}
