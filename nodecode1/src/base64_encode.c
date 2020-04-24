#include "base64_encode.h"

int base64_encode(uint8_t *inp,uint32_t in_len,char **out)
{
    uint32_t len;
    int i,j;
    uint8_t *in=NULL;
    char *res;
    char *base64_table="ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
    if(inp==NULL || in_len==0 || out==NULL)
    {
        printf("input error!\r\n");
        return -1;
    }
    char flag=0;
    if(in_len%3==0)
    {
        len=in_len/3*4;
        in=inp;
    }else{
        len=(in_len/3+1)*4;
        //如果输入的数组长度不是3的整数倍要拓展
        flag=1;
        in=(uint8_t *)aos_calloc((in_len/3+1)*3,1);
        for(uint32_t k=0;k<in_len;k++)
        {
            in[k]=inp[k];
        }
    }
    res=(char *)aos_calloc(len+1,sizeof(char));
    if(res==NULL)
    {
        printf("aos_calloc for out error!\r\n");
        return -1;
    }
    for(i=0,j=0;i<len-2;j+=3,i+=4)
    {
        res[i]=base64_table[in[j]>>2];
        res[i+1]=base64_table[(in[j] & 0x03)<<4 | (in[j+1]>>4)];
        res[i+2]=base64_table[(in[j+1] & 0x0f)<<2 | (in[j+2]>>6)];
        res[i+3]=base64_table[in[j+2] & 0x3f];
    }
    switch (in_len%3)
    {
    case 1:
        res[i-2]='=';
        res[i-1]='=';
        break;
    case 2:
        res[i-1]='=';
        break;
    default:
        break;
    }
    *out=res;
    if(flag)
    {
        aos_free(in);
    }
    return 0;
}
