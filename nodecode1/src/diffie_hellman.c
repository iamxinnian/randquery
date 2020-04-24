#include "diffie_hellman.h"

int generate_privkey(uint32_t r_time,char *privkey)
{
    if(privkey==NULL)
    {
        return -1;
    }
    big xa;
    miracl *mip=mirsys(34,16);//指定的数值不要太大，内存很珍贵
    mip->IOBASE=16;
    xa=mirvar(0);
    irand(r_time);
    bigdig(32,16,xa);
    cotstr(xa,privkey);
    mirexit();//别忘了释放内存
    return 0;
}

static int is_hex_string(const char *str)//检查字符串是否是非空16进制字符串，0是 -1不是
{
    int len=strlen(str);
    if(len==0)
    {
        return -1;
    }
    for(int i=0;i<len;i++)
    {
        if(!((*(str+i)>='0' && *(str+i)<='9') || (*(str+i)>='a' && *(str+i)<='f') || (*(str+i)>='A' && *(str+i)<='F')))
        {
            return -2;
        }
    }
    return 0;
}

int generate_key(const char *prime_num,const char *orig_root,const char *privkey,char *pubkey)
{
    if(privkey==NULL || prime_num==NULL || orig_root==NULL)
    {
        return -1;
    }
    if(is_hex_string(prime_num) || is_hex_string(orig_root) || is_hex_string(privkey))
    {
        return -2;
    }

    big p,g,prk,puk,k0;
    miracl *mip=mirsys(34,16);//指定每个大数分配的多少位的多少进制数
    mip->IOBASE=16;//指定进制
    p=mirvar(0);//初始化
    g=mirvar(0);
    prk=mirvar(0);
    puk=mirvar(0);
    k0=mirvar(0);
    convert(0,k0);

    cinstr(g,orig_root);//字符串转大数
    cinstr(p,prime_num);
    cinstr(prk,privkey);

    if(mr_compare(k0,g)==0)//判断大数是否相等
    {
        mirexit();//释放大数系统分配内存
        return -3;
    }

    powmod(g,prk,p,puk);//g^prk%p=puk

    cotstr(puk,pubkey);//大数转字符串
    mirexit();//释放大数系统分配内存
    return 0;
}