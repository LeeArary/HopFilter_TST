/**
  ******************************************************************************
  * @file    usb_int.c
  * @author  MCD Application Team
  * @version V4.0.0
  * @date    28-August-2012
  * @brief   Endpoint CTR (Low and High) interrupt's service routines
  ******************************************************************************
  * @attention
  *
  * <h2><center>&copy; COPYRIGHT 2012 STMicroelectronics</center></h2>
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
#include "usb_lib.h"

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
__IO uint16_t SaveRState;
__IO uint16_t SaveTState;

/* Extern variables ----------------------------------------------------------*/
extern void (*pEpInt_IN[7])(void);    /*  Handles IN  interrupts   */
extern void (*pEpInt_OUT[7])(void);   /*  Handles OUT interrupts   */

/* Private function prototypes -----------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/*******************************************************************************
* Function Name  : CTR_LP.
* Description    : Low priority Endpoint Correct Transfer interrupt's service
*                  routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CTR_LP(void)
{
  __IO uint16_t wEPVal = 0;

  while (((wIstr = _GetISTR()) & ISTR_CTR) != 0)//读取中断寄存器，是否正确传输
  {
    
    EPindex = (uint8_t)(wIstr & ISTR_EP_ID);//获取产生中断的端点序号
    if (EPindex == 0)//端点0
    {

      
	    SaveRState = _GetENDPOINT(ENDP0);//读取端点0状态寄存器
			
	    SaveTState = SaveRState & EPTX_STAT;//保存端点0发送状态
	    SaveRState &=  EPRX_STAT;	//保存端点0接收状态

	    _SetEPRxTxStatus(ENDP0,EP_RX_NAK,EP_TX_NAK);//设置RX，TX都是NAK, 数据处理不传数据

    
			//向PC发送数据
      if ((wIstr & ISTR_DIR) == 0)//IN令牌
      {
        _ClearEP_CTR_TX(ENDP0);//清除端点0正确发送标志位
        In0_Process();//处理IN令牌包
			
				_SetEPRxTxStatus(ENDP0,SaveRState,SaveTState);//设置端点0发送接收状态
		  return;
      }
      else//接收PC数据
      {
        wEPVal = _GetENDPOINT(ENDP0);
        //=======Setup包==========//
        if ((wEPVal &EP_SETUP) != 0)
        {
          _ClearEP_CTR_RX(ENDP0);//清除端点0接收标志
          Setup0_Process();//数据处理
					
		      _SetEPRxTxStatus(ENDP0,SaveRState,SaveTState);//设置端点0发送接收状态
          return;
        }
				//=======OUT包==========//
        else if ((wEPVal & EP_CTR_RX) != 0)
        {
          _ClearEP_CTR_RX(ENDP0);//清除端点0接收标志
          Out0_Process();//OUT包数据处理
          /* before terminate set Tx & Rx status */
     
		     _SetEPRxTxStatus(ENDP0,SaveRState,SaveTState);//设置端点0发送接收状态
          return;
        }
      }
    }
    else//非0端点
    {
    
      wEPVal = _GetENDPOINT(EPindex);
      if ((wEPVal & EP_CTR_RX) != 0)//正确接收
      {
        
        _ClearEP_CTR_RX(EPindex);//清除接收标志

        //=====================================//
        (*pEpInt_OUT[EPindex-1])();//OUT回调函数处理

      } 

      if ((wEPVal & EP_CTR_TX) != 0)//正确发送
      {
       
        _ClearEP_CTR_TX(EPindex);//清除发送标志

         //=====================================//
        (*pEpInt_IN[EPindex-1])();//IN回调函数处理
      } /* if((wEPVal & EP_CTR_TX) != 0) */

    }/* if(EPindex == 0) else */

  }/* while(...) */
}

/*******************************************************************************
* Function Name  : CTR_HP.
* Description    : High Priority Endpoint Correct Transfer interrupt's service 
*                  routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void CTR_HP(void)
{
  uint32_t wEPVal = 0;

  while (((wIstr = _GetISTR()) & ISTR_CTR) != 0)
  {
    _SetISTR((uint16_t)CLR_CTR); /* clear CTR flag */
    /* extract highest priority endpoint number */
    EPindex = (uint8_t)(wIstr & ISTR_EP_ID);
    /* process related endpoint register */
    wEPVal = _GetENDPOINT(EPindex);
    if ((wEPVal & EP_CTR_RX) != 0)
    {
      /* clear int flag */
      _ClearEP_CTR_RX(EPindex);

      /* call OUT service function */
      (*pEpInt_OUT[EPindex-1])();

    } /* if((wEPVal & EP_CTR_RX) */
    else if ((wEPVal & EP_CTR_TX) != 0)
    {
      /* clear int flag */
      _ClearEP_CTR_TX(EPindex);

      /* call IN service function */
      (*pEpInt_IN[EPindex-1])();


    } /* if((wEPVal & EP_CTR_TX) != 0) */

  }/* while(...) */
}

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
