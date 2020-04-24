#ifndef _SOCKET_SINK_H
#define _SOCKET_SINK_H

#include <stdio.h>
#include <fcntl.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <sys/types.h>
#include <sys/wait.h>

int ping_status(const char *ip);

#endif
