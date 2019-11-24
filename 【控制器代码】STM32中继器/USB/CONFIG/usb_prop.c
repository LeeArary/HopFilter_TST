/**
  ******************************************************************************
  * @file    usb_prop.c
  * @author  MCD Application Team
  * @version V4.0.0
  * @date    21-January-2013
  * @brief   All processing related to Relay Mouse Demo
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
#include "usb_lib.h"
#include "usb_conf.h"
#include "usb_prop.h"
#include "usb_desc.h"
#include "usb_pwr.h"
#include "hw_config.h"

/* Private typedef -----------------------------------------------------------*/
/* Private define ------------------------------------------------------------*/
/* Private macro -------------------------------------------------------------*/
/* Private variables ---------------------------------------------------------*/
uint32_t ProtocolValue;

/* -------------------------------------------------------------------------- */
/*  Structures initializations */
/* -------------------------------------------------------------------------- */

DEVICE Device_Table =
  {
    EP_NUM,
    1
  };

DEVICE_PROP Device_Property =
  {
    Relay_init,
    Relay_Reset,
    Relay_Status_In,
    Relay_Status_Out,
    Relay_Data_Setup,
    Relay_NoData_Setup,
    Relay_Get_Interface_Setting,
    Relay_GetDeviceDescriptor,
    Relay_GetConfigDescriptor,
    Relay_GetStringDescriptor,
    0,
    0x40 /*MAX PACKET SIZE*/
  };
USER_STANDARD_REQUESTS User_Standard_Requests =
  {
    Relay_GetConfiguration,
    Relay_SetConfiguration,
    Relay_GetInterface,
    Relay_SetInterface,
    Relay_GetStatus,
    Relay_ClearFeature,
    Relay_SetEndPointFeature,
    Relay_SetDeviceFeature,
    Relay_SetDeviceAddress
  };

ONE_DESCRIPTOR Device_Descriptor =
  {
    (uint8_t*)Relay_DeviceDescriptor,//设备描述符
    RELAY_SIZ_DEVICE_DESC//大小
  };

ONE_DESCRIPTOR Config_Descriptor =
  {
    (uint8_t*)Relay_ConfigDescriptor,//配置描述符
    RELAY_SIZ_CONFIG_DESC//大小
  };

ONE_DESCRIPTOR Relay_Report_Descriptor =
  {
    (uint8_t *)Relay_ReportDescriptor,//报告描述符
    RELAY_SIZ_REPORT_DESC//大小
  };

ONE_DESCRIPTOR Mouse_Hid_Descriptor =
  {
    (uint8_t*)Relay_ConfigDescriptor + RELAY_OFF_HID_DESC,//HID描述符
    RELAY_SIZ_HID_DESC//大小
  };

ONE_DESCRIPTOR String_Descriptor[4] =
  {
    {(uint8_t*)Relay_StringLangID, RELAY_SIZ_STRING_LANGID},//语言ID
    {(uint8_t*)Relay_StringVendor, RELAY_SIZ_STRING_VENDOR},//供应商
    {(uint8_t*)Relay_StringProduct, RELAY_SIZ_STRING_PRODUCT},//产品编号
    {(uint8_t*)Relay_StringSerial, RELAY_SIZ_STRING_SERIAL}//产品序列号
  };

/* Extern variables ----------------------------------------------------------*/
/* Private function prototypes -----------------------------------------------*/
/* Extern function prototypes ------------------------------------------------*/
/* Private functions ---------------------------------------------------------*/

/*******************************************************************************
* Function Name  : Relay_init.
* Description    : Relay Mouse init routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void Relay_init(void)
{

  /* Update the serial number string descriptor with the data from the unique
  ID*/
  Get_SerialNum();

  pInformation->Current_Configuration = 0;
  /* Connect the device */
  PowerOn();

  /* Perform basic device initialization operations */
  USB_SIL_Init();

  bDeviceState = UNCONNECTED;
}

/*******************************************************************************
* Function Name  : Relay_Reset.
* Description    : Relay Mouse reset routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void Relay_Reset(void)
{
  /* Set Relay_DEVICE as not configured */
  pInformation->Current_Configuration = 0;
  pInformation->Current_Interface = 0;/*the default Interface*/

  /* Current Feature initialization */
  pInformation->Current_Feature = Relay_ConfigDescriptor[7];
  SetBTABLE(BTABLE_ADDRESS);
  /* Initialize Endpoint 0 */
  SetEPType(ENDP0, EP_CONTROL);
  SetEPTxStatus(ENDP0, EP_TX_STALL);
  SetEPRxAddr(ENDP0, ENDP0_RXADDR);
  SetEPTxAddr(ENDP0, ENDP0_TXADDR);
  Clear_Status_Out(ENDP0);
  SetEPRxCount(ENDP0, Device_Property.MaxPacketSize);
  SetEPRxValid(ENDP0);

  /* Initialize Endpoint 1 */
	 SetEPType(ENDP1, EP_INTERRUPT);
	 SetEPRxAddr(ENDP1, ENDP1_RXADDR);
	 SetEPRxCount(ENDP1, 8);
	 SetEPRxStatus(ENDP1, EP_RX_VALID);
	 //SetEPTxStatus(ENDP1, EP_TX_DIS);
	 
	 
 /* Initialize Endpoint 2 */
      SetEPType(ENDP2, EP_INTERRUPT);
     SetEPTxAddr(ENDP2, ENDP2_TXADDR);
     SetEPTxCount(ENDP2, 8);
    // SetEPTxStatus(ENDP2, EP_TX_DIS);
     SetEPTxStatus(ENDP2, EP_TX_NAK);
 
 
  /* Set this device to response on default address */
  SetDeviceAddress(0);
  bDeviceState = ATTACHED;
}
/*******************************************************************************
* Function Name  : Relay_SetConfiguration.
* Description    : Update the device state to configured.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void Relay_SetConfiguration(void)
{
  DEVICE_INFO *pInfo = &Device_Info;

  if (pInfo->Current_Configuration != 0)
  {
    /* Device configured */
    bDeviceState = CONFIGURED;
  }
}
/*******************************************************************************
* Function Name  : Relay_SetConfiguration.
* Description    : Update the device state to addressed.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void Relay_SetDeviceAddress (void)
{
  bDeviceState = ADDRESSED;
}
/*******************************************************************************
* Function Name  : Relay_Status_In.
* Description    : Relay status IN routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void Relay_Status_In(void)
{}

/*******************************************************************************
* Function Name  : Relay_Status_Out
* Description    : Relay status OUT routine.
* Input          : None.
* Output         : None.
* Return         : None.
*******************************************************************************/
void Relay_Status_Out (void)
{}

/*******************************************************************************
* Function Name  : Relay_Data_Setup
* Description    : Handle the data class specific requests.
* Input          : Request Nb.
* Output         : None.
* Return         : USB_UNSUPPORT or USB_SUCCESS.
*******************************************************************************/
RESULT Relay_Data_Setup(uint8_t RequestNo)
{
  uint8_t *(*CopyRoutine)(uint16_t);

  CopyRoutine = NULL;
  if ((RequestNo == GET_DESCRIPTOR)
      && (Type_Recipient == (STANDARD_REQUEST | INTERFACE_RECIPIENT))
      && (pInformation->USBwIndex0 == 0))
  {
    if (pInformation->USBwValue1 == REPORT_DESCRIPTOR)
    {
//			printf("请求报告描述符\r\n");
      CopyRoutine = Relay_GetReportDescriptor;
    }
    else if (pInformation->USBwValue1 == HID_DESCRIPTOR_TYPE)
    {
//			printf("请求HID描述符\r\n");
      CopyRoutine = Relay_GetHIDDescriptor;
    }

  } /* End of GET_DESCRIPTOR */

  /*** GET_PROTOCOL ***/
  else if ((Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
           && RequestNo == GET_PROTOCOL)
  {
    CopyRoutine = Relay_GetProtocolValue;
  }
  if (CopyRoutine == NULL)
  {
    return USB_UNSUPPORT;
  }
  pInformation->Ctrl_Info.CopyData = CopyRoutine;//交给CopyData，并运行
  pInformation->Ctrl_Info.Usb_wOffset = 0;
  (*CopyRoutine)(0);
  return USB_SUCCESS;
}

/*******************************************************************************
* Function Name  : Relay_NoData_Setup
* Description    : handle the no data class specific requests
* Input          : Request Nb.
* Output         : None.
* Return         : USB_UNSUPPORT or USB_SUCCESS.
*******************************************************************************/
RESULT Relay_NoData_Setup(uint8_t RequestNo)
{
  if ((Type_Recipient == (CLASS_REQUEST | INTERFACE_RECIPIENT))
      && (RequestNo == SET_PROTOCOL))
  {
    return Relay_SetProtocol();
  }

  else
  {
    return USB_UNSUPPORT;
  }
}

/*******************************************************************************
* Function Name  : Relay_GetDeviceDescriptor.
* Description    : Gets the device descriptor.
* Input          : Length
* Output         : None.
* Return         : The address of the device descriptor.
*******************************************************************************/
uint8_t *Relay_GetDeviceDescriptor(uint16_t Length)
{
  return Standard_GetDescriptorData(Length, &Device_Descriptor);
}

/*******************************************************************************
* Function Name  : Relay_GetConfigDescriptor.
* Description    : Gets the configuration descriptor.
* Input          : Length
* Output         : None.
* Return         : The address of the configuration descriptor.
*******************************************************************************/
uint8_t *Relay_GetConfigDescriptor(uint16_t Length)
{
  return Standard_GetDescriptorData(Length, &Config_Descriptor);
}

/*******************************************************************************
* Function Name  : Relay_GetStringDescriptor
* Description    : Gets the string descriptors according to the needed index
* Input          : Length
* Output         : None.
* Return         : The address of the string descriptors.
*******************************************************************************/
uint8_t *Relay_GetStringDescriptor(uint16_t Length)
{
  uint8_t wValue0 = pInformation->USBwValue0;//从这里得到字符串索引号
  if (wValue0 > 4)//数组里面总共4个描述符
  {
    return NULL;
  }
  else
  {
    return Standard_GetDescriptorData(Length, &String_Descriptor[wValue0]);
  }
}

/*******************************************************************************
* Function Name  : Relay_GetReportDescriptor.
* Description    : Gets the HID report descriptor.
* Input          : Length
* Output         : None.
* Return         : The address of the configuration descriptor.
*******************************************************************************/
uint8_t *Relay_GetReportDescriptor(uint16_t Length)
{
  return Standard_GetDescriptorData(Length, &Relay_Report_Descriptor);
}

/*******************************************************************************
* Function Name  : Relay_GetHIDDescriptor.
* Description    : Gets the HID descriptor.
* Input          : Length
* Output         : None.
* Return         : The address of the configuration descriptor.
*******************************************************************************/
uint8_t *Relay_GetHIDDescriptor(uint16_t Length)
{
  return Standard_GetDescriptorData(Length, &Mouse_Hid_Descriptor);
}

/*******************************************************************************
* Function Name  : Relay_Get_Interface_Setting.
* Description    : tests the interface and the alternate setting according to the
*                  supported one.
* Input          : - Interface : interface number.
*                  - AlternateSetting : Alternate Setting number.
* Output         : None.
* Return         : USB_SUCCESS or USB_UNSUPPORT.
*******************************************************************************/
RESULT Relay_Get_Interface_Setting(uint8_t Interface, uint8_t AlternateSetting)
{
  if (AlternateSetting > 0)
  {
    return USB_UNSUPPORT;
  }
  else if (Interface > 0)
  {
    return USB_UNSUPPORT;
  }
  return USB_SUCCESS;
}

/*******************************************************************************
* Function Name  : Relay_SetProtocol
* Description    : Relay Set Protocol request routine.
* Input          : None.
* Output         : None.
* Return         : USB SUCCESS.
*******************************************************************************/
RESULT Relay_SetProtocol(void)
{
  uint8_t wValue0 = pInformation->USBwValue0;
  ProtocolValue = wValue0;
  return USB_SUCCESS;
}

/*******************************************************************************
* Function Name  : Relay_GetProtocolValue
* Description    : get the protocol value
* Input          : Length.
* Output         : None.
* Return         : address of the protocol value.
*******************************************************************************/
uint8_t *Relay_GetProtocolValue(uint16_t Length)
{
  if (Length == 0)
  {
    pInformation->Ctrl_Info.Usb_wLength = 1;
    return NULL;
  }
  else
  {
    return (uint8_t *)(&ProtocolValue);
  }
}

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/
