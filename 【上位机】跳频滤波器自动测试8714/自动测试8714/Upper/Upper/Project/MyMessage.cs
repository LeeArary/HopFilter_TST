using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace Upper
{
    class MyMessage
    {
        //声明 API 函数 
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern IntPtr SendMessage(int hWnd, int msg, int wParam, ref MessagsData lParam);
          
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);
        
         //定义消息常数 
        public const int DATARECIVE_MESSAGE = 0X400 + 2;//
 
        
       //向窗体发送消息的函数 
        public void SendMsgToMainForm(int MSG,MessagsData messagsData)
        {
            int WINDOW_HANDLER = FindWindow(null, "STM32自动测试系统");
            if (WINDOW_HANDLER == 0)
            {
                MessageBox.Show("未找到窗口发送消息", "", MessageBoxButtons.OK);
            }


            SendMessage(WINDOW_HANDLER, MSG, 0, ref messagsData);
        }
    }
}
