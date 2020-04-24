#ifndef _GATEWAY_H
#define _GATEWAY_H

#include <stdio.h>
#include <stdlib.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <errno.h>
#include <unistd.h>
#include <string.h>
#include <pthread.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <arpa/inet.h>
#include <netinet/in.h>
#include "cJSON.h"
#include "JSON_checker.h"
#include "sqlite3.h"

typedef struct socket_stat{
	int socket_fd;
	char ip_addr_buf[INET_ADDRSTRLEN+1];
} socket_status;


#endif
