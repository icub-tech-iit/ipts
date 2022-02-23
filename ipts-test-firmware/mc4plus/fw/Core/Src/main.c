/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
	* @author					: davide.tome@iit.it
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; Copyright (c) 2019 STMicroelectronics.
  * All rights reserved.</center></h2>
  *
  * This software component is licensed by ST under Ultimate Liberty license
  * SLA0044, the "License"; You may not use this file except in compliance with
  * the License. You may obtain a copy of the License at:
  *                             www.st.com/SLA0044
  *
  ******************************************************************************
  */
/* USER CODE END Header */

/* Includes ------------------------------------------------------------------*/
#include "main.h"
#include "adc.h"
#include "can.h"
#include "eth.h"
#include "i2c.h"
#include "spi.h"
#include "tim.h"
#include "gpio.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */
typedef enum { false, true } bool;
/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
#define TVAUX 5000
#define TVIN 24000
#define AN_X 2500
#define TOLL_V 15
#define TOLL_A 25


#define EERPOM_I2C_ADDRESS 0xA0

#define MICREL_I2C_ADDRESS 0xBF				// MICREL - driving PS0 to 1 (slave mode) - see datasheet 
#define MICREL_FAMILY_ID_ADDR 0x00		// chip family id, addr
#define MICREL_CHIP_ID_ADDR 0x01			// chip id, addr
#define MICREL_FAMILY_ID 0x88   			// chip family id, value
#define MICREL_CHIP_ID 0x02						// chip id, value (bit 7-4)
#define MICREL_CHIP_ID_REV 0x03				// revision id, value (bit 3-0)

/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/

/* USER CODE BEGIN PV */
const char Firmware_ver = 2;
const char Firmware_rev = 0;

CAN_FilterTypeDef sFilterConfig;
CAN_TxHeaderTypeDef TxHeader;
CAN_RxHeaderTypeDef RxHeader;
uint8_t TxData[8];
uint8_t RxData[8];
uint32_t TxMailbox;
uint8_t count;
char cmd;
uint8_t can_msg;
uint8_t readEncoder[100];
uint8_t timeout=0;
uint16_t read_adc[4]; 
static uint8_t pulse[2] = {10, 90}; //to set 2 different PWM dutycycle
static uint16_t currs_ok[2] = {0x0700, 0x00F0}; //currents readings for above pulse values
uint8_t adc_index;

/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_NVIC_Init(void);
/* USER CODE BEGIN PFP */
void LED_Init(void);
void HeartBeat(void);
static void CAN_Config(void);
void Send2Can(uint8_t status, uint8_t d1, uint8_t d2, uint8_t d3, 
	uint8_t d4, uint8_t d5, uint8_t d6,uint8_t d7);
void ReadAdcX(ADC_HandleTypeDef *hadc);

	
//TESTS
void TestFirmwareVersion(void);
void TestLeds(void);
void Enable_5V(uint8_t en);
void TestAdcX(uint8_t test);
void TestEeprom(void);
void TestMicrel(void);
void TestAS5048A(void);
void TestEncoderX(TIM_HandleTypeDef *htim, TIM_TypeDef *TIM, GPIO_TypeDef *GPIO, uint16_t PIN );
void TestPwm(TIM_HandleTypeDef *htim, uint32_t channel, ADC_HandleTypeDef *hadc, uint8_t pulse[2], uint8_t index);
void TestFault(uint8_t level);

/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */

/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_CAN1_Init();
  MX_I2C1_Init();
  MX_I2C3_Init();
  MX_TIM8_Init();
  MX_TIM2_Init();
  MX_TIM1_Init();
  MX_TIM5_Init();
  MX_SPI2_Init();
  MX_SPI3_Init();
  MX_ADC1_Init();
  MX_ADC2_Init();
  MX_ADC3_Init();
  MX_TIM3_Init();
  MX_TIM4_Init();
  MX_TIM6_Init();
  MX_ETH_Init();

  /* Initialize interrupts */
  MX_NVIC_Init();
  /* USER CODE BEGIN 2 */
  LED_Init();
  CAN_Config();
  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
	can_msg = 0;

  while (1)
  {
    if(count == 4) HeartBeat(); 
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
		while(can_msg == 1)
		{
			can_msg = 0;
			
			switch(cmd)
			{
				case 0x00:
					TestFirmwareVersion();
				break;
				
				case 0x01:
					Enable_5V(1);
				break;
				
				case 0x02:
					Enable_5V(0);
				break;
				
				case 0x03:
					Enable_5V(1);		
					HAL_Delay(100);	
					TestLeds();
				  Enable_5V(0);		
				break;
				
				case 0x04:
					//Test VIN
					ReadAdcX(&hadc2);
				  TestAdcX(1);				
				break;
				
				case 0x05:
					//Test VAUX (5V)
					Enable_5V(1);		
					HAL_Delay(100);					
					ReadAdcX(&hadc1);
				  TestAdcX(2);
					Enable_5V(0);					
				break;
					
				case 0x06:
					//Test AN1
					Enable_5V(1);		
					HAL_Delay(100);					
					ReadAdcX(&hadc2);
					TestAdcX(3);
					Enable_5V(0);		
				break;
				
				case 0x07: 				
					//Test AN2
					Enable_5V(1);		
					HAL_Delay(100);					
					ReadAdcX(&hadc2);
					TestAdcX(4);
					Enable_5V(0);			
				break;
					
				case 0x08: 				
					//Test AN3
					Enable_5V(1);		
					HAL_Delay(100);					
					ReadAdcX(&hadc3);
					TestAdcX(5);
					Enable_5V(0);			
				break;
					
				case 0x09: 				
					//Test AN4
					Enable_5V(1);		
					HAL_Delay(100);					
					ReadAdcX(&hadc3);
					TestAdcX(6);
					Enable_5V(0);			
				break;
					
			  case 0x0A:
					//Test Eeprom
					HAL_GPIO_WritePin(I2C1_WP_GPIO_Port, I2C1_WP_Pin, 0);		
					HAL_Delay(100);
					TestEeprom();
					HAL_GPIO_WritePin(I2C1_WP_GPIO_Port, I2C1_WP_Pin, 1);
				break;
					
				case 0x0B:
					//Test Micrel
					HAL_GPIO_WritePin(GPIOC, ETH_SLV_Pin, 1);
					HAL_Delay(100);
					TestMicrel();
					HAL_GPIO_WritePin(GPIOC, ETH_SLV_Pin, 0);
				break;

				case 0x0C:
					//Test SPI
					Enable_5V(1);
				  HAL_Delay(100);
				
					TestAS5048A();
	
 					Enable_5V(0);
				break;	
				
				case 0x0D:
					//Test Encoder 1
					Enable_5V(1);
					HAL_Delay(100);
					TestEncoderX(&htim2, TIM2, GPIOG, ENC1_INDEX_Pin); // conn. P2
					Enable_5V(0);
				break;

				case 0x0E:
					//Test Encoder 2
					Enable_5V(1);
					HAL_Delay(100);
					TestEncoderX(&htim3, TIM3, GPIOG, ENC2_INDEX_Pin); // conn. P3
					Enable_5V(0);
				break;	
				
				case 0x0F:
					//Test Encoder 3
					Enable_5V(1);
					HAL_Delay(100);
					TestEncoderX(&htim4, TIM4, GPIOG, ENC3_INDEX_Pin); // conn. P4
					Enable_5V(0);
				break;	
				
				case 0x10:
					//Test Encoder 4
					Enable_5V(1);
					HAL_Delay(100);
					TestEncoderX(&htim5, TIM5, GPIOG, ENC4_INDEX_Pin); // conn. P5
					Enable_5V(0);
				break;

				case 0x11:
					//Test PWM on P2
					Enable_5V(1);
				  HAL_GPIO_WritePin(GPIOE, EN1_Pin, 1); // enables motor driver (P2)
					HAL_Delay(100);
				  TestPwm(&htim1, TIM_CHANNEL_1, &hadc1, pulse, 0); //tests PWM on P2 connector
				  HAL_Delay(100);
				  HAL_GPIO_WritePin(GPIOE, EN1_Pin, 0); 
					Enable_5V(0);
				break;
				
				case 0x12:
					//Test PWM on P3
					Enable_5V(1);
				  HAL_GPIO_WritePin(GPIOE, EN2_Pin, 1); // enables motor driver (P3)
					HAL_Delay(100);
				  TestPwm(&htim1, TIM_CHANNEL_3, &hadc1, pulse, 1); //tests PWM on P3 connector
				  HAL_Delay(100);
				  HAL_GPIO_WritePin(GPIOE, EN2_Pin, 0); 
					Enable_5V(0);
				break;
				
				case 0x13:
					//Test PWM on P4
					Enable_5V(1);
				  HAL_GPIO_WritePin(GPIOE, EN3_Pin, 1); // enables motor driver (P4)
					HAL_Delay(100);
				  TestPwm(&htim8, TIM_CHANNEL_1, &hadc3, pulse, 2); //tests PWM on P4 connector
				  HAL_Delay(100);
				  HAL_GPIO_WritePin(GPIOE, EN3_Pin, 0); 
					Enable_5V(0);
				break;
				
				case 0x14:
					//Test PWM on P5
					Enable_5V(1);
				  HAL_GPIO_WritePin(GPIOE, EN4_Pin, 1); // enables motor driver (P5)
					HAL_Delay(100);
				  TestPwm(&htim8, TIM_CHANNEL_3, &hadc3, pulse, 3); //tests PWM on P5 connector
				  HAL_Delay(100);
				  HAL_GPIO_WritePin(GPIOE, EN4_Pin, 0); 
					Enable_5V(0);
				break;
				
				case 0x15:
					//Test Fault switch low
					Enable_5V(1);
					HAL_Delay(100);
				  TestFault(0);
				  Enable_5V(0);
				break;
				
				case 0x16:
					//Test Fault switch high
					Enable_5V(1);
					HAL_Delay(100);
				  TestFault(1);
				  Enable_5V(0);
				break;
				
				case 0x17:
					//Test CAN communication
					HAL_Delay(100);
			    Send2Can(0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break;
				
				
				default: break;
			}
		//	can_msg = 0;
	  }
		
		HAL_Delay(100);
	  count++;
		
  }
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};

  /** Configure the main internal regulator output voltage 
  */
  __HAL_RCC_PWR_CLK_ENABLE();
  __HAL_PWR_VOLTAGESCALING_CONFIG(PWR_REGULATOR_VOLTAGE_SCALE1);
  /** Initializes the CPU, AHB and APB busses clocks 
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSE;
  RCC_OscInitStruct.HSEState = RCC_HSE_BYPASS;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSE;
  RCC_OscInitStruct.PLL.PLLM = 25;
  RCC_OscInitStruct.PLL.PLLN = 168;
  RCC_OscInitStruct.PLL.PLLP = RCC_PLLP_DIV2;
  RCC_OscInitStruct.PLL.PLLQ = 4;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }
  /** Initializes the CPU, AHB and APB busses clocks 
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV4;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV2;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_5) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief NVIC Configuration.
  * @retval None
  */
static void MX_NVIC_Init(void)
{
  /* CAN1_TX_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(CAN1_TX_IRQn, 1, 0);
  HAL_NVIC_EnableIRQ(CAN1_TX_IRQn);
  /* CAN1_RX0_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(CAN1_RX0_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(CAN1_RX0_IRQn);
  /* SPI3_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(SPI3_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(SPI3_IRQn);
  /* TIM6_DAC_IRQn interrupt configuration */
  HAL_NVIC_SetPriority(TIM6_DAC_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(TIM6_DAC_IRQn);
}

/* USER CODE BEGIN 4 */

/******************************/
/* FUNZIONI PER TEST MC4-PLUS */
/******************************/

/* Trasmisisone su CAN */

void Send2Can(uint8_t status, uint8_t d1, uint8_t d2, uint8_t d3, 
	uint8_t d4, uint8_t d5, uint8_t d6,uint8_t d7)	{
	
		  TxData[0] = status;
	    TxData[1] = d1;
      TxData[2] = d2;
      TxData[3] = d3;
			TxData[4] = d4;
			TxData[5] = d5;
	    TxData[6] = d6;
      TxData[7] = d7;
		
	/* Start the Transmission process */
	if (HAL_CAN_AddTxMessage(&hcan1, &TxHeader, TxData, &TxMailbox) != HAL_OK)
	{
		/* Transmission request Error */
		Error_Handler();
	}
}
/* Inizializza i Leds e blinka 6 volte come segnale di partenza */
void LED_Init(void){
	
	HAL_GPIO_WritePin(GPIOH, Led1_Pin|Led2_Pin|Led3_Pin|LedCAN_Pin, 1);
	HAL_Delay(500);
	while(count<6)
	{
		HAL_GPIO_TogglePin(GPIOH,  Led1_Pin|Led2_Pin|Led3_Pin|LedCAN_Pin);
		HAL_Delay(100);
		count++;
	}
	count=0;	
}
//pulsazione Led3; indica fw running
void HeartBeat(){
	
	 HAL_GPIO_TogglePin(GPIOH, Led3_Pin);
	 count = 0;
}

// configurazione CAN
static void CAN_Config(void){
	
  CAN_FilterTypeDef  sFilterConfig;

  if (HAL_CAN_Init(&hcan1) != HAL_OK)
  {
    /* Initialization Error */
    Error_Handler();
  }

  /*## Configure the CAN Filter ###########################################*/
  sFilterConfig.FilterBank = 0;
  sFilterConfig.FilterMode = CAN_FILTERMODE_IDMASK;
  sFilterConfig.FilterScale = CAN_FILTERSCALE_32BIT;
  sFilterConfig.FilterIdHigh = 0x0000;
  sFilterConfig.FilterIdLow = 0x0000;
  sFilterConfig.FilterMaskIdHigh = 0x0000;
  sFilterConfig.FilterMaskIdLow = 0x0000;
  sFilterConfig.FilterFIFOAssignment = CAN_RX_FIFO0;
  sFilterConfig.FilterActivation = ENABLE;
  sFilterConfig.SlaveStartFilterBank = 14;

  if (HAL_CAN_ConfigFilter(&hcan1, &sFilterConfig) != HAL_OK)
  {
    /* Filter configuration Error */
    Error_Handler();
  }

  /*## Start the CAN peripheral ###########################################*/
  if (HAL_CAN_Start(&hcan1) != HAL_OK)
  {
    /* Start Error */
    Error_Handler();
  }

  /*## Activate CAN RX notification #######################################*/
  if (HAL_CAN_ActivateNotification(&hcan1, CAN_IT_RX_FIFO0_MSG_PENDING | CAN_IT_TX_MAILBOX_EMPTY) != HAL_OK)
	{
		/* Notification Error */
		Error_Handler();
	}
	
  /*## Configure Transmission process #####################################*/
  TxHeader.StdId = 0x551;
  TxHeader.ExtId = 0x551;
  TxHeader.RTR = CAN_RTR_DATA;
  TxHeader.IDE = CAN_ID_STD;
  TxHeader.DLC = 8;
  TxHeader.TransmitGlobalTime = DISABLE;
}


void TestFirmwareVersion(void){
	HAL_Delay(200);
  Send2Can(Firmware_ver, Firmware_rev , 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
}

void Enable_5V(uint8_t en){
	HAL_GPIO_WritePin(GPIOE, EN_5V_Pin, en);
	HAL_Delay(100);
}

void TestLeds(void){
	int i=0;
	HAL_GPIO_WritePin(GPIOH, Led1_Pin|Led2_Pin|Led3_Pin|LedCAN_Pin, 1);
	HAL_Delay(500);
	
	while(i<4)
	{
		HAL_GPIO_TogglePin(GPIOH,  Led1_Pin);
		HAL_Delay(500);
		i++;
	}
	i=0;
	while(i<4)
	{
		HAL_GPIO_TogglePin(GPIOH,  Led2_Pin);
		HAL_Delay(500);
		i++;
	}
	i=0;
	while(i<4)
	{
		HAL_GPIO_TogglePin(GPIOH,  Led3_Pin);
		HAL_Delay(500);
		i++;
	}
	i=0;	
	while(i<4)
	{
		HAL_GPIO_TogglePin(GPIOH,  LedCAN_Pin);
		HAL_Delay(500);
		i++;
	}
	
	
	count=0;	
}

void TestAdcX(uint8_t test){
	
	switch(test){
			case 1:
				if (read_adc[2] < TVIN - ((TVIN*TOLL_V)/100) || read_adc[2] > (TVIN + (TVIN*TOLL_V)/100)) Send2Can(0xBB, read_adc[2] >> 8, read_adc[2], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				else Send2Can(0xAA, read_adc[2] >> 8, read_adc[2], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break;
				
			case 2:
				if (read_adc[2] < TVAUX - ((TVAUX*TOLL_V)/100) || read_adc[2] > (TVAUX + (TVAUX*TOLL_V)/100)) Send2Can(0xBB, read_adc[2] >> 8, read_adc[2], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				else Send2Can(0xAA, read_adc[2] >> 8, read_adc[2], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break;
				
			case 3:
				if (read_adc[0] < AN_X - ((AN_X*TOLL_V)/100) || read_adc[0] > (AN_X + (AN_X*TOLL_V)/100)) Send2Can(0xBB, read_adc[0] >> 8, read_adc[0], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				else Send2Can(0xAA, read_adc[0] >> 8, read_adc[0], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break;
				
			case 4:
				if (read_adc[1] < AN_X - ((AN_X*TOLL_V)/100) || read_adc[1] > (AN_X + (AN_X*TOLL_V)/100)) Send2Can(0xBB, read_adc[1] >> 8, read_adc[1], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				else Send2Can(0xAA, read_adc[1] >> 8, read_adc[1], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break;
				
			case 5:
				if (read_adc[0] < AN_X - ((AN_X*TOLL_V)/100) || read_adc[0] > (AN_X + (AN_X*TOLL_V)/100)) Send2Can(0xBB, read_adc[0] >> 8, read_adc[0], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				else Send2Can(0xAA, read_adc[0] >> 8, read_adc[0], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break;
				
			case 6:
				if (read_adc[1] < AN_X - ((AN_X*TOLL_V)/100) || read_adc[1] > (AN_X + (AN_X*TOLL_V)/100)) Send2Can(0xBB, read_adc[1] >> 8, read_adc[1], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				else Send2Can(0xAA, read_adc[1] >> 8, read_adc[1], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
				break;
			
			default: break;
	}
} 

void TestPwm(TIM_HandleTypeDef *htim, uint32_t channel, ADC_HandleTypeDef *hadc, uint8_t pulse[2], uint8_t index)
{
	
	bool pass[2];
	uint16_t curr[2];
	uint8_t tcount[2]={0,0};
	uint8_t tcycles = 30; // times to loop the test
	
	pass[0] = false; 
	pass[1] = false; 
	
	for(uint8_t i=0; i <2; i++)
	{
		__HAL_TIM_SET_COMPARE(htim, channel, pulse[i]);
		HAL_TIM_PWM_Start(htim, channel);
			
		HAL_Delay(500);
		
		for(int8_t k = 0; k < tcycles; k++)
		{
			ReadAdcX(hadc);
		
			HAL_Delay(10);

			curr[i] = read_adc[index];
			
			if(i==0)         {if (read_adc[index] > currs_ok[i]) pass[i] = true;}
			else if( i == 1) {if (read_adc[index] < currs_ok[i]) pass[i] = true;}
	
			//curr[i] = read_adc[index];
			
 		//Send2Can(i, curr[i] >> 8, curr[i], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);

		tcount[i] = k;
	  if(pass[i]) k=tcycles;	
			
		}
	  
    HAL_TIM_PWM_Stop(htim, channel);		
	}
	
	if(pass[0] && pass[1]) Send2Can(0xAA, curr[0] >> 8, curr[0], curr[1] >> 8, curr[1], tcount[0], tcount[1], 0xFF);
	else Send2Can(0xBB,  curr[0] >> 8, curr[0], curr[1] >> 8, curr[1], tcount[0], tcount[1], 0xFF);
	

//****************************************************************************************************

//	bool passH, passL;
//	uint16_t curr[2];
//	uint8_t tcount=0;
//	uint8_t tcycles = 10; // times to loop the test
//	
//	passH = false; 
//	passL = false; 
//	
//	for(uint8_t k=0; k <tcycles; k++)
//	{
//		HAL_Delay(200);
//		
//		for(int8_t i = 0; i <2; i++)
//		{
//			__HAL_TIM_SET_COMPARE(htim, channel, pulse[i]);
//			HAL_TIM_PWM_Start(htim, channel);
//			
//			HAL_Delay(500);
//			
//			ReadAdcX(hadc);
//		
//			HAL_Delay(50);

//			curr[i] = read_adc[index];
//			
//			if(i==0)         {if (read_adc[index] > currs_ok[i]) passH = true;}
//			else if( i == 1) {if (read_adc[index] < currs_ok[i]) passL = true;}
//	
//			//curr[i] = read_adc[index];
//			
// 		Send2Can(0xDD, curr[i] >> 8, curr[i], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);

//			HAL_TIM_PWM_Stop(htim, channel);
//		}
//	  tcount = k;
//	  if(passH && passL) k=tcycles;		
//	}
//	
//	if(passH && passL) Send2Can(0xAA, curr[0] >> 8, curr[0], curr[1] >> 8, curr[1], tcount, 0xFF, 0xFF);
//	else Send2Can(0xBB,  curr[0] >> 8, curr[0], curr[1] >> 8, curr[1], tcount, 0xFF, 0xFF);

//****************************************************************************************************

//bool passH, passL;
//	uint16_t curr[2];
//	uint8_t tcount=0;
//	uint8_t tcycles = 20; // times to loop the test
//	
//	passH = false; 
//	passL = false; 
//	
//	

//		
//		__HAL_TIM_SET_COMPARE(htim, channel, pulse[0]);
//			HAL_TIM_PWM_Start(htim, channel);
//			
//			HAL_Delay(500);
//			
//	for(uint8_t k=0; k <tcycles; k++)
//	{
//			ReadAdcX(hadc);
//		
//			HAL_Delay(50);

//			curr[0] = read_adc[index];
//			
//			if (read_adc[index] > currs_ok[0]) passH = true;
//			
// 	//	Send2Can(0xDD, curr[0] >> 8, curr[0], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);

//	  tcount = k;
//	  if(passH) k=tcycles;		
//	}
//	
//	HAL_TIM_PWM_Stop(htim, channel);
//	
//	tcount =0;
//	 
//		__HAL_TIM_SET_COMPARE(htim, channel, pulse[1]);
//			HAL_TIM_PWM_Start(htim, channel);
//			
//			HAL_Delay(500);
//	
//	for(uint8_t k=0; k <tcycles; k++)
//	{
//			ReadAdcX(hadc);
//		
//			HAL_Delay(50);

//			curr[1] = read_adc[index];
//			
//			if (read_adc[index] < currs_ok[1]) passL = true;
//			
// //		Send2Can(0xDD, curr[1] >> 8, curr[1], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);

//		
//	  tcount = k;
//	  if(passL) k=tcycles;		
//	}
//	
//				HAL_TIM_PWM_Stop(htim, channel);

//	
//	if(passH && passL) Send2Can(0xAA, curr[0] >> 8, curr[0], curr[1] >> 8, curr[1], tcount, 0xFF, 0xFF);
//	else Send2Can(0xBB,  curr[0] >> 8, curr[0], curr[1] >> 8, curr[1], tcount, 0xFF, 0xFF);

}

void ReadAdcX(ADC_HandleTypeDef *hadc){
	/**ADC1 GPIO Configuration    
    PA4     ------> ADC1_IN4	CUR2
    PA5     ------> ADC1_IN5	TVAUX
    PA3     ------> ADC1_IN3 	CUR1
*/
	
	/**ADC2 GPIO Configuration    
    PC0     ------> ADC2_IN10	TVIN
    PB1     ------> ADC2_IN9	AN2
    PB0     ------> ADC2_IN8  AN1
*/
	
	/**ADC3 GPIO Configuration    
    PF4     ------> ADC3_IN14	AN3
    PF5     ------> ADC3_IN15	AN4
    PF9     ------> ADC3_IN7	CUR4
    PF8     ------> ADC3_IN6	CUR3
*/
	  read_adc[0] = 0;
		read_adc[1] = 0;
		read_adc[2] = 0;
		read_adc[3] = 0;
			
	//	HAL_Delay(150);
		
		HAL_ADC_Start(hadc); 
		HAL_ADC_PollForConversion(hadc, 500); 
		read_adc[0] = HAL_ADC_GetValue(hadc);
		HAL_ADC_PollForConversion(hadc, 500); 	
		read_adc[1] = HAL_ADC_GetValue(hadc);
		HAL_ADC_PollForConversion(hadc, 500); 
		read_adc[2] = HAL_ADC_GetValue(hadc);
    if(hadc == &hadc3){
			HAL_ADC_PollForConversion(hadc, 500); 
		  read_adc[3] = HAL_ADC_GetValue(hadc);		
		}			
		
	//	HAL_Delay(50);
		HAL_ADC_Stop(hadc);
		
		if(hadc == &hadc1){
			read_adc[0] = (read_adc[0] - 2048)*2.44;			//CUR1
			read_adc[1] = (read_adc[1] - 2048)*2.44;			//CUR2
			read_adc[2] = 1000*3.3*5/3*read_adc[2]/4096;	//TVAUX
		}
		
		if(hadc == &hadc2){
			read_adc[0] = 1000*3.3*5/3*read_adc[0]/4096;	//AN1
			read_adc[1] = 1000*3.3*5/3*read_adc[1]/4096;	//AN2
			read_adc[2] = 1000*3.3*21*read_adc[2]/4096;		//TVIN
		}
		
		if(hadc == &hadc3){
			
			read_adc[0] = 1000*3.3*5/3*read_adc[0]/4096;	//AN3
			read_adc[1] = 1000*3.3*5/3*read_adc[1]/4096;	//AN4
			read_adc[2] = (read_adc[2] - 2048)*2.44;			//CUR3
			read_adc[3] = (read_adc[3] - 2048)*2.44;			//CUR4
		}
		
		 
}

void TestEeprom(void){
	// Scrivo e rileggo 0xAA,0x55 agli indirizzi da 0 a 5
	
	uint8_t data_to_write[2] = {0xAA, 0x55};
  uint8_t data_to_read;
	bool pass = true;
	
	for(uint8_t i=0; i<2; i++)
	{
		for(uint8_t k=0; k<6; k++)
		{
			HAL_I2C_Mem_Write(&hi2c1, EERPOM_I2C_ADDRESS, k, 0xFF, &data_to_write[i], 1, 10);
			HAL_Delay(10);
			HAL_I2C_Mem_Read(&hi2c1, EERPOM_I2C_ADDRESS, k, 0xFF, &data_to_read, 1, 1);
			HAL_Delay(10);

			if(data_to_read != data_to_write[i]) pass = false;
		}
	}

	if(pass) Send2Can(0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
	else Send2Can(0xBB, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
}

void TestMicrel(void){
	// leggo i restri family_id(8bit) e chip_id (bits 7-4 id, bits 3-0 rev)
	
	uint8_t addresses[3] = {MICREL_FAMILY_ID_ADDR, MICREL_CHIP_ID_ADDR, MICREL_CHIP_ID_ADDR};
	uint8_t values[3] = {MICREL_FAMILY_ID, MICREL_CHIP_ID, MICREL_CHIP_ID_REV};
	bool pass = true;
	uint8_t read_value[3];

	for(uint8_t i=0; i<3; i++)
	{
		HAL_I2C_Mem_Read(&hi2c3, MICREL_I2C_ADDRESS, addresses[i], I2C_MEMADD_SIZE_8BIT, &read_value[i], 1, 10000);
		HAL_Delay(10);
		switch(i)
		{
			case 0: if(read_value[i] != values[i]) pass = false; break;
			case 1: if((read_value[i] >> 4) != values[i]) pass = false; break;
			case 2: if((read_value[i] & 0x0F) != values[i]) pass = false; break;

			default: break;			
		}
		
	}

	if(pass) Send2Can(0xAA, read_value[0], read_value[1] >> 4, read_value[2] & 0x0F, 0xFF, 0xFF, 0xFF, 0xFF);
	else Send2Can(0xBB, read_value[0], read_value[1] >> 4, read_value[2] & 0x0F, 0xFF, 0xFF, 0xFF, 0xFF);
}


void TestAS5048A(void){
	
	bool pass = true;
	uint8_t tx1[2] = {0x80, 0x00}; // 0x80 0x00 -> NOP ; 0xFF 0xFF -> angle
	uint8_t tx2[2] = {0x80, 0x00}; 
	uint8_t rx1[2] = {0xAA, 0xBB};
	uint8_t rx2[2] = {0xCC, 0xDD};
	uint8_t res[2] = {0xAA, 0xAA};
	SPI_HandleTypeDef *hspi;
	
for(int k=0; k<2; k++){
  switch(k){
		case 0: hspi = &hspi2; break;
		case 1: hspi = &hspi3; break;
		default : break;
	}
	
	for(int i=0; i < 10; i++){
		if(k == 1) HAL_GPIO_WritePin(GPIOC, SPI1_NSEL_Pin, 0);
		else HAL_GPIO_WritePin(GPIOI, SPI2_NSEL_Pin, 0);
		HAL_Delay(50);
   	HAL_SPI_TransmitReceive(hspi, tx1, rx1, 2, 200);
		HAL_Delay(50);
   	HAL_SPI_TransmitReceive(hspi, tx2, rx2, 2, 200);
		HAL_Delay(50);		
		if(k == 1) HAL_GPIO_WritePin(GPIOC, SPI1_NSEL_Pin, 1);
		else HAL_GPIO_WritePin(GPIOI, SPI2_NSEL_Pin, 1);
		
//		Send2Can(rx1[0], rx1[1], rx2[0], rx2[1], 0xFF, 0xFF, 0xFF, 0xFF);
	}
	
	
		if(rx1[0] != 0x00 || rx1[1] != 0x00 || rx2[0] != 0x00 || rx2[1] != 0x00){
			pass = false;
			res[k] = 0xBB;
		}
		else res[k] = 0xAA;
}
		if(pass) Send2Can(0xAA,res[0], res[1], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
	  else Send2Can(0xBB, res[0], res[1], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
	
}

void HAL_SPI_RxCpltCallback(SPI_HandleTypeDef *hspi)
{
   // Send2Can(rx2[0], rx2[1], rx2[2], rx2[3], 0xFF, 0xFF, 0xFF, 0xFF);
    volatile uint8_t y = 5;
	
}


void TestEncoderX(TIM_HandleTypeDef *htim, TIM_TypeDef *TIM, GPIO_TypeDef *GPIO, uint16_t PIN ){ 
	bool pass = true;
	uint8_t k=0;
	uint8_t index=1;
	uint8_t fail=0; //fail==1 -> fallito index, fail==2 fallito lettura
	
  HAL_TIM_Encoder_Start(htim,TIM_CHANNEL_ALL); 
	HAL_TIM_Base_Start_IT(&htim6);

	TIM->CNT = 8;
	
	// controllo che venga premuto il pulsante sul jig - timeout gestito con TIM6
	while(index == 1 && timeout <4)
	{
		index = HAL_GPIO_ReadPin(GPIO, PIN);
	}
	
	if(index == 1 || timeout == 4) {fail = 1; pass = false;}

	timeout=0;
	
	//controllo che venga girato l' encoder (e che legga) - timeout gestito con TIM6
	while(k != 0x10 && timeout < 4 && pass == true)
	{
		k=TIM->CNT;	
		HAL_Delay(10);
	}
	
	if(timeout ==4) {fail = 2; pass = false;}
	
  TIM->CNT=0;
	timeout=0;
	
  HAL_TIM_Encoder_Stop(htim,TIM_CHANNEL_ALL); 
	HAL_TIM_Base_Stop_IT(&htim6);
	
	if(pass) Send2Can(0xAA, fail, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
	else Send2Can(0xBB, fail, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);

}

void TestFault(uint8_t level){

	uint8_t mot_12_f, mot_34_f;
	
	mot_12_f = HAL_GPIO_ReadPin(GPIOA, MOT12_FAULT_Pin);
	mot_34_f = HAL_GPIO_ReadPin(GPIOI, MOT34_FAULT_Pin);

  if(mot_12_f == level && mot_34_f == level)  Send2Can(0xAA, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
	else  Send2Can(0xBB, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
}



/**********************/
/* CALLBACK */
/**********************/

void HAL_ADC_ConvCpltCallback(ADC_HandleTypeDef* hadc1)
{
// uint16_t adc_value;
//	
// adc_value = HAL_ADC_GetValue(hadc1);
// adc_value = (adc_value - 2048)*2.44;
// read_adc[adc_index] = adc_value;
// Send2Can(0xFF,  read_adc[adc_index] >> 8, read_adc[adc_index], 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
// adc_index++;
	
//HAL_ADC_Start_IT(hadc1); // Re-Start ADC1 under Interrupt
                         // this is necessary because we don'use
                         // the Continuos Conversion Mode
}

void HAL_CAN_TxMailbox0CompleteCallback(CAN_HandleTypeDef *hcan)
{
	 HAL_GPIO_TogglePin(GPIOH, LedCAN_Pin);

}

void HAL_CAN_RxFifo0MsgPendingCallback(CAN_HandleTypeDef *hcan)
{
	HAL_CAN_GetRxMessage(&hcan1,CAN_RX_FIFO0,&RxHeader,RxData);
	cmd = RxData[0];
	can_msg = 1;
//	Send2Can(cmd, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF);
  HAL_GPIO_TogglePin(GPIOH, LedCAN_Pin);
}

void HAL_TIM_PeriodElapsedCallback(TIM_HandleTypeDef *htim)
{
	timeout++;
}



/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */

  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{ 
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     tex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
