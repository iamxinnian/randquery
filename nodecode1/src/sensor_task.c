#include "sensor_task.h"

int32_t temp_t=0;
uint32_t humi_t=0;
uint32_t lux_t=0;
uint32_t press_t=0;

void sensor_local_task(void)
{
    int ret;
    temperature_data_t  temp;
    humidity_data_t     humi;
    als_data_t          alsdata;
    barometer_data_t    press;

    /* Sensor Hal start */
    ret = sensor_hal_init();
    if (ret != 0) {
        return;
    }

    if(DATA_DIMENSION>3)
    {
        /* Open the humidity sensor device */
        ret = sensor_hal_open(TAG_DEV_HUMI, 0);
        if (ret != 0) {
            return;
        }
        (void)sensor_hal_ioctl(TAG_DEV_HUMI, 0, SENSOR_IOCTL_ODR_SET, SENSOR_SAMPLE_TIME);
    }

    if(DATA_DIMENSION>2)
    {
        /* Open the pressure sensor device */
        ret = sensor_hal_open(TAG_DEV_BARO, 0);
        if (ret != 0) {
            return;
        }
        (void)sensor_hal_ioctl(TAG_DEV_BARO, 0, SENSOR_IOCTL_ODR_SET, SENSOR_SAMPLE_TIME);
    }

    if(DATA_DIMENSION>1)
    {
        /* Open the temperature sensor device */
        ret = sensor_hal_open(TAG_DEV_TEMP, 0);
        if (ret != 0) {
            return;
        }
        (void)sensor_hal_ioctl(TAG_DEV_TEMP, 0, SENSOR_IOCTL_ODR_SET, SENSOR_SAMPLE_TIME);
    }

    if(DATA_DIMENSION>0)
    {
        /* Open the ALS sensor device */
        ret = sensor_hal_open(TAG_DEV_ALS, 0);
        if (ret != 0) {
            return;
        }
        (void)sensor_hal_ioctl(TAG_DEV_ALS, 0, SENSOR_IOCTL_ODR_SET, SENSOR_SAMPLE_TIME);
    }
    
    while(1)
    {
        if(DATA_DIMENSION>3)
        {
            /* Read the humidity sensor data */
            ret = sensor_hal_read(TAG_DEV_HUMI, 0, &humi, sizeof(humi));
            if(ret > 0){
                if(DATA_BIT_NUM==8)
                {
                    humi_t=humi.h/10;
                }else if(DATA_BIT_NUM>=16){
                    humi_t=humi.h;
                }
            }
        }

        if(DATA_DIMENSION>2)
        {
            /* Read the presure sensor data */
            ret = sensor_hal_read(TAG_DEV_BARO, 0, &press, sizeof(press));
            if(ret > 0){
                if(DATA_BIT_NUM==8)
                {
                    press_t=press.p/1000;
                }else if(DATA_BIT_NUM==16)
                {
                    press_t=press.p/10;
                }else if(DATA_BIT_NUM>16)
                {
                    press_t=press.p;
                }
            }
        }

        if(DATA_DIMENSION>1)
        {
            /* Read the temperature sensor data */
            ret = sensor_hal_read(TAG_DEV_TEMP, 0, &temp, sizeof(temp));
            if(ret > 0){
                if(DATA_BIT_NUM==8)
                {
                    temp_t=temp.t/10;
                }else if(DATA_BIT_NUM>=16){
                    temp_t=temp.t;
                }
            }
        }

        if(DATA_DIMENSION>0)
        {
            /* Read the ALS sensor data */
            ret = sensor_hal_read(TAG_DEV_ALS, 0, &alsdata, sizeof(alsdata));
            if(ret > 0){
                if(DATA_BIT_NUM==8)
                {
                    if(alsdata.lux<255)
                    {
                        lux_t=alsdata.lux;
                    }else{
                        lux_t=255;
                    }
                }
                if(DATA_BIT_NUM==16)
                {
                    if(alsdata.lux<65535)
                    {
                        lux_t=alsdata.lux;
                    }else{
                        lux_t=65535;
                    }
                }
                if(DATA_BIT_NUM>=32)
                {
                    lux_t=alsdata.lux;
                }
            }
        }
        
        /* Task sleep */
        aos_msleep(100);
    }
}
