/**
  ******************************************************************************
  * @file    usb_desc.c
  * @author  MCD Application Team
  * @version V4.0.0
  * @date    21-January-2013
  * @brief   Descriptors for Relay Mouse Demo
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
#include "usb_desc.h"


//设备描述
const uint8_t Relay_DeviceDescriptor[RELAY_SIZ_DEVICE_DESC] =
  {
    0x12,                //该描述符大小
    0x01,								//设备描述符
    0x00, 0x02,      //USB版本号  USB2.0
    0x00,                //bDeviceClass       设备类代码 （USB官方）
    0x00,                //bDeviceSubClass  子类代码（USB官方）
    0x00,                //bDeviceProtocol   设备协议代码（USB官方）
    0x40,              	//端点0的最大包的大小
    0xBB,0xAA,       //生产厂商编号（USB官方）
   
    0x01,0x00,       //产品编号（制造商分配）

    0x01,0x00,        //设备出厂编号
 
    1,                      //设备厂商字符串索引
                                                 
    2,                      //产品描述字符串索引
                                                 
    3,                       //设备序列号字符串索引
                                                
    0x01                  //这个设备有多少个配置
  }
  ; 
//配置描述
const uint8_t Relay_ConfigDescriptor[RELAY_SIZ_CONFIG_DESC] =
  {
    0x09, 																												//配置描述符大小
    USB_CONFIGURATION_DESCRIPTOR_TYPE, //描述符类型
    RELAY_SIZ_CONFIG_DESC,    //总长度
    0x00,					//填充总长度
    0x01,         //总接口数
    0x01,         //配置的值
    0x00,      	//描述该配置的字符串索引，不描述
    0xE0,         //供电模式选择  (0xE0)
    0x32,        //设备需要最大电流100 mA
										//单位2mA
    /**************接口描述****************/
    /* 09 */
    0x09,      
    USB_INTERFACE_DESCRIPTOR_TYPE,//接口描述符
    0x00,         //这个是第0号接口
    0x00,         //备用接口编号
    0x02,         //使用的非0端点数
		
		/***USB HID自定义设备***/
    0x03,         //类型代码 HID
    0x00,         //子类型代码
    0x00,         //协议代码
		
    0,            		//不要字符串描述
		

		  /********************HID描述********************/
    /* 18 */
    0x09,       
    HID_DESCRIPTOR_TYPE, //HID
    0x10,0x01,     //HID版本号，BCD码
    0x21,         //设备所在国家的国家编号，美国键盘
    0x01,         //HID附属描述符数目，至少1个
		
		//=====附属描述符1=====//
    0x22,     //0x22报告描述符，0x23物理描述符
    RELAY_SIZ_REPORT_DESC,0x00,//报告描述符1大小
		//====================//
		
   
		
		 /******************** 端点描述********************/
    /* 27 */
    0x07,          /*bLength: Endpoint Descriptor size*/
    USB_ENDPOINT_DESCRIPTOR_TYPE, //端点描述符
		
		//设备端点地址 【bit7:1-->IN  0-->OUT，bit[3:0] 端点号】
    0x01,       //端点1是OUT   
	
    0x03,          //端点属性 （0x00控制，0x01同步 0x02批量 0x03中断）
    0x10, 0x00,   //该端点接收或发送的最大信息包 //不要写0XFF
    				
    0x01,          //轮询传送的时间间隔，单位ms（批量与控制可忽略，同步必须为1，中断可以选1~255）
    /* 34 */
		
	  /******************** 端点描述********************/
    
    0x07,          /*bLength: Endpoint Descriptor size*/
    USB_ENDPOINT_DESCRIPTOR_TYPE, //端点描述符
		
		//设备端点地址 【bit7:1-->IN  0-->OUT，bit[3:0] 端点号】
    0x82,       //端点2是IN
	
    0x03,          //端点属性 （0x00控制，0x01同步 0x02批量 0x03中断）
    0x10, 0x00,   //该端点接收或发送的最大信息包 //不要写0XFF
    				
    0x01,          //轮询传送的时间间隔，单位ms（批量与控制可忽略，同步必须为1，中断可以选1~255）
    /* 41 */
		
		
		
  }
	
  ;
	//报告描述符
const uint8_t Relay_ReportDescriptor[RELAY_SIZ_REPORT_DESC] =
  {
    0x05,0x01,         /*Usage Page(Generic Desktop)*/
    
    0x09,0x00,        /*Usage(0)*/ //不是0x00就是识别为其他设备
    
				0xA1,0x01,          /*Collection(Application)*/
		
				//================================//
				0x15,0x00,          /*Logical Minimum(0)*/
				
				0x25, 0xFF,          /*Logical Maximum(255)*/
			 
				0x19,0x01,          /*Usage Minimum(1)*/
				
				0x29,0x08,         /*Usage Maximum(8)*/
			
				0x95,0x08,          /*Report Count(8)*/

				0x75,0x08,          /*Report Size(8)*/
				
				0x81,0x02,					 /*Input(Data Var Abs)*/
			//================================//
			
				0x19,0x01,          /*Usage Minimum(1)*/
			
				0x29,0x08,         /*Usage Maximum(8)*/
				
				0x91,0x02,				 /*Output(Data Var Abs)*/
				//================================//
				
		0xC0									/*End Collection*/
  }
  ; 
//语言ID描述
const uint8_t Relay_StringLangID[RELAY_SIZ_STRING_LANGID] =
  {
    RELAY_SIZ_STRING_LANGID,
    USB_STRING_DESCRIPTOR_TYPE,
    0x09,//美国英语ID
    0x04
  }
  ; /* LangID = 0x0409: U.S. English */
//供应商
const uint8_t Relay_StringVendor[RELAY_SIZ_STRING_VENDOR] =
  {
		RELAY_SIZ_STRING_VENDOR,
		03,
		16,98, //成
		253,144,//都
		225,79,//信
		111,96,//息
		229,93,//工
		11,122,//程
		39,89,//大
		102,91,//学
  };
//设备名称
const uint8_t Relay_StringProduct[RELAY_SIZ_STRING_PRODUCT] =
{
18,
	03,
	243,141,//跳
	145,152,//频
	228,110,//滤
	226,108,//波
	104,86,//器
	167,99,//控
	54,82,//制
	104,86//器
};
//产品系列描述
uint8_t Relay_StringSerial[RELAY_SIZ_STRING_SERIAL] =
  {
		18,
		03,
		213,107,//毕
		26,78,//业
		190,139,//设
		161,139,//计
		86,0,//V
		49,0,//1
		46,0,//.
		48,0,//0
  };

/************************ (C) COPYRIGHT STMicroelectronics *****END OF FILE****/

