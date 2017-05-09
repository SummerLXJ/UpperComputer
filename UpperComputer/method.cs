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
using System.Diagnostics;

namespace UpperComputer
{
    class Method
    {
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

        public void ctrChanged(byte[] b,int index, string value)
        {
            string str;
            str = Convert.ToString(b[3], 2);
            str = Fill_Zero(str, 8);
            str = str.Remove(index, 1);
            str = str.Insert(index, value);
            b[3] = Convert.ToByte(str, 2);
        }


        public void SendHandle()
        {
            UdpClient clientSend = new UdpClient(GlobalVar.LocalPoint_Send);
            clientSend.Send(GlobalVar.b, GlobalVar.b.Length, GlobalVar.RemotePoint);//将数据发送到远程端点 
            clientSend.Close();
            StreamWriter sw;
            sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "发送的配置信息.txt");
            for (int i = 0; i < GlobalVar.b.Length; i++)
            {
                sw.WriteLine(GlobalVar.b[i].ToString("x2"));
            }
            sw.Close();
        }
        public void Send_Control(byte[] control, int CHANNEL)
        {
            UdpClient clientSend = new UdpClient(GlobalVar.LocalPoint_Send);
            clientSend.Send(control, control.Length, GlobalVar.RemotePoint);//将数据发送到远程端点 
            StreamWriter sw;
            sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C" + CHANNEL+"控制信号.txt");
            for (int i = 0; i < control.Length; i++)
            {
                sw.WriteLine(control[i].ToString("x2"));//x2表示两位的16进制数，为了字符整齐
            }
            sw.Close();
            clientSend.Close();
        }

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
        public byte[] FillHeadTail(byte[] b, string type)
        {
            b[0] = Convert.ToByte("eb", 16);//把16进制的数转为8bit正整数
            b[1] = Convert.ToByte("90", 16);
            b[2] = Convert.ToByte(type, 16);//控制包标识符
            b[b.Length - 2] = Convert.ToByte("be", 16);
            b[b.Length-1] = Convert.ToByte("09", 16);
            return b;
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

        

        #region 下发信号返回校验
        public delegate bool CheckHandle(byte[] origin, string type);
        bool Foo(byte[] origin, string type)
        {
            UdpClient clientRecv = new UdpClient(GlobalVar.LocalPoint);
            clientRecv.Client.ReceiveTimeout = 1000;
            bool ack = false;
            try
            {
                byte[] configAck = clientRecv.Receive(ref GlobalVar.RemotePoint);//接收数据
                byte sumVerify = origin[origin.Length - 2];
                if (configAck[0] == Convert.ToByte("eb", 16) && configAck[1] == Convert.ToByte("90", 16)
                        && configAck[2] == Convert.ToByte(type, 16) && configAck[(configAck.Length - 2)] == sumVerify)
                {
                    MessageBox.Show("信号下发成功！");
                    ack = true;
                }
                else
                {
                    MessageBox.Show("校验和有误，请重新下发！");
                    ack = false;
                }
            }
            catch
            {
                ack = false;
            }
            finally
            { clientRecv.Close(); }
            return ack;
        }
        public void BackCheck(OnOffBtn btn, byte[] sendbyte, string type)
        {
            bool ack = false;
            try
            {
                CheckHandle ch = new CheckHandle(this.Foo);
                IAsyncResult ar = ch.BeginInvoke(sendbyte, type, null, ch);
                ack = ch.EndInvoke(ar);
            }
            catch
            {
                ack = false;
            }
            finally
            {
                if (!ack)
                {
                    btn.isCheck = !btn.isCheck;
                    btn.Invalidate();
                    /*if (btn.Checked == true)
                    {
                        method.ctrChanged(, "1");
                    }
                    if (btn.Checked == false)
                    {
                        method.ctrChanged(1, "0");
                    }*/
                    //此处不恢复原来的控制，以便于在配置信号处的使用
                    MessageBox.Show("信号下发失败！");
                    //this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                               //DateTime.Now.ToLocalTime().ToString() + " 信号下发失败";
                }
            }
        }
        # endregion
        public static byte[] brf = new byte[GlobalVar.rfConfigNum];
        /*public static byte[][] configByte = new byte[GlobalVar.chanelCount][];
        public static byte[] control = new byte[GlobalVar.controlNum];
        public byte[] _control
        {
            set { control = value; }
            get { return control; }
        }
        public byte[][] _configByte
        {
            set
            {
                configByte = value; 
            }
            get { return configByte; }
        } */
    }


    public partial class GlobalVar
    {
        //帧头+帧尾+帧类型+校验
        public static int rf_num = 18;
        public static int chanelCount = 6;
        public static int channel_length = 590;
        //public static int b_num1 = 6 + rf_num + channel_length * 6;
        public static int rfConfigNum = 6 + rf_num;
        public static int configNum = 6 + channel_length;
        public static int stateOut = 6 + 100;
        public static int controlNum = 6 + 44;
       public static byte[] b = new byte[configNum];        
        public static byte[] cc_site = new byte[] { 4, 29, 29};//各配置变动标识位
       
        public static string last_updown;
        public static string uplink_data;
        public static IPEndPoint LocalPoint_Send = new IPEndPoint(IPAddress.Any, 18100);
        public static IPEndPoint LocalPoint = new IPEndPoint(IPAddress.Any, 18200);
        public static IPEndPoint RemotePoint = new IPEndPoint(IPAddress.Parse("10.129.41.2"), 19200);
        public static ArrayList dot_rx1 = new ArrayList();
        public static long dot_num = 0;
        
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
