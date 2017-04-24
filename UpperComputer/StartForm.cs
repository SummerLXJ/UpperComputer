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
    public partial class StartForm : Form
    {

        public Form1 form1 = new Form1();
        public Form2 form2 = new Form2();
        public StartForm()
        {
            InitializeComponent();
        }
        Method method = new Method();
        #region 连接网络
        private delegate void webStateDelegate(bool connectState); //委托以控制主线程控件
        private void btnConnectWeb_Click(object sender, EventArgs e)
        {
            //检测状态输出判断网络连接
            Thread thread_net = new Thread(webConnect);//多线程
            thread_net.IsBackground = true;
            thread_net.Start();
        }
        public void webConnect()//线程内容，寻找状态包帧头并返回值
        {
            bool connectState = false;
            byte[] webTest = null;
            UDP udp = new UDP(); 
            //初始化一个UDPClient并和本地接收端口绑定
            //UdpClient clientRece = new UdpClient(GlobalVar.LocalPoint);
            udp.clientRece.Client.ReceiveTimeout = 3000;//设定接收超时时间
            try
            {
                webTest = udp.clientRece.Receive(ref GlobalVar.RemotePoint);
                for (int i = 0; i < GlobalVar.stateOut; i++)
                {
                    if (webTest[i] == Convert.ToByte("eb", 16) && webTest[i + 1] == Convert.ToByte("90", 16)
                                                                && webTest[i + 2] == Convert.ToByte("45", 16))
                    {
                        connectState = true;
                        pictureBox1.BackColor = Color.Lime;
                    }
                }
            }
            catch
            {
                connectState = false;
                pictureBox1.BackColor = Color.Red;
            }
            finally
            {
                udp.clientRece.Close();
            }
            StateBack(connectState);
        }
        private void StateBack(bool connectState)//委托，由子线程改变主线程控件
        {
            if (this.label1.InvokeRequired == false)
            {
                if (connectState)
                {
                    label1.Text = "网络连接成功！";
                }
                else
                {
                    label1.Text = "网络连接失败！";
                }
            }
            else
            {
                webStateDelegate WSD = new webStateDelegate(StateBack);
                this.label1.Invoke(WSD, connectState);
            }
        }
        #endregion


        private void btnC1_Click(object sender, EventArgs e)
        {
            if (null == form1|| form1.IsDisposed == true)
            { form1 = new Form1(); }
            form1.Show();
        }

        private void btnC2_Click(object sender, EventArgs e)
        {
            if (null == form2 || form1.IsDisposed == true)
            { form2 = new Form2(); }
            form2.Show();
        }


        }
    }
