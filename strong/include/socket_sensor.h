#ifndef _SOCKET_SENSOR_H
#define _SOCKET_SENSOR_H

#include "main.h"
#include "net_status.h"

int ping_status(const char *ip);
void *sensor_main(void* arg);

#endif
