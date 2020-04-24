#include "work_core.h"
#include <cJSON.h>
#include "aes_encrypt_ecb.h"
#include "base64_encode.h"
#include "sensor_task.h"
#include "JSON_checker.h"

uint8_t join_server_flag=0;
extern char *tcp_send_buffer;
extern char *tcp_recv_buffer;
extern uint8_t tcp_send_flag;
extern uint8_t net_status_flag;
extern uint8_t tcp_connect_flag;
extern uint32_t random_t;
extern aos_timer_t g_timer;
extern aos_timer_t systime_timer;
extern system_time this_now;//软件系统时间

extern int32_t temp_t;
extern uint32_t humi_t;
extern uint32_t lux_t;
extern uint32_t press_t;

extern aos_mutex_t mutex_send;
extern aos_mutex_t mutex_recv;
unsigned char AES_PKEY[16]={0};

static int request_join(void);
static int generation_AESkey(char *buff);
static void LZeroOneCode(unsigned long int num, unsigned long int code[],char *flag);
static void comparer_generate(uint32_t num,unsigned long int codei[],char *LZeroOneStr,char flag);

void work_core_task(void)
{
    int ret=-1;
    int sensor_collect_n=0;

    char tmp_str[SENSOR_DATA_LEN]={0};
    char humi_str[SENSOR_DATA_LEN]={0};
    char press_str[SENSOR_DATA_LEN]={0};
    char lux_str[SENSOR_DATA_LEN]={0};

    while(1)
    {
        if(!(net_status_flag && tcp_connect_flag))
        {
            aos_msleep(10);
            continue;
        }
        if(join_server_flag==0)
        {
            ret=request_join();
            if(ret!=0)
            {
                printf("request_join() faile!\n\r");
            }else{
                join_server_flag++;
            }
        }

        if(join_server_flag==1)
        {
            aos_mutex_lock(&mutex_recv,AOS_WAIT_FOREVER);
            int len=strlen(tcp_recv_buffer);
            aos_mutex_unlock(&mutex_recv);

            static char tim=0;
            if(len==0)
            {
                if(tim<15)
                {
                    tim++;
                }else{
                    tim=0;
                    printf("request_join() not response!\n\r");
                    join_server_flag--;
                    continue;
                }
            }else{
                //接收到sink处理传感器请求的入网的，数据处理区
                tim=0;
                aos_mutex_lock(&mutex_recv,AOS_WAIT_FOREVER);
                //printf("recv:len=%d:contet:%s\n",len,tcp_recv_buffer);
                int ret=generation_AESkey(tcp_recv_buffer);
                if(ret!=0)
                {
                    join_server_flag=0;
                    memset(tcp_recv_buffer,'\0',len);
                    aos_mutex_unlock(&mutex_recv);
                    continue;
                }else{
                    join_server_flag++;
                }
                memset(tcp_recv_buffer,'\0',len);
                aos_mutex_unlock(&mutex_recv);
            }
        }

        if(join_server_flag==2)
        {
            aos_mutex_lock(&mutex_recv,AOS_WAIT_FOREVER);
            int len=strlen(tcp_recv_buffer);
            aos_mutex_unlock(&mutex_recv);

            static char tim=0;
            if(len==0)
            {
                if(tim<15)
                {
                    tim++;
                }else{
                    tim=0;
                    printf("request_join() not response!\n\r");
                    join_server_flag=0;
                    continue;
                }
            }else{
                //接收到sink处理传感器请求的入网的，数据处理区
                cJSON *root;
                tim=0;
                aos_mutex_lock(&mutex_recv,AOS_WAIT_FOREVER);
                //printf("recv:len=%d:contet:%s\n",len,tcp_recv_buffer);
                if(cJson_check_str(tcp_recv_buffer)!=0)
                {
                    goto error1;
                }
                root = cJSON_Parse(tcp_recv_buffer);
                memset(tcp_recv_buffer,'\0',len);
                aos_mutex_unlock(&mutex_recv);
                cJSON *json_key_status=cJSON_GetObjectItem(root,"key_status");
                if(json_key_status==NULL || json_key_status ==cJSON_NULL)
                {
                    goto error1;
                }
                if(strstr(json_key_status->valuestring,"ok")==NULL)
                {
         error1:    join_server_flag=0;
                    cJSON_Delete(root);//用完记得释放内存
                    continue;
                }else{  
                    aos_timer_stop(&g_timer);//交换密钥后释放定时器资源
                    aos_timer_free(&g_timer);//交换密钥后释放定时器资源
                    join_server_flag=3;
                    cJSON *time_status=cJSON_GetObjectItem(root,"time");
                    if(time_status==NULL || time_status ==cJSON_NULL)
                    {
                        goto error1;
                    }
                    this_now.year=0;
                    this_now.month=0;
                    this_now.day=0;
                    this_now.hour=0;
                    this_now.minutes=0;
                    this_now.seconds=0;
                    char *pt=time_status->valuestring;

                    while(*pt!='-')
                    {
                        this_now.year=this_now.year*10+(*pt-'0');
                        pt++;
                    }
                    pt++;
                    while(*pt!='-')
                    {
                        this_now.month=this_now.month*10+(*pt-'0');
                        pt++;
                    }
                    pt++;
                    while(*pt!=' ')
                    {
                        this_now.day=this_now.day*10+(*pt-'0');
                        pt++;
                    }
                    pt++;
                    while(*pt!=':')
                    {
                        this_now.hour=this_now.hour*10+(*pt-'0');
                        pt++;
                    }
                    pt++;
                    while(*pt!=':')
                    {
                        this_now.minutes=this_now.minutes*10+(*pt-'0');
                        pt++;
                    }
                    pt++;
                    while(*pt!='\0')
                    {
                        this_now.seconds=this_now.seconds*10+(*pt-'0');
                        pt++;
                    }
                    aos_timer_start(&systime_timer);
                }
                cJSON_Delete(root);//用完记得释放内存
            }
        }

        if(join_server_flag==3)
        {
            //交换密钥后的，传感器数据隐私上报处理区
            static uint32_t min_tmp,max_tmp,min_humi,max_humi,min_press,max_press,min_lux,max_lux;
            if(sensor_collect_n<SENSOR_DATA_N)
            {
                if(sensor_collect_n==0)//每个周期采集的第一个数据处理
                {
                    min_tmp=temp_t;
                    max_tmp=temp_t;
                    min_humi=humi_t;
                    max_humi=humi_t;
                    min_press=press_t;
                    max_press=press_t;
                    min_lux=lux_t;
                    max_lux=lux_t;
                    sprintf(tmp_str,"%u",temp_t);
                    sprintf(humi_str,"%u",humi_t);
                    sprintf(press_str,"%u",press_t);
                    sprintf(lux_str,"%u",lux_t);
                }else{//每个周期采集的非第一个数据处理
                    if(min_tmp>temp_t)
                    {
                        min_tmp=temp_t;
                    }

                    if(max_tmp<temp_t)
                    {
                        max_tmp=temp_t;
                    }

                    if(min_humi>humi_t)
                    {
                        min_humi=humi_t;
                    }

                    if(max_humi<humi_t)
                    {
                        max_humi=humi_t;
                    }

                    if(min_press>press_t)
                    {
                        min_press=press_t;
                    }

                    if(max_press<press_t)
                    {
                        max_press=press_t;
                    }

                    if(min_lux>lux_t)
                    {
                        min_lux=lux_t;
                    }

                    if(max_lux<lux_t)
                    {
                        max_lux=lux_t;
                    }

                    sprintf(tmp_str,"%s,%u",tmp_str,temp_t);
                    sprintf(humi_str,"%s,%u",humi_str,humi_t);
                    sprintf(press_str,"%s,%u",press_str,press_t);
                    sprintf(lux_str,"%s,%u",lux_str,lux_t);
                }
                aos_msleep(SENSOR_SAMPLE_TIME);
                sensor_collect_n++;
            }else{
                char *time_str=(char *)aos_calloc(21,1);
                char *data_str=NULL;
                data_str=(char *)aos_calloc(SENSOR_DATA_LEN*16/3+3,1);

                sprintf(time_str,"%d-%02d-%02d %02d:%02d:%02d", this_now.year, this_now.month, this_now.day, this_now.hour, this_now.minutes, this_now.seconds);

                sensor_collect_n=0;
                aes_confg aescfg;
                aescfg.key=AES_PKEY;
                aescfg.key_len=16;
                aescfg.is_enc=1;
                uint8_t myout[SENSOR_DATA_LEN]={0};
                uint8_t myout_len=0;
                uint8_t myin_len=0;
                char *data_aes_base64=NULL;

                if(DATA_DIMENSION>0)
                {
                    if(strlen(lux_str)%16==0)
                    {
                        myin_len=strlen(lux_str);
                    }else{
                        myin_len=(1+strlen(lux_str)/16)*16;
                    }
                    encrypt_aes_ecb(&aescfg,lux_str,myin_len,myout,&myout_len);
                    base64_encode(myout,myout_len,&data_aes_base64);
                    sprintf(data_str,"%s",data_aes_base64);
                    free(data_aes_base64);
                    memset(myout,0,myout_len);
                }

                if(DATA_DIMENSION>1)
                {
                    if(strlen(tmp_str)%16==0)
                    {
                        myin_len=strlen(tmp_str);
                    }else{
                        myin_len=(1+strlen(tmp_str)/16)*16;
                    }
                    encrypt_aes_ecb(&aescfg,tmp_str,myin_len,myout,&myout_len);
                    base64_encode(myout,myout_len,&data_aes_base64);
                    sprintf(data_str,"%s,%s",data_str,data_aes_base64);
                    free(data_aes_base64);
                    memset(myout,0,myout_len);
                }

                if(DATA_DIMENSION>2)
                {
                    if(strlen(press_str)%16==0)
                    {
                        myin_len=strlen(press_str);
                    }else{
                        myin_len=(1+strlen(press_str)/16)*16;
                    }
                    encrypt_aes_ecb(&aescfg,press_str,myin_len,myout,&myout_len);
                    base64_encode(myout,myout_len,&data_aes_base64);
                    sprintf(data_str,"%s,%s",data_str,data_aes_base64);
                    free(data_aes_base64);
                    memset(myout,0,myout_len);
                }

                if(DATA_DIMENSION>3)
                {
                    if(strlen(humi_str)%16==0)
                    {
                        myin_len=strlen(humi_str);
                    }else{
                        myin_len=(1+strlen(humi_str)/16)*16;
                    }
                    encrypt_aes_ecb(&aescfg,humi_str,myin_len,myout,&myout_len);
                    base64_encode(myout,myout_len,&data_aes_base64);
                    sprintf(data_str,"%s,%s",data_str,data_aes_base64);
                    free(data_aes_base64);
                    memset(myout,0,myout_len);
                }
                uint64_t codei[DATA_BIT_NUM+1]={0};
                char *LZeroOneStr=(char *)aos_calloc(3000,1);

                if(DATA_DIMENSION>0)
                {
                //光照 min max 使用左向01编码，再base64编码 
                comparer_generate(min_lux,codei,LZeroOneStr,1);
                comparer_generate(max_lux,codei,LZeroOneStr,0);
                printf("min_lux=%d   max_lux=%d\r\n",min_lux,max_lux);
                printf("lux_comp=%s\r\n",LZeroOneStr);
                }

                if(DATA_DIMENSION>1)
                {
                //温度 min max 使用左向01编码，再base64编码 
                comparer_generate(min_tmp,codei,LZeroOneStr,0);
                comparer_generate(max_tmp,codei,LZeroOneStr,0);
                }

                if(DATA_DIMENSION>2)
                {
                //大气压 min max 使用左向01编码，再base64编码 
                comparer_generate(min_press,codei,LZeroOneStr,0);
                comparer_generate(max_press,codei,LZeroOneStr,0);
                }

                if(DATA_DIMENSION>3)
                {
                    //湿度 min max 使用左向01编码，再base64编码 
                    comparer_generate(min_humi,codei,LZeroOneStr,0);
                    comparer_generate(max_humi,codei,LZeroOneStr,0);
                }

                cJSON *root;
                root = cJSON_CreateObject();
                if (root == NULL) {
                    printf("cJSON_CreateObject():fail\r\n");
                }
                cJSON_AddStringToObject(root, "type", "private data upload");
                cJSON_AddStringToObject(root, "devid", DEVID);

                cJSON_AddStringToObject(root, "time", time_str);
                cJSON_AddStringToObject(root, "data", data_str);
                aos_free(data_str);
                //cJSON_AddItemToObject(root,"data",json_data);
                
                char *index_str=(char *)aos_calloc(100,1);
                if(DATA_DIMENSION>0)
                {
                    sprintf(index_str,"%s,%s,%u,%u",DEVID,time_str,min_lux,max_lux);
                }

                if(DATA_DIMENSION>1)
                {
                    sprintf(index_str,"%s,%u,%u",index_str,min_tmp,max_tmp);
                }

                if(DATA_DIMENSION>2)
                {
                    sprintf(index_str,"%s,%u,%u",index_str,min_press,max_press);
                }

                if(DATA_DIMENSION>3)
                {
                    sprintf(index_str,"%s,%u,%u",index_str,min_humi,max_humi);
                }
                
                sprintf(index_str,"%s,%d",index_str,SENSOR_DATA_N);
                //printf("%s\r\n",index_str);

                if(strlen(index_str)%16==0)
                {
                    myin_len=strlen(index_str);
                }else{
                    myin_len=(1+strlen(index_str)/16)*16;
                }
                encrypt_aes_ecb(&aescfg,index_str,myin_len,myout,&myout_len);
                base64_encode(myout,myout_len,&data_aes_base64);

                cJSON_AddStringToObject(root, "comparer",LZeroOneStr);
                free(LZeroOneStr);

                cJSON_AddStringToObject(root, "index",data_aes_base64);
                free(data_aes_base64);
                aos_free(time_str);
                aos_free(index_str);
                memset(myout,0,myout_len);

                //清空缓存区
                memset(tmp_str,0,strlen(tmp_str));
                memset(humi_str,0,strlen(humi_str));
                memset(press_str,0,strlen(press_str));
                memset(lux_str,0,strlen(lux_str));
                

                char *strjson = cJSON_Print(root);
                //printf("send:%s\r\n",strjson);
                aos_mutex_lock(&mutex_send,AOS_WAIT_FOREVER);
                if(tcp_send_buffer!=NULL)
                {
                    sprintf(tcp_send_buffer,"%s",strjson);
                    tcp_send_flag=1;
                }
                aos_mutex_unlock(&mutex_send);
                
                cJSON_Delete(root);//用完记得释放内存
                free(strjson);//用完记得释放内存
                aos_msleep(500); 
            }
        }

        aos_msleep(5);
    }
}

//字符串转十六进制字符数组
static void ChangeHEX(char s[],int slen,char bits[]) {
    for(int i = 0,n=0;i<slen; i += 2,n++) {
        if(s[i] >= 'A' && s[i] <= 'F')
        {
            bits[n] = s[i] - 'A' + 10;
        }else{
            bits[n] = s[i] - '0';
        } 
        if(s[i + 1] >= 'A' && s[i + 1] <= 'F')
        {
            bits[n] = (bits[n] << 4) | (s[i + 1] - 'A' + 10);
        }else{
            bits[n] = (bits[n] << 4) | (s[i + 1] - '0');
        }
    }
}

static int request_join(void)
{
    cJSON *root;
    root = cJSON_CreateObject();

    if (root == NULL) {
        return -1;
    }
    cJSON_AddStringToObject(root, "type", "request join net");
    cJSON_AddStringToObject(root, "devid", DEVID);

    char *strjson = cJSON_Print(root);
    
    aos_mutex_lock(&mutex_send,AOS_WAIT_FOREVER);
    if(tcp_send_buffer!=NULL)
    {
        sprintf(tcp_send_buffer,"%s",strjson);
        tcp_send_flag=1;
    }
    aos_mutex_unlock(&mutex_send);
    cJSON_Delete(root);//用完记得释放内存
    free(strjson);//用完记得释放内存
    return 0;
}

static int generation_AESkey(char *buf)
{
    cJSON *root;
    cJSON *croot;
    croot = cJSON_CreateObject();
    char sxb[33]={0};
    char syb[33]={0};
    char spb[33]={0};

    if(cJson_check_str(buf)!=0)
    {
        return -1;
    }
    root = cJSON_Parse(buf);
    cJSON *json_dh_g=cJSON_GetObjectItem(root,"dh_g");
    cJSON *json_dh_p=cJSON_GetObjectItem(root,"dh_p");
    cJSON *json_dh_ya=cJSON_GetObjectItem(root,"dh_ya");
    cJSON *json_dh_devid=cJSON_GetObjectItem(root,"devid");
    
    if(json_dh_g ==NULL || json_dh_p==NULL || json_dh_ya==NULL || json_dh_devid==NULL ||
    json_dh_g ==cJSON_NULL || json_dh_p==cJSON_NULL || json_dh_ya==cJSON_NULL || json_dh_devid==cJSON_NULL)
    {
        return -1;
    }

    if(strcmp(json_dh_devid->valuestring,DEVID)!=0)
    {
        printf("devid is not matching\r\n");
        return -1;
    }

    generate_privkey(random_t,sxb);
    int ret=generate_key(json_dh_p->valuestring,json_dh_g->valuestring,sxb,syb);
    ret |=generate_key(json_dh_p->valuestring,json_dh_ya->valuestring,sxb,spb);
    if(ret!=0)
    {
        return -2;
    }
    for(int i=0;i<32;i++)
    {
        if(spb[i]=='\0')
        {
            spb[i]='0';
        }
    }
    //printf("pb=%s\r\n",spb);//打印生成的密钥
    ChangeHEX(spb,32,AES_PKEY);

    aes_confg aescfg;
    aescfg.key=AES_PKEY;
    aescfg.key_len=16;
    aescfg.is_enc=1;
    uint8_t myout[33]={0};
    uint8_t myout_len=0;
    char *dh_check_hexstr=NULL;
    encrypt_aes_ecb(&aescfg,syb,32,myout,&myout_len);
    base64_encode(myout,32,&dh_check_hexstr);

    cJSON_AddStringToObject(croot, "type", "request join generat KEY");
    cJSON_AddStringToObject(croot, "devid", DEVID);
    cJSON_AddStringToObject(croot, "dh_yb",syb);
    cJSON_AddStringToObject(croot, "dh_check",dh_check_hexstr);

    char *strjson = cJSON_Print(croot);

    aos_mutex_lock(&mutex_send,AOS_WAIT_FOREVER);
    if(tcp_send_buffer!=NULL)
    {
        sprintf(tcp_send_buffer,"%s",strjson);
        tcp_send_flag=1;
    }
    aos_mutex_unlock(&mutex_send);

    cJSON_Delete(root);//用完记得释放内存  
    cJSON_Delete(croot);//用完记得释放内存  
    free(strjson);//用完记得释放内存
    free(dh_check_hexstr);
    return 0;
}

static void LZeroOneCode(unsigned long int num, unsigned long int code[],char *flag)
{
	char bit = 0;
	char j = 0, k = 0;
	for (char i = 0; i < DATA_BIT_NUM; i++)
	{
		if (num & (0x1 << i))
		{//L1编码
			code[j] = (num >> i)<<i;
			j++;
		}else
		{//L0编码
			code[DATA_BIT_NUM-k] = ((num >> i) | 0x1)<<i;
			k++;
		}
	}
    *flag=j;
}

static void comparer_generate(uint32_t num,unsigned long int codei[],char *LZeroOneStr,char flag)
{
    char iflag=0;
    uint8_t digest64out[16]={0};
    char codei_tmp[17]={0};
    char *data_aes_base64=NULL;
    LZeroOneCode(num,codei,&iflag);
    if(iflag>(DATA_BIT_NUM/2))
    {
        if(flag==1)
        {
            sprintf(LZeroOneStr,"%s","0");
        }else{
            sprintf(LZeroOneStr,"%s#%s",LZeroOneStr,"0");
        }
    }else{
        if(flag==1)
        {
            sprintf(LZeroOneStr,"%s","1");
        }else{
            sprintf(LZeroOneStr,"%s#%s",LZeroOneStr,"1");
        }
    }
    if(iflag>(DATA_BIT_NUM/2))
    {
        for(char i=iflag+1;i<DATA_BIT_NUM+1;i++)
        {
            sprintf(codei_tmp,"%016x",codei[i]);
            //HMAC摘要运算
            ali_hmac_digest(MD5,(uint8_t *)AES_PKEY,16,(uint8_t *)codei_tmp,strlen(codei_tmp),digest64out);
            base64_encode(digest64out,16,&data_aes_base64);
            sprintf(LZeroOneStr,"%s,%s",LZeroOneStr,data_aes_base64);
            free(data_aes_base64);
            memset(digest64out,0,16);
        }
    }else{
        for(char i=0;i<iflag;i++)
        {
            sprintf(codei_tmp,"%016x",codei[i]);
            //HMAC摘要运算
            ali_hmac_digest(MD5,(uint8_t *)AES_PKEY,16,(uint8_t *)codei_tmp,strlen(codei_tmp),digest64out);
            base64_encode(digest64out,16,&data_aes_base64);
            sprintf(LZeroOneStr,"%s,%s",LZeroOneStr,data_aes_base64);
            free(data_aes_base64);
            memset(digest64out,0,16);
        }
    }
    memset(codei,0,DATA_BIT_NUM+1);

    for(char i=0;i<DATA_BIT_NUM+1;i++)
    {
        codei[i]=0;
    }
}
