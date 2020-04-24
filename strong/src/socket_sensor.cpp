#include "socket_sensor.h"
#include <sys/time.h>
#include <string>

extern int errno;
extern int sink_socket;
extern int sink_num;
static sqlite3 * mydb=0;
extern pthread_mutex_t socket_mutex;

static char recv_finsh=0;
int sensor_data_deal(char *buf,int len,int socketfd,char *pri_data,char *pack_tmp,char *sql_buf);
int save_private_data(cJSON *json_obj,char *sql_buf);

void *sensor_main(void* arg)
{
	socket_status * sensor_status=static_cast<socket_status *>(arg);
	int sensor_socket=sensor_status->socket_fd;
	char sensor_ip_addr[INET_ADDRSTRLEN+1]={0};
	char *buf=(char *)calloc(1026,1);
	char *pri_data=(char *)calloc(40960,1);
	char *pack_tmp=(char *)calloc(20,1024);
	char *sql_buf=(char *)calloc(7,1024);
	memset(pri_data,0,40960);
	memset(pack_tmp,0,20*1024);
	void *return_erro;
	strcpy(sensor_ip_addr,sensor_status->ip_addr_buf);
	free(sensor_status);
	char src_tmp[50]={0};
	time_t timep;
	struct tm *p;
	time(&timep);
	p=gmtime(&timep);
	sprintf(src_tmp,"/usr/local/sqlite_db/graduation_%d-%02d-%02d",(1900+p->tm_year), (1+p->tm_mon),p->tm_mday);
	if(access(src_tmp,F_OK)==-1)
	{//文件不存在
		int ret = sqlite3_open(src_tmp, &mydb);
		if( ret != SQLITE_OK ) {
			fprintf(stderr, "无法打开数据库: %s", sqlite3_errmsg(mydb));
			return return_erro;
		}
		char create_table_cmd[]="CREATE TABLE sensordata(_id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,sensorid VARCHAR(15),lux_data VARCHAR(400),temp_data VARCHAR(400),press_data VARCHAR(400),humi_data VARCHAR(400),lux_min_cmp VARCHAR(900),lux_max_cmp VARCHAR(900),temp_min_cmp VARCHAR(900),temp_max_cmp VARCHAR(900),press_min_cmp VARCHAR(900),press_max_cmp VARCHAR(900),humi_min_cmp VARCHAR(900),humi_max_cmp VARCHAR(900),myindex VARCHAR(200),time VARCHAR(25));";
		char *zErrMsg=NULL;
		ret=sqlite3_exec(mydb,create_table_cmd,NULL,NULL,&zErrMsg);
		if( ret != SQLITE_OK ) {
			printf("sqlite3 error:%s\n",zErrMsg);
		}else{
			free(zErrMsg);
		}
		printf("数据库建立成功!\n");
	}else{//文件存在
		int ret = sqlite3_open(src_tmp, &mydb);
		if( ret != SQLITE_OK ) {
			fprintf(stderr, "无法打开数据库: %s", sqlite3_errmsg(mydb));
			return return_erro;
		}
		printf("数据库连接成功!\n");
	}
	while(1)
	{
		memset(buf,'\0',strlen(buf));
		int len=recv(sensor_socket,buf,1025,0);//MSG_DONTWAIT
		if(len>0)
		{
			//接受到数据代码区
			sensor_data_deal(buf,len,sensor_socket,pri_data,pack_tmp,sql_buf);//传感器发来数据处理函数
		}else if(len==-1){
			//没接收到数据代码区
			int ret=ping_status(sensor_ip_addr);
			if(ret)
			{
				printf("sensor abnormal disconnect!\n");
				break;
			}
		}else{
			//断开连接和异常处理代码区
			printf("sensor disconnect!\n");
			break;
		}
	}
	close(sensor_socket);
	free(buf);
	free(pri_data);
	free(pack_tmp);
	free(sql_buf);
	sqlite3_close(mydb);
}

int sensor_data_deal(char *buf,int len,int socketfd,char *pri_data,char *pack_tmp,char *sql_buf)
{
	if(buf==NULL || len==0)
	{
		printf("sensor_data_deal():buf is NULL or len=0\r\n");
		return -1;
	}
	if(sink_socket<0)
	{
		printf("sensor_data_deal():sink_socket<0\r\n");
		return -1;
	}

	char temp[20]={0};
	time_t timep;
	struct tm *p; 
	time(&timep);
	p=gmtime(&timep);
	sprintf(temp,"%d-%02d-%02d %02d:%02d:%02d",(1900+p->tm_year), (1+p->tm_mon),p->tm_mday,p->tm_hour, p->tm_min, p->tm_sec);

	if(strstr(buf,"private data upload")==NULL && strstr(buf,"{")==buf)
	{//处理非隐私数据上传业务
		printf("recv:len=%d:contet:%s\n",len,buf);
		cJSON *json_obj=cJSON_Parse(buf);
		//cJSON *json_type=cJSON_GetObjectItem(json_obj,"type");
		cJSON_AddStringToObject(json_obj,"time",temp);
		cJSON_AddNumberToObject(json_obj,"socketfd",socketfd);
		char *strjson = cJSON_Print(json_obj);
		printf("new:%s\n",strjson);
		if(sink_num==1)
		{
			int slen;
			pthread_mutex_lock(&socket_mutex);
			slen=send(sink_socket,strjson,strlen(strjson),MSG_NOSIGNAL);//MSG_NOSIGNAL防止进程退出
			pthread_mutex_unlock(&socket_mutex);
			if(slen<=0)
			{
				printf("sensor:send():error=%d:%s\n",errno,strerror(errno));
				return -1;
			}
		}else{
			printf("send fail,sink_num==0\r\n");
			return -2;
		}
		free(strjson);//用完记得释放内存
		cJSON_Delete(json_obj);//用完记得释放内存
	}else{
		//处理隐私数据上传业务
		/*********1、数据是否分包，若分包进行合并成完整包***********/
		char *chr_add=strchr(buf,'}');
		if(chr_add==NULL)
		{
			sprintf(pri_data,"%s%s",pri_data,buf);
		}else{
			strncat(pri_data,buf,(chr_add-buf)+1);
			sprintf(pack_tmp,"%s",pri_data);
			if(!cJson_check_str(pack_tmp))
			{
				cJSON *json_obj=cJSON_Parse(pack_tmp);
				printf("data:%s\n",pack_tmp);
				save_private_data(json_obj,sql_buf);
				cJSON_Delete(json_obj);//用完记得释放内存
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

int save_private_data(cJSON *json_obj,char *sql_buf)
{
	cJSON *json_devid=cJSON_GetObjectItem(json_obj,"devid");
	cJSON *json_data=cJSON_GetObjectItem(json_obj,"data");
	cJSON *json_comparer=cJSON_GetObjectItem(json_obj,"comparer");
	cJSON *json_index=cJSON_GetObjectItem(json_obj,"index");
	cJSON *json_time=cJSON_GetObjectItem(json_obj,"time");
	//分割解析数据帧
	char strnull[]="NULL";
	char *datastr[4];
	for (char i = 0; i < 4; i++)
	{
		datastr[i] = strnull;
	}
	char *ptr=json_data->valuestring,*retptr;
	for (char i=0;(retptr = strtok(ptr, ",")) != NULL;i++)
	{
		char *tmp_s = (char *)calloc(strlen(retptr)+1, 1);
		sprintf(tmp_s, "%s", retptr);
		datastr[i] = tmp_s;
		ptr = NULL;
	}

	char * cmpstr[8];
	for (char i = 0; i < 8; i++)
	{
		cmpstr[i] = strnull;
	}
	ptr=json_comparer->valuestring;
	for (char i=0;(retptr = strtok(ptr, "#")) != NULL;i++)
	{
		char *tmp_s = (char *)calloc(strlen(retptr)+1, 1);
		sprintf(tmp_s, "%s", retptr);
		cmpstr[i] = tmp_s;
		ptr = NULL;
	}

	memset(sql_buf,0,strlen(sql_buf));
	sprintf(sql_buf,"INSERT INTO 'sensordata' VALUES(NULL,'%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s','%s');",
			json_devid->valuestring,datastr[0],datastr[1],datastr[2],datastr[3],cmpstr[0],cmpstr[1],
			cmpstr[2],cmpstr[3],cmpstr[4],cmpstr[5],cmpstr[6],cmpstr[7],json_index->valuestring,json_time->valuestring);
	char *zErrMsg=NULL;
	int ret=sqlite3_exec(mydb,sql_buf,NULL,NULL,&zErrMsg);
	if( ret != SQLITE_OK ) {
		printf("sqlite3 error:%s\n",zErrMsg);
	}else{
		free(zErrMsg);
	}

	for (char i = 0; i < 4; i++)//释放内存防止内存泄露
	{
		if (!strstr(datastr[i], "NULL"))
		{
			free(datastr[i]);
		}
	}
	for (char i = 0; i < 8; i++)//释放内存防止内存泄露
	{
		if (!strstr(cmpstr[i], "NULL"))
		{
			free(cmpstr[i]);
		}
	}
}
