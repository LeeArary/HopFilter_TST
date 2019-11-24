using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using NPOI.XSSF.Streaming;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF;
using NPOI.XSSF.UserModel;
using System.Threading;
using System.Runtime.InteropServices;
namespace Upper
{

   
    public partial class Main_Form : Form
    {
        //构造函数
        public Main_Form()
        {
            InitializeComponent();
            
            Control.CheckForIllegalCrossThreadCalls = false;
         
            label_DesktopPath.Text = Device_info.DesktopPath;

            //串行共605个数据
            progressBar1.Maximum = 605;
            progressBar1.Minimum = 0;

            //并行共2046个数据
            progressBar2.Maximum = 2048;
            progressBar1.Minimum = 0;
            progressBar3.Maximum = 1000;
            progressBar3.Minimum = 0;

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            
            //按键变化
            SerialButton_All(true, true, false);
            ParallButton_All(true, true, false);
            DownloadButton_All(false);

            Mytimer.Heart.Enabled = true;//开启心跳

        }

        #region 按键变化函数
        public void SerialButton_All(bool IsAuto,bool IsManual,bool sw)
        {
            if (IsAuto)
            {
                button11.Enabled = sw;//【串行】一键配置

            }
            if (IsManual)
            {
                button24.Enabled = sw;//【串行】手动置数-1
                button25.Enabled = sw;//【串行】+1
                button22.Enabled = sw;//【串行】-10
                button23.Enabled = sw;//【串行】+10
                comboBox2.Enabled = sw;//【串行】手动置数下拉框

                button3.Enabled = sw;//【串行】-1
                button2.Enabled = sw;//【串行】+1
                button5.Enabled = sw;//【串行】-10
                button4.Enabled = sw;//【串行】+10

                button7.Enabled = sw;//【串行】108
                button8.Enabled = sw;//【串行】140
                button6.Enabled = sw;//【串行】174
                button9.Enabled = sw;//【串行】225
                button14.Enabled = sw;//【串行】260.2
                button13.Enabled = sw;//【串行】300.2
                button15.Enabled = sw;//【串行】340.2
                button12.Enabled = sw;//【串行】400.2


                button16.Enabled = sw;//【串行】跳频
                button10.Enabled = sw;//【串行】跳频 终止

            }
            if (IsAuto && IsManual)
            {
                butto.Enabled = sw;//【串行】自动测试
                button18.Enabled = sw;//【串行】自动测试终止

                button17.Enabled = sw;//【串行】自动生成报表
                button19.Enabled = sw;//【串行】自动生成报表 终止
            }


        }
        public void ParallButton_All(bool IsAuto, bool IsManual, bool sw)
        {
            if (IsAuto)
            {
                button42.Enabled = sw;//【并行】一键配置
            }
            if (IsManual)
            {
                button45.Enabled = sw;//【并行】手动置数-1
                button46.Enabled = sw;//【并行】+1
                button43.Enabled = sw;//【并行】-10
                button44.Enabled = sw;//【并行】+10
                comboBox4.Enabled = sw;//【并行】手动置数下拉框

                button38.Enabled = sw;//【并行】-1
                button39.Enabled = sw;//【并行】+1
                button36.Enabled = sw;//【并行】-10
                button37.Enabled = sw;//【并行】+10

                button34.Enabled = sw;//【并行】108
                button33.Enabled = sw;//【并行】129.9
                button35.Enabled = sw;//【并行】150.2
                button32.Enabled = sw;//【并行】174
                button30.Enabled = sw;//【并行】225
                button29.Enabled = sw;//【并行】250.2
                button31.Enabled = sw;//【并行】299.9
                button28.Enabled = sw;//【并行】350.3
                button54.Enabled = sw;//【并行】400

                button41.Enabled = sw;//【并行】跳频
                button40.Enabled = sw;//【并行】跳频 终止
            }
            if (IsAuto && IsManual)
            {
                button47.Enabled = sw;//【并行】自动测试
                button48.Enabled = sw;//【并行】自动测试终止

                button27.Enabled = sw;//【并行】自动生成报表
                button26.Enabled = sw;//【并行】自动生成报表 终止

            }


        }
        public void DownloadButton_All(bool sw)
        {
            button51.Enabled = sw;//【下载】擦除
            button50.Enabled = sw;//【下载】下载
            button52.Enabled = sw;//【下载】验证
            button53.Enabled = sw;//【下载】一键下载
        }
        #endregion

        #region 界面绘制逻辑
        //绘制PC与失网连接线
        public void DrawLine_GPIB_SW(bool connect)
        {
            Pen pen;
            if (connect)
            {
                pen = new Pen(Color.Red, 5);
            }
            else
            {
                pen = new Pen(Color.Black, 5);
            }
            Point point1 = new Point(pictureGPIB.Left + pictureGPIB.Size.Width, pictureGPIB.Top + pictureGPIB.Size.Height / 2);
            Point point2 = new Point(pictureSW.Left, pictureGPIB.Top + pictureGPIB.Size.Height / 2);

            Graphics g = Page_Sta.CreateGraphics();
            g.DrawLine(pen, point1, point2);

        }
        //绘制PC与GPIB连接线
        public void DrawLine_PC_GPIB(bool connect)
        {
            Pen pen;
            if (connect)
            {
                pen = new Pen(Color.Red, 5);

            }
            else
            {
                pen = new Pen(Color.Black, 5);
            }
            Point point1 = new Point(picturePc.Left + 30, picturePc.Top);
            Point point2 = new Point(picturePc.Left + 30, pictureGPIB.Top + pictureGPIB.Size.Height / 2);
            Point point3 = new Point(pictureGPIB.Left, pictureGPIB.Top + pictureGPIB.Size.Height / 2);
            Graphics g = Page_Sta.CreateGraphics();
            g.DrawLine(pen, point1, point2);
            g.DrawLine(pen, point2, point3);
        }
        //绘制PC与STM连接线
        public void DrawLine_PC_STM(bool connect)
        {
            Pen pen;
            if (connect)
            {
                pen = new Pen(Color.Red, 5);
            }
            else
            {
                pen = new Pen(Color.Black, 5);
            }
            Point point1 = new Point(picturePc.Left + 30, picturePc.Top + picturePc.Size.Height + 20);
            Point point2 = new Point(picturePc.Left + 30, pictureSTM.Top + pictureSTM.Size.Height / 2);
            Point point3 = new Point(pictureSTM.Left, pictureSTM.Top + pictureSTM.Size.Height / 2);
            Graphics g = Page_Sta.CreateGraphics();
            g.DrawLine(pen, point1, point2);
            g.DrawLine(pen, point2, point3);
        }
        //绘制STM与LBQ连接线
        public void DrawLine_STM_LBQ(bool connect)
        {
            Pen pen;
            if (connect)
            {
                pen = new Pen(Color.Red, 5);
            }
            else
            {
                pen = new Pen(Color.Black, 5);
            }
            Point point1 = new Point(pictureSTM.Left + pictureSTM.Size.Width, pictureSTM.Top + pictureSTM.Size.Height / 2);
            Point point2 = new Point(pictureLBQ.Left, pictureSTM.Top + pictureSTM.Size.Height / 2);

            Graphics g = Page_Sta.CreateGraphics();
            g.DrawLine(pen, point1, point2);

        }
        //连接状态页面重绘
        private void Page_Sta_Paint(object sender, PaintEventArgs e)
        {
            if (Device_info.USART_info.connect_NA) DrawLine_GPIB_SW(true);
            else DrawLine_GPIB_SW(false);

            if (Device_info.USART_info.connect_uart) DrawLine_PC_GPIB(true);
            else DrawLine_PC_GPIB(false);

            if (Device_info.USB_info.connect_STM)
            {
                DrawLine_PC_STM(true);
                DrawLine_STM_LBQ(true);
            }
            else
            {
                DrawLine_PC_STM(false);
                DrawLine_STM_LBQ(false);
            }


        }
        #endregion

        #region button逻辑
        #region 开始页面按钮
        private void button1_Click(object sender, EventArgs e)//输出路径
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Device_info.DesktopPath = dialog.SelectedPath;
                label_DesktopPath.Text = Device_info.DesktopPath;
            }
        }
        private void button20_Click(object sender, EventArgs e)//开启心跳
        {
            if (button20.Text == "关闭")
            {

                Mytimer.Heart.Stop();
                Mytimer.Config.Stop();
                Mytimer.Serial_Autotest.Stop();
                Mytimer.Serial_Hop.Stop();
                Mytimer.Serial_Reportest.Stop();

                Mytimer.Paralle_Autotest.Stop();
                Mytimer.Paralle_Hop.Stop();
                Mytimer.Paralle_Reportest.Stop();

                button20.Text = "开启";
            }
            else if (button20.Text == "开启")
            {
                Mytimer.Heart.Start();
                Mytimer.Config.Stop();
                Mytimer.Serial_Autotest.Stop();
                Mytimer.Serial_Hop.Stop();
                Mytimer.Serial_Reportest.Stop();

                Mytimer.Paralle_Autotest.Stop();
                Mytimer.Paralle_Hop.Stop();
                Mytimer.Paralle_Reportest.Stop();

                button20.Text = "关闭";
            }
        }
       
        #endregion
        #region 串行页面按钮
        private void button1_Click_1(object sender, EventArgs e)//自动测试
        {
            Device_info.USART_info.Receive_mode = 1;//串口接收的是自动测试数据

            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//左上，选择trace1
            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = 0;
            Device_info.USB_info.SendData[3] = 0;
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);

            Device_info.Config_info.Config_sta = 2;//配置第一段
            Device_info.Config_info.Config_Seq = 0;//配置计数清零

            //按键变化
            SerialButton_All(true, true, false);
            ParallButton_All(true, true, false);
            DownloadButton_All(false);
            button18.Enabled = true;//终止按键打开


            Device_info.Serial_info.Autotest_Form = new Form2();
            Mytimer.Heart.Stop();
            Mytimer.Config.Start();//开启配置
        }
        private void button18_Click(object sender, EventArgs e)//自动测试终止
        {
            //按键变化
            SerialButton_All(true, true, true);
            ParallButton_All(true, true, true);
            DownloadButton_All(true);

            button18.Enabled = false;//终止按键关闭
            button19.Enabled = false;
            button10.Enabled = false;
            button48.Enabled = false;
            button26.Enabled = false;
            button40.Enabled = false;


            Mytimer.Config.Stop();
            Mytimer.Serial_Autotest.Stop();
            progressBar1.Value = 0;

            Mytimer.Heart.Start();
        }
        private void button2_Click(object sender, EventArgs e)//手动测试+1
        {
            string str = textBox4.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex + 1 <= 604) hex++;
            MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

            //理论频率
            if (hex > 165)
                label18.Text = (225 + (hex - 166) * 0.4).ToString() + "MHz";
            else
                label18.Text = (108 + hex * 0.4).ToString() + "MHz";

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox4.Text = hex.ToString();

        }
        private void button3_Click(object sender, EventArgs e)//手动测试-1
        {
            string str = textBox4.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex != 0) hex--;
            MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

            //理论频率
            if (hex > 165)
                label18.Text = (225 + (hex - 166) * 0.4).ToString() + "MHz";
            else
                label18.Text = (108 + hex * 0.4).ToString() + "MHz";


            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox4.Text = hex.ToString();
        }
        private void button4_Click(object sender, EventArgs e)//手动测试+10
        {
            string str = textBox4.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex + 10 <= 604) hex += 10;

            MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            //理论频率
            if (hex > 165)
                label18.Text = (225 + (hex - 166) * 0.4).ToString() + "MHz";
            else
                label18.Text = (108 + hex * 0.4).ToString() + "MHz";


            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox4.Text = hex.ToString();
            
        }
        private void button5_Click(object sender, EventArgs e)//手动测试-10
        {
            string str = textBox4.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex >= 10) hex -= 10;
            MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            //理论频率
            if (hex > 165)
                label18.Text = (225 + (hex - 166) * 0.4).ToString() + "MHz";
            else
                label18.Text = (108 + hex * 0.4).ToString() + "MHz";


            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox4.Text = hex.ToString();
        }
        private void button16_Click(object sender, EventArgs e)//跳频
        {
            //按键变化
            SerialButton_All(true, true, false);
            ParallButton_All(true, true, false);
            DownloadButton_All(false);
            button10.Enabled = true;//终止按键打开


            Mytimer.Heart.Stop();
            Mytimer.Serial_Hop.Start();
        }
        private void button10_Click(object sender, EventArgs e)//终止跳频
        {
            //按键变化
            SerialButton_All(true, true, true);
            ParallButton_All(true, true, true);
            DownloadButton_All(true);
            button18.Enabled = false;//终止按键关闭
            button19.Enabled = false;
            button10.Enabled = false;
            button48.Enabled = false;
            button26.Enabled = false;
            button40.Enabled = false;


            progressBar1.Value = 0;

            Mytimer.Serial_Hop.Stop();
            Mytimer.Heart.Start();
        }
        private void button11_Click(object sender, EventArgs e)//一键配置
        {
            if (Device_info.USART_info.connect_NA)
            {
                string BW1_dBm = textBox_BW1dB.Text.ToString();
                string BW2_dBm = textBox_BW2dB.Text.ToString();

                string Star1_Frq = textBox_STAR1Frq.Text.ToString();
                string Stop1_Frq = textBox_STOP1Frq.Text.ToString();

                string Star2_Frq = textBox_STAR2Frq.Text.ToString();
                string Stop2_Frq = textBox_STOP2Frq.Text.ToString();

                string PWR_dBm = textBox_PWRdB.Text.ToString();
                string Offset = textBox_OFFSET.Text.ToString();


                //存储BW1，BW2，起始，终止，温飘
                Device_info.Serial_info.Bw1 = double.Parse(BW1_dBm);
                Device_info.Serial_info.Bw2 = double.Parse(BW2_dBm);

                Device_info.Serial_info.Start1Frq = double.Parse(Star1_Frq);
                Device_info.Serial_info.Start2Frq = double.Parse(Star2_Frq);

                Device_info.Serial_info.Stop1Frq = double.Parse(Stop1_Frq);
                Device_info.Serial_info.Stop2Frq = double.Parse(Stop2_Frq);

                Device_info.Serial_info.OffSet = double.Parse(Offset);
                Device_info.Serial_info.PWR_dBm = double.Parse(PWR_dBm);


                Device_info.Config_info.Config_content[1] = "SOUR1:POW:LEV:IMM:AMPL " + Device_info.Serial_info.PWR_dBm.ToString() + "E0";
                Device_info.Config_info.Config_content[3] = "SENS1:FREQ:STAR " + Device_info.Serial_info.Start1Frq.ToString() + "E6";
                Device_info.Config_info.Config_content[4] = "SENS1:FREQ:STOP " + Device_info.Serial_info.Stop1Frq.ToString() + "E6";
                Device_info.Config_info.Config_content[5] = "CALC1:MARK1:BWID " + Device_info.Serial_info.Bw1.ToString() + "E0";



                //按键变化
                SerialButton_All(true, false, false);
                ParallButton_All(true, false, false);


                Device_info.Config_info.Config_Seq = 0;//配置计数清零
                Device_info.Config_info.Config_sta = 1;//一键配置仪器状态
                progressBar1.Value = 0;//进度条为0
                Mytimer.Heart.Stop();
                Mytimer.Config.Start();//开启配置
            }

            else
                MessageBox.Show("请先连接失网", "错误", MessageBoxButtons.OK);
        }
        private void button17_Click(object sender, EventArgs e)//自动生成报表
        {
            string fileName = Device_info.DesktopPath + "\\产品检测记录.xls";
            if (File.Exists(fileName))
            {
                try
                {
                    using (FileStream stream = File.OpenRead(fileName))
                        Device_info.Serial_info.MyWorkbook = new HSSFWorkbook(stream);
                }
                catch
                {
                    MessageBox.Show("请关闭\"产品检测记录.xls\"", "错误", MessageBoxButtons.OK);
                    return;
                }



                //Device_info.Serial_info.MyWorkbook.Close();
                Device_info.Serial_info.test_step = 0;
                Device_info.Serial_info.test_sta = 0;


                //按键变化
                SerialButton_All(true, true, false);
                ParallButton_All(true, true, false);
                DownloadButton_All(false);
                button19.Enabled = true;//终止按键打开

                Device_info.Serial_info.temperture_mode = comboBox1.SelectedIndex;
                Device_info.USART_info.Receive_mode = 2;//串口接收数据是自动生成报表的

                Mytimer.Heart.Stop();
                Mytimer.Serial_Reportest.Start();
            }
            else
            {

                MessageBox.Show("请先在" + Device_info.DesktopPath + "创建\"产品检测记录.xls\"", "", MessageBoxButtons.OK);


            }
        }
        private void button19_Click(object sender, EventArgs e)//自动生成报表，终止
        {
            //按键变化
            SerialButton_All(true, true, true);
            ParallButton_All(true, true, true);
            DownloadButton_All(true);
            button18.Enabled = false;//终止按键关闭
            button19.Enabled = false;
            button10.Enabled = false;
            button48.Enabled = false;
            button26.Enabled = false;
            button40.Enabled = false;


            Device_info.Serial_info.MyWorkbook.Close();//关闭Excel工作簿
            progressBar1.Value = 0;
            Mytimer.Serial_Reportest.Stop();
            Mytimer.Heart.Start();
        }
        private void button7_Click(object sender, EventArgs e)//180
        {
            UInt16 hex = System.Convert.ToUInt16((108 - 108) / 0.4);
            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button8_Click(object sender, EventArgs e)//140
        {
            UInt16 hex = System.Convert.ToUInt16((140 - 108) / 0.4);

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button6_Click(object sender, EventArgs e)//174
        {
            UInt16 hex = System.Convert.ToUInt16((174 - 108) / 0.4);

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button9_Click(object sender, EventArgs e)//225
        {
            UInt16 hex = System.Convert.ToUInt16((225 - 225) / 0.4 + 166);

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button14_Click(object sender, EventArgs e)//260.2
        {
            UInt16 hex = System.Convert.ToUInt16((260.2 - 225) / 0.4 + 166);

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button13_Click(object sender, EventArgs e)//300.2
        {
            UInt16 hex = System.Convert.ToUInt16((300.2 - 225) / 0.4 + 166);

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button15_Click(object sender, EventArgs e)//340.2
        {
            UInt16 hex = System.Convert.ToUInt16((340.2 - 225) / 0.4 + 166);

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button12_Click(object sender, EventArgs e)//400.2
        {
            UInt16 hex = System.Convert.ToUInt16((400.2 - 225) / 0.4 + 166);

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }        
        private void button24_Click(object sender, EventArgs e)//手动置数-1
        {
            string str = textBox1.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            if (hex != 0) hex--;
            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            textBox1.Text = hex.ToString("X");
            if (comboBox2.SelectedIndex == 1)//225-512
                hex |= 0x1000;

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);

        }
        private void button25_Click(object sender, EventArgs e)//手动置数+1
        {
            string str = textBox1.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            hex++;
            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            textBox1.Text = hex.ToString("X");
            if (comboBox2.SelectedIndex == 1)//225-512
                hex |= 0x1000;

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button22_Click(object sender, EventArgs e)//手动置数-10
        {
            string str = textBox1.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            if (hex >= 10) hex -= 10;
            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            textBox1.Text = hex.ToString("X");
            if (comboBox2.SelectedIndex == 1)//225-512
                hex |= 0x1000;

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button23_Click(object sender, EventArgs e)//手动置数+10
        {
            string str = textBox1.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            hex += 10;
            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            textBox1.Text = hex.ToString("X");
            if (comboBox2.SelectedIndex == 1)//225-512
                hex |= 0x1000;

            Device_info.USB_info.SendData[1] = 0x01;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        #endregion
        #region 并行页面按钮
        private void button46_Click(object sender, EventArgs e)//手动置数+1
        {
            string str = textBox16.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            if (hex < 1023) hex++;
            //srialPort1_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            textBox16.Text = hex.ToString("X");
            if (comboBox4.SelectedIndex == 1)//225-512
                hex |= 0x400;

            Device_info.USB_info.SendData[1] = 0x02;//并行功能
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button45_Click(object sender, EventArgs e)//手动置数-1
        {
            string str = textBox16.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            if (hex != 0) hex--;

            textBox16.Text = hex.ToString("X");
            if (comboBox4.SelectedIndex == 1)//225-512
                hex |= 0x400;

            Device_info.USB_info.SendData[1] = 0x02;//并行功能
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button44_Click(object sender, EventArgs e)//手动置数+10
        {
            string str = textBox16.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            if (hex + 10 <= 1023) hex += 10;
            //srialPort1_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            textBox16.Text = hex.ToString("X");
            if (comboBox4.SelectedIndex == 1)//225-512
                hex |= 0x400;

            Device_info.USB_info.SendData[1] = 0x02;//并行功能
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button43_Click(object sender, EventArgs e)//手动置数-10
        {
            string str = textBox16.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            if (hex - 10 >= 0) hex -= 10;
            //srialPort1_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            textBox16.Text = hex.ToString("X");
            if (comboBox4.SelectedIndex == 1)//225-512
                hex |= 0x400;

            Device_info.USB_info.SendData[1] = 0x02;//并行功能
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button42_Click(object sender, EventArgs e)//一键配置
        {
            if (Device_info.USART_info.connect_NA)
            {
                string BW1_dBm = textBox15.Text.ToString();


                string Star1_Frq = textBox11.Text.ToString();
                string Stop1_Frq = textBox10.Text.ToString();

                string Star2_Frq = textBox9.Text.ToString();
                string Stop2_Frq = textBox8.Text.ToString();

                string PWR_dBm = textBox14.Text.ToString();
                string Offset = textBox13.Text.ToString();


                //存储BW1，起始，终止，温飘
                Device_info.Parall_info.Bw1 = Convert.ToDouble(BW1_dBm);


                Device_info.Parall_info.Start1Frq = Convert.ToDouble(Star1_Frq);
                Device_info.Parall_info.Start2Frq = Convert.ToDouble(Star2_Frq);

                Device_info.Parall_info.Stop1Frq = Convert.ToDouble(Stop1_Frq);
                Device_info.Parall_info.Stop2Frq = Convert.ToDouble(Stop2_Frq);

                Device_info.Parall_info.OffSet = Convert.ToDouble(Offset);
                Device_info.Parall_info.PWR_dBm = Convert.ToDouble(PWR_dBm);


                Device_info.Config_info.Config_content[1] = "SOUR1:POW:LEV:IMM:AMPL " + Device_info.Parall_info.PWR_dBm.ToString() + "E0";
                Device_info.Config_info.Config_content[3] = "SENS1:FREQ:STAR " + Device_info.Parall_info.Start1Frq.ToString() + "E6";
                Device_info.Config_info.Config_content[4] = "SENS1:FREQ:STOP " + Device_info.Parall_info.Stop1Frq.ToString() + "E6";
                Device_info.Config_info.Config_content[5] = "CALC1:MARK1:BWID " + Device_info.Parall_info.Bw1.ToString() + "E0";



                //按键变化
                SerialButton_All(true, false, false);
                ParallButton_All(true, false, false);

                Device_info.Config_info.Config_Seq = 0;//配置计数清零
                Device_info.Config_info.Config_sta = 1;//一键配置仪器状态
                progressBar2.Value = 0;//进度条为0
                Mytimer.Heart.Stop();
                Mytimer.Config.Start();//开启配置
            }
        }
        private void button47_Click(object sender, EventArgs e)//自动测试
        {
            Device_info.USART_info.Receive_mode = 4;//串口接收的是自动测试数据

            //按键变化
            SerialButton_All(true, true, false);
            ParallButton_All(true, true, false);
            DownloadButton_All(false);
            button48.Enabled = true;//终止按键打开


            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = 0;
            Device_info.USB_info.SendData[3] = 0;
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);

            Device_info.Config_info.Config_sta = 4;//并行配置第一段
            Device_info.Config_info.Config_Seq = 0;//配置计数清零

            Device_info.Parall_info.Autotest_resForm = new Form3();
            Mytimer.Heart.Stop();
            Mytimer.Config.Start();//开启配置
        }
        private void button48_Click(object sender, EventArgs e)//自动测试，终止
        {
            //按键变化
            SerialButton_All(true, true, true);
            ParallButton_All(true, true, true);
            DownloadButton_All(true);

            button18.Enabled = false;//终止按键关闭
            button19.Enabled = false;
            button10.Enabled = false;
            button48.Enabled = false;
            button26.Enabled = false;
            button40.Enabled = false;

            Mytimer.Config.Stop();
            Mytimer.Paralle_Autotest.Stop();
            progressBar2.Value = 0;

            Mytimer.Heart   .Start();
        }        
        private void button38_Click(object sender, EventArgs e)//手动测试-1
        {
            string str = textBox2.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex != 0) hex--;
            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

            //理论频率
            if (hex > 255)
                label46.Text = (225 + (hex - 256) * 0.7).ToString() + "MHz";
            else
                label46.Text = (108 + hex * 0.264).ToString() + "MHz";


            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox2.Text = hex.ToString();
        }
        private void button39_Click(object sender, EventArgs e)//手动测试+1
        {
            string str = textBox2.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex + 1 <= 511) hex++;
            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

            //理论频率
            if (hex > 255)
                label46.Text = (225 + (hex - 256) * 0.7).ToString() + "MHz";
            else
                label46.Text = (108 + hex * 0.264).ToString() + "MHz";


            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox2.Text = hex.ToString();

        }
        private void button36_Click(object sender, EventArgs e)//手动测试-10
        {
            string str = textBox2.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex >= 10) hex -= 10;

            //理论频率
            if (hex > 255)
                label46.Text = (225 + (hex - 256) * 0.7).ToString() + "MHz";
            else
                label46.Text = (108 + hex * 0.264).ToString() + "MHz";


            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox2.Text = hex.ToString();
        }
        private void button37_Click(object sender, EventArgs e)//手动测试+10
        {
            string str = textBox2.Text;
            UInt16 hex = UInt16.Parse(str);
            if (hex + 10 <= 511) hex += 10;

            MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1
            //理论频率
            if (hex > 255)
                label46.Text = (225 + (hex - 256) * 0.7).ToString() + "MHz";
            else
                label46.Text = (108 + hex * 0.264).ToString() + "MHz";


            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            textBox2.Text = hex.ToString();
        }
        private void button34_Click(object sender, EventArgs e)//108
        {
            UInt16 hex = System.Convert.ToUInt16((108 - 108) / 0.264);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button33_Click(object sender, EventArgs e)//129.9
        {
            UInt16 hex = System.Convert.ToUInt16((129.9 - 108) / 0.264);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button35_Click(object sender, EventArgs e)//150.2
        {
            UInt16 hex = System.Convert.ToUInt16((150.2 - 108) / 0.264);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button32_Click(object sender, EventArgs e)//174
        {
            UInt16 hex = System.Convert.ToUInt16((174 - 108) / 0.264);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button30_Click(object sender, EventArgs e)//225
        {
            UInt16 hex = System.Convert.ToUInt16((225 - 225) / 0.7 + 256);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button29_Click(object sender, EventArgs e)//250.2
        {
            UInt16 hex = System.Convert.ToUInt16((250.2 - 225) / 0.7 + 256);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button31_Click(object sender, EventArgs e)//299.9
        {
            UInt16 hex = System.Convert.ToUInt16((299.9 - 225) / 0.7 + 256);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button28_Click(object sender, EventArgs e)//350.3
        {
            UInt16 hex = System.Convert.ToUInt16((350.3 - 225) / 0.7 + 256);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button54_Click(object sender, EventArgs e)//400
        {
            UInt16 hex = System.Convert.ToUInt16((400 - 225) / 0.7 + 256);
            Device_info.USB_info.SendData[1] = 0x02;//功能帧
            Device_info.USB_info.SendData[2] = (byte)(hex / 256);
            Device_info.USB_info.SendData[3] = (byte)(hex % 256);
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
        }
        private void button27_Click(object sender, EventArgs e)//自动生成报表
        {
            string fileName = Device_info.DesktopPath + "\\并行产品检测记录.xls";
            if (File.Exists(fileName))
            {
                try
                {
                    using (FileStream stream = File.OpenRead(fileName))
                        Device_info.Parall_info.MyWorkbook = new HSSFWorkbook(stream);
                }
                catch
                {
                    MessageBox.Show("请关闭\"并行产品检测记录.xls\"", "错误", MessageBoxButtons.OK);
                    return;
                }



                Device_info.Parall_info.test_step = 0;
                Device_info.Parall_info.test_sta = 0;

                //按键变化
                SerialButton_All(true, true, false);
                ParallButton_All(true, true, false);
                DownloadButton_All(false);
                button26.Enabled = true;//终止按键打开

                Device_info.Parall_info.temperture_mode = comboBox3.SelectedIndex;
                Device_info.USART_info.Receive_mode = 5;//并行串口接收模式

                Mytimer.Heart.Stop();
                Mytimer.Paralle_Reportest.Start();
            }
            else
            {

                MessageBox.Show("请先在" + Device_info.DesktopPath + "创建\"产品检测记录.xls\"", "", MessageBoxButtons.OK);


            }
        }
        private void button26_Click(object sender, EventArgs e)//自动生成报表，终止
        {
            //按键变化
            SerialButton_All(true, true, true);
            ParallButton_All(true, true, true);
            DownloadButton_All(true);
            button18.Enabled = false;//终止按键关闭
            button19.Enabled = false;
            button10.Enabled = false;
            button48.Enabled = false;
            button26.Enabled = false;
            button40.Enabled = false;


            Device_info.Parall_info.MyWorkbook.Close();//关闭Excel工作簿
            progressBar2.Value = 0;
            Mytimer.Paralle_Reportest.Stop();
            Mytimer.Heart.Start();
        }
        private void button41_Click(object sender, EventArgs e)//跳频
        {
            //按键变化
            SerialButton_All(true, true, false);
            ParallButton_All(true, true, false);
            DownloadButton_All(false);
            button40.Enabled = true;//终止按键打开

            Mytimer.Paralle_Hop.Start();
            Mytimer.Heart.Stop();

        }
        private void button40_Click(object sender, EventArgs e)//跳频，终止
        {
            //按键变化
            SerialButton_All(true, true, true);
            ParallButton_All(true, true, true);
            DownloadButton_All(true);
            button18.Enabled = false;//终止按键关闭
            button19.Enabled = false;
            button10.Enabled = false;
            button48.Enabled = false;
            button26.Enabled = false;
            button40.Enabled = false;


            progressBar2.Value = 0;

            Mytimer.Paralle_Hop.Stop();
            Mytimer.Heart.Start();
        }
        #endregion
        #region 下载页面按钮
        private void button49_Click(object sender, EventArgs e)//选择文件
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Device_info.DesktopPath;//默认打开C：
            fileDialog.Filter = "文本文件(*.txt)|*.txt|所有文件(*.*)|*.*";
            //fileDialog1.FilterIndex = 1;//如果您设置 FilterIndex 属性，则当显示对话框时，将选择该筛选器。
            //fileDialog1.RestoreDirectory = true;//取得或设定值，指出对话方块是否在关闭前还原目前的目录。
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Device_info.Parall_info.DownloadFilePath = fileDialog.FileName;
                label27.Text = fileDialog.SafeFileName;
                StreamReader fileRead = new StreamReader(Device_info.Parall_info.DownloadFilePath);
                richTextBox1.Text = fileRead.ReadToEnd();
                fileRead.Close();
            }

        }
        private void button51_Click(object sender, EventArgs e)//擦出
        {

            label_erase.Text = "";
            //擦除
            Device_info.USB_info.SendData[1] = 0x05;//擦除
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);

        }
        private void button50_Click(object sender, EventArgs e)//下载
        {
            if (Device_info.Parall_info.DownloadFilePath == null)
            {
                MessageBox.Show("选择一项下载文件");
                return;
            }
            else
            {
                StreamReader File = new StreamReader(Device_info.Parall_info.DownloadFilePath, Encoding.Default);

                int Rowcnt = 0;//行数
                while (File.ReadLine() != null)
                {
                    Rowcnt++;
                }
                Device_info.Parall_info.DownloadFileRow = Rowcnt;
                File.Close();
            }
            label_dnload.Text = "";
            StreamReader DownloadFile = new StreamReader(Device_info.Parall_info.DownloadFilePath, Encoding.Default);

            Device_info.USB_info.SendData[1] = 0x03;//下载
            Device_info.USB_info.SendData[2] = 0xAA;//起始信号
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            UInt16 i;
            for (i = 0; i < Device_info.Parall_info.DownloadFileRow; i += 2)
            {
                try
                {
                    progressBar3.Value = progressBar3.Maximum * i / Device_info.Parall_info.DownloadFileRow;
                    string Line1 = DownloadFile.ReadLine();
                    string Line2 = DownloadFile.ReadLine();
                    string[] strs1 = Line1.Split(' ');
                    string[] strs2 = Line2.Split(' ');
                    UInt16 hex1 = Convert.ToUInt16(strs1[3], 16);
                    UInt16 hex2 = Convert.ToUInt16(strs2[3], 16);

                    Device_info.USB_info.SendData[1] = 0x03;//下载
                    Device_info.USB_info.SendData[2] = 0xBB;//发送中

                    Device_info.USB_info.SendData[3] = (byte)(hex1 / 256);//data1
                    Device_info.USB_info.SendData[4] = (byte)(hex1 % 256);

                    Device_info.USB_info.SendData[5] = (byte)(hex2 / 256);//data2
                    Device_info.USB_info.SendData[6] = (byte)(hex2 % 256);

                    MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                    Thread.Sleep(1);
                }
                catch
                {
                    break;
                }
            }
            progressBar3.Value = 0;
            if (i == Device_info.Parall_info.DownloadFileRow)
            {
                label_dnload.Text = "√";
                label_dnload.ForeColor = Color.Green;
                Device_info.USB_info.SendData[1] = 0x03;//下载
                Device_info.USB_info.SendData[2] = 0xCC;//终止信号
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);



                //     Device_info.USB_info.SendData[1] = 0x04;//验证
                //     MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            }
            else
            {
                label_dnload.Text = "×";
                label_dnload.ForeColor = Color.Red;
            }




        }
        private void button52_Click(object sender, EventArgs e)//验证
        {
            if (Device_info.Parall_info.DownloadFilePath == null)
            {
                MessageBox.Show("选择一项下载文件");
                return;
            }
            else
            {
                label_cali.Text = "";
                StreamReader File = new StreamReader(Device_info.Parall_info.DownloadFilePath, Encoding.Default);

                int Rowcnt = 0;//行数
                while (File.ReadLine() != null)
                {
                    Rowcnt++;
                }
                Device_info.Parall_info.DownloadFileRow = Rowcnt;
                File.Close();
                Device_info.USB_info.SendData[1] = 0x04;//验证
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            }

        }
        private void button53_Click(object sender, EventArgs e)//一键下载
        {
            //擦除

            label_cali.Text = "";
            label_dnload.Text = "";
            label_erase.Text = "";

            progressBar3.Value = 0;
            Device_info.USB_info.SendData[1] = 0x05;//擦除
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);

            Thread.Sleep(1000);
            if (Device_info.Parall_info.DownloadFilePath == null)
            {
                MessageBox.Show("选择一项下载文件");
                return;
            }
            else
            {
                StreamReader File = new StreamReader(Device_info.Parall_info.DownloadFilePath, Encoding.Default);

                int Rowcnt = 0;//行数
                while (File.ReadLine() != null)
                {
                    Rowcnt++;
                }
                Device_info.Parall_info.DownloadFileRow = Rowcnt;
                File.Close();
            }

            StreamReader DownloadFile = new StreamReader(Device_info.Parall_info.DownloadFilePath, Encoding.Default);

            Device_info.USB_info.SendData[1] = 0x03;//下载
            Device_info.USB_info.SendData[2] = 0xAA;//起始信号
            MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            UInt16 i;
            for (i = 0; i < Device_info.Parall_info.DownloadFileRow; i += 2)
            {
                try
                {
                    progressBar3.Value = progressBar3.Maximum * i / Device_info.Parall_info.DownloadFileRow;
                    string Line1 = DownloadFile.ReadLine();
                    string Line2 = DownloadFile.ReadLine();
                    string[] strs1 = Line1.Split(' ');
                    string[] strs2 = Line2.Split(' ');
                    UInt16 hex1 = Convert.ToUInt16(strs1[3], 16);
                    UInt16 hex2 = Convert.ToUInt16(strs2[3], 16);

                    Device_info.USB_info.SendData[1] = 0x03;//下载
                    Device_info.USB_info.SendData[2] = 0xBB;//发送中

                    Device_info.USB_info.SendData[3] = (byte)(hex1 / 256);//data1
                    Device_info.USB_info.SendData[4] = (byte)(hex1 % 256);

                    Device_info.USB_info.SendData[5] = (byte)(hex2 / 256);//data2
                    Device_info.USB_info.SendData[6] = (byte)(hex2 % 256);

                    MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                    Thread.Sleep(10);
                }
                catch
                {
                    break;
                }

            }

            if (i == Device_info.Parall_info.DownloadFileRow)
            {
                label_dnload.Text = "√";
                label_dnload.ForeColor = Color.Green;
                Device_info.USB_info.SendData[1] = 0x03;//下载
                Device_info.USB_info.SendData[2] = 0xCC;//终止信号
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                Thread.Sleep(8000);


                progressBar3.Value = 0;
                Device_info.USB_info.SendData[1] = 0x04;//验证
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            }
            else
            {
                label_dnload.Text = "×";
                label_dnload.ForeColor = Color.Red;
            }


        }
        #endregion
        #endregion

        #region 事件：编辑框回车
        //事件：编辑框的回车按键
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)//
        {
            if (e.KeyChar == '\r')
            {
                string str = textBox1.Text;
                UInt16 hex = Convert.ToUInt16(str, 16);

                //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

                if (comboBox2.SelectedIndex == 1)//225-512
                    hex |= 0x1000;


                Device_info.USB_info.SendData[1] = 0x01;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            }
        }
        //事件：编辑框的回车按键
        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)//事件：手动测试的回车按键
        {
            if (e.KeyChar == '\r')
            {
                string str = textBox4.Text;
                UInt16 hex = UInt16.Parse(str);

                MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

                //理论频率
                if (hex > 165)
                    label18.Text = (225 + (hex - 166) * 0.4).ToString() + "MHz";
                else
                    label18.Text = (108 + hex * 0.4).ToString() + "MHz";


                Device_info.USB_info.SendData[1] = 0x01;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
                textBox4.Text = hex.ToString();

                Thread.Sleep(500);
                MyCommunicate.Usart_Write("CALC1:MARK1:BWID:DATA?");//查询实际频率，等待接收
                Device_info.USART_info.Receive_mode = 3;//串口接收的是手动测试数据

            }

        }
        //事件：编辑框的回车按键
        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                string str = textBox16.Text;
                UInt16 hex = Convert.ToUInt16(str, 16);

                //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

                if (comboBox4.SelectedIndex == 1)//225-512
                    hex |= 0x400;


                Device_info.USB_info.SendData[1] = 0x02;//并行功能
                Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            }
        }
        //事件：手动置数下拉框改变
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)//事件：手动置数的下拉框改变
        {
            string str = textBox1.Text;
            UInt16 hex = Convert.ToUInt16(str, 16);

            //MyCommunicate.Usart_Write("CALC1:PAR1:SEL");//2,左上选为tr1

            if (comboBox2.SelectedIndex == 1)//225-512
                hex |= 0x1000;

            if (Device_info.USB_info.connect_STM == true)
            {
                Device_info.USB_info.SendData[1] = 0x01;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);
            }
        }
        //事件：编辑框的回车按键
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (e.KeyChar == '\r')
            {
                string str = textBox2.Text;
                UInt16 hex = UInt16.Parse(str);
                if (hex > 511) return;

                //理论频率
                if (hex > 255)
                    label46.Text = (225 + (hex - 256) * 0.7).ToString() + "MHz";
                else
                    label46.Text = (108 + hex * 0.264).ToString() + "MHz";


                Device_info.USB_info.SendData[1] = 0x02;//功能帧
                Device_info.USB_info.SendData[2] = (byte)(hex / 256);
                Device_info.USB_info.SendData[3] = (byte)(hex % 256);
                MyCommunicate.USB_Write(Device_info.USB_info.SendData);

            }  
        }
        #endregion


        


    }

   
}