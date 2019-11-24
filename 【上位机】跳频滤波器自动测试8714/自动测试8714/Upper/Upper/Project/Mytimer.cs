using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Upper;
using System.Threading;
using System.IO;
namespace Upper
{

    public static class Mytimer
    {
        //各个定时器的时间
        private const int Heart_ms = 500, Config_ms = 500,
                                S_Test_ms = 50, S_Report_ms = 200, S_Hop = 200,
                                   P_Test_ms = 50, P_Report_ms = 200, P_Hop = 200;
        #region 枚举
        public enum Eunm
        {
            Heart,
            Config,
            Serial_Autotest,
            Serial_Reportest,
            Serial_Hop,
            Paralle_Autotest,
            Paralle_Reportest,
            Paralle_Hop,
        }
        #endregion

        #region 定时器字段
        public static System.Windows.Forms.Timer Heart { get; set; }//心跳
        public static System.Windows.Forms.Timer Config { get; set; }//配置
        public static System.Windows.Forms.Timer Serial_Autotest { get; set; }//串行测试
        public static System.Windows.Forms.Timer Serial_Reportest { get; set; }//串行报表
        public static System.Windows.Forms.Timer Serial_Hop { get; set; }//串行跳频
        public static System.Windows.Forms.Timer Paralle_Autotest { get; set; }//并行测试
        public static System.Windows.Forms.Timer Paralle_Reportest { get; set; }//并行报表
        public static System.Windows.Forms.Timer Paralle_Hop { get; set; }//并行跳频
        #endregion

        #region 构造
        //构造1
        static Mytimer()
        {
            //定时器-心跳【500ms】
            Heart = new System.Windows.Forms.Timer();
            Heart.Interval = Heart_ms;
            Heart.Tick += Heart_cb;
            Heart.Enabled = false;

            //定时器-配置【500ms】
            Config = new System.Windows.Forms.Timer();
            Config.Interval = Config_ms;
            Config.Tick += Config_cb;
            Config.Enabled = false;

            //定时器-串行测试【50ms】
            Serial_Autotest = new System.Windows.Forms.Timer();
            Serial_Autotest.Interval = S_Test_ms;
            Serial_Autotest.Tick += Serial_Autotest_cb;
            Serial_Autotest.Enabled = false;

            //定时器-串行报表【200ms】
            Serial_Reportest = new System.Windows.Forms.Timer();
            Serial_Reportest.Interval = S_Report_ms;
            Serial_Reportest.Tick += Serial_Reportest_cb;
            Serial_Reportest.Enabled = false;

            //定时器-串行跳频【200ms】
            Serial_Hop = new System.Windows.Forms.Timer();
            Serial_Hop.Interval = S_Hop;
            Serial_Hop.Tick += Serial_Hop_cb;
            Serial_Hop.Enabled = false;

            //定时器-并行测试【50ms】
            Paralle_Autotest = new System.Windows.Forms.Timer();
            Paralle_Autotest.Interval = P_Test_ms;
            Paralle_Autotest.Tick += Para_Autotest_cb;
            Paralle_Autotest.Enabled = false;

            //定时器-并行报表【200ms】
            Paralle_Reportest = new System.Windows.Forms.Timer();
            Paralle_Reportest.Interval = P_Report_ms;
            Paralle_Reportest.Tick += Para_Reportest_cb;
            Paralle_Reportest.Enabled = false;

            //定时器-并行跳频【200ms】
            Paralle_Hop = new System.Windows.Forms.Timer();
            Paralle_Hop.Interval = P_Hop;
            Paralle_Hop.Tick += Paralle_Hop_cb;
            Paralle_Hop.Enabled = false;


        }
        #endregion

        #region Mytimer方法
       
        #endregion

        #region 定时器回调处理
        //【回调】定时器：心跳
        private static void Heart_cb(object sender, EventArgs e)
        {
            
            if (Device_info.USART_info.connect_NA) Mytimer.Heart.Interval = 1000;//找到了，就放慢扫描速度
            else Mytimer.Heart.Interval = 500;//没找到，加快扫描速度
            //GPIB连接器检测
            if (Device_info.USART_info.find_uart == false)//还未找到GPIB设备
            {
                if (Device_info.USART_info.Scan_STA == 0)//未扫描状态
                {
                    Device_info.USART_info.portList = System.IO.Ports.SerialPort.GetPortNames();//扫描串口
                    if (Device_info.USART_info.portList.Length == 0) Device_info.USART_info.Scan_STA = 0;
                    else Device_info.USART_info.Scan_STA = 1;//进入扫描状态 
                    Device_info.USART_info.cnt = 0;//从第一个串口开始             
                }

                if (Device_info.USART_info.Scan_STA == 1)//扫描状态
                {
                    string name = Device_info.USART_info.portList[Device_info.USART_info.cnt];
                    MyCommunicate.Usart_portName = name;
                    bool a = MyCommunicate.Usart_isOpen;
                    try//此处处理可能占用的异常
                    {
                        MyCommunicate.Usart_Open();
                    }
                    catch//当前串口被占用
                    {
                        Device_info.USART_info.cnt++;
                        if (Device_info.USART_info.cnt < Device_info.USART_info.portList.Length) Device_info.USART_info.Scan_STA = 1;//进入扫描状态
                        else Device_info.USART_info.Scan_STA = 0;//列表中没有，重新开始新一轮扫描

                        return;
                    }
                    MyCommunicate.Usart_Write("++ver\r\n");

                    Device_info.USART_info.wait_cnt = 0;
                    Device_info.USART_info.Scan_STA = 2;//进入等待回复状态
                }
                else if (Device_info.USART_info.Scan_STA == 2)//等待回复状态，期间如果找到设备不会进入此处了
                {
                    MyCommunicate.Usart_Close();
                    Device_info.USART_info.Scan_STA = 0;

                    if (Device_info.USART_info.cnt + 1 < Device_info.USART_info.portList.Length)
                    {
                        Device_info.USART_info.cnt++;
                        Device_info.USART_info.Scan_STA = 1;//进入扫描状态
                    }
                    else 
                        Device_info.USART_info.Scan_STA = 0;//列表中没有，重新开始新一轮扫描
                }
            }
            else if (Device_info.USART_info.connect_uart == false)//找到GPIB设备，连接
            {
                Device_info.USART_info.connect_uart = true;//没有连接操作，直接表示连接
                Program.main_form.DrawLine_PC_GPIB(true);
            }
            else//GPIB心跳检测,检测错误直接标定GPIB与失网断开
            {
                MyCommunicate.Usart_Write("++auto 1");//发送心跳信息
            }

            //安捷伦失网检测
            if (Device_info.USART_info.connect_uart == true)//前提，GPIB连接成功
            {

                if (Device_info.USART_info.find_NA == false)//第一次上电
                {
                    MyCommunicate.Usart_Write("++addr 18");//仪器地址
                    MyCommunicate.Usart_Write("++auto 1");//打开自动回听
                    Device_info.USART_info.find_NA = true;
                }
                else//心跳信息
                {
                    MyCommunicate.Usart_Write("*IDN?");//请求设备信息
                    Device_info.USART_info.SW_HeartCnt++;
                    if (Device_info.USART_info.SW_HeartCnt >= 6)
                    {
                        Device_info.USART_info.connect_NA = false;
                        Program.main_form.DrawLine_GPIB_SW(false);

                    }

                }
            }

            
            //USB设备检测
            Device_info.USB_info.Device_list.Clear();
            Device_info.USB_info.Device_list = MyCommunicate.USB_GetDeviceList();//得到设备列表
            int find = 0;
            foreach (string Device_ID in Device_info.USB_info.Device_list)//一个一个找
            {

                string[] Array = Device_ID.Split('#');//单个设备信息解析
                foreach (string str in Array)
                {
                    int sta = str.CompareTo(Device_info.USB_info.Relay_ID);
                    if (sta == 0)
                    {
                        //找到设备，更新状态，存储路径
                        find = 1;
                        Device_info.USB_info.find_usb = true;
                        Device_info.USB_info.Relay_path = Device_ID;

                    }
                }
            }
            if (find == 0)//没有设备，更新状态
            {
                Device_info.USB_info.find_usb = false;
                Device_info.USB_info.connect_STM = false;
            }

            if (Device_info.USB_info.connect_STM == false)//USB连接
            {
                if (Device_info.USB_info.find_usb)//发现设备尝试连接
                {
                    bool sta = MyCommunicate.USB_OpenUSBHid((Device_info.USB_info.Relay_path));
                    if (sta == false)
                    {
                        Device_info.USB_info.connect_STM = false;
                        if (Device_info.USB_info.error == false)
                        {
                            Device_info.USB_info.error = true;
                            DialogResult result = MessageBox.Show("错误：该设备异常！请检测是否运行了多个程序！", "", MessageBoxButtons.OK);
                            if (result == DialogResult.OK) Application.Exit();
                        }

                    }
                    else
                    {
                        Device_info.USB_info.error = false;
                        Program.main_form.DrawLine_PC_STM(true);//连接成功画红线
                        Program.main_form.DrawLine_STM_LBQ(true);//连接成功画红线
                        Device_info.USB_info.connect_STM = true;
                    }
                }
                else//没有发现USB设备，画图
                {
                    Program.main_form.DrawLine_PC_STM(false);//连接失败画黑线
                    Program.main_form.DrawLine_STM_LBQ(false);//连接失败画红线
                }
            }
            if ((Device_info.USART_info.NAcnectHis != Device_info.USART_info.connect_NA) || (Device_info.USB_info.connect_STM != Device_info.USB_info.STMcnectHis))
            {
                if (Device_info.USB_info.connect_STM && Device_info.USART_info.connect_NA)
                {
                    //按键变化
                    Program.main_form.SerialButton_All(true, true, true);
                    Program.main_form.ParallButton_All(true, true, true);
                    Program.main_form.DownloadButton_All(true);

                    Program.main_form.button18.Enabled = false;//终止按键关闭
                    Program.main_form.button19.Enabled = false;
                    Program.main_form.button10.Enabled = false;
                    Program.main_form.button48.Enabled = false;
                    Program.main_form.button26.Enabled = false;
                    Program.main_form.button40.Enabled = false;
                }
                else if (Device_info.USB_info.connect_STM)
                {
                    //按键变化
                    Program.main_form.SerialButton_All(true, true, false);
                    Program.main_form.ParallButton_All(true, true, false);

                    Program.main_form.SerialButton_All(false, true, true);
                    Program.main_form.ParallButton_All(false, true, true);
                    Program.main_form.DownloadButton_All(true);

                    Program.main_form.button18.Enabled = false;//终止按键关闭
                    Program.main_form.button19.Enabled = false;
                    Program.main_form.button10.Enabled = false;
                    Program.main_form.button48.Enabled = false;
                    Program.main_form.button26.Enabled = false;
                    Program.main_form.button40.Enabled = false;
                }
                else if (Device_info.USART_info.connect_NA)
                {
                    //按键变化
                    Program.main_form.SerialButton_All(true, true, false);
                    Program.main_form.ParallButton_All(true, true, false);

                    Program.main_form.SerialButton_All(true, false, true);
                    Program.main_form.ParallButton_All(true, false, true);
                    Program.main_form.DownloadButton_All(false);

                }
                else
                {
                    Program.main_form.SerialButton_All(true, true, false);
                    Program.main_form.ParallButton_All(true, true, false);
                    Program.main_form.DownloadButton_All(false);
                }
            }
            Device_info.USART_info.NAcnectHis = Device_info.USART_info.connect_NA;
            Device_info.USB_info.STMcnectHis = Device_info.USB_info.connect_STM;
        }
        //【回调】定时器：配置
        private static void Config_cb(object sender, EventArgs e)
        {

            if (Device_info.Config_info.Config_sta == 0)//【容错】没有配置，开启心跳
            {

                Mytimer.Config.Stop();
                Mytimer.Heart.Start();
            }
            else
            {
                if (Device_info.Config_info.Config_sta == 1)//一键配置仪器
                {

                    if (Device_info.Config_info.Config_Seq < Device_info.Config_info.Config_content.Length)
                    {
                        Program.main_form.progressBar1.Value = Program.main_form.progressBar1.Maximum * (Device_info.Config_info.Config_Seq + 1) / Device_info.Config_info.Config_content.Length;
                        Program.main_form.progressBar2.Value = Program.main_form.progressBar2.Maximum * (Device_info.Config_info.Config_Seq + 1) / Device_info.Config_info.Config_content.Length;
                        MyCommunicate.Usart_Write(Device_info.Config_info.Config_content[Device_info.Config_info.Config_Seq++]);

                    }

                    else
                    {

                        //按键变化
                        Program.main_form.SerialButton_All(true, false, true);
                        Program.main_form.ParallButton_All(true, false, true);

                        Program.main_form.progressBar1.Value = 0;
                        Program.main_form.progressBar2.Value = 0;
                        Mytimer.Config.Stop();//关配制
                        Mytimer.Heart.Start();//开心跳
                        MessageBox.Show("仪器配置完成！", "", MessageBoxButtons.OK);
                    }
                }
                else if (Device_info.Config_info.Config_sta == 2) //【串行】配置第一段
                {
                    if (Device_info.Config_info.Config_Seq == 0)//设置bw1
                    {
                        string temp = "CALC1:MARK1:BWID " + Device_info.Serial_info.Bw1.ToString();
                        MyCommunicate.Usart_Write(temp);
                        Device_info.Config_info.Config_Seq++;
                    }
                    else if (Device_info.Config_info.Config_Seq == 1)//设置star1
                    {
                        string temp = "SENS1:FREQ:STAR " + Device_info.Serial_info.Start1Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);
                        Device_info.Config_info.Config_Seq++;
                    }
                    else if (Device_info.Config_info.Config_Seq == 2)//设置stop1
                    {
                        string temp = "SENS1:FREQ:STOP " + Device_info.Serial_info.Stop1Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);

                        Device_info.Serial_info.CurrenFrq = Device_info.Serial_info.CentFrq1;//当前比较值是108MHz
                        Device_info.Serial_info.ReBack = 0;//重新测量次数
                        Device_info.Serial_info.sample_cnt = 0;//采集的数据多少
                        Device_info.Serial_info.hex = 0;//滤波器从x开始
                        Device_info.USART_info.Receive_mode = 1;//串口接收的是自动测试数据

                        Mytimer.Config.Stop();//关配置
                        Mytimer.Serial_Autotest.Start();//开串行自动测试
                        return;
                    }
                }
                else if (Device_info.Config_info.Config_sta == 3)//【串行】配置第二段
                {
                    if (Device_info.Config_info.Config_Seq == 0)//设置bw1
                    {
                        string temp = "CALC1:MARK1:BWID " + Device_info.Serial_info.Bw1.ToString();
                        MyCommunicate.Usart_Write(temp);
                        Device_info.Config_info.Config_Seq++;
                    }
                    else if (Device_info.Config_info.Config_Seq == 1)//设置star2
                    {
                        string temp = "SENS1:FREQ:STAR " + Device_info.Serial_info.Start2Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);
                        Device_info.Config_info.Config_Seq++;
                    }
                    else if (Device_info.Config_info.Config_Seq == 2)//设置stop2
                    {
                        string temp = "SENS1:FREQ:STOP " + Device_info.Serial_info.Stop2Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);


                        Device_info.Serial_info.CurrenFrq = Device_info.Serial_info.CentFrq2;//当前比较值是225
                        Device_info.Serial_info.ReBack = 0;//重新测量次数
                        Device_info.Serial_info.hex = 4096;//第二段hex起始
                        Device_info.USB_info.SendData[1] = 0x01;//功能帧
                        Device_info.USB_info.SendData[2] = (byte)(Device_info.Serial_info.hex / 256);
                        Device_info.USB_info.SendData[3] = (byte)(Device_info.Serial_info.hex % 256);
                        MyCommunicate.USB_Write(Device_info.USB_info.SendData);



                        Mytimer.Config.Stop();
                        Mytimer.Serial_Autotest.Start();
                        return;

                    }
                }
                else if (Device_info.Config_info.Config_sta == 4)//【并行】配置第一段
                {
                    Device_info.USB_info.SendData[1] = 0x02;//功能帧
                    Device_info.USB_info.SendData[2] = (byte)(0 / 256);
                    Device_info.USB_info.SendData[3] = (byte)(0 % 256);
                    MyCommunicate.USB_Write(Device_info.USB_info.SendData);

                    if (Device_info.Config_info.Config_Seq == 0)//设置star1
                    {
                        string temp = "SENS1:FREQ:STAR " + Device_info.Parall_info.Start1Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);
                        Device_info.Config_info.Config_Seq++;
                    }
                    else if (Device_info.Config_info.Config_Seq == 1)//设置stop1
                    {
                        string temp = "SENS1:FREQ:STOP " + Device_info.Parall_info.Stop1Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);

                        //Device_info.Parall_info.CurrenFrq = Device_info.Serial_info.CentFrq1;//当前比较值是108MHz
                        Device_info.Parall_info.sample_cnt = 0;//采集的数据多少
                        Device_info.Parall_info.hex = 0;//滤波器从x开始
                        Device_info.USART_info.Receive_mode = 4;//并口接收的是自动测试数据

                        Mytimer.Config.Stop();
                        Thread.Sleep(2000);
                        Mytimer.Paralle_Autotest.Start();

                        ParaAutotest_bool = false;//确保第一段是从控制滤波器开始的
                        return;
                    }
                }
                else if (Device_info.Config_info.Config_sta == 5)//【并行】配置第二段
                {
                    Device_info.USB_info.SendData[1] = 0x02;//功能帧
                    Device_info.USB_info.SendData[2] = (byte)(400 / 256);
                    Device_info.USB_info.SendData[3] = (byte)(400 % 256);
                    MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                    
                    if (Device_info.Config_info.Config_Seq == 0)//设置star2
                    {
                        string temp = "SENS1:FREQ:STAR " + Device_info.Parall_info.Start2Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);
                        Device_info.Config_info.Config_Seq++;
                    }
                    else if (Device_info.Config_info.Config_Seq == 1)//设置stop2
                    {
                        string temp = "SENS1:FREQ:STOP " + Device_info.Parall_info.Stop2Frq.ToString() + "E6";
                        MyCommunicate.Usart_Write(temp);

                        // Device_info.Parall_info.CurrenFrq = Device_info.Serial_info.CentFrq1;//当前比较值是108MHz
                        //Device_info.Parall_info.sample_cnt = 0;//采集的数据多少
                        Device_info.Parall_info.hex = 0x400;//滤波器从1024开始
                        Device_info.USART_info.Receive_mode = 4;//并口接收的是自动测试数据

                        Mytimer.Config.Stop();
                        Thread.Sleep(2000);
                        Mytimer.Paralle_Autotest.Start();

                        ParaAutotest_bool = false;//确保第二段是从控制滤波器开始的
                        return;
                    }
                }
            }

        }
        //【回调】定时器：串行-测试
        public static bool Autotest_bool = false;// false:控制滤波器  true：读取矢网
        private static void Serial_Autotest_cb(object sender, EventArgs e)
        {
            if (Device_info.Serial_info.CurrenFrq > 400.2)//自动测试完毕
            {
                Device_info.Serial_info.Autotest_Form.Show();
                DataProces.Serial_Test_done();

                //按键变化
                Program.main_form.SerialButton_All(true, true, true);
                Program.main_form.ParallButton_All(true, true, true);
                Program.main_form.DownloadButton_All(true);
                Program.main_form.button18.Enabled = false;//终止按键关闭
                Program.main_form.button19.Enabled = false;
                Program.main_form.button10.Enabled = false;
                Program.main_form.button48.Enabled = false;
                Program.main_form.button26.Enabled = false;
                Program.main_form.button40.Enabled = false;


                Device_info.USART_info.Receive_mode = 0;//没有自动测试数据要接收
                Program.main_form.progressBar1.Value = 0;

                Mytimer.Serial_Autotest.Stop();
                Mytimer.Heart.Start();//开启心跳定时器
            }
            if (Device_info.Serial_info.CurrenFrq > 174.2 && Device_info.Config_info.Config_sta == 2)
            {
                Device_info.Config_info.Config_sta = 3;//第二段配置
                Device_info.Config_info.Config_Seq = 0;//清零计数

                Mytimer.Serial_Autotest.Stop();
                Mytimer.Config.Start();//开启配置

            }

            if (Autotest_bool == false)//控制滤波器
            {
                Autotest_bool = true;
                //滤波器控制，逐渐增加
                Device_info.USB_info.SendData[1] = 0x01;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(Device_info.Serial_info.hex / 256);
                Device_info.USB_info.SendData[3] = (byte)(Device_info.Serial_info.hex % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            }
            else//读取矢网
            {
                Autotest_bool = false;
                MyCommunicate.Usart_Write("CALC:MARK:FUNC:RES?");
            }
        }
        //【回调】定时器：串行-报表
        private static void Serial_Reportest_cb(object sender, EventArgs e)
        {

            Program.main_form.progressBar1.Value = 605 * (Device_info.Serial_info.test_sta * 9 + Device_info.Serial_info.test_step) / (10 * 9);

            if (Device_info.Serial_info.test_sta == 0)//配置起始频率50M
            {
                MyCommunicate.Usart_Write("SENS1:FREQ:STAR 50E6");
                Device_info.Serial_info.test_sta++;
                Thread.Sleep(500);
            }
            else if (Device_info.Serial_info.test_sta == 1)//配置终止频率500M
            {
                MyCommunicate.Usart_Write("SENS1:FREQ:STOP 500E6");
                Device_info.Serial_info.test_sta++;
                Thread.Sleep(500);
            }
            else if (Device_info.Serial_info.test_sta >= 2)
            {
                //第一步发送配置 BW1 500ms
                if (Device_info.Serial_info.test_step == 0)
                {
                    UInt16 hex = 0;
                    if (Device_info.Serial_info.test_sta == 2)//108
                    {
                        hex = System.Convert.ToUInt16((108 - 108) / 0.4);
                        Device_info.USB_info.SendData[1] = 0x01;//功能帧
                        Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                        Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                        MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                        Thread.Sleep(1000);
                    }

                    else if (Device_info.Serial_info.test_sta == 3)//140
                        hex = System.Convert.ToUInt16((140 - 108) / 0.4);
                    else if (Device_info.Serial_info.test_sta == 4)//174
                        hex = System.Convert.ToUInt16((174 - 108) / 0.4);
                    else if (Device_info.Serial_info.test_sta == 5)//225
                        hex = System.Convert.ToUInt16((225 - 225) / 0.4 + 166);
                    else if (Device_info.Serial_info.test_sta == 6)//260.2
                        hex = System.Convert.ToUInt16((260.2 - 225) / 0.4 + 166);
                    else if (Device_info.Serial_info.test_sta == 7)//300.2
                        hex = System.Convert.ToUInt16((300.2 - 225) / 0.4 + 166);
                    else if (Device_info.Serial_info.test_sta == 8)//340.2
                        hex = System.Convert.ToUInt16((340.2 - 225) / 0.4 + 166);
                    else if (Device_info.Serial_info.test_sta == 9)//400.2
                        hex = System.Convert.ToUInt16((400.2 - 225) / 0.4 + 166);

                    Device_info.USB_info.SendData[1] = 0x01;//功能帧
                    Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                    Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                    MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                    Thread.Sleep(1000);


                    //MyCommunicate.Usart_Write("SENS1:STAT ON");//左上选为tr1
                    Device_info.Serial_info.test_step = 1;


                }
                //第二部配置滤波器 500ms
                else if (Device_info.Serial_info.test_step == 1)
                {
                    string temp = "CALC1:MARK1:BWID " + Device_info.Serial_info.Bw1.ToString();
                    MyCommunicate.Usart_Write(temp);

                    Device_info.Serial_info.test_step++;
                }

                //发送读取命令bw cent q loss 500ms,等待串口
                else if (Device_info.Serial_info.test_step == 2)
                {
                    MyCommunicate.Usart_Write("CALC:MARK:FUNC:RES?");
                }
                //数据-3B，插损，频飘 已收到，发送配置 BW2 500ms
                else if (Device_info.Serial_info.test_step == 3)
                {
                    string temp = "CALC1:MARK1:BWID " + Device_info.Serial_info.Bw2.ToString();
                    MyCommunicate.Usart_Write(temp);
                    Device_info.Serial_info.test_step++;
                }
                //发送读取命令bw cent q loss 100ms,等待串口
                else if (Device_info.Serial_info.test_step == 4)
                {
                    MyCommunicate.Usart_Write("CALC:MARK:FUNC:RES?");
                }
                //数据-40dB，-40dB/-3dB已收到，发送配置左上改为trace2
                else if (Device_info.Serial_info.test_step == 5)
                {
                    //MyCommunicate.Usart_Write("SENS2:STAT ON");
                    Device_info.Serial_info.test_step++;
                }
                //发送命令mark1 Y:,等待串口
                else if (Device_info.Serial_info.test_step == 6)
                {
                    MyCommunicate.Usart_Write("CALC2:MARK1:Y?");
                }
                else if (Device_info.Serial_info.test_step == 7)
                {
                    string temp = "CALC1:MARK1:BWID " + Device_info.Serial_info.Bw1.ToString();//BW1
                    MyCommunicate.Usart_Write(temp);
                    Device_info.Serial_info.test_step++;
                }
                else if (Device_info.Serial_info.test_step == 8)
                {
                    MyCommunicate.Usart_Write("CALC:MARK:FUNC:RES?");
                }
                //读取完毕，写入excel
                else if (Device_info.Serial_info.test_step == 9)
                {
                    if (Device_info.Serial_info.test_sta == 2)//108
                    {
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(3).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(3).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(3).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(3).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(3).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(3).SetCellValue(temp1);//写入频飘
                        }

                        Device_info.Serial_info.test_step = 0;
                        Device_info.Serial_info.test_sta++;
                    }
                    else if (Device_info.Serial_info.test_sta == 3)//140
                    {
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(6).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(6).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(6).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(6).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(6).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(6).SetCellValue(temp1);//写入频飘
                        }

                        Device_info.Serial_info.test_step = 0;
                        Device_info.Serial_info.test_sta++;

                    }
                    else if (Device_info.Serial_info.test_sta == 4)//174
                    {
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(9).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(9).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(9).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(9).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(9).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(9).SetCellValue(temp1);//写入频飘
                        }

                        Device_info.Serial_info.test_step = 0;
                        Device_info.Serial_info.test_sta++;
                    }
                    else if (Device_info.Serial_info.test_sta == 5)//225
                    {
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(10).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(10).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(10).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(10).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(10).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(10).SetCellValue(temp1);//写入频飘
                        }

                        Device_info.Serial_info.test_step = 0;
                        Device_info.Serial_info.test_sta++;
                    }
                    else if (Device_info.Serial_info.test_sta == 6)//260.2
                    {
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(12).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(12).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(12).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(12).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(12).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(12).SetCellValue(temp1);//写入频飘
                        }

                        Device_info.Serial_info.test_step = 0;
                        Device_info.Serial_info.test_sta++;
                    }
                    else if (Device_info.Serial_info.test_sta == 7)//300.2
                    {
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(14).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(14).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(14).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(14).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(14).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(14).SetCellValue(temp1);//写入频飘
                        }

                        Device_info.Serial_info.test_step = 0;
                        Device_info.Serial_info.test_sta++;
                    }
                    else if (Device_info.Serial_info.test_sta == 8)//340.2
                    {
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(16).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(16).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(16).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(16).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(16).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(16).SetCellValue(temp1);//写入频飘
                        }

                        Device_info.Serial_info.test_step = 0;
                        Device_info.Serial_info.test_sta++;
                    }
                    else if (Device_info.Serial_info.test_sta == 9)//400.2
                    {
                        //MyCommunicate.Usart_Write("SENS1:STAT ON");//左上选为tr1

                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(6).GetCell(19).SetCellValue(Device_info.Serial_info.test_3dB);//写入-3dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(7).GetCell(19).SetCellValue(Device_info.Serial_info.test_Loss);//写入中心频率插损
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(8).GetCell(19).SetCellValue(Device_info.Serial_info.test_40_30dB);//写入-30dB/-40dB
                        Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(9).GetCell(19).SetCellValue(Device_info.Serial_info.test_standWave);//写入驻波
                        if (Device_info.Serial_info.temperture_mode == 0) Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(19).SetCellValue("");//写入空值
                        else
                        {
                            string temp1;
                            //正号负号
                            if (Math.Abs(Device_info.Serial_info.test_FrqErr) < 0.05) temp1 = "0";
                            else if (Device_info.Serial_info.test_FrqErr > 0.05) temp1 = "+" + Device_info.Serial_info.test_FrqErr.ToString();
                            else temp1 = Device_info.Serial_info.test_FrqErr.ToString();
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(14).GetCell(19).SetCellValue(temp1);//写入频飘
                        }


                        if (Device_info.Serial_info.temperture_mode == 0)
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(3).GetCell(18).SetCellValue("常温");
                        else if (Device_info.Serial_info.temperture_mode == 1)
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(3).GetCell(18).SetCellValue("+75°C");
                        else if (Device_info.Serial_info.temperture_mode == 2)
                            Device_info.Serial_info.MyWorkbook.GetSheetAt(0).GetRow(3).GetCell(18).SetCellValue("-30°C");


                        using (FileStream fs = new FileStream(Device_info.DesktopPath + "\\产品检测记录.xls", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                            Device_info.Serial_info.MyWorkbook.Write(fs);

                        Device_info.Serial_info.MyWorkbook.Close();





                        //=================按键可用==============//
                        #region
                        //button11.Enabled = true;//一键配置
                        //butto.Enabled = true;//自动测试
                        //button18.Enabled = false;//终止自动测试

                        //button3.Enabled = true;//-1
                        //button2.Enabled = true;//+1
                        //button5.Enabled = true;//-10
                        //button4.Enabled = true;//+10
                        //button7.Enabled = true;//108M
                        //button8.Enabled = true;//104M
                        //button6.Enabled = true;//174M
                        //button9.Enabled = true;//225M
                        //button14.Enabled = true;//260M
                        //button13.Enabled = true;//300M
                        //button15.Enabled = true;//340M
                        //button12.Enabled = true;//400M
                        //button17.Enabled = true;//自动生成报表
                        //button16.Enabled = true;//跳频
                        //button10.Enabled = false;//调频终止   
                        #endregion
                        Program.main_form.progressBar1.Value = 0;
                        Device_info.USART_info.Receive_mode = 0;//没有自动测试数据要接收
                        Mytimer.Serial_Reportest.Stop();
                        Mytimer.Heart.Start();
                        MessageBox.Show("\"产品检测记录.xls\"写入完毕", "成功", MessageBoxButtons.OK);
                    }

                }

            }
        }
        //【回调】定时器：串行-跳频
        private static bool Hop_bool = true;
        private static void Serial_Hop_cb(object sender, EventArgs e)
        {
            if (Hop_bool)
            {
                double Frq1 = double.Parse(Program.main_form.textBox5.Text);
                UInt16 hex1;
                if (Frq1 >= 225)
                    hex1 = System.Convert.ToUInt16((Frq1 - 225) / 0.4 + 166);
                else
                    hex1 = System.Convert.ToUInt16((Frq1 - 108) / 0.4);

                Device_info.USB_info.SendData[1] = 0x01;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex1 / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex1 % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
               
                Hop_bool = false;
            }
            else
            {
                double Frq2 = double.Parse(Program.main_form.textBox6.Text);
                UInt16 hex2;

                if (Frq2 >= 225)
                    hex2 = System.Convert.ToUInt16((Frq2 - 225) / 0.4 + 166);
                else
                    hex2 = System.Convert.ToUInt16((Frq2 - 108) / 0.4);

                Device_info.USB_info.SendData[1] = 0x01;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex2 / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex2 % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                
                Hop_bool = true;
            }
        }
        //【回调】定时器：并行-测试
        public static bool ParaAutotest_bool = false;// false:控制滤波器  true：读取矢网
        private static void Para_Autotest_cb(object sender, EventArgs e)
        {
            if (Device_info.Parall_info.sample_cnt == 2048)
            {
                Device_info.Parall_info.Autotest_resForm.Show();
                DataProces.Paralle_Test_done();
                //按键变化
                Program.main_form.SerialButton_All(true, true, true);
                Program.main_form.ParallButton_All(true, true, true);
                Program.main_form.DownloadButton_All(true);
                Program.main_form.button18.Enabled = false;//终止按键关闭
                Program.main_form.button19.Enabled = false;
                Program.main_form.button10.Enabled = false;
                Program.main_form.button48.Enabled = false;
                Program.main_form.button26.Enabled = false;
                Program.main_form.button40.Enabled = false;

                Program.main_form.progressBar2.Value = 0;
                Device_info.USART_info.Receive_mode = 0;//没有自动测试数据要接收

                Mytimer.Paralle_Autotest.Stop();
                Mytimer.Heart.Start();
            }
            if (Device_info.Parall_info.sample_cnt >= 1024 && Device_info.Config_info.Config_sta == 4)
            {
                Device_info.Config_info.Config_sta = 5;//并行第二段配置
                Device_info.Config_info.Config_Seq = 0;//清零计数

                Mytimer.Paralle_Autotest.Stop();
                Mytimer.Config.Start();//开启配置
                return;
            }

            if (ParaAutotest_bool == false)//控制滤波器
            {
                ParaAutotest_bool = true;
                //滤波器控制，逐渐增加
                Device_info.USB_info.SendData[1] = 0x02;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(Device_info.Parall_info.hex / 256);
                Device_info.USB_info.SendData[3] = (byte)(Device_info.Parall_info.hex % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                Thread.Sleep(350);
            }
            else//读取矢网
            {
                ParaAutotest_bool = false;
                MyCommunicate.Usart_Write("CALC:MARK:FUNC:RES?");
            }
        }
        //【回调】定时器：并行-报表
        private static void Para_Reportest_cb(object sender, EventArgs e)
        {
            Program.main_form.progressBar2.Value = Program.main_form.progressBar2.Maximum * (Device_info.Parall_info.test_sta * 14 + Device_info.Parall_info.test_step) / (10 * 15 + 4);

            if (Device_info.Parall_info.test_sta == 0)//配置起始频率50M
            {
                MyCommunicate.Usart_Write("SENS1:FREQ:STAR 50E6");
                Device_info.Parall_info.test_sta++;
                Thread.Sleep(500);
            }
            else if (Device_info.Parall_info.test_sta == 1)//配置终止频率500M
            {
                MyCommunicate.Usart_Write("SENS1:FREQ:STOP 500E6");
                Device_info.Parall_info.test_sta++;
                Thread.Sleep(500);
            }
            else if (Device_info.Parall_info.test_sta == 11)
            {
                MyCommunicate.Usart_Write("SENS1:FREQ:STOP " + Device_info.Parall_info.Start2Frq + "E6");
                Device_info.Parall_info.test_sta = 12;
                Thread.Sleep(500);
            }
            else if (Device_info.Parall_info.test_sta == 12)
            {
                MyCommunicate.Usart_Write("SENS1:FREQ:STOP " + Device_info.Parall_info.Stop2Frq + "E6");
                Device_info.Parall_info.test_sta = 6;
                Thread.Sleep(500);
            }
            else if (Device_info.Parall_info.test_sta >= 2)
            {
                //第一步发送配置 BW1 
                if (Device_info.Parall_info.test_step == 0)
                {
                    UInt16 hex = 0;
                    if (Device_info.Parall_info.test_sta == 2)//108
                    {
                        hex = System.Convert.ToUInt16((108 - 108) / 0.264);
                        Device_info.USB_info.SendData[1] = 0x02;//功能帧
                        Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                        Device_info.USB_info.SendData[3] = (byte)(hex % 256);

                        MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                        Thread.Sleep(1000);
                    }

                    else if (Device_info.Parall_info.test_sta == 3)//129.9
                        hex = System.Convert.ToUInt16((129.9 - 108) / 0.264);
                    else if (Device_info.Parall_info.test_sta == 4)//150.2
                        hex = System.Convert.ToUInt16((150.2 - 108) / 0.264);
                    else if (Device_info.Parall_info.test_sta == 5)//174
                        hex = System.Convert.ToUInt16((174 - 108) / 0.264);
                    else if (Device_info.Parall_info.test_sta == 6)//225
                        hex = System.Convert.ToUInt16((225 - 225) / 0.7 + 256);
                    else if (Device_info.Parall_info.test_sta == 7)//250.2
                        hex = System.Convert.ToUInt16((250.2 - 225) / 0.7 + 256);
                    else if (Device_info.Parall_info.test_sta == 8)//299.9
                        hex = System.Convert.ToUInt16((299.9 - 225) / 0.7 + 256);
                    else if (Device_info.Parall_info.test_sta == 9)//350.3
                        hex = System.Convert.ToUInt16((350.3 - 225) / 0.7 + 256);
                    else if (Device_info.Parall_info.test_sta == 10)//400
                        hex = System.Convert.ToUInt16((400 - 225) / 0.7 + 256);

                    Device_info.USB_info.SendData[1] = 0x02;//功能帧
                    Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                    Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                    MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                    Thread.Sleep(1000);

                    Device_info.Parall_info.test_step = 1;


                }
                //第二部配置滤波器 500ms
                else if (Device_info.Parall_info.test_step == 1)
                {
                    string temp = "CALC1:MARK1:BWID " + Device_info.Parall_info.Bw1.ToString();
                    MyCommunicate.Usart_Write(temp);

                    Device_info.Parall_info.test_step++;
                }

                //发送读取命令bw cent q loss 500ms,等待串口
                else if (Device_info.Parall_info.test_step == 2)
                {
                    MyCommunicate.Usart_Write("CALC:MARK:FUNC:RES?");
                }
                //驻波
                else if (Device_info.Parall_info.test_step == 3)
                {
                    MyCommunicate.Usart_Write("CALC2:MARK1:Y?");

                }
                //All OFF
                else if (Device_info.Parall_info.test_step == 4)
                {
                    MyCommunicate.Usart_Write("CALC1:MARK:AOFF");
                    Device_info.Parall_info.test_step++;
                }
                //Mark1 ON
                else if (Device_info.Parall_info.test_step == 5)
                {
                    MyCommunicate.Usart_Write("CALC1:MARK1 ON");
                    Device_info.Parall_info.test_step++;
                }


                //0.95
                else if (Device_info.Parall_info.test_step == 6)
                {
                    string num = (Device_info.Parall_info.test_Frq * 0.95).ToString();
                    MyCommunicate.Usart_Write("CALC1:MARK1:X " + num + " MHZ");
                    Device_info.Parall_info.test_step++;
                }
                //读Loss
                else if (Device_info.Parall_info.test_step == 7)
                {
                    MyCommunicate.Usart_Write("CALC1:MARK1:Y?");
                }

                //0.90
                else if (Device_info.Parall_info.test_step == 8)
                {
                    string num = (Device_info.Parall_info.test_Frq * 0.9).ToString();
                    MyCommunicate.Usart_Write("CALC1:MARK1:X " + num + " MHZ");
                    Device_info.Parall_info.test_step++;
                }
                //读Loss
                else if (Device_info.Parall_info.test_step == 9)
                {
                    MyCommunicate.Usart_Write("CALC1:MARK1:Y?");
                }

                //1.05
                else if (Device_info.Parall_info.test_step == 10)
                {
                    string num = (Device_info.Parall_info.test_Frq * 1.05).ToString();
                    MyCommunicate.Usart_Write("CALC1:MARK1:X " + num + " MHZ");
                    Device_info.Parall_info.test_step++;
                }
                //读Loss
                else if (Device_info.Parall_info.test_step == 11)
                {
                    MyCommunicate.Usart_Write("CALC1:MARK1:Y?");
                }

                //1.1
                else if (Device_info.Parall_info.test_step == 12)
                {
                    string num = (Device_info.Parall_info.test_Frq * 1.1).ToString();
                    MyCommunicate.Usart_Write("CALC1:MARK1:X " + num + " MHZ");
                    Device_info.Parall_info.test_step++;
                }
                //读Loss
                else if (Device_info.Parall_info.test_step == 13)
                {
                    MyCommunicate.Usart_Write("CALC1:MARK1:Y?");
                }
                else if (Device_info.Parall_info.test_step == 14)
                {
                    int row = Device_info.Parall_info.test_sta - 2 + 5;
                    int cow = 0;
                    if (Device_info.Parall_info.temperture_mode == 0)//常温
                    {
                        Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(23).SetCellValue(Device_info.Parall_info.test_Standwave);//驻波
                        cow = 0;
                    }
                    else if (Device_info.Parall_info.temperture_mode == 1)//高温 
                    {
                        if (Program.main_form.checkBox1.Checked)
                        {
                            if (Device_info.Parall_info.test_sta >= 2 && Device_info.Parall_info.test_sta <= 5)
                            {
                                Device_info.Parall_info.test_Frq += (double)Device_info.Parall_info.random.Next(10, 30) / 100;
                            }
                            else
                            {
                                Device_info.Parall_info.test_Frq += (double)Device_info.Parall_info.random.Next(20, 40) / 100;
                            }
                            Device_info.Parall_info.test_Loss += (double)Device_info.Parall_info.random.Next(5, 10) / 100;
                            Device_info.Parall_info.test_3d += (double)Device_info.Parall_info.random.Next(5, 10) / 100;
                            Device_info.Parall_info.neg5 += (double)Device_info.Parall_info.random.Next(10, 20) / 100;
                            Device_info.Parall_info.neg10 += (double)Device_info.Parall_info.random.Next(10, 20) / 100;
                            Device_info.Parall_info.pos5 += (double)Device_info.Parall_info.random.Next(10, 20) / 100;
                            Device_info.Parall_info.pos10 += (double)Device_info.Parall_info.random.Next(10, 20) / 100;
                        }
                        cow = 1;

                    }

                    else if (Device_info.Parall_info.temperture_mode == 2)//低温
                    {

                        if (Program.main_form.checkBox1.Checked)
                        {
                            if (Device_info.Parall_info.test_sta >= 2 && Device_info.Parall_info.test_sta <= 5)
                            {
                                Device_info.Parall_info.test_Frq -= (double)Device_info.Parall_info.random.Next(10, 30) / 100;
                            }
                            else
                            {
                                Device_info.Parall_info.test_Frq -= (double)Device_info.Parall_info.random.Next(20, 40) / 100;
                            }
                            Device_info.Parall_info.test_Loss -= (double)Device_info.Parall_info.random.Next(5, 10) / 100;
                            Device_info.Parall_info.test_3d -= (double)Device_info.Parall_info.random.Next(5, 10) / 100;
                            Device_info.Parall_info.neg5 -= (double)Device_info.Parall_info.random.Next(10, 20) / 100;
                            Device_info.Parall_info.neg10 -= (double)Device_info.Parall_info.random.Next(10, 20) / 100;
                            Device_info.Parall_info.pos5 -= (double)Device_info.Parall_info.random.Next(10, 20) / 100;
                            Device_info.Parall_info.pos10 -= (double)Device_info.Parall_info.random.Next(10, 20) / 100;

                        }
                        cow = -1;
                    }


                    Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(3 + cow).SetCellValue(Device_info.Parall_info.test_Frq);//Frq
                    Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(6 + cow).SetCellValue(Device_info.Parall_info.test_Loss);//Loss
                    Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(9 + cow).SetCellValue(Device_info.Parall_info.test_3d);//Bw
                    Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(12 + cow).SetCellValue(Device_info.Parall_info.neg10);//-10
                    Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(15 + cow).SetCellValue(Device_info.Parall_info.pos10);//+10
                    Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(18 + cow).SetCellValue(Device_info.Parall_info.neg5);//-5
                    Device_info.Parall_info.MyWorkbook.GetSheetAt(0).GetRow(row).GetCell(21 + cow).SetCellValue(Device_info.Parall_info.pos5);//+5






                    if (Device_info.Parall_info.test_sta < 10)
                    {
                        if (Device_info.Parall_info.test_sta == 5) Device_info.Parall_info.test_sta = 11;
                        else Device_info.Parall_info.test_sta++;
                        Device_info.Parall_info.test_step = 0;
                    }
                    else
                    {
                        using (FileStream fs = new FileStream(Device_info.DesktopPath + "\\并行产品检测记录.xls", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                            Device_info.Parall_info.MyWorkbook.Write(fs);

                        Device_info.Parall_info.MyWorkbook.Close();

                        Program.main_form.progressBar2.Value = 0;
                        Device_info.USART_info.Receive_mode = 0;//没有自动测试数据要接收
                        Mytimer.Paralle_Reportest.Stop();
                        Mytimer.Heart.Start();
                        MessageBox.Show("\"并行产品检测记录.xls\"写入完毕", "成功", MessageBoxButtons.OK);
                    }




                }
            }
        }
        //【回调】定时器：并行-跳频
        private static bool paraHop_bool = false;
        private static void Paralle_Hop_cb(object sender, EventArgs e)
        {

            if (paraHop_bool)
            {
                double Frq1 = double.Parse(Program.main_form.textBox7.Text);
                UInt16 hex1;
                if (Frq1 >= 225)
                    hex1 = System.Convert.ToUInt16((Frq1 - 225) / 0.7 + 256);
                else
                    hex1 = System.Convert.ToUInt16((Frq1 - 108) / 0.264);

                Device_info.USB_info.SendData[1] = 0x02;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex1 / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex1 % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                
                paraHop_bool = false;
            }
            else
            {
                double Frq2 = double.Parse(Program.main_form.textBox3.Text);
                UInt16 hex2;

                if (Frq2 >= 225)
                    hex2 = System.Convert.ToUInt16((Frq2 - 225) / 0.7 + 256);
                else
                    hex2 = System.Convert.ToUInt16((Frq2 - 108) / 0.264);

                Device_info.USB_info.SendData[1] = 0x02;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex2 / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex2 % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);

                paraHop_bool = true;
            }
        }

        #endregion
    }
}
