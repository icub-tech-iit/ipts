# MTB4 - Test Firmware 

This repo contains the test firmware used by [IPTS](https://github.com/icub-tech-iit/proto/tree/master/proto-utils/ipts-test-suite) test suite.

![fig.1 Mc4Plus](img/fig_1_MTB4.jpg)

---

## 1. Contents

The repo contains:

>- ``STM32 CubeMx`` project (to configure uP pheriperals and pins and create the project scheleton)
>- ``ARM Keil`` project

---

## 2. Firmware commands description

The test firmware loops waiting for a **8 byte lenght CAN message**.
 The MSB byte of the message corresponds to a test to be performed by the firmware or to a specifc action to be done.

>**TX message**    : d0=0x?? (??=command) , d1-d7= unused
>
>**RX message**     : d0=0x?? (??=AA -> test PASS, ??=BB -> test FAIL, ??=FF -> NOP) , d1-d7 = log messages (optional)


Here is the table containing the accepted commands and related responses

| Command | Deescription | Result |  |  |
|---|---|---|---|---|
| 0x00 | Get Firmware version | d1=Firmware version <br> d2=Firmware revision|  |  |
| 0x01 | Test Leds DL1,DL2 OFF| N/A |  |  |
| 0x02 | Test Leds DL1,DL2 ON| N/A |  |  |
| 0x03 | Test Temperature (U5)| d0=0xAA or 0xBB (pass/fail) <br> d1=temperature>>8 <br> d2=temperature |  |  |
| 0x04 | Test GPIO (J3)| N/A|  |  |
| 0x05 | Test BNO055 (U4)| d0=0xAA or 0xBB (pass/fail) <br> d1=CHIP_ID |  |  |
| 0x06 | Test CAN communication J1| d0=0xAA or 0xBB (pass/fail)  |  |  |
| 0x07 | Test CAN communication J2| d0=0xAA or 0xBB (pass/fail)  |  |  |
| 0x08 | Test uP DeviceID U1| d0=DEVICE_ID >> 8 <br> d1=DEVICE_ID  |  |  |

---

## 3. Action/Tests not implemented in the firmware

The following tests and actions are performed outside the test firmware by the IPTS test suite

| Test/Action | tool | Description |  |  |
|---|---|---|---|---|
| Test Firmware Download | ST-LINK programmer/batch file (``mtb4_test.bat``) | IPTS launch the script and check if the operation went sucessfully|  |  |
| Application Firmware Download | ST-LINK programmer/batch file (``mtb4_prod.bat``) | IPTS launch the script and check if the operation went sucessfully|  |  |

