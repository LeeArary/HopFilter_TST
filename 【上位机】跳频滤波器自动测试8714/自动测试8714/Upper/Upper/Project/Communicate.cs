using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;
namespace Upper
{
    //下载Flash使用，用于接收下位机的校验数据
    public struct MessagsData
    {
        public byte FuncByte;
        public byte[] data;

    }

    public static class MyCommunicate
    {
        #region 字段
        private static SerialPort usart;
        private static USBHID usb;
     
        #endregion

        #region 属性
        public static string Usart_portName
        {
            get { return usart.PortName; }
            set { usart.PortName = value; }
        }

        public static bool Usart_isOpen
        {
            get { return usart.IsOpen; }
            set{}
        }

        #endregion
        #region 构造
  
        static MyCommunicate()
        {
            usb = new USBHID();
            usart = new SerialPort("COM1", 115200);
            usart.DataReceived += Usart_Recieve_cb;//串口接收事件指定回调函数
        }
        #endregion

        #region MyCommunicate的函数


        //USB-HID发送
        public static void USB_Write(byte[] aray)
        {
            usb.sendReport(aray);
        }
        //获取全部USB设备列表
        public static List<string> USB_GetDeviceList()
        {
            return usb.GetDeviceList();
        }
        //连接USB设备
        public static bool USB_OpenUSBHid(string devString)
        {
            return usb.OpenUSBHid(devString);
        }

        //串口发送
        public static void Usart_Write(string str)
        {
            try
            {
                usart.WriteLine(str);
            }
            catch
            {
                Device_info.USART_info.find_uart = false;
                Device_info.USART_info.find_NA = false;
                Device_info.USART_info.connect_NA = false;
                Device_info.USART_info.connect_uart = false;
                Device_info.USART_info.Scan_STA = 0;
                
                Program.main_form.DrawLine_GPIB_SW(false);
                Program.main_form.DrawLine_PC_GPIB(false);

            }
        }
        //打开串口
        public static void Usart_Open()
        {
            usart.Open();
        }
        //关闭串口
        public static void Usart_Close()
        {
            usart.Close();
        }

        
        #endregion

        #region 串口接收与USB接收
        //串口接收
        private static void Usart_Recieve_cb(object sender, SerialDataReceivedEventArgs e)
        {
            string str_res = "";

            try
            {
                str_res = usart.ReadLine();
            }
            catch//异常处理：矢网，GPIB全部断开
            {
                Device_info.USART_info.find_uart = false;
                Device_info.USART_info.find_NA = false;
                Device_info.USART_info.connect_NA = false;
                Device_info.USART_info.connect_uart = false;
                Device_info.USART_info.Scan_STA = 0;

                Program.main_form.DrawLine_GPIB_SW(false);
                Program.main_form.DrawLine_PC_GPIB(false);
            }

            //=======================数据处理======================//

            //GPIB连接器信息回复
            if (str_res.IndexOf("Prologix") != -1) 
                Device_info.USART_info.find_uart = true;
            //安捷伦矢网IDN回复
            else if (str_res.IndexOf("8714B") != -1)
            {
                Device_info.USART_info.SW_HeartCnt = 0;//心跳复位
                Device_info.USART_info.connect_NA = true;

                Program.main_form.DrawLine_GPIB_SW(true);
            }
            switch (Device_info.USART_info.Receive_mode)
            { 
                case 1:
                    DataProces.Serial_Test_ing(str_res);//【串行】读取到的是矢网的Bw，CNT，Q，LOSS
                    break;
                case 2:
                    DataProces.Serial_Report_ing(str_res);//【串行】报表数据
                    break;
                case 4:
                    DataProces.Paralle_Test_ing(str_res);//【并行】读取到的是矢网的Bw，CNT，Q，LOSS
                    break;
                case 5:
                    DataProces.Paralle_Report_ing(str_res);//【并行】报表数据
                    break;
            }
        }

        

        #endregion
    }
}
