/****************************************************
*  MTB4 board - iCub Facility - 2017                *
*  BNO055 configuration                             *
*                                                   *
*  Written by Andrea Mura                           *
*  <andrea.mura@iit.it>                             *
****************************************************/


#include "stm32l4xx_hal.h"
#include "i2c.h"
#include "usart.h"
#include "BNO055.h"

uint8_t BNO055_I2C_ADDRESS=0x52;    // left shifted device address 0x29

uint8_t BNO055_TxBuffer[2] = {0};
uint8_t BNO055_RxBuffer[BNO055_RxBuffer_SIZE] = {0};

uint8_t BNO055_UART_STARTBYTE=0xAA;
uint8_t IMU_UART_TxBuffer[4] = {0};

extern uint8_t BNO055_RegisterValue;

// -----------------------------------------------------------------------------------------------------------------------------
// Initialize the BNO055
// -----------------------------------------------------------------------------------------------------------------------------
void BNO055_init(void){
  HAL_GPIO_WritePin(BNO055_SELCOM_GPIO_Port, BNO055_SELCOM_Pin, GPIO_PIN_RESET);  // I2C interface
  //HAL_GPIO_WritePin(BNO055_SELCOM_GPIO_Port, BNO055_SELCOM_Pin, GPIO_PIN_SET);  // UART interface
  HAL_Delay(500);       // Start-Up time > 400ms
  HAL_GPIO_WritePin(BNO055_BOOT_GPIO_Port, BNO055_BOOT_Pin, GPIO_PIN_SET);
  HAL_GPIO_WritePin(BNO055_RESET_GPIO_Port, BNO055_RESET_Pin, GPIO_PIN_SET);
  HAL_Delay(800);       // POR time >650ms
  
  BNO055_Write(PWR_MODE, PWR_MODE_Normal);   // BNO055 initialization Power Mode
  HAL_Delay(500);
  BNO055_Write(OPR_MODE, OPR_MODE_ACCONLY);   // BNO055 initialization Operation Mode
  HAL_Delay(500);
}

// -----------------------------------------------------------------------------------------------------------------------------
// De-initialize the BNO055
// -----------------------------------------------------------------------------------------------------------------------------
void BNO055_deinit(void){
  
}

// -----------------------------------------------------------------------------------------------------------------------------
// I2C Read the BNO055 registers
// -----------------------------------------------------------------------------------------------------------------------------
void BNO055_Read(uint8_t REGISTER_ADDRESS){
  BNO055_TxBuffer[0] = REGISTER_ADDRESS;
  HAL_I2C_Master_Transmit_DMA(&hi2c2, (uint16_t)BNO055_I2C_ADDRESS, (uint8_t*)BNO055_TxBuffer, 1);
  //HAL_I2C_Master_Transmit_DMA(&hi2c2, (uint16_t)BNO055_I2C_ADDRESS, (uint8_t*)BNO055_TxBuffer, sizeof(BNO055_TxBuffer));
}

// -----------------------------------------------------------------------------------------------------------------------------
// I2C Read the BNO055 registers
// -----------------------------------------------------------------------------------------------------------------------------
void BNO055_Write(uint8_t REGISTER_ADDRESS, uint8_t VALUE){
  BNO055_TxBuffer[0] = REGISTER_ADDRESS;
  BNO055_TxBuffer[1] = VALUE;
  HAL_I2C_Master_Transmit_DMA(&hi2c2, (uint16_t)BNO055_I2C_ADDRESS, (uint8_t*)BNO055_TxBuffer, sizeof(BNO055_TxBuffer));
}

// -----------------------------------------------------------------------------------------------------------------------------
// UART Read the BNO055 
// -----------------------------------------------------------------------------------------------------------------------------
void BNO055_UART_Read(void){
  IMU_UART_TxBuffer[0]=BNO055_UART_STARTBYTE;
  IMU_UART_TxBuffer[1]=0x01;
  IMU_UART_TxBuffer[2]=0x5A;
  IMU_UART_TxBuffer[3]=0x01;
  HAL_UART_Transmit_IT(&huart2, (uint8_t*)IMU_UART_TxBuffer, 4);
}


