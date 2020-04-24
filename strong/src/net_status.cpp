#include "net_status.h"

//测试网络是否通常
int ping_status(const char *ip)
{
	int i, status;
	pid_t pid;
	for (i = 0; i < 3; ++i)
	{
		if ((pid = vfork()) < 0)// 新建一个进程来执行ping命令
		{
			printf("vfork():error=%d:%s\n",errno,strerror(errno));
			continue;
		}

		if (pid == 0)
		{
			char pingtmp[50]={0};
			sprintf(pingtmp,"ping -c 1 %s >/dev/null 2>&1",ip);
			if ( execlp("/bin/sh", "/bin/sh", "-c", pingtmp,(char*)0) < 0)// 执行ping命令
			{
				printf("execlp():error=%d:%s\n",errno,strerror(errno));
				exit(1);
			}
		}
		waitpid(pid, &status, 0);

		if (status == 0)// 相等说明正常
			return 0;
	}
	return -1;
}
