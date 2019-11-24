#ifndef __JVTWB_H
#define  __JVTWB_H
#include "sys.h"
#include "delay.h"

extern u8 rec_data;
#define DIN PCout(0)	
#define CLK PCout(1)	
#define SEL PCout(2)	



//并行编程接口
#define A10 PBout(0)
#define nWE PBout(1)
#define nOE PBout(3)
#define An1 PBout(12)
#define nByte PBout(13)

#define RnB PBin(2)


void JVTWB_SrialSetData(u16 Data);
void JVTWB_ParallSetData(u16 Data);
void JVTWB_Init(void);

void ParaDataLine_OUT();
void ParaDataLine_IN();
void oneBusCycle(u16 addr, u8 data);
void ereaseSector(u8 idx);
void WriteAdata(u16 addr,u8 data);
u8 ReadAdata(u16 addr);
#endif
