using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Upper
{
    public static class DataProces
    {
        #region 串行处理函数
        //串行测试-串口接收处理
        public static void Serial_Test_ing(string res)
        {
            string[] arrstr = res.Split(',');//分隔开四个数据
            double Cent = double.Parse(arrstr[1]) / Math.Pow(10, 6);//中心频率换为MHz


            double err = Math.Abs(Cent - (Device_info.Serial_info.CurrenFrq + Device_info.Serial_info.OffSet));
            if (err < 0.1 && Cent > 107 && Cent < 401)//是合适的值
            {
                ListViewItem item = new ListViewItem();
                string seq = (Device_info.Serial_info.sample_cnt + 1).ToString();//序号string化
                string TheoryFrq = (Device_info.Serial_info.CurrenFrq + Device_info.Serial_info.OffSet).ToString("f2") + "MHz";//实际频率string化，保留2位小数
                string RealFeq = Cent.ToString("f2") + "MHz";//中心频率string化，保留2位小数
                string Hex_str;//减去13位

                //窗口中，第二段hex显示依然从0开始
                if (Device_info.Serial_info.hex > 4096)
                    Hex_str = (Device_info.Serial_info.hex - 4096).ToString("X");//Flash值16进制string化
                else
                    Hex_str = Device_info.Serial_info.hex.ToString("X");//Flash值16进制string化
                //显示格式:序号，理论频率，实际频率
                item.SubItems[0].Text = seq; item.SubItems.Add(TheoryFrq); item.SubItems.Add(RealFeq); item.SubItems.Add(Hex_str);//listview添加一行数据
                Device_info.Serial_info.Autotest_Form.listView1.Items.Add(item);

                Device_info.Serial_info.sample_cnt++;//采样序号加1
                Device_info.Serial_info.CurrenFrq += Device_info.Serial_info.step;//增长0.4
                Device_info.Serial_info.ReBack = 0;//回退次数清零
                Program.main_form.progressBar1.Value = Device_info.Serial_info.sample_cnt;

            }
            else if ((Cent - (Device_info.Serial_info.CurrenFrq + Device_info.Serial_info.OffSet)) > 0.1)//实际超过了标准0.1
            {

                Device_info.Serial_info.hex -= 1;//回退1重新测量    

                Device_info.Serial_info.ReBack++;
                if (Device_info.Serial_info.ReBack >= 20)//重新测量多次不行
                {
                    string str = Device_info.Serial_info.CurrenFrq.ToString();
                    MessageBox.Show(str + "值没有找到");
                    Device_info.Serial_info.ReBack = 0;
                    Device_info.Serial_info.CurrenFrq += Device_info.Serial_info.step;//开始下一个吧，这个不管了

                }
                return;
            }
            if (Device_info.Config_info.Config_sta == 2)
            {
                if (err > 10) Device_info.Serial_info.hex += 40;
                if (err > 5) Device_info.Serial_info.hex += 15;
                else if (err > 2) Device_info.Serial_info.hex += 2;
                else Device_info.Serial_info.hex++;
            }
            else if (Device_info.Config_info.Config_sta == 3)
            {
                if (err > 10) Device_info.Serial_info.hex += 20;
                else if (err > 5) Device_info.Serial_info.hex += 7;
                else Device_info.Serial_info.hex++;
            }
        }
        //串行测试-最后处理
        public static void Serial_Test_done()
        {
            StreamWriter hexBao = new StreamWriter(Device_info.DesktopPath + "\\Hex报表.txt");
            StreamWriter hexProgram = new StreamWriter(Device_info.DesktopPath + "\\Hex编程文件.hex");
            UInt16 cnt = 0;
            UInt16 First_hex = 0;
            foreach (ListViewItem item in Device_info.Serial_info.Autotest_Form.listView1.Items)
            {

                string frq = item.SubItems[1].Text;
                string cent = item.SubItems[2].Text;
                string hex = item.SubItems[3].Text;//读取Hex值
                hexBao.Write(frq + " " + cent + " " + hex + "\r\n");//报表文件写入

                UInt16 Hex = Convert.ToUInt16(hex, 16);
                string subFrq = frq.Substring(0, frq.Length - 3);//删除后面的单位
                if (double.Parse(subFrq) > 220)
                    Hex |= 0x2000;//添加13位段选
                else
                    Hex |= 0x1000;//添加13位段选


                byte hex1 = (byte)(Hex / 256);
                byte hex2 = (byte)(Hex % 256);


                byte cnt1 = (byte)(cnt / 256);
                byte cnt2 = (byte)(cnt % 256);


                //计算校验值=（ 2 + 地址 + 0 + 数据）%256 取补码
                byte sum = (byte)((2 + cnt1 + cnt2 + hex1 + hex2) % 256);
                sum = (byte)(~sum + 1);//取补码

                //合成hex
                string temp = ":02" + cnt.ToString("X4") + "00" + Hex.ToString("X4") + sum.ToString("X2");
                hexProgram.Write(temp + "\r\n");//编程文件写入

                if (cnt == 0) First_hex = Hex; //记录第一个数据用于最后填充

                cnt += 2;
            }
            for (; cnt < 0x2000; cnt += 2)
            {

                byte hex1 = (byte)(First_hex / 256);
                byte hex2 = (byte)(First_hex % 256);


                byte cnt1 = (byte)(cnt / 256);
                byte cnt2 = (byte)(cnt % 256);


                //计算校验值=（ 2 + 地址 + 0 + 数据）%256 取补码
                byte sum = (byte)((2 + cnt1 + cnt2 + hex1 + hex2) % 256);
                sum = (byte)(~sum + 1);//取补码


                string temp = ":02" + cnt.ToString("X4") + "00" + First_hex.ToString("X4") + sum.ToString("X2");

                hexProgram.Write(temp + "\r\n");//编程文件写入
                if (cnt + 2 >= 0x2000)//最后一个，再加一个文件结尾，没有回车
                {
                    hexProgram.Write(":00000001FF");

                }

            }
            Mytimer.Autotest_bool = false;//默认先发送hex，再读取
            hexBao.Close();//报表关闭
            hexProgram.Close();//编程文件关闭
        }
        //串行报表-串口接收处理
        public static void Serial_Report_ing(string res)
        {
            if (Device_info.Serial_info.test_step == 2)//收到Bw，CNT，Q，LOSS
            {

                string[] arrstr = res.Split(',');//分隔开四个数据
                Device_info.Serial_info.test_3dB = Math.Round(double.Parse(arrstr[0]) / Math.Pow(10, 6), 2);//-3dB换为MHz
                double Cent = double.Parse(arrstr[1]) / Math.Pow(10, 6);//中心频率换为MHz
                Device_info.Serial_info.test_Loss = Math.Round(double.Parse(arrstr[3]), 2);//LossdB
                //得到频率漂移
                if (Device_info.Serial_info.test_sta == 2) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 108, 2);
                else if (Device_info.Serial_info.test_sta == 3) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 140, 2);
                else if (Device_info.Serial_info.test_sta == 4) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 174, 2);
                else if (Device_info.Serial_info.test_sta == 5) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 225, 2);
                else if (Device_info.Serial_info.test_sta == 6) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 260.2, 2);
                else if (Device_info.Serial_info.test_sta == 7) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 300.2, 2);
                else if (Device_info.Serial_info.test_sta == 8) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 340.2, 2);
                else if (Device_info.Serial_info.test_sta == 9) Device_info.Serial_info.test_FrqErr = Math.Round(Cent - 400.2, 2);

                //             if (Device_info.Serial_info.test_FrqErr < -0.00) Device_info.Serial_info.test_FrqErr = 0;

                //-3dB不能大于17MHz，重新设置
                if (Device_info.Serial_info.test_3dB > 17) Device_info.Serial_info.test_step = 1;
                else Device_info.Serial_info.test_step = 3;
            }
            else if (Device_info.Serial_info.test_step == 4)//读取-40dB的BW
            {
                string[] arrstr = res.Split(',');//分隔开四个数据
                Device_info.Serial_info.test_40dB = Math.Round(double.Parse(arrstr[0]) / Math.Pow(10, 6), 2);//-40dB换为MHz
                Device_info.Serial_info.test_40_30dB = Math.Round(Device_info.Serial_info.test_40dB / Device_info.Serial_info.test_3dB, 2);//计算-40dB/-3dB，保留2位小数

                //-40dB不能小于17MHz，重新设置
                if (Device_info.Serial_info.test_40dB < 17) Device_info.Serial_info.test_step = 3;
                else Device_info.Serial_info.test_step = 5;
            }
            else if (Device_info.Serial_info.test_step == 6)//读取mark1的Y
            {
                string[] arrstr = res.Split(',');//分隔开四个数据
                Device_info.Serial_info.test_standWave = Math.Round(double.Parse(res), 2);//Y值
                Device_info.Serial_info.test_step = 7;
            }
            else if (Device_info.Serial_info.test_step == 8)
            {
                string[] arrstr = res.Split(',');//分隔开四个数据
                double temp = Math.Round(double.Parse(arrstr[0]) / Math.Pow(10, 6), 2);//-3dB换为MHz

                //-3dB不能大于17MHz，重新设置
                if (temp > 17) Device_info.Serial_info.test_step = 7;
                else Device_info.Serial_info.test_step = 9;
            }
        }
        #endregion

        #region 并行处理函数
        //并行测试-串口接收处理
        public static int ParaAutotest_my = 4;//我增加20191005
        public static void Paralle_Test_ing(string res)
        {
            string[] arrstr = res.Split(',');//分隔开四个数据

            double Cent = Math.Round(double.Parse(arrstr[1]) / Math.Pow(10, 6), 2);//中心频率换为MHz
            if (Device_info.Parall_info.sample_cnt == 0) Device_info.Parall_info.preFrq = Cent;
            else
            {
                if (Math.Abs(Cent - Device_info.Parall_info.preFrq) < 0.05 && (ParaAutotest_my > 0))//最多再读4次 原为0.01我改为0.05，第二个与条件，我增加
                {
                    Mytimer.ParaAutotest_bool = false;
                    ParaAutotest_my = ParaAutotest_my - 1;// 我增加
                    return;
                }
                else//更新历史值
                {
                    Device_info.Parall_info.preFrq = Cent;

                    ParaAutotest_my = 4; //我增加20191005
                }


            }



            double Loss = Math.Round(double.Parse(arrstr[3]), 2);

            //存储在内存之中
            Device_info.AutoTest_Datas_t temp = new Device_info.AutoTest_Datas_t();
            temp.Seq = Device_info.Parall_info.sample_cnt + 1;
            temp.Frq = Cent;
            temp.Loss = Loss;
            temp.Hex = Device_info.Parall_info.hex;
            Device_info.Parall_info.AutoTest_datas.Add(temp);

            //存储在窗体控件中

            ListViewItem item = new ListViewItem();
            string seq = (Device_info.Parall_info.sample_cnt + 1).ToString();//序号string化

            string Hex_str = "";
            //窗口中，第二段hex显示依然从0开始
            if (Device_info.Serial_info.hex >= 1024)
                Hex_str = (Device_info.Parall_info.hex - 1024).ToString("X");//Flash值16进制string化
            else
                Hex_str = Device_info.Parall_info.hex.ToString("X");//Flash值16进制string化
            //显示格式:序号，理论频率，实际频率
            item.SubItems[0].Text = seq;
            item.SubItems.Add(Device_info.Parall_info.AutoTest_datas[Device_info.Parall_info.sample_cnt].Frq.ToString() + "MHz");
            item.SubItems.Add(Device_info.Parall_info.AutoTest_datas[Device_info.Parall_info.sample_cnt].Loss.ToString());
            item.SubItems.Add(Device_info.Parall_info.AutoTest_datas[Device_info.Parall_info.sample_cnt].Hex.ToString("X"));
            Device_info.Parall_info.Autotest_resForm.listView1.Items.Add(item);




            //进度条
            Program.main_form.progressBar2.Value = Device_info.Parall_info.sample_cnt;

            Device_info.Parall_info.hex++;
            Device_info.Parall_info.sample_cnt++;

            //MessageBox.Show("【中心频率】" + Cent.ToString() + "【Loss】" + arrstr[3]);
            //
            //if (Device_info.Parall_info.sample_cnt >= 100)//自动测试完毕
            //{
            //    //Autotest_DataProcess();//数据处理


            //    Device_info.Receive_mode = 0;//没有自动测试数据要接收
            //    progressBar2.Value = 0;
            //    timer_paraAuotest.Stop();
            //    timer_heart.Start();//开启心跳定时器
            //}
        }
        //并行测试-最后处理
        public static void Paralle_Test_done()
        {
            //【调试】
            StreamWriter file1 = new StreamWriter(Device_info.DesktopPath + "\\调试数据.txt");

            //数据样本排序
            Device_info.Parall_info.AutoTest_datas.Sort((x, y) => { return x.Frq.CompareTo(y.Frq); });

            for (int i = 0; i < Device_info.Parall_info.AutoTest_datas.Count; i++)
                file1.Write(Device_info.Parall_info.AutoTest_datas[i].Seq.ToString() + " "
                              + Device_info.Parall_info.AutoTest_datas[i].Frq.ToString() + " "
                              + Device_info.Parall_info.AutoTest_datas[i].Loss.ToString() + "\r\n");
            file1.Close();





            //数据样本排序
            // Device_info.Parall_info.AutoTest_datas.Sort((x, y) => { return x.Frq.CompareTo(y.Frq); }); 调试注释掉一行；20191007

            List<Device_info.AutoTest_Datas_t> BestData = new List<Device_info.AutoTest_Datas_t>();//存储最终结果
            double deltaFrq = 0;//用于判别合适频率的范围
            Device_info.Parall_info.CurrenFrq = Device_info.Parall_info.CentFrq1;
            for (int i = 0; i < 502; i++)//共502个点
            {
                List<Device_info.AutoTest_Datas_t> BadFrqdelta_ = new List<Device_info.AutoTest_Datas_t>();//找不到合适频率，存储频率差值,左侧
                List<Device_info.AutoTest_Datas_t> BadFrqdelta = new List<Device_info.AutoTest_Datas_t>();//找不到合适频率，存储频率差值,右侧
                List<Device_info.AutoTest_Datas_t> Frqsuitable = new List<Device_info.AutoTest_Datas_t>();//找到合适频率，存储合适的频率的数据

                for (int j = 0; j < Device_info.Parall_info.AutoTest_datas.Count; j++)//一个频点遍历2048
                {
                    if (Device_info.Parall_info.CurrenFrq >= 108 && Device_info.Parall_info.CurrenFrq <= 135) deltaFrq = 0.1;
                    else if (Device_info.Parall_info.CurrenFrq > 135 && Device_info.Parall_info.CurrenFrq <= 174) deltaFrq = 0.2;//低频在0.1MHz内
                    else if (Device_info.Parall_info.CurrenFrq >= 225 && Device_info.Parall_info.CurrenFrq <= 300) deltaFrq = 0.2;
                    else if (Device_info.Parall_info.CurrenFrq > 300 && Device_info.Parall_info.CurrenFrq <= 400) deltaFrq = 0.3;

                    if (Math.Abs(Device_info.Parall_info.AutoTest_datas[j].Frq - Device_info.Parall_info.CurrenFrq) < deltaFrq)//频率差值在△以内
                    {
                        Device_info.AutoTest_Datas_t temp = new Device_info.AutoTest_Datas_t();
                        temp.Seq = Device_info.Parall_info.AutoTest_datas[j].Seq;
                        temp.Frq = Device_info.Parall_info.AutoTest_datas[j].Frq;
                        temp.Loss = Math.Abs(Device_info.Parall_info.AutoTest_datas[j].Loss); ;
                        temp.Hex = Device_info.Parall_info.AutoTest_datas[j].Hex;
                        Frqsuitable.Add(temp);

                    }

                }
                if (Frqsuitable.Count == 0)//没有找到合适的值
                {
                    for (int j = 0; j < Device_info.Parall_info.AutoTest_datas.Count; j++)//各个点对该点求差值
                    {

                        Device_info.AutoTest_Datas_t temp = new Device_info.AutoTest_Datas_t();

                        temp.Seq = Device_info.Parall_info.AutoTest_datas[j].Seq;
                        temp.Frq = Device_info.Parall_info.AutoTest_datas[j].Frq - Device_info.Parall_info.CurrenFrq;

                        if (temp.Frq > 0)
                            BadFrqdelta.Add(temp);//右侧统计
                        else
                            BadFrqdelta_.Add(temp);//左侧统计

                    }


                    BadFrqdelta.Sort((x, y) => { return x.Frq.CompareTo(y.Frq); });//右侧统计的数据排序
                    BadFrqdelta_.Sort((x, y) => { return y.Frq.CompareTo(x.Frq); });//左侧统计的数据排序
                    int BestSeq = 0;
                    if (Device_info.Parall_info.CurrenFrq >= 107.99 && Device_info.Parall_info.CurrenFrq <= 174.01)//第一段
                    {
                        if ((Device_info.Parall_info.CurrenFrq >= 149.99 && Device_info.Parall_info.CurrenFrq <= 174.01)
                            && (Math.Abs(BadFrqdelta[0].Frq - Math.Abs(BadFrqdelta_[0].Frq))) < 0.2)//范围0.2内选左侧的
                            BestSeq = BadFrqdelta_[0].Seq;
                        else//范围外间隔最小的
                        {
                            if (Math.Abs(BadFrqdelta[0].Frq) > Math.Abs(BadFrqdelta_[0].Frq))
                                BestSeq = BadFrqdelta_[0].Seq;
                            else
                                BestSeq = BadFrqdelta[0].Seq;
                        }

                    }
                    else if (Device_info.Parall_info.CurrenFrq >= 224.99 && Device_info.Parall_info.CurrenFrq <= 400.01)//第二段范围为0.3
                    {
                        if ((Device_info.Parall_info.CurrenFrq >= 349.99 && Device_info.Parall_info.CurrenFrq <= 400.01) &&
                            (Math.Abs(BadFrqdelta[0].Frq - Math.Abs(BadFrqdelta_[0].Frq))) < 0.3)//范围内选左侧的
                            BestSeq = BadFrqdelta_[0].Seq;
                        else//范围外间隔最小的
                        {
                            if (Math.Abs(BadFrqdelta[0].Frq) > Math.Abs(BadFrqdelta_[0].Frq))
                                BestSeq = BadFrqdelta_[0].Seq;
                            else
                                BestSeq = BadFrqdelta[0].Seq;
                        }
                    }
                    for (int h = 0; h < 2048; h++)//通过序号找到这个最佳值
                    {
                        if (Device_info.Parall_info.AutoTest_datas[h].Seq == BestSeq)
                        {
                            Device_info.AutoTest_Datas_t temp = new Device_info.AutoTest_Datas_t();
                            temp.Seq = Device_info.Parall_info.AutoTest_datas[h].Seq;
                            temp.Frq = Device_info.Parall_info.AutoTest_datas[h].Frq;
                            temp.Loss = Device_info.Parall_info.AutoTest_datas[h].Loss;
                            temp.Hex = Device_info.Parall_info.AutoTest_datas[h].Hex;

                            BestData.Add(temp);
                            break;
                        }
                    }

                }
                else//找到合适的值,选loss最低的
                {
                    Frqsuitable.Sort((x, y) => { return x.Loss.CompareTo(y.Loss); });

                    for (int h = 0; h < 2048; h++)
                    {
                        if (Device_info.Parall_info.AutoTest_datas[h].Seq == Frqsuitable[0].Seq)
                        {
                            Device_info.AutoTest_Datas_t temp = new Device_info.AutoTest_Datas_t();
                            temp.Seq = Device_info.Parall_info.AutoTest_datas[h].Seq;
                            temp.Frq = Device_info.Parall_info.AutoTest_datas[h].Frq;
                            temp.Loss = Device_info.Parall_info.AutoTest_datas[h].Loss;
                            temp.Hex = Device_info.Parall_info.AutoTest_datas[h].Hex;

                            BestData.Add(temp);
                            break;
                        }
                    }

                }


                //下一个比较的频率值
                if (i < 250) Device_info.Parall_info.CurrenFrq += Device_info.Parall_info.step1;
                else if (i == 250) Device_info.Parall_info.CurrenFrq = Device_info.Parall_info.CentFrq2;
                else if (i < 502) Device_info.Parall_info.CurrenFrq += Device_info.Parall_info.step2;

            }

            //显示筛选结果
            StreamWriter filewrite = new StreamWriter(Device_info.DesktopPath + "\\并行下载数据.txt");
            Form3 fm4 = new Form3();
            UInt16 BestDataidx = 0;
            for (int i = 0; i < 512; i++)
            {
                ListViewItem item = new ListViewItem();


                if (i <= 250)
                {
                    item.SubItems[0].Text = BestData[BestDataidx].Seq.ToString();
                    item.SubItems.Add(BestData[BestDataidx].Frq.ToString() + "MHz");
                    item.SubItems.Add(BestData[BestDataidx].Loss.ToString());
                    item.SubItems.Add(BestData[BestDataidx].Hex.ToString("X"));
                    fm4.listView1.Items.Add(item);

                    filewrite.Write((i + 1).ToString() + ":" + " "
                                    + BestData[BestDataidx].Frq.ToString() + "MHz" + " "
                                    + BestData[BestDataidx].Loss.ToString() + " "
                                    + (BestData[BestDataidx].Hex + 0x1400).ToString("X"));
                    BestDataidx++;
                }
                else if (i >= 251 && i <= 255)
                {
                    filewrite.Write((i + 1).ToString() + ":" + " "
                                    + "MaxMHz" + " "
                                    + "----" + " "
                                    + "17FF");
                }
                else if (i >= 256 && i <= 506)
                {
                    item.SubItems[0].Text = BestData[BestDataidx].Seq.ToString();
                    item.SubItems.Add(BestData[BestDataidx].Frq.ToString() + "MHz");
                    item.SubItems.Add(BestData[BestDataidx].Loss.ToString());
                    item.SubItems.Add(BestData[BestDataidx].Hex.ToString("X"));
                    fm4.listView1.Items.Add(item);

                    filewrite.Write((i + 1).ToString() + ":" + " "
                                       + BestData[BestDataidx].Frq.ToString() + "MHz" + " "
                                       + BestData[BestDataidx].Loss.ToString() + " "
                                       + (BestData[BestDataidx].Hex + 0x2400).ToString("X"));
                    BestDataidx++;
                }
                else if (i >= 507 && i <= 511)
                {
                    filewrite.Write((i + 1).ToString() + ":" + " "
                                    + "MaxMHz" + " "
                                    + "----" + " "
                                    + "2BFF");
                }

                if (i < 511) filewrite.Write("\r\n");

            }
            filewrite.Close();
            fm4.Show();

        }
        //并行报表-串口接收处理
        public static void Paralle_Report_ing(string res)
        {
            if (Device_info.Parall_info.test_step == 2)
            {

                string[] arrstr = res.Split(',');//分隔开四个数据

                Device_info.Parall_info.test_3d = Math.Round(Convert.ToDouble(arrstr[0]) / Math.Pow(10, 6), 2);//3dB
                Device_info.Parall_info.test_Frq = Math.Round(Convert.ToDouble(arrstr[1]) / Math.Pow(10, 6), 2);//Frq
                Device_info.Parall_info.test_Loss = Math.Round(Convert.ToDouble(arrstr[3]), 2);//Loss

                Device_info.Parall_info.test_step = 3;
            }
            else if (Device_info.Parall_info.test_step == 3)
            {
                string[] arrstr = res.Split(',');//分隔开四个数据
                Device_info.Parall_info.test_Standwave = Math.Round(double.Parse(res), 2);//Y值
                Device_info.Parall_info.test_step = 4;
            }
            else if (Device_info.Parall_info.test_step == 7)
            {


                Device_info.Parall_info.neg5 = Convert.ToDouble(res);
                Device_info.Parall_info.neg5 -= Device_info.Parall_info.test_Loss;
                Device_info.Parall_info.neg5 = Math.Round(Device_info.Parall_info.neg5, 2);
                Device_info.Parall_info.test_step = 8;
            }
            else if (Device_info.Parall_info.test_step == 9)
            {


                Device_info.Parall_info.neg10 = Convert.ToDouble(res);
                Device_info.Parall_info.neg10 -= Device_info.Parall_info.test_Loss;
                Device_info.Parall_info.neg10 = Math.Round(Device_info.Parall_info.neg10, 2);
                Device_info.Parall_info.test_step = 10;
            }
            else if (Device_info.Parall_info.test_step == 11)
            {


                Device_info.Parall_info.pos5 = Convert.ToDouble(res);
                Device_info.Parall_info.pos5 -= Device_info.Parall_info.test_Loss;
                Device_info.Parall_info.pos5 = Math.Round(Device_info.Parall_info.pos5, 2);
                Device_info.Parall_info.test_step = 12;
            }
            else if (Device_info.Parall_info.test_step == 13)
            {


                Device_info.Parall_info.pos10 = Convert.ToDouble(res);
                Device_info.Parall_info.pos10 -= Device_info.Parall_info.test_Loss;
                Device_info.Parall_info.pos10 = Math.Round(Device_info.Parall_info.pos10, 2);
                Device_info.Parall_info.test_step = 14;
            }
            
        }
          #endregion


        #region USB接收处理函数
        //USB信息处理
        public static void USB_Progres(MessagsData messagsData)
        {
           switch(messagsData.FuncByte)
           {
               case 0x02://接收到擦除信号
                   if (messagsData.data[0] == 0xFF)
                   {
                    Program.main_form.label_erase.Text = "×";
                    Program.main_form.label_erase.ForeColor = Color.Red;
                   }
                   else if (messagsData.data[0] == 0x00)
                   {
                    Program.main_form.label_erase.Text = "√";
                    Program.main_form.label_erase.ForeColor = Color.Green;
                   }
                   break;
               case 0x04://接收到校验数据
                   if (messagsData.data[0] == 0xAA)
                    {

                        Device_info.Parall_info.CaliData.Clear();
                    }
                    else if (messagsData.data[0] == 0xBB)
                    {
                        UInt16 hex1 = (UInt16)(messagsData.data[1] << 8 | messagsData.data[2]);
                        UInt16 hex2 = (UInt16)(messagsData.data[3] << 8 | messagsData.data[4]);

                        if (hex1 != 0xFFFF) Device_info.Parall_info.CaliData.Add(hex1);//增加if判断，忽略最后的两个OXffff  20191016
                        if (hex2 != 0xFFFF) Device_info.Parall_info.CaliData.Add(hex2); //增加if判断，忽略最后的两个OXffff
                        Program.main_form.progressBar3.Value = Program.main_form.progressBar3.Maximum * (Device_info.Parall_info.CaliData.Count) / 2048; // 动态数据后进度条,用的最长值，没有动态调整

                    }
                    else if (messagsData.data[0] == 0xCC)
                    {
                        Program.main_form.progressBar3.Value = 1000;
                        Program.main_form.richTextBox1.Text = "";
                        //显示
                        for (int i = 0; i < Device_info.Parall_info.CaliData.Count; i++)
                        {
                            Program.main_form.richTextBox1.Text += "(" + (i + 1).ToString() + ") " + Device_info.Parall_info.CaliData[i].ToString("X");
                            if (i != Device_info.Parall_info.CaliData.Count - 1) Program.main_form.richTextBox1.Text += "\r\n";
                            Program.main_form.progressBar3.Value = 1000;
                        }
                        //验证
                        StreamReader CaliFile = new StreamReader(Device_info.Parall_info.DownloadFilePath);
                        int j;
                        for (j = 0; j < Device_info.Parall_info.CaliData.Count; j++)
                        {
                            Program.main_form.progressBar3.Value = 1000;
                            try
                            {
                                string[] strs = CaliFile.ReadLine().Split(' ');
                                UInt16 hex = Convert.ToUInt16(strs[3], 16);
                                if (Device_info.Parall_info.CaliData[j] != hex) break;

                            }
                            catch
                            {
                                break;
                            }


                        }
                        CaliFile.Close();

                        if (j == Device_info.Parall_info.CaliData.Count)
                        {
                            Program.main_form.label_cali.Text = "√";
                            Program.main_form.label_cali.ForeColor = Color.Green;
                            Program.main_form.progressBar3.Value = 0;

                        }
                        else
                        {
                            Program.main_form.label_cali.Text = "×";
                            Program.main_form.label_cali.ForeColor = Color.Red;
                            Program.main_form.progressBar3.Value = 0;

                        }

                    }
                   break;

               default:
                   break;
              }

           }
            

        

        #endregion
    }
}
