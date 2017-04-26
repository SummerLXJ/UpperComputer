using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UpperComputer
{
    public partial class MainForm : Form
    {
        Method method = new Method();
        public static byte[] brf = new byte[GlobalVar.rfConfigNum];
        public static byte[][] _configByte = new byte[6][];
        public static byte[] _control = new byte[GlobalVar.controlNum];
        public byte[] control
        {
            set { _control = value; }
            get { return _control; }
        }
        public byte[][] configByte
        {
            set { _configByte = value; }
            get { return _configByte; }
        }
        public MainForm()
        {
            InitializeComponent();
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
        private void SendwithCheck(OnOffBtn btn, byte[] sendbyte, string type)
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
                    this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                               DateTime.Now.ToLocalTime().ToString() + " 信号下发失败";
                }
            }
        }
        # endregion
        #region 控制开关
        //控制信号改变和显示
        private void ControlChange(int index, bool status)
        {
            string str = null;
            switch (index)
            {
                case 1: str = "遥控1"; break;
                case 2: str = "遥控2"; break;
                case 3: str = "遥测1"; break;
                case 4: str = "遥测2"; break;
                case 5: str = "遥测3"; break;
                case 6: str = "遥测4"; break;
            }
            if (status)
            {
                method.ctrChanged(control,1, "1");
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                           DateTime.Now.ToLocalTime().ToString() + " 打开" + str + "通道";
            }
            else
            {
                method.ctrChanged(control,1, "0");
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                           DateTime.Now.ToLocalTime().ToString() + " 关闭" + str + "通道";
            }
        }
        private void onOffBtn1_Click(object sender, EventArgs e)
        {
            ControlChange(1, onOffBtn1.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            SendwithCheck(onOffBtn1, control, "34");
        }

        private void onOffBtn2_Click(object sender, EventArgs e)
        {
            ControlChange(2, onOffBtn2.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            SendwithCheck(onOffBtn2, control, "34");
        }
        private void onOffBtn3_Click(object sender, EventArgs e)
        {
            ControlChange(3, onOffBtn3.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            SendwithCheck(onOffBtn3, control, "34");
        }
        private void onOffBtn4_Click(object sender, EventArgs e)
        {
            ControlChange(4, onOffBtn4.Checked);
           control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            SendwithCheck(onOffBtn4, control, "34");
        }
        private void onOffBtn5_Click(object sender, EventArgs e)
        {
            ControlChange(5, onOffBtn5.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            SendwithCheck(onOffBtn5, control, "34");
        }
        private void onOffBtn6_Click(object sender, EventArgs e)
        {
            ControlChange(6, onOffBtn6.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            SendwithCheck(onOffBtn6, control, "34");
        }
        #endregion
        private void 加载配置文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                FileStream fr = File.OpenRead(fileName);
                byte[] pb = new byte[GlobalVar.configNum];
                fr.Read(pb, 0, pb.Length);
                byte chanelindex = pb[3];
                int configIndex = Convert.ToInt32(Convert.ToString(chanelindex).Substring(2, 6));
                configuration(configIndex, pb);
                GlobalVar.b = pb;
                fr.Close();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                           DateTime.Now.ToLongTimeString() + "加载配置文件 " + fileName;
            }
            method.SendHandle();
            method.no_change_set(GlobalVar.b);
        }
        public void configuration(int configIndex, byte[] b)
        {
            //帧头 0~1
            //配置参数标识 2
            //射频参数18字节，从3~20
            //上行标识3
            //上行变动标识4
            //Method method = new Method();
            TC1 TC1 = new TC1();
            TC2 TC2 = new TC2();
            RG1 RG1 = new RG1();
            RG2 RG2 = new RG2();
            RG3 RG3 = new RG3();
            RG4 RG4 = new RG4();
            int size = b.Length;
            string[] bs = new string[size];
            for (int i = 0; i < size; i++)
            {
                bs[i] = method.Fill_Zero(Convert.ToString(b[i], 2), 8); //将每个byte数据高位填0，计算时结果不变
            }
            switch (configIndex)
            {
                case 1: TC1.Channel_Config(b, bs); break;
                case 2: TC2.Channel_Config(b, bs); break;
                case 3: RG1.Channel_Config(b, bs); break;
                case 4: RG2.Channel_Config(b, bs); break;
                case 5: RG3.Channel_Config(b, bs); break;
                case 6: RG4.Channel_Config(b, bs); break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void 遥控1配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TC1 TC1 = new TC1();
            TC1.Show();
        }


    }
}
