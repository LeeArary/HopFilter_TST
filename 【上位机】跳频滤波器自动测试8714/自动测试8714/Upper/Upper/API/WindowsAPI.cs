using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Upper
{
    public class WindowsAPI
    {

        //获取HID设备的全局GUID4
        [DllImport("hid.dll")]
        private static extern void HidD_GetHidGuid(ref Guid HidGuid);


        //获取设备接口信息集合
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(ref Guid ClassGuid, uint Enumerator,   IntPtr HwndParent, USBHIDEnum.DIGCF Flags);


        //粗略获取设备接口信息，单个
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr deviceInfoSet, IntPtr deviceInfoData, ref Guid interfaceClassGuid, UInt32 memberIndex, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        //结构体
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid interfaceClassGuid;
            public int flags;
            public int reserved;
        }


        //详细获取设备接口信息，单个
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData, IntPtr deviceInterfaceDetailData, int deviceInterfaceDetailDataSize, ref int requiredSize, SP_DEVINFO_DATA deviceInfoData);

        
        //结构体
        [StructLayout(LayoutKind.Sequential)]
        public class SP_DEVINFO_DATA
        {
            public int cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
            public Guid classGuid = Guid.Empty; // temp
            public int devInst = 0; // dumy
            public int reserved = 0;
        }

        //结构体
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            internal int cbSize;
            internal short devicePath;
        }

        //释放设备接口集合
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Boolean SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        
        [DllImport("kernel32.dll", SetLastError = true)]
        //开启设备通信
        static extern IntPtr CreateFile(
       string FileName,                // 文件名
       uint DesiredAccess,             // 访问模式
       uint ShareMode,                 // 共享模式
       uint SecurityAttributes,        // 安全属性
       uint CreationDisposition,       // 如何创建
       uint FlagsAndAttributes,        // 文件属性
       int hTemplateFile               // 模板文件的句柄
       );


        [DllImport("hid.dll")]
        private static extern Boolean HidD_GetAttributes(IntPtr hidDeviceObject, out HIDD_ATTRIBUTES attributes);


        [DllImport("hid.dll")]
        private static extern Boolean HidD_GetPreparsedData(IntPtr hidDeviceObject, out IntPtr PreparsedData);

        [DllImport("hid.dll")]
        private static extern uint HidP_GetCaps(IntPtr PreparsedData, out HIDP_CAPS Capabilities);

        [DllImport("hid.dll")]
        private static extern Boolean HidD_FreePreparsedData(IntPtr PreparsedData);

        [DllImport("kernel32.dll")]
        internal static extern int CloseHandle(int hObject);

        //获取HID设备全局GUID，注意是全局GUID
        internal void GetDeviceGuid(ref Guid HIDGuid)
        {
            HidD_GetHidGuid(ref HIDGuid);
        }


        //通过HID全局GUID，获取所有设备的接口集合HIDInfoSet
        internal IntPtr GetClassDevOfHandle(Guid HIDGuid)
        {
            return SetupDiGetClassDevs(ref HIDGuid, 0, IntPtr.Zero, USBHIDEnum.DIGCF.DIGCF_PRESENT | USBHIDEnum.DIGCF.DIGCF_DEVICEINTERFACE);
        }

        //释放设备接口集合HIDInfoSet
        internal void DestroyDeviceInfoList(IntPtr HIDInfoSet)
        {
            SetupDiDestroyDeviceInfoList(HIDInfoSet);
        }


        //粗略获取设备接口，单个设备,用index依次访问
        internal bool GetEnumDeviceInterfaces(IntPtr HIDInfoSet, ref Guid HIDGuid, uint index, ref SP_DEVICE_INTERFACE_DATA interfaceInfo)
        {
            return SetupDiEnumDeviceInterfaces(HIDInfoSet, IntPtr.Zero, ref HIDGuid, index, ref interfaceInfo);
        }

        //详细获取设备接口，单个设备
        //第一次缓冲
        //第二次缓冲，true则列入DeviceListS
        internal bool GetDeviceInterfaceDetail(IntPtr HIDInfoSet, ref SP_DEVICE_INTERFACE_DATA interfaceInfo, IntPtr pDetail, ref int buffsize)
        {
            return SetupDiGetDeviceInterfaceDetail(HIDInfoSet, ref interfaceInfo, pDetail, buffsize, ref buffsize, null);
        }

    

        //开启通信
        internal IntPtr CreateDeviceFile(string device)
        {
            return CreateFile(device,//文件名，在DeviceList中
                              DESIREDACCESS.GENERIC_READ | DESIREDACCESS.GENERIC_WRITE,//访问模式，读和写
                              0, //共享模式
                              0, //安全属性
                              CREATIONDISPOSITION.OPEN_EXISTING,//打开方式
                              FLAGSANDATTRIBUTES.FILE_FLAG_OVERLAPPED,//设备属性
                              0//模板文件句柄，这里没有模板
                              );
        }

        //宏-设备属性
        static class FLAGSANDATTRIBUTES
        {
            public const uint FILE_FLAG_WRITE_THROUGH = 0x80000000;
            public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
            public const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
            public const uint FILE_FLAG_RANDOM_ACCESS = 0x10000000;
            public const uint FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000;
            public const uint FILE_FLAG_DELETE_ON_CLOSE = 0x04000000;
            public const uint FILE_FLAG_BACKUP_SEMANTICS = 0x02000000;
            public const uint FILE_FLAG_POSIX_SEMANTICS = 0x01000000;
            public const uint FILE_FLAG_OPEN_REPARSE_POINT = 0x00200000;
            public const uint FILE_FLAG_OPEN_NO_RECALL = 0x00100000;
            public const uint FILE_FLAG_FIRST_PIPE_INSTANCE = 0x00080000;
        }

        //宏-打开方式
        static class CREATIONDISPOSITION
        {
            public const uint CREATE_NEW = 1;
            public const uint CREATE_ALWAYS = 2;
            public const uint OPEN_EXISTING = 3;
            public const uint OPEN_ALWAYS = 4;
            public const uint TRUNCATE_EXISTING = 5;
        }

        //宏-访问方式
        static class DESIREDACCESS
        {
            public const uint GENERIC_READ = 0x80000000;
            public const uint GENERIC_WRITE = 0x40000000;
            public const uint GENERIC_EXECUTE = 0x20000000;
            public const uint GENERIC_ALL = 0x10000000;
        }

        /// 获取设备属性
        /// VID，PID
        internal void GETDeviceAttribute(IntPtr device, out HIDD_ATTRIBUTES attributes)
        {
            HidD_GetAttributes(device, out attributes);
        }

  
        internal void GetPreparseData(IntPtr device, out IntPtr preparseData)
        {
            HidD_GetPreparsedData(device, out preparseData);
        }


        internal void GetCaps(IntPtr preparseData, out HIDP_CAPS caps)
        {
            HidP_GetCaps(preparseData, out caps);
        }

        //释放PreparseData的临时指针
        internal void FreePreparseData(IntPtr preparseData)
        {
            HidD_FreePreparsedData(preparseData);
        }
    }
}
