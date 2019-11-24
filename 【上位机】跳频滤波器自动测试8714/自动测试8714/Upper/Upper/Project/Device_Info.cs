using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.XSSF.Streaming;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
namespace Upper
{

    //系统总体信息
    public static class Device_info
    {
        #region 结构体定义
        //仪器与中继器配置信息
        public struct Config_Info_t
        {
            public int Config_Seq;//当前配置的序号，string[]下标
            public string[] Config_content;//配置信息
            public UInt16 Config_sta;//当前在配置什么
            #region 说明
            /*
             0：没有配置
             1：一键仪器配置
             2：串行接口第一段配置
             3：串行接口第二段配置
             */

            #endregion
        }

        //串行设备信息
        public struct Serial_Info_t
        {
            public double CurrenFrq;//当前比较值
            public double CentFrq1;//用于对比的频率，第一段108-174MHz
            public double CentFrq2;//用于对比的频率，第二段225-400.2MHz
            public double step;//增长步进Mhz
            public double ReBack;//回退次数


            //=======仪器配置界面参数=======//
            public double Bw1;//Bw1
            public double Bw2;//Bw2

            public double Start1Frq;//第一段起始频率
            public double Stop1Frq;//第一段终止频率

            public double Start2Frq;//第二段起始频率
            public double Stop2Frq;//第二段终止频率

            public double PWR_dBm;//矢网输出功率
            public double OffSet;//温飘
            //=========================//



            public int sample_cnt;//采样计数

            public UInt16 hex;//自动测试下的发往跳频滤波器的hex
            public HSSFWorkbook MyWorkbook;//Excel表实例

            public Form2 Autotest_Form;//自动测试后的界面展示


            //用于自动报表生成

            public int test_wait;

            public int test_sta;//固定频点
            public int test_step;//一个固定频点测试的测试步骤

            public double test_3dB;//BW-3dB
            public double test_40dB;//BW-40dB
            public double test_40_30dB;
            public double test_Loss;//插损
            public double test_FrqErr;//频率漂移
            public double test_standWave;//驻波

            public int temperture_mode;//三种温度
        }

        //并行设备信息
        public struct Parall_Info_t
        {
            //并行自动测试用的数组

            public List<AutoTest_Datas_t> AutoTest_datas;
            public double preFrq;//用于自动读取时，记录上一个频率值检查是否读取错误

            public double CurrenFrq;//当前比较值
            public double CentFrq1;//用于对比的频率，第一段108-174MHz
            public double CentFrq2;//用于对比的频率，第二段225-400MHz
            public double step1;//增长步进0.432Mhz
            public double step2;//增长步进0.7Mhz

            //=======仪器配置界面参数=======//
            public double Bw1;//Bw1


            public double Start1Frq;//第一段起始频率
            public double Stop1Frq;//第一段终止频率

            public double Start2Frq;//第二段起始频率
            public double Stop2Frq;//第二段终止频率

            public double PWR_dBm;//矢网输出功率
            public double OffSet;//温飘
            //=========================//



            public Form3 Autotest_resForm;

            public int sample_cnt;//采样计数

            public UInt16 hex;//自动测试下的发往跳频滤波器的hex
            public HSSFWorkbook MyWorkbook;//Excel表实例


            //用于自动报表生成



            public int test_sta;//频点
            public int test_step;//一个频点测试步骤

            public double test_Frq;//频率漂移
            public double test_Loss;//插损
            public double test_3d;
            public double test_Standwave;


            public double pos5;
            public double pos10;
            public double neg5;
            public double neg10;

            public int temperture_mode;//三种温度



            public string DownloadFilePath;//编程文件路径
            public int DownloadFileRow;//编程文件行数

            public List<UInt16> CaliData;


            public Random random;

        }

        //并行自动测试，存储采集数据
        public struct AutoTest_Datas_t
        {
            public int Seq;
            public UInt16 Hex;
            public double Frq;
            public double Loss;
        }

        //USB状态信息
        public struct USB_Info_t
        {
            //USB状态
            public bool connect_STM;
            public bool find_usb;//发现中继器的usb
            public bool error;
            public byte[] ReciveData;
            public byte[] SendData;

            
            public List<String> Device_list;//当前USB所有设备信息
            public string Relay_ID;//USB中继设备ID
            public string Relay_path;//USB中继设备地址

            //USB上次连接历史
            public bool STMcnectHis;

        }
        //串口状态信息
        public struct USART_Info_t
        {
            //串口状态
            public bool connect_uart;//串口连接
            public bool find_uart;//发现GPIB连接器的串口

            public bool connect_NA;//连接矢网
            public bool find_NA;//发现矢量网络分析仪

            public int Scan_STA;

            public string[] portList;//串口设备列表
            public int cnt;//记录列表的下标

            public int wait_cnt;//等待回复时间计数

            public int SW_HeartCnt;//失网连接的心跳

            public bool NAcnectHis;

            //【信息】串口接收模式
            public int Receive_mode;//串口读取数据的三种类型
            #region 串口接收模式的说明
            //0：没有读取任务
            //1：读取自动测试
            //2：读取报表测试
            //3：读取手动测试的频率
            //4：并行接口自动测试

            #endregion
        }
        #endregion

        #region 全局系统信息
        //【信息】记录桌面路径
        public static string DesktopPath;
        //【信息】配置信息
        public static Config_Info_t Config_info;
        //【信息】串行接口滤波器
        public static Serial_Info_t Serial_info;
        //【信息】并行接口滤波器
        public static Parall_Info_t Parall_info;
        //【信息】USB信息
        public static USB_Info_t USB_info;
        //【信息】串口信息
        public static USART_Info_t USART_info;


        #endregion

        #region 静态构造
        static Device_info()
        {
            //中继器的唯一ID
            Device_info.USB_info.Relay_ID = "vid_aabb&pid_0001";
            //初始未连接，未发现
            Device_info.USB_info.connect_STM = false;
            Device_info.USB_info.find_usb = false;

            Device_info.USART_info.connect_NA = false;
            Device_info.USART_info.find_NA = false;

            Device_info.USART_info.connect_uart = false;
            Device_info.USART_info.find_uart = false;

            Device_info.USART_info.Scan_STA = 0;//还未开始扫描

            Device_info.USB_info.STMcnectHis = false;
            Device_info.USART_info.NAcnectHis = false;
            //分配接收与发送数组
            Device_info.USB_info.ReciveData = new byte[10];
            Device_info.USB_info.SendData = new byte[8];


            //测试阶段，暂时默认发送数据是这个
            Device_info.USB_info.SendData[0] = 0xFA;//帧头
            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = 0x00;//数据1
            Device_info.USB_info.SendData[3] = 0x00;//数据2
            Device_info.USB_info.SendData[4] = 0x00;//数据3
            Device_info.USB_info.SendData[5] = 0x00;//数据4
            Device_info.USB_info.SendData[6] = 0x00;//数据5
            Device_info.USB_info.SendData[7] = 0xFA;//数据5
            Device_info.USB_info.Device_list = new List<string>();

            Device_info.Config_info.Config_content = new string[]{
                
                "SENS1:STAT ON",//MEAS1
                "SOUR1:POW:LEV:IMM:AMPL ",//变量 输出功率 
                "DISP:WIND1:TRAC:Y:RPOS 8E0",//参考位置 8
                "SENS1:FREQ:STAR ",//变量 起始50M第一段start
                "SENS1:FREQ:STOP ",//变量 终止500M第一段stop
                "CALC1:MARK1:BWID ",//变量 带宽-3dB
                "CALC1:MARK1:FUNC:TRAC ON"//自动追踪
                //"SENS2:STAT ON"//MEAS2
           
            };

            #region 串行数据初始化


            Device_info.Serial_info.CentFrq1 = 108;//第一段起始频率
            Device_info.Serial_info.CentFrq2 = 225;//第二段起始频率
            Device_info.Serial_info.step = 0.4;//步进
            Device_info.Serial_info.OffSet = 0;//温飘




            #endregion

            #region 并行数据初始化

            Device_info.Parall_info.AutoTest_datas = new List<Device_info.AutoTest_Datas_t>();


            Device_info.Parall_info.CentFrq1 = 108;//第一段起始频率
            Device_info.Parall_info.CentFrq2 = 225;//第二段起始频率
            Device_info.Parall_info.step1 = 0.264;//第一段步进
            Device_info.Parall_info.step2 = 0.7;//第二段步进
            Device_info.Parall_info.OffSet = 0;//温飘
            Device_info.Parall_info.random = new Random();
            #endregion
            Device_info.Config_info.Config_sta = 0;//没有配置任务

            Device_info.USART_info.Receive_mode = 0;//没有需要读取串口数据的


            //获取本机桌面路径
            string strDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Device_info.DesktopPath = strDesktopPath;

            Device_info.Parall_info.DownloadFilePath = null;

            Device_info.Parall_info.CaliData = new List<UInt16>();
        }
        #endregion
    }
}
