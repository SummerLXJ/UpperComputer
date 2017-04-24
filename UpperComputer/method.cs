using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Threading;
using System.Drawing;

namespace UpperComputer
{
    class Method
    {
        //public static Method method = new Method();
        public void load_data_to_datagridview(DataGridView dataGridView, byte[] tc_data)
        {
            dataGridView.Rows.Clear();
            string[] str_tc = new string[17];
            for (int kk = 0; kk < 16; kk++)
            {
                str_tc[kk + 1] = " ";
            }
            if (tc_data.Length % 16 == 0)
            {

                for (int i = 0; i < tc_data.Length / 16; i++)
                {
                    str_tc[0] = i.ToString("x2");
                    dataGridView.Rows.Add(str_tc);
                    for (int j = 1; j < str_tc.Length; j++)
                    {
                        dataGridView.Rows[i].Cells[0].Value = str_tc[0];
                        dataGridView.Rows[i].Cells[j].Value = tc_data[i * 16 + j - 1].ToString("x2");
                        dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                    }

                }
            }
            else
            {
                for (int i = 0; i < tc_data.Length / 16 + 1; i++)
                {
                    str_tc[0] = i.ToString("x2");
                    dataGridView.Rows.Add(str_tc);
                    if (i < tc_data.Length / 16)
                    {
                        for (int j = 1; j < str_tc.Length; j++)
                        {
                            dataGridView.Rows[i].Cells[0].Value = str_tc[0];
                            dataGridView.Rows[i].Cells[j].Value = tc_data[i * 16 + j - 1].ToString("x2");
                            dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                        }
                    }
                    else
                    {
                        for (int j = 1; j < tc_data.Length % 16 + 1; j++)
                        {
                            dataGridView.Rows[i].Cells[0].Value = str_tc[0];
                            dataGridView.Rows[i].Cells[j].Value = tc_data[i * 16 + j - 1].ToString("x2");
                            dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
                        }
                    }
                }
            }
        }

        public void ctrChanged(int index, string value)
        {
            string str;
            str = Convert.ToString(GlobalVar.control[3], 2);
            str = Fill_Zero(str, 8);
            str = str.Remove(index, 1);
            str = str.Insert(index, value);
            GlobalVar.control[3] = Convert.ToByte(str, 2);
        }
        UdpClient clientSend = new UdpClient(GlobalVar.LocalPoint_Send);
        public void SendHandle()
        {
            //UdpClient client = null;
            //IPAddress remoteIP = IPAddress.Parse("10.129.41.96");
            //int remotePort = 19200;
            //IPEndPoint remotePoint = new IPEndPoint(remoteIP, remotePort);//实例化一个远程端点   
            //client = new UdpClient(new IPEndPoint(IPAddress.Any, 18200));
            //client.Send(GlobalVar.b, GlobalVar.b.Length, remotePoint);//将数据发送到远程端点
            clientSend.Send(GlobalVar.b, GlobalVar.b.Length, GlobalVar.RemotePoint);//将数据发送到远程端点 
            //client.Close();//关闭连接   
            //FileStream sr = File.WriteAllBytes(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + ".txt", Pub_Variable.b);
            StreamWriter sw;
            sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "发送的配置信息.txt");
            for (int i = 0; i < GlobalVar.b.Length; i++)
            {
                sw.WriteLine(GlobalVar.b[i].ToString("x2"));
            }
            sw.Close();
        }
        public void Send_Control()
        {
            //UdpClient client = null;
            //client = new UdpClient(GlobalVar.LocalPoint_Send);
            //client.Client.SendTimeout = 3000;
            //try
            //{
            clientSend.Send(GlobalVar.control, GlobalVar.control.Length, GlobalVar.RemotePoint);//将数据发送到远程端点 

            StreamWriter sw;
            sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "发送的控制信息.txt");
            for (int i = 0; i < GlobalVar.control.Length; i++)
            {
                sw.WriteLine(GlobalVar.control[i].ToString("x2"));//x2表示两位的16进制数，为了字符整齐
            }
            sw.Close();
            
        }
        #region 发送校验
       public class CheckParam
        {
            public byte[] origin;
            public string type;           
        }
        CheckParam param = new CheckParam();
        private delegate void stateCheckDelegate(bool ACK); //委托以控制主线程控件   
        /* public void sendCheckThread()//(object sender, EventArgs e)
        {
            Thread thread_check = new Thread(new ParameterizedThreadStart(Send_Check));//多线程           
            thread_check.IsBackground = true;
            thread_check.Start(param);
        }*/
        /*public void Send_Check(object status)
        {
            CheckParam param = status as CheckParam;
            bool ACK = false;
            clientRecv.Client.SendTimeout = 3000;
            //UdpClient client = new UdpClient(GlobalVar.LocalPoint);
            try
            {
                byte[] configAck = clientRecv.Receive(ref GlobalVar.RemotePoint);//接收数据
                MessageBox.Show("异常");
                byte sumVerify = param.origin[param.origin.Length - 2];
                if (configAck[0] == Convert.ToByte("eb", 16) && configAck[1] == Convert.ToByte("90", 16)
                        && configAck[2] == Convert.ToByte(param.type, 16) && configAck[(configAck.Length - 2)] == sumVerify)
                {
                    ACK = true;
                }
                //client.Close();
            }
            catch
            {
                ACK = false;
               
            }
            /*finally
            {
                client.Close();
            }
            StateBack(ACK);
        }*/
        private void StateBack(bool ACK)
        {
            stateCheckDelegate SCD = new stateCheckDelegate(StateBack);
            if (ACK)
            {
                MessageBox.Show("配置文件已下发！");
            }
            else
            {
                MessageBox.Show("配置文件未成功接收,请重新下发！");
                
            }
        }
        #endregion


        public string bytetostring(byte[] b)   //字节转八位二进制
        {
            string str = null;
            for (int i = 0; i < b.Length; i++)
            {
                str = str + Fill_Zero(Convert.ToString(b[i], 2), 8);
            }
            return str;
        }

        public int error_code_com(string str1, string str2)
        {
            int sum = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1.Substring(i, 1) != str2.Substring(i, 1))
                {
                    sum = sum + 1;
                }
            }
            return sum;
        }

        public string stringtobitstring(string str1)
        {
            string str = null;
            string bs = null;
            for (int i = 0; i < str1.Length; i++)
            {
                str = str1.Substring(i, 1);
                str = Convert.ToString(Convert.ToByte(str, 16), 2);
                str = Fill_Zero(str, 4);
                bs = bs + str;
            }
            return bs;
        }

        public byte[] remove_header(byte[] b, int num)
        {
            byte[] data = new byte[b.Length - num];
            for (int i = 0; i < b.Length - num; i++)
            {
                data[i] = b[i + num];
            }
            return data;
        }

        public string remove_char(string str)
        {
            string str1;
            str1 = str;
            for (int i = 0; i < str1.Length; i++)
            {
                if (str1.Substring(i, 1) == " " || str1.Substring(i, 1) == "\n" || str1.Substring(i, 1) == "/" || str1.Substring(i, 1) == Environment.NewLine)
                {
                    str1 = str1.Remove(i, 1);
                    if (i > 0)
                    {
                        i = i - 1;
                    }
                }
            }
            return str1;

        }

        public byte[] remove_tail(byte[] b, int num)
        {
            byte[] data = new byte[b.Length - num];
            for (int i = 0; i < b.Length - num; i++)
            {
                data[i] = b[i];
            }
            return data;
        }

        public byte sum_verify(byte[] b)   //求和校验
        {
            int sum = 0;
            for (int i = 0; i < b.Length - 6; i++)
            {
                sum = sum + b[i + 3];
            }
            string str = null;
            str = Fill_Zero(Convert.ToString(sum, 2), 32);
            str = str.Substring(str.Length - 8, 8);
            byte verify;
            verify = Convert.ToByte(str, 2);
            return verify;
        }
        public byte sum_verify(byte[] b, byte front, byte behind)   //求和校验,去掉前后无效位
        {
            int sum = 0;
            for (int i = 0; i < b.Length - front - behind; i++)
            {
                sum = sum + b[i + front];
            }
            string str = null;
            str = Fill_Zero(Convert.ToString(sum, 2), 32);
            str = str.Substring(str.Length - 8, 8);
            byte verify;
            verify = Convert.ToByte(str, 2);//最后8位为校验位
            return verify;
        }

        public string Fill_Zero(string str, int num) //高位补零
        {
            string str1 = str;
            //int k;
            if (str.Length < num)
            {
                for (int i = 0; i < num - str.Length; i++)
                {
                    //k = str1.Length;
                    str1 = str1.Insert(0, "0");
                }
            }
            return str1;
        }
        public byte[] array_joint(byte[] data1, byte[] data2)//两个数列合为一个
        {
            byte[] joint = new byte[data1.Length + data2.Length];
            for (int i = 0; i < data1.Length; i++)
            {
                joint[i] = data1[i];
            }
            for (int i = 0; i < data2.Length; i++)
            {
                joint[data1.Length + i] = data2[i];
            }
            return joint;
        }
        public struct Variance
        {
            public double mean;
            public double variance;
        }

        public Variance variance_cal(double[] data)
        {
            Variance var;
            var.mean = 0;
            var.variance = 0;
            double sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum = sum + data[i];
            }
            var.mean = sum / data.Length;
            sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                sum = sum + Math.Pow((data[i] - var.mean), 2);
            }
            var.variance = sum / data.Length;//平方和的平均
            return var;
        }

        public struct Find
        {
            public bool header_finded;
            public int find_location;
        }

        public byte[] no_change_set(byte[] data)//将输入参数的所有变动标识置0
        {
            int sum = 0;
            for (int i = 0; i < GlobalVar.cc_site.Length; i++)
            {
                sum = sum + GlobalVar.cc_site[i];
                data[sum] = 0;
            }
            return data;
        }

        public Find header_find(byte[] data)
        {

            Find find;
            find.find_location = -1;
            find.header_finded = false;
            for (int i = 0; i < data.Length - 1; i++)
            {
                if (data[i] == Convert.ToByte("eb", 16) && data[i + 1] == Convert.ToByte("90", 16))
                {
                    find.header_finded = true;
                    find.find_location = i;
                    break;
                }
            }
            return find;
        }

    }


    public partial class GlobalVar
    {
        //帧头+帧尾+帧类型+校验
        public static int rf_num = 18;
        public static int channel_length = 76;
        //public static int b_num1 = 6 + rf_num + channel_length * 6;
        public static int rfConfig = 6 + rf_num;
        public static int configNum = 6 + channel_length;
        public static int stateOut = 6 + 100;
        public static int b_num2 = 6 + 44;
        public static byte[] brf = new byte[rfConfig];
        public static byte[] b = new byte[configNum];

        public static byte[] control = new byte[b_num2];
        public static byte[] cc_site = new byte[] { 4, 18, 29, 29, 18, 29, 29,  18, 
                                                        29, 29, 18, 29, 29, 18, 29, 29, 18, 29, 29 };//各配置变动标识位
        //public static byte[] cc_site = new byte[] { 4, 18, 25+4, 29, 13+4, 25+4, 29, 14+4, 24+4, 22, 24+4, 22, 24+4, 22, 24+4, 22, 5, 14, 5 }; 
        public static string last_updown;
        public static string uplink_data;
        public static IPEndPoint LocalPoint_Send = new IPEndPoint(IPAddress.Any, 18100);
        public static IPEndPoint LocalPoint = new IPEndPoint(IPAddress.Any, 18000);
        public static IPEndPoint RemotePoint = new IPEndPoint(IPAddress.Parse("10.129.41.2"), 19200);
        public static ArrayList dot_rx1 = new ArrayList();
        public static long dot_num = 0;

    }
    public class UDP
    {
        public UdpClient clientRece = new UdpClient(GlobalVar.LocalPoint);
        public UdpClient clientSend = new UdpClient(GlobalVar.LocalPoint_Send);
    }
    public class Fruit
    {
        public string No { get; set; }
        public string c0 { get; set; }
        public byte c1 { get; set; }
        public byte c2 { get; set; }
        public byte c3 { get; set; }
        public byte c4 { get; set; }
        public byte c5 { get; set; }
        public byte c6 { get; set; }
        public byte c7 { get; set; }
        public byte c8 { get; set; }
        public byte c9 { get; set; }
        public byte ca { get; set; }
        public byte cb { get; set; }
        public byte cc { get; set; }
        public byte cd { get; set; }
        public byte ce { get; set; }
        public byte cf { get; set; }
        public string[] Name = new string[] { "c0", "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "ca", "cb", "cc", "cd", "ce", "cf" };

    }
    public class R_Signal
    {
        public string name { get; set; }
        public string R1 { get; set; }
        public string R2 { get; set; }
        public string R3 { get; set; }
        public string R4 { get; set; }

    }
    public class RX_signal
    {
        public string name { get; set; }
        public string Ranging { get; set; }
    }
    public class RX_statistics
    {
        public string dot_count { get; set; }
        public string mean { get; set; }
        public string variance { get; set; }
    }

}
