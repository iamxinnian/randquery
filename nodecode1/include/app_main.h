#ifndef _APP_MAIN_H
#define _APP_MAIN_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdarg.h>

#include "aos/cli.h"
#include "aos/kernel.h"
#include "ulog/ulog.h"
#include "aos/yloop.h"

#include <hal/wifi.h>

#include "netmgr.h"
#include "linkkit/infra/infra_compat.h"
#include "network.h"

typedef struct sys_time
{
	int year;
	char month;
	char day;
	char hour;
	char minutes;
	char seconds;
} system_time;
//定义设备ID
#define DEVID "8f654a2d1182"
//控制传感器采集数据位数
#define DATA_BIT_NUM 16

#define SENSOR_SAMPLE_TIME 400 /* sensor sampling period is 1000 ms */
#define DATA_DIMENSION 4 /* sensor 采集维数 */

//每个周期每维数据采集多少次
#define SENSOR_DATA_N 10
//每维数据每周期缓存大小byte
#define SENSOR_DATA_LEN (8*SENSOR_DATA_N)

#endif