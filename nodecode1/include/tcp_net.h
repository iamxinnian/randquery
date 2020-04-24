#ifndef _TCPNET_H
#define _TCPNET_H

#include "app_main.h"

void wifi_service_event(input_event_t *event, void *priv_data);
void start_netmgr(void *p);
int tcp_send_task(void);

#endif