NAME := nodecode1

$(NAME)_MBINS_TYPE := app
$(NAME)_VERSION := 1.0.1
$(NAME)_SUMMARY := node code 1
$(NAME)_SOURCES := app_main.c
$(NAME)_PREBUILT_LIBRARY := ./lib/miracl.a

$(NAME)_COMPONENTS += osal_aos netmgr linkkit_sdk_c cjson alicrypto sensor udata

$(NAME)_INCLUDES += ./include
$(NAME)_SOURCES += ./src/tcp_net.c
$(NAME)_SOURCES += ./src/diffie_hellman.c
$(NAME)_SOURCES += ./src/work_core.c
$(NAME)_SOURCES += ./src/aes_encrypt_ecb.c
$(NAME)_SOURCES += ./src/base64_encode.c
$(NAME)_SOURCES += ./src/sensor_task.c
$(NAME)_SOURCES += ./src/JSON_checker.c
