/**
  ******************************************************************************
  * File Name          : main.c
  * Description        : Main program body
  ******************************************************************************
  ** This notice applies to any and all portions of this file
  * that are not between comment pairs USER CODE BEGIN and
  * USER CODE END. Other portions of this file, whether 
  * inserted by the user or by software development tools
  * are owned by their respective copyright owners.
  *
  * COPYRIGHT(c) 2017 STMicroelectronics
  *
  * Redistribution and use in source and binary forms, with or without modification,
  * are permitted provided that the following conditions are met:
  *   1. Redistributions of source code must retain the above copyright notice,
  *      this list of conditions and the following disclaimer.
  *   2. Redistributions in binary form must reproduce the above copyright notice,
  *      this list of conditions and the following disclaimer in the documentation
  *      and/or other materials provided with the distribution.
  *   3. Neither the name of STMicroelectronics nor the names of its contributors
  *      may be used to endorse or promote products derived from this software
  *      without specific prior written permission.
  *
  * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
  * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
  * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
  * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
  * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
  * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
  * CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
  * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
  *
  ******************************************************************************
  */

/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "stm32l4xx_hal.h"
#include "can.h"
#include "dma.h"
#include "i2c.h"
#include "tim.h"
#include "usart.h"
#include "gpio.h"
#include <stdlib.h>
#include <stdbool.h>


/* USER CODE BEGIN Includes */
#include "can_utility.h"
#include "Si705x.h"
#include "BNO055.h"
//#include "EOsensor_BNO055.h"
#include "AD7147.h"
#include "AD7147RegMap.h"
#include "I2C_Multi_SDA.h"

/* USER CODE END Includes */

/* Private variables ---------------------------------------------------------*/

/* USER CODE BEGIN PV */
/* Private variables ---------------------------------------------------------*/

// ----------------------------------------------------------------------------
// Firmware Version
// ----------------------------------------------------------------------------
char Firmware_vers = 2;
char Revision_vers = 1;
char Build_number  = 0;

uint8_t can_message = 0;
uint8_t count = 0;
uint8_t ToggleFlag = 0;
uint8_t ledcount = 100;

#define DEVICE_ID (*(unsigned long *)0xE0042000)

uint16_t Temperature_code;
uint16_t Temperature_board;
uint16_t Temperature_board2;
float Temp_media,f;
char Temp_user[5];
uint8_t text[5] = {0};
uint8_t UART_Rx_buffer[5];
char  cmd;
char *s;
bool blink = true;
uint8_t *BNO055_SensorValue;

unsigned int channel=0;
extern unsigned int AD7147Registers[2][16][12]; // il primo campo rappresenta il numero dei canali
extern unsigned int CapOffset[2][16][12];       // il primo campo rappresenta il numero dei canali
extern error_cap err[16];
extern unsigned char AD7147_ADD[4];


/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);

/* USER CODE BEGIN PFP */
/* Private function prototypes -----------------------------------------------*/
void LED_Init(void);
void GPIO_Init(void);
void GPIO_Test(void);
void TIMER_Init(void);
void TEMPERATURE(void);
void BNO055(void);
bool BNO055_readID(void);
void BNO055_UART(void);
void GPIO_Toggle(void);

void Send2Can(uint8_t status, uint8_t d1, uint8_t d2, uint8_t d3, 
	uint8_t d4, uint8_t d5, uint8_t d6,uint8_t d7);
void TestFirmwareVersion(void);
void TestLed1(void);
void TestLed2(void);
void CheckDeviceId(void);


void FillMessages8bit(unsigned char Channel,unsigned char triangleN);
void UpdateTouchMap(void);
void TouchMapHistoryCheck(void);
void SkinRawData(void);

/* USER CODE END PFP */

/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

int main(void)
{

  HAL_Init();

  SystemClock_Config();

 
  MX_GPIO_Init();
  MX_DMA_Init();
  MX_TIM6_Init();
  MX_CAN1_Init();
  //MX_USART1_UART_Init();
  MX_I2C1_Init();
  MX_I2C2_Init();
  //MX_USART2_UART_Init();

  /* USER CODE BEGIN 2 */
  TIMER_Init();  
  LED_Init();
	GPIO_Init();
  CAN_Config();
  Si705x_init();
  BNO055_init();
  
 
  if (HAL_CAN_Receive_IT(&hcan1, CAN_FIFO0) != HAL_OK)
  {
    /* Reception Error */
    Error_Handler();
  }
 	
	  HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_RESET);

  while(1)
  { 
		
		 if(can_message == 1)
		{
		  cmd = hcan1.pRxMsg->Data[0];
			blink = true;
      can_message = 0;
			
			switch(cmd)
		 {
			 case 0x00 :
				 //Check revisione fw collaudo
			  HAL_Delay(200);
				TestFirmwareVersion();
  		 break;
			 
			 case 0x01 :
				 //Test led rosso e blu spenti
			 blink = false;
			 TestLed1();			   
			 break; 
			 
			 case 0x02 :
				  //Test led rosso e blu accesi
			  blink = false;
				TestLed2();
				break; 
			 
			 case 0x03 :
			 //test sensore temperatura; rivcevo temp letta dallo user e la comparo con quella letta dal sensore +/- 25%
//			 Temp_user[0] = hcan1.pRxMsg->Data[1];
//			 Temp_user[1] = hcan1.pRxMsg->Data[2];
//			 Temp_user[2] = hcan1.pRxMsg->Data[3];
//			 Temp_user[3] = hcan1.pRxMsg->Data[4];
//			 Temp_user[4] = '\0';
//       
//			 f=atof(Temp_user);
			
			for (int i=0; i<4; i++)
			 {
				 TEMPERATURE();
			   if(i > 0) Temp_media += Temperature_board;
				 HAL_Delay(500);
			 }
			 
			 Temp_media = Temp_media / 3;
			 Temperature_board2 = Temp_media;
			 Temp_media = Temp_media / 100;
			 
			 uint8_t temp = hcan1.pRxMsg->Data[1];
			 
			 if(Temp_media < temp -((temp*30)/100) || Temp_media > temp+((temp*30)/100)) Send2Can(0xBB, Temperature_board2>>8, Temperature_board2, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);//0xBB = fail			 
			 else Send2Can(0xAA, Temperature_board2>>8, Temperature_board2, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
			 
			// if(Temp_media < f-((f*30)/100) || Temp_media > f+((f*30)/100)) Send2Can(0xBB, Temperature_board2>>8, Temperature_board2, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);//0xBB = fail			 
			//  else Send2Can(0xAA, Temperature_board2>>8, Temperature_board2, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);//0xAA = pass
				 break;
			 
			 case 0x04 :
				//Test GPIO con schedina led esterna - si accendono in sequenza
				GPIO_Test(); 
				GPIO_Init();		
				 break; 
			 
			 case 0x05 :
				 //Test IMU; leggo chip id
			   BNO055_readID();
			   HAL_Delay(100);
			   if(BNO055_readID()) Send2Can(0xAA, *(BNO055_SensorValue), 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
         else Send2Can(0xBB, *(BNO055_SensorValue), 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				 break;
			 
			 case 0x06 :
				 //Test secondo connettore CAN 
			 	 HAL_Delay(100);
			   Send2Can(0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break; 
			 
			 case 0x07 :
				//Test CAN
				HAL_Delay(200);
			  Send2Can(0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
  		 break;
			 
			 case 0x08 :
				 //check device id
			   CheckDeviceId();
			 break;

			 default: 
				 
			   break;
		 }
 	   
		}
		
		HAL_Delay(5);
    ledcount--;		
		
    if(ledcount == 0 && blink)   
		{
			HAL_GPIO_TogglePin(LED_BLUE_GPIO_Port,LED_BLUE_Pin);
			ledcount = 100;
		}
	
  }
  /* USER CODE END 3 */

}

/** System Clock Configuration
*/
void SystemClock_Config(void)
{

  RCC_OscInitTypeDef RCC_OscInitStruct;
  RCC_ClkInitTypeDef RCC_ClkInitStruct;
  RCC_PeriphCLKInitTypeDef PeriphClkInit;

    /**Initializes the CPU, AHB and APB busses clocks 
    */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.HSICalibrationValue = 16;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_NONE;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    _Error_Handler(__FILE__, __LINE__);
  }

    /**Initializes the CPU, AHB and APB busses clocks 
    */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_HSI;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV1;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_0) != HAL_OK)
  {
    _Error_Handler(__FILE__, __LINE__);
  }

  PeriphClkInit.PeriphClockSelection = RCC_PERIPHCLK_USART1|RCC_PERIPHCLK_USART2
                              |RCC_PERIPHCLK_I2C1|RCC_PERIPHCLK_I2C2;
  PeriphClkInit.Usart1ClockSelection = RCC_USART1CLKSOURCE_PCLK2;
  PeriphClkInit.Usart2ClockSelection = RCC_USART2CLKSOURCE_PCLK1;
  PeriphClkInit.I2c1ClockSelection = RCC_I2C1CLKSOURCE_PCLK1;
  PeriphClkInit.I2c2ClockSelection = RCC_I2C2CLKSOURCE_PCLK1;
  if (HAL_RCCEx_PeriphCLKConfig(&PeriphClkInit) != HAL_OK)
  {
    _Error_Handler(__FILE__, __LINE__);
  }

    /**Configure the main internal regulator output voltage 
    */
  if (HAL_PWREx_ControlVoltageScaling(PWR_REGULATOR_VOLTAGE_SCALE1) != HAL_OK)
  {
    _Error_Handler(__FILE__, __LINE__);
  }

    /**Configure the Systick interrupt time 
    */
  HAL_SYSTICK_Config(HAL_RCC_GetHCLKFreq()/1000);

    /**Configure the Systick 
    */
  HAL_SYSTICK_CLKSourceConfig(SYSTICK_CLKSOURCE_HCLK);

  /* SysTick_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(SysTick_IRQn, 0, 0);
}

/* USER CODE BEGIN 4 */
void HAL_SYSTICK_Callback(void)
{
	static uint32_t count=0;
	if(count==1000){
		//HAL_GPIO_TogglePin(LED_BLUE_GPIO_Port, LED_BLUE_Pin);
		count=0;
	}
	count++;
}



// -----------------------------------------------------------------------------------------------------------------------------
// Timers initialization
// -----------------------------------------------------------------------------------------------------------------------------
void TIMER_Init(void){
  //HAL_TIM_Base_Start_IT(&htim6);    // 100us
  //HAL_TIM_IRQHandler(&htim6);
}

// -----------------------------------------------------------------------------------------------------------------------------
// LEDs initialization
// -----------------------------------------------------------------------------------------------------------------------------
void LED_Init(void){
  HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, GPIO_PIN_SET);
  HAL_GPIO_WritePin(LED_BLUE_GPIO_Port, LED_BLUE_Pin, GPIO_PIN_SET);
}

// -----------------------------------------------------------------------------------------------------------------------------
// LEDs initialization
// -----------------------------------------------------------------------------------------------------------------------------
void GPIO_Init(void){
  HAL_GPIO_WritePin(SCK0_GPIO_Port, SCK0_Pin, GPIO_PIN_SET);
  HAL_GPIO_WritePin(SDA0_GPIO_Port, SDA0_Pin, GPIO_PIN_SET);
  HAL_GPIO_WritePin(SDA1_GPIO_Port, SDA1_Pin, GPIO_PIN_SET);
  HAL_GPIO_WritePin(SDA2_GPIO_Port, SDA2_Pin, GPIO_PIN_SET);
  HAL_GPIO_WritePin(SDA3_GPIO_Port, SDA3_Pin, GPIO_PIN_SET);
  HAL_GPIO_WritePin(ANALOG_0_GPIO_Port, ANALOG_0_Pin, GPIO_PIN_SET);    // NB: in this test, ANALOG_0 is set to GPIO_OUTPUT
}

// -----------------------------------------------------------------------------------------------------------------------------
// GPIOs Toggle
// -----------------------------------------------------------------------------------------------------------------------------
void GPIO_Test(void){ 
  HAL_GPIO_WritePin(SCK0_GPIO_Port, SCK0_Pin, GPIO_PIN_RESET);
	HAL_Delay(500);
  HAL_GPIO_WritePin(SDA0_GPIO_Port, SDA0_Pin, GPIO_PIN_RESET);
	HAL_Delay(500);
  HAL_GPIO_WritePin(SDA1_GPIO_Port, SDA1_Pin, GPIO_PIN_RESET);
	HAL_Delay(500);
  HAL_GPIO_WritePin(SDA2_GPIO_Port, SDA2_Pin, GPIO_PIN_RESET);
	HAL_Delay(500);
  HAL_GPIO_WritePin(SDA3_GPIO_Port, SDA3_Pin, GPIO_PIN_RESET);
	HAL_Delay(500);
  HAL_GPIO_WritePin(ANALOG_0_GPIO_Port, ANALOG_0_Pin, GPIO_PIN_RESET);    // NB: in this test, ANALOG_0 is set to GPIO_OUTPUT
	HAL_Delay(500);
}


// -----------------------------------------------------------------------------------------------------------------------------
// GPIOs Toggle
// -----------------------------------------------------------------------------------------------------------------------------
void GPIO_Toggle(void){  
  ToggleFlag=ToggleFlag^1;
  HAL_GPIO_WritePin(SCK0_GPIO_Port, SCK0_Pin, ToggleFlag);
  HAL_GPIO_WritePin(SDA0_GPIO_Port, SDA0_Pin, ToggleFlag);
  HAL_GPIO_WritePin(SDA1_GPIO_Port, SDA1_Pin, ToggleFlag);
  HAL_GPIO_WritePin(SDA2_GPIO_Port, SDA2_Pin, ToggleFlag);
  HAL_GPIO_WritePin(SDA3_GPIO_Port, SDA3_Pin, ToggleFlag);
  HAL_GPIO_WritePin(ANALOG_0_GPIO_Port, ANALOG_0_Pin, ToggleFlag);    // NB: in this test, ANALOG_0 is set to GPIO_OUTPUT
}

// -----------------------------------------------------------------------------------------------------------------------------
// CAN messages management
// -----------------------------------------------------------------------------------------------------------------------------
void CANBUS(void){
      hcan1.pTxMsg->StdId   = 0x551;              // Polling
      hcan1.pTxMsg->Data[0] = *(BNO055_SensorValue+0);
      hcan1.pTxMsg->Data[1] = *(BNO055_SensorValue+1);
      hcan1.pTxMsg->Data[2] = *(BNO055_SensorValue+2);
      hcan1.pTxMsg->Data[3] = *(BNO055_SensorValue+3);
      hcan1.pTxMsg->Data[4] = *(BNO055_SensorValue+4);
      hcan1.pTxMsg->Data[5] = *(BNO055_SensorValue+5);
      hcan1.pTxMsg->Data[6] = Temperature_board>>8;
      hcan1.pTxMsg->Data[7] = Temperature_board;
      HAL_CAN_Transmit_IT(&hcan1);
}


void Send2Can(uint8_t status, uint8_t d1, uint8_t d2, uint8_t d3, 
	uint8_t d4, uint8_t d5, uint8_t d6,uint8_t d7)	{
	
      hcan1.pTxMsg->StdId   = 0x551;            
      hcan1.pTxMsg->Data[0] = status;
	
	    hcan1.pTxMsg->Data[1] = d1;
      hcan1.pTxMsg->Data[2] = d2;
      hcan1.pTxMsg->Data[3] = d3;
			hcan1.pTxMsg->Data[4] = d4;
			hcan1.pTxMsg->Data[5] = d5;
	    hcan1.pTxMsg->Data[6] = d6;
      hcan1.pTxMsg->Data[7] = d7;
	    
      HAL_CAN_Transmit_IT(&hcan1);
}

void TestFirmwareVersion(void){
	
      hcan1.pTxMsg->StdId   = 0x551;
			
      hcan1.pTxMsg->Data[0] = Firmware_vers;
      hcan1.pTxMsg->Data[1] = Revision_vers;
      hcan1.pTxMsg->Data[2] = Build_number;
	
			hcan1.pTxMsg->Data[3] = 0xFF;
			hcan1.pTxMsg->Data[4] = 0xFF;
			hcan1.pTxMsg->Data[5] = 0xFF;
			hcan1.pTxMsg->Data[6] = 0xFF;
			hcan1.pTxMsg->Data[7] = 0xFF;
	
      HAL_CAN_Transmit_IT(&hcan1);
}

void TestLed1(void){
	  HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, 1);
		HAL_GPIO_WritePin(LED_BLUE_GPIO_Port, LED_BLUE_Pin, 1);
}

void TestLed2(void){	
   	HAL_GPIO_WritePin(LED_RED_GPIO_Port, LED_RED_Pin, 0);
		HAL_GPIO_WritePin(LED_BLUE_GPIO_Port, LED_BLUE_Pin, 0);  
}

bool BNO055_readID(void){
  BNO055_Read(BNO055_REG_ID);
	if(*(BNO055_SensorValue) == 0xA0) return true;
	else return false;
}

void CheckDeviceId(){
	//Device ID @0xE0042000 , bits 0:11 , value 0x435 
	Send2Can((DEVICE_ID >> 8) & 0x0f, DEVICE_ID, 0xff, 0xff, 0xff, 0xff,0xff,0xff);
}

// -----------------------------------------------------------------------------------------------------------------------------
// Read the Sensor Temperature
// -----------------------------------------------------------------------------------------------------------------------------
void TEMPERATURE(void){
  Si705x_ReadTemperature();
}

// -----------------------------------------------------------------------------------------------------------------------------
// Read the IMU BNO055 I2C
// -----------------------------------------------------------------------------------------------------------------------------
void BNO055(void){
  BNO055_Read(BNO055_FirstValue);
  
}

// -----------------------------------------------------------------------------------------------------------------------------
// Read the IMU BNO055 UART
// -----------------------------------------------------------------------------------------------------------------------------
void BNO055_UART(void){
  BNO055_UART_Read();
}



void HAL_I2C_MasterTxCpltCallback(I2C_HandleTypeDef *I2cHandle)
{
  if(I2cHandle->Instance==I2C1){
    HAL_I2C_Master_Receive_DMA(&hi2c1, (uint16_t)Si705x_I2C_ADDRESS, (uint8_t*)Si705x_I2C_RxBuffer, sizeof(Si705x_I2C_RxBuffer));
  }
  if(I2cHandle->Instance==I2C2){
    HAL_I2C_Master_Receive_DMA(&hi2c2, (uint16_t)BNO055_I2C_ADDRESS, (uint8_t*)BNO055_RxBuffer, sizeof(BNO055_RxBuffer));
    
  }
}

void HAL_I2C_MasterRxCpltCallback(I2C_HandleTypeDef *I2cHandle)
{
  if(I2cHandle->Instance==I2C1){
    Temperature_code = (Si705x_I2C_RxBuffer[0]<<8) + Si705x_I2C_RxBuffer[1];
    Temperature_board = ((17572*Temperature_code)/65536) - 4685;  // HEX temperature in degrees Celsius (x100)

  }
  if(I2cHandle->Instance==I2C2){
    BNO055_SensorValue = BNO055_RxBuffer;
  }
}

void HAL_UART_TxCpltCallback(UART_HandleTypeDef *huart){
  //HAL_GPIO_TogglePin(LED_BLUE_GPIO_Port, LED_BLUE_Pin);
  HAL_UART_Receive_DMA(&huart1,(uint8_t*)UART_Rx_buffer, sizeof(UART_Rx_buffer));
}

void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart){
  //HAL_GPIO_TogglePin(LED_RED_GPIO_Port, LED_RED_Pin);
  HAL_UART_Transmit_DMA(&huart1,(uint8_t*)UART_Rx_buffer, sizeof(UART_Rx_buffer));
}

/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @param  None
  * @retval None
  */
void _Error_Handler(char * file, int line)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  while(1) 
  {
  }
  /* USER CODE END Error_Handler_Debug */ 
}

#ifdef USE_FULL_ASSERT

/**
   * @brief Reports the name of the source file and the source line number
   * where the assert_param error has occurred.
   * @param file: pointer to the source file name
   * @param line: assert_param error line source number
   * @retval None
   */
void assert_failed(uint8_t* file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
    ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */

}

#endif

/**
  * @}
  */ 

/**
  * @}
*/ 

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
