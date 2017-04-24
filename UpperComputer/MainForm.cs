using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace UpperComputer
{
    public partial class MainForm : Form
    {
        Method method = new Method();
        UdpClient clientRecv = new UdpClient(GlobalVar.LocalPoint);
        public MainForm()
        {
            InitializeComponent();
        }
        #region 下发信号返回校验
        public delegate bool CheckHandle(byte[] origin, string type);
        bool Foo(byte[] origin, string type)
        {
            bool ACK = false;
            clientRecv.Client.ReceiveTimeout = 3000;
            try
            {
                byte[] configAck = clientRecv.Receive(ref GlobalVar.RemotePoint);//接收数据
                byte sumVerify = origin[origin.Length - 2];
                if (configAck[0] == Convert.ToByte("eb", 16) && configAck[1] == Convert.ToByte("90", 16)
                        && configAck[2] == Convert.ToByte(type, 16) && configAck[(configAck.Length - 2)] == sumVerify)
                {
                    MessageBox.Show("控制信号下发成功！");
                    ACK = true;
                }
                else
                {
                    MessageBox.Show("校验和有误，请重新下发控制信号！");
                    ACK = false;
                }
            }
            catch
            {
                ACK = false;
            }
            return ACK;
        }
        # endregion
        private void onOffBtn1_Click(object sender, EventArgs e)
        {
            GlobalVar.control[GlobalVar.control.Length - 2] = method.sum_verify(GlobalVar.control);

            if (onOffBtn1.Checked == true)
            {
                method.ctrChanged(1, "1");
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLocalTime().ToString()
                                            + " 打开遥控1通道";
            }
            if (onOffBtn1.Checked == false)
            {
                method.ctrChanged(1, "0");
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLocalTime().ToString()
                                            + " 关闭遥控1通道";
            }
            bool ack = false;
            try
            {
                method.Send_Control();
                CheckHandle ch = new CheckHandle(this.Foo);
                IAsyncResult ar = ch.BeginInvoke(GlobalVar.control, "34", null, ch);
                ack = ch.EndInvoke(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString() + "控制信号下发失败！");
                ack = false;
            }
            finally
            {
                if (!ack)
                {
                    onOffBtn1.isCheck = !onOffBtn1.isCheck;
                    onOffBtn1.Invalidate();
                    if (onOffBtn1.Checked == true)
                    {
                        method.ctrChanged(1, "1");
                    }
                    if (onOffBtn1.Checked == false)
                    {
                        method.ctrChanged(1, "0");
                    }
                    this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLocalTime().ToString()
                                            + " 通道控制信号未下发";
                }
            }
        }

        private void onOffBtn1_Load(object sender, EventArgs e)
        {

        }

        private void onOffBtn3_Load(object sender, EventArgs e)
        {

        }




    }
}
