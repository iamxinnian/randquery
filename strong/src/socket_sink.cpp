#include "socket_sink.h"
#include <sys/time.h>
#include <semaphore.h>

#define SINK_IPADDR "192.168.3.102"
#define SINK_PORT 8989
extern int errno;
extern int sink_socket;
extern int sink_num;
static char recv_finsh=0;
static sqlite3 * mydb=0;
pthread_mutex_t socket_mutex;
sem_t sem_pthread;

extern int ping_status(const char *ip);
int sink_data_deal(char *buf,int len,char *pri_data,char *pack_tmp);
void private_data_query(char* arg_buf);

void *sink_main(void *arg)
{
	char *pri_data=(char *)calloc(50,1024);
	char *pack_tmp=(char *)calloc(40,1024);
	char *buf=(char *)calloc(1026,1);
	void *return_erro;
	time_t timep;
	struct tm *p;
	memset(pri_data,0,40960);
	memset(pack_tmp,0,20*1024);

	if (pthread_mutex_init(&socket_mutex, NULL)!= 0)//初始化互斥锁
	{
		printf("socket mutex error！\n");
	}
	sem_init(&sem_pthread, 0, 1);

	while(1)
	{
		if(sink_num==0)
		{
			struct sockaddr_in sink_add_inf;
			sink_socket = socket(AF_INET, SOCK_STREAM, 0);
			if(sink_socket<0)
			{
				printf("socket():error=%d:%s\n",errno,strerror(errno));
			}
			bzero(&sink_add_inf,sizeof(sink_socket));
			sink_add_inf.sin_family=AF_INET;
			sink_add_inf.sin_addr.s_addr=inet_addr(SINK_IPADDR);;
			sink_add_inf.sin_port=htons(SINK_PORT);

			if (connect(sink_socket, (struct sockaddr*)&sink_add_inf, sizeof(struct sockaddr)) != 0) {
				printf("connect() to %s:error=%d:%s\n",SINK_IPADDR,errno,strerror(errno));
				sleep(1);
				close(sink_socket);
				continue;
			}
			printf("sink connect ok !\r\n");
			sink_num=1;
		}

		if(sink_num==1)
		{
			memset(buf,0,1026);
			int len=recv(sink_socket,buf,1024,0);//MSG_DONTWAIT
			if(len>0)
			{
				//接受到数据代码区
				sink_data_deal(buf,len,pri_data,pack_tmp);//传感器发来数据处理函数
			}else if(len==-1){
				//没接收到数据代码区
				int ret=ping_status(SINK_IPADDR);
				if(ret)
				{
					printf("sink abnormal disconnect!\n");
					sink_num=0;
					close(sink_socket);
					continue;
				}
			}else{
				//断开连接和异常处理代码区
				printf("sink disconnect!\n");
				sink_num=0;
				close(sink_socket);
				continue;
			}
		}
	}
	free(buf);
	free(pri_data);
	free(pack_tmp);
	close(sink_socket);
	sqlite3_close(mydb);
}

int sink_data_deal(char *buf,int len,char *pri_data,char *pack_tmp)
{
	if(buf==NULL || len==0)
	{
		printf("sensor_data_deal():buf is NULL or len=0\r\n");
		return -1;
	}

	if(sink_num!=1)
	{
		printf("sink_num():sink_num=0\r\n");
		return -2;
	}

	if(strstr(buf,"private data query cmd")==NULL && strstr(buf,"{")==buf)
	{//处理非查询命令业务
		char temp[20]={0};
		time_t timep;
		struct tm *p; 
		time(&timep);
		p=gmtime(&timep);
		sprintf(temp,"%d-%02d-%02d %02d:%02d:%02d",(1900+p->tm_year), (1+p->tm_mon),p->tm_mday,p->tm_hour, p->tm_min, p->tm_sec);
		if(cJson_check_str(buf))
		{
			printf("buf format is error!\n");
		}else
		{
			cJSON *json_obj=cJSON_Parse(buf);
			cJSON *to_socketfd=cJSON_GetObjectItem(json_obj,"socketfd");
			cJSON *json_type=cJSON_GetObjectItem(json_obj,"type");
			cJSON_AddStringToObject(json_obj,"time",temp);
			if(!(to_socketfd==NULL || json_type==NULL))
			{
				char *strjson = cJSON_Print(json_obj);
				int slen=send(to_socketfd->valueint,strjson,strlen(strjson),MSG_NOSIGNAL);//MSG_NOSIGNAL防止进程退出
				if(slen<=0)
				{
					printf("sink:send():error=%d:%s\n",errno,strerror(errno));
					return -3;
				}else
				{
					printf("sink_data_deal():send:len:%d,buff:%s\nok!\n",slen,strjson);
				}
				free(strjson);
			}
			cJSON_Delete(json_obj);//用完记得释放内存
		}
		//printf("%s:%d\n",to_socketfd->string,to_socketfd->valueint);//valuestring
		//printf("sink_data_deal():len:%d,buff:%s\n\n",len,buf);
	}else{//处理查询命令业务
		/*********1、数据是否分包，若分包进行合并成完整包,增强数据包粘连处理***********/
		char *chr_add=strchr(buf,'}');
		if(chr_add==NULL)
		{
			sprintf(pri_data,"%s%s",pri_data,buf);
		}else{
			strncat(pri_data,buf,(chr_add-buf)+1);
			sprintf(pack_tmp,"%s",pri_data);
			if(!cJson_check_str(pack_tmp))
			{
				private_data_query(pack_tmp);
			}else{
				printf("error_data:\n%s\n",pack_tmp);
			}
			memset(pri_data,0,strlen(pri_data));
			if(*(chr_add+1)!='\0')
			{
				strcpy(pri_data,(chr_add+1));
			}
		}
	}
	return 0;
}

void *div_package_sendtosink(void * arg)
{
	//使用互斥锁保证在同一时间只用一个进程使用该socket
	pthread_mutex_lock(&socket_mutex);//对加锁sink_socket，如果已经锁上，阻塞等待解锁			
	char *sendbuf=static_cast<char *>(arg);
	/************当数据包大于1024byte进行分包发送，否则一次性发送*************/
	int send_buf_len=strlen(sendbuf);
	if(send_buf_len<=1024)
	{
		int slen;
		slen=send(sink_socket,sendbuf,send_buf_len,MSG_NOSIGNAL);//MSG_NOSIGNAL防止进程退出
		//usleep(50000);
		if(slen<=0)
		{
			printf("sink:send():error=%d:%s\n",errno,strerror(errno));
			goto retn;
		}
	}else{
		char pack_num=send_buf_len/1024+1;//计算分几个包能把数据完整发送出去
		int send_len_each=1024;//每个包的大小

		for(char i=0;i<pack_num;i++)//循环发发送每个分包
		{
			if(i==pack_num-1)
			{
				send_len_each=send_buf_len-1024*(pack_num-1);
			}
			//使用互斥锁保证在同一时间只用一个进程使用该socket
			int slen=send(sink_socket,sendbuf+i*1024,send_len_each,MSG_NOSIGNAL);//MSG_NOSIGNAL防止进程退出
			//usleep(50000);
			if(slen<=0)
			{
				printf("sink:send():error=%d:%s\n",errno,strerror(errno));
				goto retn;
			}
		}

	}
	free(sendbuf);
	retn:pthread_mutex_unlock(&socket_mutex);//对sink_socket解锁;
	sem_post(&sem_pthread);
}

//返回：1 有交集 0空集
int intersection_isnull(char *str1, char *str2)
{
	char *ptr, *retptr;
	char *tmp=(char *)calloc(strlen(str2)+2,1);
	strcpy(tmp,str2);
	ptr = tmp;
	int ret = 0;
	while ((retptr = strtok(ptr, ",")) != NULL) 
	{
		if (strstr(str1, retptr)!=NULL)
		{
			ret = 1;
			break;
		}
		ptr = NULL;
	}
	free(tmp);
	return ret;
}
int check_within_bounds(char *checkstr, char *comarezerone[5])
{
	if (*checkstr == '1')
	{
		if (!(strlen(checkstr)==1 && strlen(comarezerone[1])==0) && strcmp(checkstr + 2, comarezerone[1]) !=0)//注意0的处理
		{
			if (!intersection_isnull(checkstr + 2, comarezerone[0]))
			{
				return - 1;
			}
		}

		if (!(strlen(checkstr)==1 && strlen(comarezerone[3])==0) && strcmp(checkstr + 2, comarezerone[3]) !=0)//注意0的处理
		{
			if (intersection_isnull(checkstr + 2, comarezerone[2]))
			{
				return -1;
			}
		}
	}
	else if (*checkstr == '0')
	{
		if (strcmp(checkstr + 2, comarezerone[0]) !=0)
		{
			if (intersection_isnull(checkstr + 2, comarezerone[1]))
			{
				return -1;
			}
		}

		if (strstr(checkstr + 2, comarezerone[2]) !=0)
		{
			if (!intersection_isnull(checkstr + 2, comarezerone[3]))
			{
				return -1;
			}
		}
	}

	return 0;
}

void private_data_query(char *arg_buf)
{
	cJSON *json_obj=cJSON_Parse(arg_buf);
	printf("recv:\n%s\n\n",arg_buf);
	cJSON *json_devid=cJSON_GetObjectItem(json_obj,"devid");
	cJSON *json_star_time=cJSON_GetObjectItem(json_obj,"star_time");
	cJSON *json_end_time=cJSON_GetObjectItem(json_obj,"end_time");
	cJSON *json_lux_min_one=cJSON_GetObjectItem(json_obj,"lux_min_one");
	cJSON *json_lux_min_zero=cJSON_GetObjectItem(json_obj,"lux_min_zero");
	cJSON *json_lux_max_one=cJSON_GetObjectItem(json_obj,"lux_max_one");
	cJSON *json_lux_max_zero=cJSON_GetObjectItem(json_obj,"lux_max_zero");
	cJSON *json_temp_min_one=cJSON_GetObjectItem(json_obj,"temp_min_one");
	cJSON *json_temp_min_zero=cJSON_GetObjectItem(json_obj,"temp_min_zero");
	cJSON *json_temp_max_one=cJSON_GetObjectItem(json_obj,"temp_max_one");
	cJSON *json_temp_max_zero=cJSON_GetObjectItem(json_obj,"temp_max_zero");
	cJSON *json_press_min_one=cJSON_GetObjectItem(json_obj,"press_min_one");
	cJSON *json_press_min_zero=cJSON_GetObjectItem(json_obj,"press_min_zero");
	cJSON *json_press_max_one=cJSON_GetObjectItem(json_obj,"press_max_one");
	cJSON *json_press_max_zero=cJSON_GetObjectItem(json_obj,"press_max_zero");
	cJSON *json_humi_min_one=cJSON_GetObjectItem(json_obj,"humi_min_one");
	cJSON *json_humi_min_zero=cJSON_GetObjectItem(json_obj,"humi_min_zero");
	cJSON *json_humi_max_one=cJSON_GetObjectItem(json_obj,"humi_max_one");
	cJSON *json_humi_max_zero=cJSON_GetObjectItem(json_obj,"humi_max_zero");
	
	if(!(json_devid==NULL || json_star_time==NULL || json_end_time==NULL))
	{
		char sqlcmd[110]={0};
		char date_str[12]={0};
		for(char i=0;*(json_star_time->valuestring+i)!=' ';i++)
		{
			date_str[i]=*(json_star_time->valuestring+i);
		}
		if(strlen(json_end_time->valuestring)!=0)
		{
			sprintf(sqlcmd,"SELECT * FROM sensordata WHERE time >= '%s' AND time <= '%s' AND sensorid='%s';",json_star_time->valuestring,json_end_time->valuestring,json_devid->valuestring);
		}else{
			sprintf(sqlcmd,"SELECT * FROM sensordata WHERE time >= '%s' AND sensorid='%s';",json_star_time->valuestring,json_devid->valuestring);
		}
		printf("sqlcmd:\n%s\n\n",sqlcmd);
		memset(arg_buf,0,strlen(arg_buf));
		char **azResult=NULL;//二维数组存放结果
		char *zErrMsg =NULL;//存放错误信息
		int nrow=0;//查出数据条数
		int ncolumn = 0;//数据列数
		char src_tmp[50]={0};
		sprintf(src_tmp,"/usr/local/sqlite_db/graduation_%s",date_str);

		if(access(src_tmp,F_OK)==-1)
		{
			printf("日期为%s的数据不存在！\n",date_str);
			goto return_erro;
		}else{
			int ret = sqlite3_open(src_tmp, &mydb);
			if( ret != SQLITE_OK ) {
				fprintf(stderr, "无法打开数据库:%s", sqlite3_errmsg(mydb));
				goto return_erro;
			}
		}
		sqlite3_get_table(mydb,sqlcmd,&azResult,&nrow,&ncolumn,&zErrMsg);
		
		for(int i=0;i<nrow;i++)
		{
			int j=ncolumn*(i+1);
			char *comarezerone[5];
			/********************************************************************************/
			/*                      sqlite3数据库查出的字段与序号对应关系表                		*/
			/* 1~ncolumn 字段名称不是数据					ncolumn*(i+1) 主键_id的数据			*/
			/* ncolumn*(i+1)+1 devid的数据				ncolumn*(i+1)+2 光照强度数据			*/
			/* ncolumn*(i+1)+3 温度数据					ncolumn*(i+1)+4 大气压数据			*/
			/* ncolumn*(i+1)+5 湿度数据					ncolumn*(i+1)+6 光照最小值比较因子数据	*/
			/* ncolumn*(i+1)+7 光照最大值比较因子数据		ncolumn*(i+1)+8 温度最小值比较因子数据	*/
			/* ncolumn*(i+1)+9 温度最大值比较因子数据		ncolumn*(i+1)+10 大气压最小值比较因子数据*/
			/* ncolumn*(i+1)+11 大气压最大值比较因子数据		ncolumn*(i+1)+12 湿度最小值比较因子数据	*/
			/* ncolumn*(i+1)+13 湿度最大值比较因子数据		ncolumn*(i+1)+14 索引数据				*/
			/* ncolumn*(i+1)+15 采集时间数据													*/
			/********************************************************************************/
			int ret1=1;
			if(!(json_lux_min_one==NULL || json_lux_min_zero==NULL || json_lux_max_one==NULL || json_lux_max_zero==NULL))
			{
				comarezerone[0] = json_lux_min_zero->valuestring;
				comarezerone[1] = json_lux_min_one->valuestring;
				comarezerone[2] = json_lux_max_zero->valuestring;
				comarezerone[3] = json_lux_max_one->valuestring;
				if(strcmp(azResult[j+6],"NULL")==0 || strcmp(azResult[j+7],"NULL")==0)
				{
					ret1=1;
				}else{
					ret1=check_within_bounds(azResult[j+6],comarezerone);
					ret1 &=check_within_bounds(azResult[j+7],comarezerone);
				}
			}else{
				ret1=0;
			}
			
			int ret2=1;
			if(!(json_temp_min_one==NULL || json_temp_min_zero==NULL || json_temp_max_one==NULL || json_temp_max_zero==NULL))
			{
				comarezerone[0] = json_temp_min_zero->valuestring;
				comarezerone[1] = json_temp_min_one->valuestring;
				comarezerone[2] = json_temp_max_zero->valuestring;
				comarezerone[3] = json_temp_max_one->valuestring;
				if(strcmp(azResult[j+8],"NULL")==0 || strcmp(azResult[j+9],"NULL")==0)
				{
					ret2=1;
				}else{
					ret2=check_within_bounds(azResult[j+8],comarezerone);
					ret2 &=check_within_bounds(azResult[j+9],comarezerone);
				}
			}else{
				ret2=0;
			}
			
			int ret3=1;
			if(!(json_press_min_one==NULL || json_press_min_zero==NULL || json_press_max_one==NULL || json_press_max_zero==NULL))
			{
				comarezerone[0] = json_press_min_zero->valuestring;
				comarezerone[1] = json_press_min_one->valuestring;
				comarezerone[2] = json_press_max_zero->valuestring;
				comarezerone[3] = json_press_max_one->valuestring;
				if(strcmp(azResult[j+10],"NULL")==0 || strcmp(azResult[j+11],"NULL")==0)
				{
					ret3=1;
				}else{
					ret3=check_within_bounds(azResult[j+10],comarezerone);
					ret3 &=check_within_bounds(azResult[j+11],comarezerone);
				}
			}else{
				ret3=0;
			}
			
			int ret4=1;
			if(!(json_press_min_one==NULL || json_humi_min_zero==NULL || json_humi_max_one==NULL || json_humi_max_zero==NULL))
			{
				comarezerone[0] = json_humi_min_zero->valuestring;
				comarezerone[1] = json_humi_min_one->valuestring;
				comarezerone[2] = json_humi_max_zero->valuestring;
				comarezerone[3] = json_humi_max_one->valuestring;
				if(strcmp(azResult[j+12],"NULL")==0 || strcmp(azResult[j+13],"NULL")==0)
				{
					ret4=1;
				}else{
				ret4=check_within_bounds(azResult[j+12],comarezerone);
				ret4 &=check_within_bounds(azResult[j+13],comarezerone);
				}
			}else{
				ret4=0;
			}
			
			cJSON *json_retmp;
			char *strjson=NULL;
			json_retmp = cJSON_CreateObject();
			if(!(ret1 || ret2 || ret3 || ret4))//都在范围内
			{
				cJSON_AddStringToObject(json_retmp,"type","private data query result");
				cJSON_AddStringToObject(json_retmp,"devid",azResult[j+1]);
				cJSON_AddStringToObject(json_retmp,"result","in limits");
				cJSON_AddStringToObject(json_retmp,"lux_data",azResult[j+2]);
				cJSON_AddStringToObject(json_retmp,"temp_data",azResult[j+3]);
				cJSON_AddStringToObject(json_retmp,"press_data",azResult[j+4]);
				cJSON_AddStringToObject(json_retmp,"humi_data",azResult[j+5]);
				cJSON_AddStringToObject(json_retmp,"data_index",azResult[j+14]);
				cJSON_AddStringToObject(json_retmp,"time",azResult[j+15]);
				strjson = cJSON_Print(json_retmp);
			}else{
				cJSON_AddStringToObject(json_retmp,"type","private data query result");
				cJSON_AddStringToObject(json_retmp,"devid",azResult[j+1]);
				cJSON_AddStringToObject(json_retmp,"result","not in limits");
				cJSON_AddStringToObject(json_retmp,"data_index",azResult[j+14]);
				cJSON_AddStringToObject(json_retmp,"time",azResult[j+15]);
				strjson = cJSON_Print(json_retmp);
			}
			sem_wait(&sem_pthread);
			pthread_t thread_id;
			int ret=pthread_create(&thread_id, NULL, &div_package_sendtosink,static_cast<void *>(strjson));
			if(ret!=0)
			{
				printf("pthread_create error!\n");
			}
			pthread_detach(thread_id);
			cJSON_Delete(json_retmp);//用完记得释放内存
			//usleep(200000);
		}

		sqlite3_free_table(azResult);
		sqlite3_free(zErrMsg);
	}else{
		printf("error format!\n");
	}
	cJSON_Delete(json_obj);//用完记得释放内存
	return_erro:;
}
