#include "JVTWB.h"

void JVTWB_Init(void)
{
	 GPIO_InitTypeDef  GPIO_InitStructure;
 	
	 RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);	 
		
	 GPIO_InitStructure.GPIO_Pin = GPIO_Pin_All;			
	 GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP; 		
	 GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;		 
	 GPIO_Init(GPIOC, &GPIO_InitStructure);	
	 GPIO_ResetBits(GPIOC,GPIO_InitStructure.GPIO_Pin);
	 DIN =0;
	 SEL=0;
	 CLK=0;
	
	
	//并行控制线 A10 nWE nOE An1 nByte
	RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB, ENABLE);	 
	GPIO_InitStructure.GPIO_Pin = GPIO_Pin_0|
																GPIO_Pin_1|
																GPIO_Pin_3|
																GPIO_Pin_12|
																GPIO_Pin_13;
	
	GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP; 		
	GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;		 
	GPIO_Init(GPIOB, &GPIO_InitStructure);	
	A10=0;
	nWE=1;
	nOE=1;
	An1=0;
	nByte=0;
	//并行忙状态 RnB
	GPIO_InitStructure.GPIO_Pin=GPIO_Pin_2;
	//上拉  GPIO_InitStructure.GPIO_Mode =GPIO_Mode_IPD
	//下拉  GPIO_InitStructure.GPIO_Mode =GPIO_Mode_IPU
	//浮空  GPIO_InitStructure.GPIO_Mode =GPIO_Mode_IN_FLOATING
	GPIO_InitStructure.GPIO_Mode =GPIO_Mode_IPD;
	GPIO_Init(GPIOB, &GPIO_InitStructure);	
	
	//并行数据线默认输出模式
	ParaDataLine_OUT();
}
//并行数据线-输出模式
void ParaDataLine_OUT()
{
	 GPIO_InitTypeDef  GPIO_InitStructure;
 	
	 RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB, ENABLE);	 
		
	 GPIO_InitStructure.GPIO_Pin = GPIO_Pin_4|GPIO_Pin_5|
																GPIO_Pin_6|GPIO_Pin_7|
																GPIO_Pin_8|GPIO_Pin_9|
															GPIO_Pin_10|GPIO_Pin_11;
															
	 GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP; 		
	 GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;		 
	 GPIO_Init(GPIOB, &GPIO_InitStructure);	
	 GPIO_ResetBits(GPIOB,GPIO_InitStructure.GPIO_Pin);
}
	
//并行数据线-输入模式
void ParaDataLine_IN()
{
	 GPIO_InitTypeDef  GPIO_InitStructure;
 	
	 RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB, ENABLE);	 
		
	 GPIO_InitStructure.GPIO_Pin = GPIO_Pin_4|GPIO_Pin_5|
																GPIO_Pin_6|GPIO_Pin_7|
																GPIO_Pin_8|GPIO_Pin_9|
															GPIO_Pin_10|GPIO_Pin_11;
															
		GPIO_InitStructure.GPIO_Mode =GPIO_Mode_IN_FLOATING; 
	 GPIO_Init(GPIOB, &GPIO_InitStructure);	
	
}

void JVTWB_SrialSetData(u16 Data)
{
	int i;
	Data<<=3;
	for(i=0;i<13;i++)
	{
		CLK=0;
		if(Data&0x8000) DIN=1;
		else DIN=0;
		delay_us(2);
		CLK=1;
		delay_us(2);
		CLK=0;
		Data<<=1;
		delay_us(2);
	}
	delay_us(2);
	SEL=1;
	delay_us(2);
	SEL=0;
}

void JVTWB_ParallSetData(u16 Data)
{
	GPIO_Write(GPIOC,Data);
}



void oneBusCycle(u16 addr, u8 data)
{
	u16 u16data=data;
	u16 temp;
	// setup address
	nOE = 1;
	nWE = 1;
//地址
	
	GPIO_Write(GPIOC,addr);
	if(addr&0x400) A10=1;//bug
	else A10=0;
	
	delay_us(200);
	nWE = 0; // address latch
	// setup data
//数据
	temp=0x300F;
	temp=temp&GPIO_ReadOutputData(GPIOB);
	temp|=(u16data<<4);
	
	GPIO_Write(GPIOB,temp);
	
	delay_us(100);	
	nWE = 1; // data latch
	delay_us(100);	
}

void ereaseSector(u8 idx)
{
	nOE = 1;	
	oneBusCycle(0x555, 0xAA);
	oneBusCycle(0x2AA, 0x55);
	oneBusCycle(0x555, 0x80);
	oneBusCycle(0x555, 0xAA);
	oneBusCycle(0x2AA, 0x55);
	oneBusCycle(idx, 0x30);
//	oneBusCycle(0x55, 0x10, 0x5); // chip erease
}


void WriteAdata(u16 addr,u8 data)
{	
		int timecnt=0;
		u16 u16data=data;
		u16 temp;

		// write, 4 cycles
		//
		oneBusCycle(0x555, 0xAA);
		oneBusCycle(0x2AA, 0x55);
		oneBusCycle(0x555, 0xA0);
		// the last cycle
		nOE = 1;
		nWE = 1;
    
		if(addr & 0x01)An1=1;
		else An1=0;
	
		addr = addr>>1;
		GPIO_Write(GPIOC,addr);
		if(addr&0x400) A10=1;
		else A10=0;
	
		delay_us(100);
	
		nWE = 0;
	
	
	
		temp=0x300F;
		temp=temp&GPIO_ReadOutputData(GPIOB);
		temp|=(u16data<<4);
		
		GPIO_Write(GPIOB,temp);
	
		delay_us(100);

		nWE = 1;
		//delay(5000);
		timecnt=0;
		while(RnB == 0)
		{
			timecnt++;
			delay_us(100);
			if(timecnt>200) break;
		}
	
}

u8 ReadAdata(u16 addr)
{
	u8 data;
	u16 u16data;
	ParaDataLine_IN();
	
	nOE = 0;
	nWE = 1;

	if(addr & 0x01)An1=1;
	else An1=0;
	addr = addr>>1;
	GPIO_Write(GPIOC,addr);
	if(addr&0x400) A10=1;
	else A10=0;

	delay_us(100);
		
	
	
	u16data=GPIO_ReadInputData(GPIOB);
	data=0x00FF&(u16data>>4);


			
	ParaDataLine_OUT();
	
	return data;
}