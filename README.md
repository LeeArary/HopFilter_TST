# 跳频滤波器自动测试 安捷伦8714与5062版本（基于GPIB与SCIP)
## 【功能介绍】
##### 该项目针对在成品测试上对跳频滤波器的参数自动测试设计了上位机与连接器（下位机）两个部分
## 【仪器平台】
##### "安捷伦"矢量网络分析仪8714
## 【硬件方案】
##### 1.连接器1: 51（内嵌USB）GPIB接口设备  
##### 2.连接器2: STM32RCT6（内嵌USB，USART）  
##### 3.转接板: 跳频滤波器-并行使用 
## 【软件方案】
##### 1.  C# USB NPIO组件 USART
##### 2.  使用GPIB接口，SCPI协议(仪器通用协议)
## 【功能说明】
### 跳频滤波器-串行接口
##### 1.  自动测试滤波器对应频点的Flash值，并生成.hex文件
##### 2.  下载.hex文件后，进行第二次自动报表测试。测试数据有：插入损耗/驻波/矩形系数/-3dB带宽/实际频率与频率漂移量
##### 3.  上位机提供-跳频功能
### 跳频滤波器-并行接口
##### 1.  自动测试滤波器对应频点的Flash值，并生成.hex文件
##### 2.  下载.hex文件后，进行第二次自动报表测试。测试数据有：插入损耗/驻波/-3dB带宽/中心频率偏移±10%的带宽/中心频率偏移±5%的带宽/反射系数
##### 3.  上位机提供-跳频功能
## 【V2.0开发版 2019-11-25】
================5062=====================  
### 该开发版没有改变5062，其代码与V1.0正式版一致

================8714=====================  
## 【1】更改了新的程序框架  
###   1.在project文件夹中增加以下模块  
        Communicate.cs  USB与串口的发送接收  
        DataProces.cs   数据处理  
        Device_Info.cs  全局变量与系统需要的信息  
        MyMessage.cs    封装了向窗口发送消息的方法  
        Mytimer.cs      各个环节的定时器，以及定时器的回调函数  
        Program.cs      程序入口，不用管  
###   2.在Form文件夹中有以下模块  
        Form1.cs 主窗口  
        Form2.cs 数据显示窗口1  
        Form3.cs 数据显示窗口2  
###    3.在API文件夹中增加以下模块  
        USBHID.cs       调用windows API再次封装成自己的函数，主要实现USB连接，接收与发送  
        USBHIDEnum.cs   调用Windwos API所需要的枚举类型  
        WindowsAPI.cs   调用各种dll库，封装成了该windowsAPI  

## 【2】V2.0开发版目标  
	1.解决框架问题  
	2.改写窗口名称  
	3.增加优化变量定义与使用  
    4.收集V1.0问题并解决  

