using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Upper
{
    class USBHIDEnum
    {
        /// <summary>
        /// Flags controlling what is included in the device information set built by SetupDiGetClassDevs
        /// </summary>
        public enum DIGCF
        {
            DIGCF_DEFAULT = 0x00000001, // only valid with DIGCF_DEVICEINTERFACE                 
            DIGCF_PRESENT = 0x00000002,
            DIGCF_ALLCLASSES = 0x00000004,
            DIGCF_PROFILE = 0x00000008,
            DIGCF_DEVICEINTERFACE = 0x00000010
        }
         
        /// <summary>
        /// HID STATUS
        /// </summary>
        public enum HID_STATUS
        {
            SUCCESS,
            NO_DEVICE,
            NO_FIND,
            OPEND,
            WRITE_FAID,
            READ_FAID
        }
    }
}
