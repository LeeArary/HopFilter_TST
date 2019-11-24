/**
  ******************************************************************************
  * @file    usb_endp.c
  * @author  MCD Application Team
  * @version V4.0.0
  * @date    21-January-2013
  * @brief   Endpoint routines
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT 2013 STMicroelectronics</center></h2>
  *
  * Licensed under MCD-ST Liberty SW License Agreement V2, (the "License");
  * You may not use this file except in compliance with the License.
  * You may obtain a copy of the License at:
  *
  *        http://www.st.com/software_license_agreement_liberty_v2
  *
  * Unless required by applicable law or agreed to in writing, software 
  * distributed under the License is distributed on an "AS IS" BASIS, 
  * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  * See the License for the specific language governing permissions and
  * limitations under the License.
  *
  ******************************************************************************
  */


/* Includes ------------------------------------------------------------------*/
#include "hw_config.h"
#include "usb_lib.h"
#include "usb_istr.h"
#include "usart.h"
#include "led.h"
#include "JVTWB.h"
/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
extern __IO uint8_t PrevXferComplete;

/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/
/*******************************************************************************
* Function Name  : EP1_OUT_Callback.
* Description    : EP1 OUT Callback Routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
//缓冲
u8 HID_Buf[8];
u8 temp[8];
//接收成功处理
void EP1_OUT_Callback(void)
{
		PMAToUserBufferCopy(HID_Buf, ENDP1_RXADDR, 8);
		PMAToUserBufferCopy(temp, ENDP1_RXADDR, 8);
	//EP1接收
	
	printf("aaa");
	if((HID_Buf[0]==0xFA)&&(HID_Buf[7]==0xFA))
	{

		rec_data=1;
		
			
	}
	 SetEPRxStatus(ENDP1, EP_RX_VALID);
	

	
	
}
//发送成功处理
void EP2_IN_Callback(void)//发送成功中断
{

//	 uint8_t ii;
//     for (ii=0;ii<8;ii++) HID_Buf[ii] = 0x00;
}

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/

