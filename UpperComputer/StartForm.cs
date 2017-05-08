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
using System.Diagnostics;

namespace UpperComputer
{
    public partial class StartForm : Form
    {
        Form1 form1 = new Form1();
        Form2 form2 = new Form2();
        Method method = new Method();
        public static string chanelTotal;
        public StartForm()
        {
            InitializeComponent();
        }
        //Method method = new Method();

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
            //初始化一个UDPClient并和本地接收端口绑定
            UdpClient clientRecv = new UdpClient(GlobalVar.LocalPoint);
            clientRecv.Client.ReceiveTimeout = 3000;//设定接收超时时间
            try
            {
                webTest = clientRecv.Receive(ref GlobalVar.RemotePoint);
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
                clientRecv.Close();
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
            if (null == form1 || form1.IsDisposed == true)
            { form1 = new Form1(); }
            chanelTotal = "1";
            form1.Show();
        }

        private void btnC2_Click(object sender, EventArgs e)
        {
            if (null == form2 || form2.IsDisposed == true)
            { form2 = new Form2(); }
            chanelTotal = "2";
            form2.Show();
        }

        private void StartForm_Load(object sender, EventArgs e)
        {

        }
        private void StartForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
        Thread thread1;
        bool thread_control = false;
        private void button1_Click(object sender, EventArgs e)
        {

            int thread1_count = 0;//UdpClient clienRecv = new UdpClient(GlobalVar.LocalPoint);
            thread_control = !thread_control;

            if (thread1_count == 0)
            {
                if (thread1_count == 0)
                {
                    thread1 = new Thread(new ThreadStart(ReceiveHandle));
                    thread1.Start();
                    //mf.richTextBox3.Text = mf.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 启动接收线程";
                    this.button1.Text = "暂停接收数据";
                }
            }
            else
            {
                if (!thread_control)
                {
                    thread1.Suspend();
                    //mf.richTextBox3.Text = mf.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 暂停接收线程";
                    this.button1.Text = "开始接收数据";
                }
                else
                {
                    thread1.Resume();
                    //mf.richTextBox3.Text = mf.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 重启接收线程";
                    this.button1.Text = "暂停接收数据";
                }
            }
            thread1_count = thread1_count + 1;
        }
        int frame_count = 0;
        bool find_head = false;
        bool stop = true;
        byte[] receiveData = null;
        byte[] next_receiveData = null;
        void ReceiveHandle()
        {
            frame_count = 0; //帧计数
            byte[] Data = null;
            UdpClient client_receive = null;
            client_receive = new UdpClient(GlobalVar.LocalPoint);
            while (true)
            {
                Data = client_receive.Receive(ref GlobalVar.RemotePoint);//接收数据
                ReceiveData_Handle(Data);
                //sw.WriteLine(Data);   //将接收到的数据写入text文件
            }
        }
        public void ReceiveData_Handle(byte[] Data)
        {
            if (stop && this.IsHandleCreated)
            {
                //receiveData=method.remove_tail(method.remove_header(Data, 8), 4);
                receiveData = Data;
                find_head = method.header_find(receiveData).header_finded;
                if (find_head) { stop = false; }
            }
            else
            {
                //next_receiveData = method.remove_tail(method.remove_header(Data, 8), 4);
                next_receiveData = Data;
                byte[] combine_data = new byte[receiveData.Length + next_receiveData.Length];
                combine_data = method.array_joint(receiveData, next_receiveData);
                FrameData_Handle(receiveData, combine_data);
                receiveData = next_receiveData;
            }
        }
        public void FrameData_Handle(byte[] receiveData, byte[] combine_data)
        {
            string combineString = null;
            combineString = method.bytetostring(combine_data);
            for (int i = 0; i < receiveData.Length; i++)
            {
                if (combine_data[i] == Convert.ToByte("eb", 16) && combine_data[i + 1] == Convert.ToByte("90", 16))
                {
                    byte channelbyte = combine_data[i + 2];
                    string chanel = method.Fill_Zero(Convert.ToString(channelbyte, 2), 8).Substring(0, 2);
                    if (chanel == "00")
                    {
                        //chanelTotal = "1";
                    }
                    if (chanel == "11")
                    {
                        //chanelTotal = "2";
                    }
                }
            }
        }
    }
}
