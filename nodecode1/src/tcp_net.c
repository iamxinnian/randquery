#include "tcp_net.h"

uint8_t net_status_flag=0;
uint8_t tcp_connect_flag=0;
uint8_t tcp_send_flag=0;

aos_mutex_t mutex_send;
aos_mutex_t mutex_recv;
extern uint8_t join_server_flag;

char *tcp_send_buffer=NULL;
char *tcp_recv_buffer=NULL;
char gwip[16]={0};


void wifi_service_event(input_event_t *event, void *priv_data)
{
    if (event->type != EV_WIFI) {
        return;
    }

    if (event->code == CODE_ON_AP_COSE) {
        net_status_flag=0;
    }

    if (event->code != CODE_WIFI_ON_GOT_IP) {
        return;
    }
    netmgr_ap_config_t config;
    hal_wifi_ip_stat_t ip_stat;
    memset(&config, 0, sizeof(netmgr_ap_config_t));
    netmgr_get_ap_config(&config);

    memset(&ip_stat, 0, sizeof(ip_stat));
    hal_wifi_get_ip_stat(NULL, &ip_stat, STATION);
    LOG("wifi_service_event config.ssid:%s,gw:%s", config.ssid,ip_stat.gate);
    memcpy(gwip,ip_stat.gate,16);
    net_status_flag=1;
}

void start_netmgr(void *p)
{
    netmgr_init();

    aos_register_event_filter(EV_WIFI, wifi_service_event, NULL);
    netmgr_start(true);
    aos_task_exit(0);
}

int tcp_send_task(void)
{
    while(1)
    {
        int  ret = 0;
        static int  fd = -1;
        static int err_conn_num=0;
        char *pcdestport ="8181";

        if(tcp_connect_flag==0 && net_status_flag==1)
        {
            struct sockaddr_in addr;
            struct timeval timeout;
            memset(&addr, 0, sizeof(addr));

            addr.sin_addr.s_addr = inet_addr(gwip);

            if (IPADDR_NONE == addr.sin_addr.s_addr){
                printf("invalid input addr info %s \r\n", gwip);
                continue;
            }

            addr.sin_port = htons((short)atoi(pcdestport));

            if (0 == addr.sin_port){
                printf("invalid input port info %s \r\n", pcdestport);
                continue;
            }
            
            addr.sin_family = AF_INET;

            if(fd>=0)
            {
                close(fd);
                fd = -1;
            }
            fd = socket(AF_INET,SOCK_STREAM,0);
            if (fd < 0){
                printf("fail to creat socket errno = %d \r\n", errno);
                continue;
            }

            if (setsockopt (fd, SOL_SOCKET, SO_RCVTIMEO, (char *)&timeout,
                        sizeof(timeout)) < 0) {
                printf("setsockopt failed, errno: %d\r\n", errno);
                close(fd);
                continue;
            }

            if (connect(fd, (struct sockaddr*)&addr, sizeof(addr)) != 0) {
                printf("Connect failed, errno = %d, ip %s port %s \r\n", errno, gwip, pcdestport);
                if(err_conn_num>35)
                {
                    aos_reboot();
                }else{
                    printf("err_conn_num=%d\r\n",err_conn_num);
                    close(fd);
                    err_conn_num++;
                } 
                aos_msleep(500);
                continue;
            }

            tcp_connect_flag=1;
        }
    
        if(net_status_flag && tcp_connect_flag)
        {
            if(tcp_send_flag)
            {
                aos_mutex_lock(&mutex_send,AOS_WAIT_FOREVER);
                int send_len=strlen(tcp_send_buffer);
                /************当数据包大于1024byte进行分包发送，否则一次性发送*************/
                if(send_len>1024)
                {
                    char pack_num=send_len/1024+1;//计算分几个包能把数据完整发送出去
                    int send_len_each=1024;
                    for(char i=0;i<pack_num;i++)//循环发发送每个分包
                    {
                        if(i==pack_num-1)
                        {
                            send_len_each=send_len-1024*(pack_num-1);
                        }
                        if ((ret = send(fd, tcp_send_buffer+i*1024,send_len_each, 0)) <= 0)
                        {
                            printf("send data failed, errno = %d. for the %d time \r\n", errno, time);
                            tcp_connect_flag=0;
                            close(fd);
                            aos_mutex_unlock(&mutex_send);
                            continue;
                        }else{
                            tcp_send_flag=0;
                        }
                    }
                }else{
                    if ((ret = send(fd, tcp_send_buffer,send_len, 0)) <= 0)
                    {
                        printf("send data failed, errno = %d. for the %d time \r\n", errno, time);
                        tcp_connect_flag=0;
                        close(fd);
                        aos_mutex_unlock(&mutex_send);
                        continue;
                    }else{
                        tcp_send_flag=0;
                    }
                }
                aos_mutex_unlock(&mutex_send);
            }
            struct timeval timeout={1,0};//10s
            int ret=setsockopt(fd,SOL_SOCKET,SO_RCVTIMEO,(const char*)&timeout,sizeof(timeout));
            if(ret)
            {
                printf("setsockopt():error");
            }

            aos_mutex_lock(&mutex_recv,AOS_WAIT_FOREVER);
            int len=recv(fd,tcp_recv_buffer,300,0);
            aos_mutex_unlock(&mutex_recv);

            if(len>0)
            {   
                //接受到数据代码区
            }else if(len==-1){
                
            }else{
                //断开连接和异常处理代码区
                printf("disconnect!\n");
                tcp_connect_flag=0;
                close(fd);
                continue;
            }
        }
        aos_msleep(5);
    }
}
