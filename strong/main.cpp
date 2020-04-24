#include "main.h"
#include "socket_sink.h"
#include "socket_sensor.h"

#define GATEWAY_PORT 8181
#define MAX_LINK_NUM 253

extern int errno;
int sink_socket=-1;
int sink_num=0;

int main(void)
{
	struct sockaddr_in server_socket;
	pthread_t thread_id;
	int tmp=0;

	pthread_create(&thread_id, NULL, &sink_main,static_cast<void *>(&tmp));//开辟线程处理sink节点TCP数据
	pthread_detach(thread_id);
	int sockt_s=socket(AF_INET,SOCK_STREAM,0);
	if(sockt_s<0)
	{
		printf("socket():error=%d:%s\n",errno,strerror(errno));
	}
	bzero(&server_socket,sizeof(server_socket));
	server_socket.sin_family=AF_INET;
	server_socket.sin_addr.s_addr=htonl(INADDR_ANY);
	server_socket.sin_port=htons(GATEWAY_PORT);
	if(bind(sockt_s,(struct sockaddr*)&server_socket,sizeof(struct sockaddr_in))<0)
	{
		printf("bind():error=%d:%s\n",errno,strerror(errno));
		close(sockt_s);
		return -1;
	}

	if(listen(sockt_s,MAX_LINK_NUM)<0)
	{
		printf("listen():error=%d:%s\n",errno,strerror(errno));
		close(sockt_s);
		return -2;
	}
	printf("server socket success!\n");

	while(1)
	{
		socklen_t len=0;
		struct sockaddr_in socket_addr;
		int client_socket=accept(sockt_s,(struct sockaddr*)&socket_addr,&len);
		if(client_socket<0)
		{
			printf("accept():error=%d:%s\n",errno,strerror(errno));
			return -3;
		}
		//获取连接上的客户端信息
		//设置接收超时为1s
		struct timeval timeout={1,0};//10s
		int recvbuff = 5*1024*1024;
		int ret=setsockopt(client_socket,SOL_SOCKET,SO_RCVTIMEO,(const char*)&timeout,sizeof(timeout));
		ret &=setsockopt(client_socket,SOL_SOCKET,SO_RCVBUF,(const char*)&recvbuff,sizeof(int));
		if(ret)
		{
			printf("setsockopt():error=%d:%s\n",errno,strerror(errno));
		}
		char buf_ip[INET_ADDRSTRLEN+1];
		memset(buf_ip,'\0',sizeof(buf_ip)+1);
		struct sockaddr_in client_addr;
		memset(&client_addr,'\0',sizeof(struct sockaddr_in));
		socklen_t client_addr_len=sizeof(client_addr);
		getpeername(client_socket,(struct sockaddr *)&client_addr,&client_addr_len);
		inet_ntop(AF_INET,&client_addr.sin_addr,buf_ip,sizeof(buf_ip));
		printf("get connect,ip is%s;port=%d\n",buf_ip,ntohs(client_addr.sin_port));
		//将子线程所用到的客户端信息封装在自定义结构体
		socket_status *socket_client_status=(socket_status *)malloc(sizeof(socket_status));
		memset(socket_client_status,'\0',sizeof(socket_status));
		socket_client_status->socket_fd=client_socket;
		strcpy(socket_client_status->ip_addr_buf,buf_ip);
		//根据IP段，判断客户端的类型，
		if(strstr(buf_ip,"192.168.3")!=0)
		{
			//传感节点处理函数
			pthread_create(&thread_id, NULL, &sensor_main, static_cast<void *>(socket_client_status));
		}else{
			;
		}
		pthread_detach(thread_id);
	}
	close(sockt_s);
	return 0;
}
