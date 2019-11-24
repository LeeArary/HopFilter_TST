using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using SP_DEVICE_INTERFACE_DATA = Upper.WindowsAPI.SP_DEVICE_INTERFACE_DATA;
using SP_DEVICE_INTERFACE_DETAIL_DATA = Upper.WindowsAPI.SP_DEVICE_INTERFACE_DETAIL_DATA;
using System.Windows.Forms;

namespace Upper
{
    public class USBHID
    {
        private int outputReportLength;
        private int inputReportLength;

        private FileStream hidDevice;
        private const int MAX_USB_DEVICES = 64;
        private int Hid_Hand;
        WindowsAPI windowsApi = new WindowsAPI();

        //设备列表
        private List<String> deviceList = new List<string>();
        
        public List<String> GetDeviceList()
        {
            deviceList.Clear();
            GetDeviceList(ref deviceList);//刷新一次设备
            return deviceList;//返回给外部需要的变量
        }

        public USBHID()//构造函数
        {
            GetDeviceList(ref deviceList);//刷新一次设备
        }
        private IntPtr tmp;
        public bool OpenUSBHid(string deviceStr)
        {
            //创建，打开设备文件
            //马甲，就是调用CreateFile()
            //返回，设备句柄
            IntPtr device = windowsApi.CreateDeviceFile(deviceStr);
            if (device == new IntPtr(-1))
                return false;
            tmp = device;
            Hid_Hand = (int)device;
         
            HIDD_ATTRIBUTES attributes;
            //获取PID，VID
            windowsApi.GETDeviceAttribute(device, out attributes);

            
            //获取报文长度
            IntPtr preparseData;
            HIDP_CAPS caps;
            windowsApi.GetPreparseData(device, out preparseData);
            windowsApi.GetCaps(preparseData, out caps);
            windowsApi.FreePreparseData(preparseData);
            outputReportLength = caps.OutputReportByteLength;//9
            inputReportLength = caps.InputReportByteLength;//9

            //最终得到设备句柄
            hidDevice = new FileStream(new SafeFileHandle(device, false),//非安全访问
                                        FileAccess.ReadWrite, //拥有读写权限
                                        inputReportLength, 
                                        true);
            BeginAsyncRead();
            return true;
        }

        private void BeginAsyncRead()//准备开始报告。异步操作完成，调用ReadCompleted处理
        {
            byte[] inputBuff = new byte[inputReportLength];
            hidDevice.BeginRead(inputBuff, 0, inputReportLength, new AsyncCallback(ReadCompleted), inputBuff);
            //hidDevice.BeginWrite
        }
        private void ReadCompleted(IAsyncResult iResult)//回调函数
        {
            byte[] readBuff = (byte[])(iResult.AsyncState);
            if (readBuff[1] == 0) return;//没有发送帧头为0x00的数据，这代表了设备异常拔出

            hidDevice.EndRead(iResult);//读取结束,如果读取错误就会产生一个异常
            byte[] reportData = new byte[readBuff.Length - 1];
            for (int i = 1; i < readBuff.Length; i++)//提取数据
                reportData[i - 1] = readBuff[i];
            DataProcess(reportData);
            BeginAsyncRead();//启动下一次读操作
        }
        private void DataProcess(byte[] data)//下位机发送的数据处理
        {
            //仅检查帧头，帧尾。不做其他处理
            if (data[0] == 0xFA && data[7] == 0xFA)
            {
                MessagsData messagsData = new MessagsData();
                messagsData.data = new byte[7];
                messagsData.FuncByte = data[1];//功能帧
                messagsData.data[0] = data[2];//数据1
                messagsData.data[1] = data[3];//数据2
                messagsData.data[2] = data[4];//数据3
                messagsData.data[3] = data[5];//数据4
                messagsData.data[4] = data[6];//数据5
                //messagsData.data[5] = data[7];//数据6
                //messagsData.data[6] = data[8];//数据7

                DataProces.USB_Progres(messagsData);
                 // MyMessage Note = new MyMessage();
                //Note.SendMsgToMainForm(MyMessage.DATARECIVE_MESSAGE, messagsData);
            }

            
            
        }

        public void CloseDevice()
        {

            // WindowsAPI.SetupDiDestroyDeviceInfoList(tmp);
            //EventArgs ex = new EventArgs();
           // OnDeviceRemoved(ex);//发出设备移除消息
            //hidDevice.Close();
            //hidDevice.Close();
            //Hid_Hand
            //WindowsAPI.CloseHandle(Hid_Hand);
            
        }

        ///DeviceList获取
        private void GetDeviceList(ref List<string> deviceList)
        {
            Guid HIDGuid = Guid.Empty;
            //获取HID的全局GUID  
            windowsApi.GetDeviceGuid(ref HIDGuid);

            //马甲，就是调用SetupDiGetClassDevs()函数
            //获取所有HID设备的接口集合
            IntPtr HIDInfoSet = windowsApi.GetClassDevOfHandle(HIDGuid);
           
            if (HIDInfoSet != IntPtr.Zero)
            {
                SP_DEVICE_INTERFACE_DATA interfaceInfo = new SP_DEVICE_INTERFACE_DATA();
                interfaceInfo.cbSize = Marshal.SizeOf(interfaceInfo);

                //遍历所有设备
                for (uint index = 0; index < MAX_USB_DEVICES; index++)
                {
                    
                    
                    ///按照index自增，依次访问每个设备接口
                    if (!windowsApi.GetEnumDeviceInterfaces(HIDInfoSet, ref HIDGuid, index, ref interfaceInfo))
                        continue;

                    int buffsize = 0;

                    //获取接口详细信息；第一次读取错误，但可取得信息缓冲区的大小
                    //详细信息交给SP_DEVICE_INTERFACE_DATA结构体，即interfaceInfo
                    windowsApi.GetDeviceInterfaceDetail(HIDInfoSet, ref interfaceInfo, IntPtr.Zero, ref buffsize);

                    //第一次接受缓冲
                    IntPtr pDetail = Marshal.AllocHGlobal(buffsize);
                    SP_DEVICE_INTERFACE_DETAIL_DATA detail = new WindowsAPI.SP_DEVICE_INTERFACE_DETAIL_DATA();
                    detail.cbSize = Marshal.SizeOf(typeof(Upper.WindowsAPI.SP_DEVICE_INTERFACE_DETAIL_DATA));
                    Marshal.StructureToPtr(detail, pDetail, false);

                    //第二次接受缓冲，返回为true则添加设备到deviceList
                    if (windowsApi.GetDeviceInterfaceDetail(HIDInfoSet, ref interfaceInfo, pDetail, ref buffsize))//第二次读取接口详细信息
                        deviceList.Add(Marshal.PtrToStringAuto((IntPtr)((int)pDetail + 4)));
                    
                    //释放pDetail
                    Marshal.FreeHGlobal(pDetail);
                }
            }

            //删除HID设备接口集合
            windowsApi.DestroyDeviceInfoList(HIDInfoSet);
        }

    
        internal static string ByteToHexString(byte[] p)
        {
            string str = string.Empty;
            if (p != null)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    str += p[i].ToString("X2");
                }
            }
            return str;
        }

        //发送报告
        internal void sendReport(byte[] array)
        {
            byte[] arrays=new byte[9];
            arrays[0]=0;//原有数据，加上报告ID
            for (int i = 1; i <= array.Length; i++)
                arrays[i] = array[i - 1];
            hidDevice.Write(arrays, 0, arrays.Length);
        }
    }

    //设备VID，PID属性数据结构
    public struct HIDD_ATTRIBUTES
    {
        public int Size;
        public ushort VendorID;
        public ushort ProductID;
        public ushort VersionNumber;
    }

    public struct HIDP_CAPS
    {
        public ushort Usage;
        public ushort UsagePage;
        public ushort InputReportByteLength;
        public ushort OutputReportByteLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
        public ushort[] Reserved;
        public ushort NumberLinkCollectionNodes;
        public ushort NumberInputButtonCaps;
        public ushort NumberInputValueCaps;
        public ushort NumberInputDataIndices;
        public ushort NumberOutputButtonCaps;
        public ushort NumberOutputValueCaps;
        public ushort NumberOutputDataIndices;
        public ushort NumberFeatureButtonCaps;
        public ushort NumberFeatureValueCaps;
        public ushort NumberFeatureDataIndices;
    }
    //报告的数据结构，调试
    public class report : EventArgs
    {
        public readonly byte reportID;
        public readonly byte[] reportBuff;
        public report(byte id, byte[] arrayBuff)
        {
            reportID = id;
            reportBuff = arrayBuff;
        }
    }
}
