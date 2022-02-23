/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
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

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32f4xx_hal.h"
#include "stm32f4xx_hal.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */

/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */

/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define EN_5V_Pin GPIO_PIN_1
#define EN_5V_GPIO_Port GPIOE
#define ENC3_INDEX_Pin GPIO_PIN_14
#define ENC3_INDEX_GPIO_Port GPIOG
#define ENC2_INDEX_Pin GPIO_PIN_13
#define ENC2_INDEX_GPIO_Port GPIOG
#define ENC4_INDEX_Pin GPIO_PIN_15
#define ENC4_INDEX_GPIO_Port GPIOG
#define ENC1_INDEX_Pin GPIO_PIN_12
#define ENC1_INDEX_GPIO_Port GPIOG
#define DRV1FLT_Pin GPIO_PIN_0
#define DRV1FLT_GPIO_Port GPIOD
#define DRV2FLT_Pin GPIO_PIN_1
#define DRV2FLT_GPIO_Port GPIOD
#define SPI1_NSEL_Pin GPIO_PIN_13
#define SPI1_NSEL_GPIO_Port GPIOC
#define MOT34_FAULT_Pin GPIO_PIN_4
#define MOT34_FAULT_GPIO_Port GPIOI
#define DRV4FLT_Pin GPIO_PIN_3
#define DRV4FLT_GPIO_Port GPIOD
#define DRV3FLT_Pin GPIO_PIN_2
#define DRV3FLT_GPIO_Port GPIOD
#define SPI2_NSEL_Pin GPIO_PIN_0
#define SPI2_NSEL_GPIO_Port GPIOI
#define Led1_Pin GPIO_PIN_2
#define Led1_GPIO_Port GPIOH
#define Led2_Pin GPIO_PIN_3
#define Led2_GPIO_Port GPIOH
#define ETH_SLV_Pin GPIO_PIN_8
#define ETH_SLV_GPIO_Port GPIOC
#define Led3_Pin GPIO_PIN_4
#define Led3_GPIO_Port GPIOH
#define AN3_Pin GPIO_PIN_4
#define AN3_GPIO_Port GPIOF
#define LedCAN_Pin GPIO_PIN_5
#define LedCAN_GPIO_Port GPIOH
#define AN4_Pin GPIO_PIN_5
#define AN4_GPIO_Port GPIOF
#define CUR4_Pin GPIO_PIN_9
#define CUR4_GPIO_Port GPIOF
#define CUR3_Pin GPIO_PIN_8
#define CUR3_GPIO_Port GPIOF
#define TVIN_Pin GPIO_PIN_0
#define TVIN_GPIO_Port GPIOC
#define CUR2_Pin GPIO_PIN_4
#define CUR2_GPIO_Port GPIOA
#define EN2_Pin GPIO_PIN_13
#define EN2_GPIO_Port GPIOE
#define I2C1_WP_Pin GPIO_PIN_10
#define I2C1_WP_GPIO_Port GPIOD
#define MOT12_FAULT_Pin GPIO_PIN_6
#define MOT12_FAULT_GPIO_Port GPIOA
#define TVAUX_Pin GPIO_PIN_5
#define TVAUX_GPIO_Port GPIOA
#define EN3_Pin GPIO_PIN_14
#define EN3_GPIO_Port GPIOE
#define CUR1_Pin GPIO_PIN_3
#define CUR1_GPIO_Port GPIOA
#define AN2_Pin GPIO_PIN_1
#define AN2_GPIO_Port GPIOB
#define AN1_Pin GPIO_PIN_0
#define AN1_GPIO_Port GPIOB
#define EN1_Pin GPIO_PIN_12
#define EN1_GPIO_Port GPIOE
#define EN4_Pin GPIO_PIN_15
#define EN4_GPIO_Port GPIOE
/* USER CODE BEGIN Private defines */

/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
