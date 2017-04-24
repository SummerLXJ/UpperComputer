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
    public partial class Main_Form : Form
    {
        public byte[][] configByte = new byte[6][];
        //IPEndPoint RemotePoint = new IPEndPoint(IPAddress.Parse("10.129.41.96"), 19200);
        //IPEndPoint LocalPoint = new IPEndPoint(IPAddress.Any, 18100);
        public TC1 TC1 = new TC1();
        public TC2 TC2 = new TC2();
        public RG1 RG1 = new RG1();
        public RG2 RG2 = new RG2();
        public RG3 RG3 = new RG3();
        public RG4 RG4 = new RG4();
        public RF_set rf_set = new RF_set();
       /* public RX1_curve rx1_curve = new RX1_curve();
        public RX2_curve rx2_curve = new RX2_curve();
        public RX3_curve rx3_curve = new RX3_curve();
        public RX4_curve rx4_curve = new RX4_curve();*/
        private Order_Set order_set = new Order_Set();
        public string filename;
        public int times = 0;
        Method method = new Method();
        double power = System.Math.Pow(2, 32);
        double p1 = 80.0;
        double m = 1000000.0;
        public Main_Form()
        {
            InitializeComponent();
            //order_set.send += new Order_Set.send_length(receive_length);
        }

        public void datagridview_initial(DataGridView dataGridView)
        {
            dataGridView.Rows.Clear();
            dataGridView.ColumnHeadersHeight = 30;
            dataGridView.AutoGenerateColumns = true;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.ColumnCount = 17;
            dataGridView.Columns[0].Name = "地址";
            //从0-E编号
            for (int i = 1; i < 17; i++)
            {
                dataGridView.Columns[i].Name = (i - 1).ToString("x");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GlobalVar.dot_num = 0;//统计点数
            timer1.Interval = 1000;
            timer1.Enabled = true;
            for (int i = 0; i < GlobalVar.b_num2; i++)
            {
                GlobalVar.control[i] = 0;
            }
            //赋值意义：控制包
            GlobalVar.control[0] = Convert.ToByte("eb", 16);//把16进制的数转为8bit正整数
            GlobalVar.control[1] = Convert.ToByte("90", 16);
            GlobalVar.control[2] = Convert.ToByte("34", 16);//控制包标识符
            GlobalVar.control[GlobalVar.control.Length - 2] = Convert.ToByte("be", 16);
            GlobalVar.control[GlobalVar.control.Length - 1] = Convert.ToByte("09", 16);
            //初始自动加载参数            
            filename = System.AppDomain.CurrentDomain.BaseDirectory + "上次的配置+控制信息.txt";//还未存
            for (int i = 0; i < 6; i++)
            {
                configByte[i] = new byte[GlobalVar.configNum];
            }
            bool exist = File.Exists(filename);
            string strline = null;
            ArrayList blist = new ArrayList();
            //if (exist)
            try
            {
                StreamReader sr = new StreamReader(filename);
                while ((strline = sr.ReadLine()) != null)//逐行将配置信息赋给blist
                {
                    blist.Add(strline);
                }
                string[] myArr = (string[])blist.ToArray(typeof(string));//将blist复制到元素类型为string的数组中
                for (int i = 0; i < configByte.Length; i++)
                {
                    for (int j = 0; j < configByte[i].Length; j++)
                    {
                        configByte[i][j] = Convert.ToByte(myArr[i * GlobalVar.configNum + j], 16);
                    }
                    configuration(i, configByte[i]);
                    //GlobalVar.b[i] = Convert.ToByte(myArr[i], 16);
                }
                for (int i = 0; i < GlobalVar.rfConfig; i++)
                {
                    GlobalVar.brf[i] = Convert.ToByte(myArr[myArr.Length - 23], 16);
                }
                rfConfiguration(GlobalVar.brf);//brf还未赋值，后续处理
                //configuration(GlobalVar.b);//配置
            }
            catch
            {
                MessageBox.Show("请自行配置参数！");
            }

            #region 初始化各个DatagridView
            // rx
            string[] enterclose = new string[] { "名称", "R1", "R2", "R2", "R4" };
            string[] Name = new string[]{ "伪码锁定", "载波锁定", "位同步", "帧同步", "前一采样伪码锁定", "前一采样载波锁定", "信噪比", "测量帧位计数",
                                "伪码周期数", "伪码相位", "伪码CHIP相位", "载波多普勒频偏" };
            List<R_Signal> r_signal = new List<R_Signal>();
            r_signal.Add(new R_Signal()
            {
                name = "名称",
                R1 = "测量通道1",
                R2 = " 测量通道",
                R3 = " 测量通道",
                R4 = "测量通道 "
            }
                    );
            for (int j = 0; j < Name.Length; j++)
            {
                r_signal.Add(new R_Signal()
                {
                    name = Name[j],
                    R1 = " ",
                    R2 = " ",
                    R3 = " ",
                    R4 = " "
                }
                    );
            }

            dataGridView2.DataSource = r_signal;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;  //调整行高
            //dataGridView2.ColumnHeadersHeight = 20;  //设置列标题高度
            dataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dataGridView2.ColumnHeadersVisible = false;
            dataGridView2.RowHeadersVisible = false;
            string[] name1 = new string[] { "载波NCO", "伪码NCO", "SNR" };
            List<RX_signal> rx_signal = new List<RX_signal>();
            rx_signal.Add(new RX_signal() { name = "名称", Ranging = "下行测量状态" });

            for (int j = 0; j < name1.Length; j++)
            {
                rx_signal.Add(new RX_signal()
                {
                    name = name1[j],
                    Ranging = " "
                }
                    );
            }

            dataGridView4.DataSource = rx_signal;
            dataGridView4.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView4.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;  //调整行高
            dataGridView4.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dataGridView4.RowHeadersVisible = false;
            dataGridView4.ColumnHeadersVisible = false;
            string[] name2 = new string[] { "距离(m)", "距离(ns)", "速度(m/s)", "星地时差" };
            List<R_Signal> rx_result = new List<R_Signal>();
            rx_result.Add(new R_Signal() { name = "名称", R1 = "通道1", R2 = "通道2", R3 = "通道3", R4 = "通道4" });

            for (int j = 0; j < 6; j++)
            {
                rx_result.Add(new R_Signal()
                {
                    name = Name[j + 6],
                    R1 = " ",
                    R2 = " ",
                    R3 = " ",
                    R4 = " ",

                }
                    );
            }

            for (int j = 0; j < name2.Length; j++)
            {
                rx_result.Add(new R_Signal()
                {
                    name = name2[j],
                    R1 = " ",
                    R2 = " ",
                    R3 = " ",
                    R4 = " "
                }
                    );
            }

            dataGridView3.DataSource = rx_result;
            dataGridView3.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView3.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;  //调整行高
            dataGridView3.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            dataGridView3.RowHeadersVisible = false;
            dataGridView3.ColumnHeadersVisible = false;

            //tc
            datagridview_initial(dataGridView5);

            //tm
            datagridview_initial(dataGridView1);
            #endregion
        }

        #region 将当前配置byte显示到界面
        public void rfConfiguration(byte[] b)//射频配置
        {
            Method method = new Method();
            int size = b.Length;
            string[] bs = new string[size];
            for (int i = 0; i < size; i++)
            {
                bs[i] = method.Fill_Zero(Convert.ToString(b[i], 2), 8); //将每个byte数据高位填0，计算时结果不变
            }
            //上行射频频点
            rf_set.textBox1.Text = Convert.ToUInt32(bs[5] + bs[6] + bs[7] + bs[8], 2).ToString();
            //textBox57.Text = Convert.ToUInt32(bs[5]).ToString();
            //上行增益
            rf_set.textBox2.Text = Convert.ToString(b[9]);
            //下行射频频点
            rf_set.textBox3.Text = Convert.ToUInt32(bs[10] + bs[11] + bs[12] + bs[13], 2).ToString();
            //下行增益
            rf_set.textBox4.Text = Convert.ToString(b[14]);
            //闪断次数
            rf_set.textBox5.Text = Convert.ToUInt16(bs[15] + bs[16], 2).ToString();
            //闪断时间
            rf_set.textBox6.Text = Convert.ToUInt16(bs[17] + bs[18], 2).ToString();
            //闪断间隔
            rf_set.textBox7.Text = Convert.ToUInt16(bs[19] + bs[20], 2).ToString();
        }
        public void configuration(int configIndex, byte[] b)
        {
            //帧头 0~1
            //配置参数标识 2
            //射频参数18字节，从3~20
            //上行标识3
            //上行变动标识4
            Method method = new Method();
            int size = b.Length;
            string[] bs = new string[size];
            for (int i = 0; i < size; i++)
            {
                bs[i] = method.Fill_Zero(Convert.ToString(b[i], 2), 8); //将每个byte数据高位填0，计算时结果不变
            }
            switch (configIndex)
            {
                case 0: TC1.Channel_Config(b, bs); break;
                case 1: TC2.Channel_Config(b, bs); break;
                case 2: RG1.Channel_Config(b, bs); break;
                case 3: RG2.Channel_Config(b, bs); break;
                case 4: RG3.Channel_Config(b, bs); break;
                case 5: RG4.Channel_Config(b, bs); break;
            }
        }
        #endregion
        #region 保存/加载配置文件
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fr = File.OpenRead(openFileDialog1.FileName);
                byte[] pb = new byte[GlobalVar.configNum];
                fr.Read(pb, 0, pb.Length);
                configuration(0,pb);
                GlobalVar.b = pb;
                fr.Close();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "加载配置文件 " + openFileDialog1.FileName;
            }
            method.SendHandle();
            method.no_change_set(GlobalVar.b);
        }
        private void 加载配置文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream fr = File.OpenRead(openFileDialog1.FileName);
                byte[] pb = new byte[GlobalVar.configNum];
                fr.Read(pb, 0, pb.Length);
                configuration(0,pb);
                GlobalVar.b = pb;
                fr.Close();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "加载配置文件 " + openFileDialog1.FileName;
            }

            method.SendHandle();
            method.no_change_set(GlobalVar.b);
        }
        private void 保存配置文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rf_set.rf_parameter_down();
            TC1.count = 21;
            TC1.parameter_down();
            TC2.count = 21 + GlobalVar.channel_length;
            TC2.parameter_down();
            RG1.count = 21 + GlobalVar.channel_length * 2;
            RG1.parameter_down();
            RG2.count = 21 + GlobalVar.channel_length * 3;
            RG2.parameter_down();
            RG3.count = 21 + GlobalVar.channel_length * 4;
            RG3.parameter_down();
            RG4.count = 21 + GlobalVar.channel_length * 5;
            RG4.parameter_down();
            int sum = 0;
            for (int i = 0; i < GlobalVar.cc_site.Length; i++)
            {
                sum = sum + GlobalVar.cc_site[i];
                GlobalVar.b[sum] = Convert.ToByte("cc", 16);
            }
            saveFileDialog1.Filter = "*.txt|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream sr = File.OpenWrite(saveFileDialog1.FileName);
                sr.Write(GlobalVar.b, 0, GlobalVar.b.Length);
                //sr.Write(para_and_control, 0, para_and_control.Length);
                sr.Close();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "保存配置文件到 " + saveFileDialog1.FileName;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            rf_set.rf_parameter_down();
            TC1.count = 21;
            TC1.parameter_down();
            TC2.count = 21 + GlobalVar.channel_length;
            TC2.parameter_down();
            RG1.count = 21 + GlobalVar.channel_length * 2;
            RG1.parameter_down();
            RG2.count = 21 + GlobalVar.channel_length * 3;
            RG2.parameter_down();
            RG3.count = 21 + GlobalVar.channel_length * 4;
            RG3.parameter_down();
            RG4.count = 21 + GlobalVar.channel_length * 5;
            RG4.parameter_down();
            int sum = 0;
            for (int i = 0; i < GlobalVar.cc_site.Length; i++)
            {
                sum = sum + GlobalVar.cc_site[i];
                GlobalVar.b[sum] = Convert.ToByte("cc", 16);
            }
            saveFileDialog1.Filter = "*.txt|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream sr = File.OpenWrite(saveFileDialog1.FileName);
                sr.Write(GlobalVar.b, 0, GlobalVar.b.Length);
                sr.Close();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "保存配置文件到 " + saveFileDialog1.FileName;
            }
        }
        #endregion

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
            UdpClient clientRece = new UdpClient(GlobalVar.LocalPoint);
            clientRece.Client.ReceiveTimeout = 3000;//设定接收超时时间
            try
            {
                webTest = clientRece.Receive(ref GlobalVar.RemotePoint);
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
                clientRece.Close();
            }
            StateBack(connectState);
        }
        private void StateBack(bool connectState)//委托，由子线程改变主线程控件
        {
            if (this.label38.InvokeRequired == false)
            {
                if (connectState)
                {
                    label38.Text = "网络连接成功！";
                }
                else
                {
                    label38.Text = "网络连接失败！";
                }
            }
            else
            {
                webStateDelegate WSD = new webStateDelegate(StateBack);
                this.label38.Invoke(WSD, connectState);
            }
        }
        #endregion

        #region  主界面控制信号下发
        private void checkBox8_CheckedChanged(object sender, EventArgs e)  //遥控1开关
        {
            if (checkBox8.Checked == true)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(6, 1);
                str = str.Insert(6, "1");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString()
                                            + " 打开遥控1通道";
            }
            if (checkBox8.Checked == false)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(6, 1);
                str = str.Insert(6, "0");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString()
                                            + " 关闭遥控1通道";
            }
            GlobalVar.control[GlobalVar.control.Length - 3] = method.sum_verify(GlobalVar.control);
            method.Send_Control();

        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e) //遥控2开关
        {
            if (checkBox9.Checked == true)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(5, 1);
                str = str.Insert(5, "1");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 打开遥控2通道";
            }
            if (checkBox9.Checked == false)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);//转为二进制
                str = method.Fill_Zero(str, 8);
                str = str.Remove(5, 1);
                str = str.Insert(5, "0");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 关闭遥控2通道";
            }
            GlobalVar.control[GlobalVar.control.Length - 3] = method.sum_verify(GlobalVar.control);
            method.Send_Control();
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox13.Checked == true)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(4, 1);
                str = str.Insert(4, "1");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 打开测量1通道";
            }
            if (checkBox13.Checked == false)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(4, 1);
                str = str.Insert(4, "0");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 关闭测量1通道";
            }
            GlobalVar.control[GlobalVar.control.Length - 3] = method.sum_verify(GlobalVar.control);
            method.Send_Control();
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox17.Checked == true)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(3, 1);
                str = str.Insert(3, "1");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 打开测量2通道";
            }
            if (checkBox17.Checked == false)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(3, 1);
                str = str.Insert(3, "0");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 关闭测量2通道";
            }
            GlobalVar.control[GlobalVar.control.Length - 3] = method.sum_verify(GlobalVar.control);
            method.Send_Control();

        }
        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox21.Checked == true)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(2, 1);
                str = str.Insert(2, "1");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 打开测量3通道";
            }
            if (checkBox21.Checked == false)
            {
                string str;
                str = Convert.ToString(GlobalVar.control[3], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(2, 1);
                str = str.Insert(2, "0");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 关闭测量3通道";
            }
            GlobalVar.control[GlobalVar.control.Length - 3] = method.sum_verify(GlobalVar.control);
            method.Send_Control();

        }
        private void checkBox25_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox25.Checked == true)
            {
                string str;
                str = Convert.ToString(GlobalVar.b[340], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(1, 1);
                str = str.Insert(1, "1");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 打开测量4通道";
            }
            if (checkBox25.Checked == false)
            {
                string str;
                str = Convert.ToString(GlobalVar.b[340], 2);
                str = method.Fill_Zero(str, 8);
                str = str.Remove(1, 1);
                str = str.Insert(1, "0");
                GlobalVar.control[3] = Convert.ToByte(str, 2);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 关闭测量4通道";
            }
            GlobalVar.control[GlobalVar.control.Length - 3] = method.sum_verify(GlobalVar.control);
            method.Send_Control();
        }
        #endregion

        #region 参数配置界面打开
        private void 射频参数ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == rf_set || rf_set.IsDisposed == true)
            { rf_set = new RF_set(); }
            rf_set.Show();
        }
        private void 遥控通道1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TeleControl telecontrol = new TeleControl(this);
            if (null == TC1 || TC1.IsDisposed == true)
            { TC1 = new TC1(); }
            TC1.Show();
        }
        private void 遥控通道2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == TC2 || TC2.IsDisposed == true)
            { TC2 = new TC2(); }
            TC2.Show();

        }
        private void 上行测量通道1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (null == RG1 || RG1.IsDisposed == true)
            { RG1 = new RG1(); }
            RG1.Show();
        }
        private void 上行测量通道2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == RG2 || RG2.IsDisposed == true)
            { RG2 = new RG2(); }
            RG2.Show();
        }
        private void 上行测量通道3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == RG3 || RG3.IsDisposed == true)
            { RG3 = new RG3(); }
            RG3.Show();
        }
        private void 上行测量通道4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == RG4 || RG4.IsDisposed == true)
            { RG4 = new RG4(); }
            RG4.Show();
        }
        #endregion

        /*#region 测量统计通道
        private void button22_Click(object sender, EventArgs e)  //测量统计界面
        {
            if (null == rx1_curve || rx1_curve.IsDisposed == true)//测量通道1
            { rx1_curve = new RX1_curve(); }
            rx1_curve.Show();
        }
        private void button25_Click(object sender, EventArgs e)
        {
            if (null == rx2_curve || rx2_curve.IsDisposed == true)//测量通道2
            { rx2_curve = new RX2_curve(); }
            rx2_curve.Show();
        }
        private void button24_Click(object sender, EventArgs e)//通道3
        {
            if (null == rx3_curve || rx3_curve.IsDisposed == true)
            { rx3_curve = new RX3_curve(); }
            rx3_curve.Show();
        }
        private void button23_Click(object sender, EventArgs e)
        {
            if (null == rx4_curve || rx4_curve.IsDisposed == true)
            { rx4_curve = new RX4_curve(); }
            rx4_curve.Show();
        }
        #endregion*/

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString(); //+DateTime.Now.ToShortTimeString();
            //时间没显示
        }

        #region 指令生成和发送
        public void send_order(byte[] order)//发送order
        {
            UdpClient client = null;
            IPAddress remoteIP = IPAddress.Parse("10.129.41.96");
            int remotePort = 19200;
            IPEndPoint remotePoint = new IPEndPoint(remoteIP, remotePort);//实例化一个远程端点   
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            client.Send(order, order.Length, remotePoint);//将数据发送到远程端点 
            client.Close();
        }
        private byte[] generate_order(string id, string gate, int length, DataGridView datagridview)  //生成遥控/遥测帧，用于指令保存和发送前
        {
            byte[] order = new byte[length + 10];
            order[0] = Convert.ToByte("eb", 16);
            order[1] = Convert.ToByte("90", 16);
            order[2] = Convert.ToByte(id, 16);
            order[3] = Convert.ToByte(gate, 16);
            order[order.Length - 2] = Convert.ToByte("be", 16);
            order[order.Length - 1] = Convert.ToByte("09", 16);
            string str;
            str = (length).ToString("x4");
            order[4] = Convert.ToByte(str.Substring(0, 2), 16);
            order[5] = Convert.ToByte(str.Substring(2, 2), 16);
            order[6] = Convert.ToByte("c0", 16); //默认为循环发送
            for (int i = 0; i < length; i++)
            {
                order[i + 7] = Convert.ToByte(datagridview.Rows[i / 16].Cells[i % 16 + 1].Value.ToString(), 16);
            }
            order[length + 7] = method.sum_verify(order, 3, 3);
            return order;
        }
        #region  下发遥控信息
        public byte[] tc_down_data = null;
        private void button11_Click(object sender, EventArgs e)  //加载遥控指令文件
        {
            openFileDialog2.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog2.FileName;
                FileStream fs = File.OpenRead(filename);
                byte[] length = new byte[7];
                fs.Read(length, 0, 7); //文件的前2byte是下发信息的长度
                tc1_order_length = Convert.ToInt32(Convert.ToString(length[4], 2) + Convert.ToString(length[5], 2), 2);
                byte[] tc_data = new byte[tc1_order_length];
                fs.Read(tc_data, 0, tc_data.Length); // tc_data为下发的遥控信息
                this.textBox50.Text = tc_data[1].ToString() + " " + tc_data.Length.ToString();
                method.load_data_to_datagridview(this.dataGridView5, tc_data);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "加载遥控指令; " + saveFileDialog1.FileName;
                this.textBox53.Text = tc1_order_length.ToString();
            }
        }
        int tc1_order_length;
        public void receive_length(int length, string str, string name) //接收指令长度
        {
            if (str == "tc1")
            {
                tc1_order_length = length;
                byte[] data = new byte[tc1_order_length];
                for (int i = 0; i < tc1_order_length; i++)
                {
                    data[i] = 0;
                }
                this.dataGridView5.Rows.Clear();
                method.load_data_to_datagridview(this.dataGridView5, data);
                this.textBox53.Text = tc1_order_length.ToString();

            }
            if (str == "tm")
            {
                tm_order_length = length;
                byte[] data = new byte[tm_order_length];
                for (int i = 0; i < tm_order_length; i++)
                {
                    data[i] = 0;
                }
                this.dataGridView1.Rows.Clear();
                method.load_data_to_datagridview(this.dataGridView1, data);
                this.textBox54.Text = tm_order_length.ToString();
            }
        }
        private void button26_Click(object sender, EventArgs e)  //增加遥控指令
        {
            if (order_set == null || order_set.IsDisposed == true)
            {
                order_set = new Order_Set();
                order_set.send += new Order_Set.send_length(receive_length);
            }
            order_set.Show();
            order_set.str = "tc1";
        }
        private void danzhen_Click(object sender, EventArgs e)  //遥控指令单帧发送
        {
            byte[] tc_frame = generate_order("56", "11", tc1_order_length, this.dataGridView5);
            tc_frame[6] = Convert.ToByte("0c", 16);
            send_order(tc_frame);
            this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 遥控指令单帧发送";
        }

        bool send_id = false;
        private void xunhuan_Click(object sender, EventArgs e)//遥控指令循环发送
        {
            send_id = !send_id;
            byte[] tc_frame = generate_order("56", "11", tc1_order_length, this.dataGridView5);
            if (send_id == true)
            {
                xunhuan.Text = "停止循环发送";
                tc_frame[6] = Convert.ToByte("c0", 16);
                tc_frame[tc_frame.Length - 3] = method.sum_verify(tc_frame, 3, 3);
                send_order(tc_frame);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 遥控指令循环发送";
            }
            else
            {
                xunhuan.Text = "循环发送";
                tc_frame[6] = Convert.ToByte("cc", 16);
                tc_frame[tc_frame.Length - 3] = method.sum_verify(tc_frame, 3, 3);
                send_order(tc_frame);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 停止遥控指令循环发送";
            }
        }

        private void button27_Click(object sender, EventArgs e) //保存遥控指令
        {
            byte[] tc_frame = generate_order("56", "11", tc1_order_length, this.dataGridView5);
            saveFileDialog1.Filter = "*.txt|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, tc_frame);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "保存遥控指令到 " + saveFileDialog1.FileName;
            }
        }
        #endregion

        #region 下发遥测信息
        public byte[] tm_down_data = null;
        int tm_order_length;
        private void button14_Click(object sender, EventArgs e)  //加载遥测文件
        {
            openFileDialog2.Filter = "文本文件(*.txt)|*.txt";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string filename = openFileDialog2.FileName;
                FileStream fs = File.OpenRead(filename);
                byte[] length = new byte[7];
                fs.Read(length, 0, 7); //文件的前2byte是下发信息的长度
                tm_order_length = Convert.ToInt32(Convert.ToString(length[4], 2) + Convert.ToString(length[5], 2), 2);
                byte[] tm_data = new byte[tm_order_length];
                fs.Read(tm_data, 0, tm_data.Length); // tc_data为下发的遥控信息
                this.textBox50.Text = tm_data[1].ToString() + " " + tm_data.Length.ToString();
                method.load_data_to_datagridview(this.dataGridView1, tm_data);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "加载遥测指令; " + openFileDialog2.FileName;
                this.textBox54.Text = tm_order_length.ToString();
            }
        }
        private void button13_Click(object sender, EventArgs e)  //增加遥测指令
        {
            if (order_set == null || order_set.IsDisposed == true)
            { order_set = new Order_Set(); order_set.send += new Order_Set.send_length(receive_length); }
            order_set.Show();
            order_set.str = "tm";
        }
        private void button20_Click(object sender, EventArgs e) //保存遥测文件
        {
            byte[] tm_frame = generate_order("78", "88", tm_order_length, this.dataGridView1);
            saveFileDialog1.Filter = "*.txt|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                File.WriteAllBytes(saveFileDialog1.FileName, tm_frame);
                // fs.WriteAllBytes(saveFileDialog1.FileName, tm_frame);
                //sr.Write(tm_frame, 0, tm_frame.Length);
                //fs.Close();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + "保存遥测指令到 " + saveFileDialog1.FileName;
            }
        }

        private void button10_Click(object sender, EventArgs e) //遥测指令单帧发送
        {
            byte[] tm_frame = generate_order("78", "88", tm_order_length, this.dataGridView1);
            tm_frame[6] = Convert.ToByte("0c", 16);
            send_order(tm_frame);
            this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 遥测指令单帧发送";
        }
        bool tm_is_loop = false;
        private void button9_Click(object sender, EventArgs e) //遥测指令循环发送
        {
            tm_is_loop = !tm_is_loop;
            byte[] tm_frame = generate_order("78", "88", tm_order_length, this.dataGridView1);
            if (tm_is_loop)
            {
                this.button9.Text = "停止循环发送";
                tm_frame[6] = Convert.ToByte("c0", 16);
                tm_frame[tm_frame.Length - 3] = method.sum_verify(tm_frame, 7, 3);
                send_order(tm_frame);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 遥控指令循环发送";
            }
            else
            {
                this.button9.Text = "循环发送";
                tm_frame[6] = Convert.ToByte("cc", 16);
                tm_frame[tm_frame.Length - 3] = method.sum_verify(tm_frame, 7, 3);
                send_order(tm_frame);
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 停止遥控指令循环发送";
            }
        }
        #endregion
        #endregion

        #region 开始接收数据
        bool thread_control = false;
        int thread1_count = 0;
        Thread thread1;
        private void button15_Click(object sender, EventArgs e)  //启动线程
        {
            // ReceiveHandle();

            thread_control = !thread_control;
            if (thread1_count == 0)
            {
                thread1 = new Thread(new ThreadStart(ReceiveHandle));
                thread1.Start();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 启动接收线程";
                this.button15.Text = "暂停接收数据";
            }
            else
            {
                if (!thread_control)
                {
                    thread1.Suspend();
                    this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 暂停接收线程";
                    this.button15.Text = "开始接收数据";
                }
                else
                {
                    thread1.Resume();
                    this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " 重启接收线程";
                    this.button15.Text = "暂停接收数据";
                }
            }
            thread1_count = thread1_count + 1;
        }
        #endregion

        int count = 0;
        public delegate void locked(PictureBox picturebox1, PictureBox picturebox2, bool IsLocked);
        public delegate void locked_num(TextBox textbox, int num);
        public delegate void up_mess(string str);
        public delegate void R_show(byte[] data);
        public delegate void tc1_mess(int count, string str, int sum, int code, double code_rate);
        StreamWriter sw;
        StreamWriter sw_tm;
        StreamWriter sw_tm_decode;
        StreamWriter sw_tc1;
        int frame_count = 0;
        bool find_head = false;
        bool stop = true;
        byte[] receiveData = null;
        byte[] next_receiveData = null;
        void ReceiveHandle()  //today is 8.5 
        {
            frame_count = 0; //帧计数
            byte[] Data = null;
            UdpClient client_receive = null;
            client_receive = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            //本地接收
            sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "上传数据保存.txt");
            sw_tc1 = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "遥控解调数据保存.txt");
            sw_tm = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "遥测数据保存.txt");
            sw_tm_decode = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "遥测解调数据保存.txt");
            while (true)
            {
                Data = client_receive.Receive(ref GlobalVar.RemotePoint);//接收数据
                ReceiveData_Handle(Data);
                //sw.WriteLine(Data);   //将接收到的数据写入text文件
            }
            sw_tm.Close();
            sw_tm_decode.Close();
            sw_tc1.Close();
            client_receive.Close();
        }

        public void ReceiveData_Handle(byte[] Data)
        {
            if (stop && this.IsHandleCreated)
            {
                //receiveData=method.remove_tail(method.remove_header(Data, 8), 4);
                receiveData = Data;
                find_head = method.header_find(receiveData).header_finded;
                if (find_head)
                { stop = false; }
                else
                {
                    //next_receiveData = method.remove_tail(method.remove_header(Data, 8), 4);
                    next_receiveData = Data;
                    byte[] combine_data = new byte[receiveData.Length + next_receiveData.Length];//if 语句有问题，此时receiveData为空
                    combine_data = method.array_joint(receiveData, next_receiveData);
                    FrameData_Handle(receiveData, combine_data);
                    receiveData = next_receiveData;
                }
            }//修改if结构，仍然找到了帧头的数据包不经过处理？
        }
        int tm_frame_count = 0;
        int tc1_frame_count = 0;
        public void FrameData_Handle(byte[] receiveData, byte[] combine_data)
        {
            string combineString = null;
            combineString = method.bytetostring(combine_data);
            for (int i = 0; i < receiveData.Length; i++)
            {
                if (combine_data[i] == Convert.ToByte("eb", 16) && combine_data[i + 1] == Convert.ToByte("90", 16))
                {
                    #region 上传状态信息
                    if (combine_data[i + 2] == Convert.ToByte("45", 16) && this.IsHandleCreated)
                    {
                        int frame_length = 94;//状态输出的帧长度，帧头尾 4 + 类型、校验 2 + 状态输出数据 88(704bit)
                        State_Handle(i, combine_data, combineString, frame_length);
                        i = i + frame_length - 1;
                    }
                    #endregion

                    #region 遥控数据解调
                    if (combine_data[i + 2] == Convert.ToByte("67", 16))
                    {
                        int frame_length = Convert.ToInt32(combine_data[i + 3].ToString("x2") + combine_data[i + 4].ToString("x2"), 16);
                        sw_tc1 = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "遥控数据保存.txt");
                        //if (TC1_statistics.decodedIsset && tc1_write_begin)
                        if (tc1_write_begin)
                        {
                            tc1_frame_count = tc1_frame_count + 1;
                            string frame_string = null;
                            for (int j = 0; j < frame_length + 8; j++)
                            {
                                frame_string = frame_string + combine_data[j].ToString("x2") + " ";
                            }
                            sw_tc1.WriteLine(tc1_frame_count.ToString() + " " + frame_string);
                        }
                        TC1_Handle(i, combine_data, combineString, frame_length);
                        i = i + frame_length - 1;
                    }
                    #endregion

                    #region 遥测数据解调
                    if (combine_data[i + 2] == Convert.ToByte("89", 16))
                    {
                        int frame_length = Convert.ToInt32(combine_data[i + 3].ToString("x2") + combine_data[i + 4].ToString("x2"), 16);
                        //if (TM_statistics.decodedIsset && tm_write_begin)
                        if (tm_write_begin)
                        {
                            tm_frame_count = tm_frame_count + 1;
                            string frame_string = null;
                            for (int j = 0; j < frame_length + 8; j++)
                            {
                                frame_string = frame_string + combine_data[j].ToString("x2") + " ";
                            }
                            sw_tm.WriteLine(tm_frame_count.ToString() + " " + frame_string);
                        }
                        TM_Handle(i, combine_data, combineString, frame_length, tm_frame_count);
                        i = i + frame_length - 1;

                    }
                    #endregion

                    #region 测量帧信息
                    if (combine_data[i + 2] == Convert.ToByte("9a", 16) && this.IsHandleCreated)   //9a
                    {
                        //int frame_length = Convert.ToInt32(combine_data[i + 3].ToString("x2") + combine_data[i + 4].ToString("x2"), 16);
                        int frame_length = 126 + 6;
                        //int frame_length = 94; 
                        RG_Handle(i, combine_data, combineString, frame_length);
                        i = i + frame_length - 1;
                    }
                    #endregion
                }

            }

        }

        #region 状态数据处理
        public void State_Handle(int i, byte[] combine_data, string combineString, int frame_length)
        {
            string str = null;
            byte[] frame = new byte[frame_length];
            str = combineString.Substring(i * 8, frame_length * 8);//取出第i个数据，每个数据长94
            for (int cc = 0; cc < frame_length; cc++)
            {
                frame[cc] = combine_data[i + cc];
            }
            string str1;
            str1 = method.Fill_Zero(Convert.ToString(method.sum_verify(frame), 2), 8);
            str = str + str1;//数据后8位续上frame的校验和
            frame_count = frame_count + 1;
            str1 = Convert.ToString(frame_count, 2);
            str1 = method.Fill_Zero(str1, 32);
            str = str + str1;//再续上32位帧计数
            //倒数32位是帧计数，倒数33-40是和校验数

            up_mess up_mess_show = new up_mess(Up_Show);
            this.BeginInvoke(up_mess_show, new object[] { str });
            //string frame_string = null;
            //for (int j = 0; j < frame_length; j++)
            //{
            //    frame_string = frame_string + frame[j].ToString() + " ";
            //}
            //sw.WriteLine(frame_string);   
        }
        public void Up_Show(string str1)
        {
            string[] bs = new string[GlobalVar.b.Length];
            for (int i = 0; i < GlobalVar.b.Length; i++)
            {
                //bs[i] = method.Fill_Zero(Convert.ToString(b[i], 2), 8); //将每个byte数据低位填0，计算时结果不变
            }
            string str;
            //this.textBox43.Text = str1;
            double power = System.Math.Pow(2, 32);//2^32
            double m = 1000000.0;
            double p1 = 80.0;
            str = str1.Substring(str1.Length - 32, 32);  //帧计数 
            this.textBox37.Text = Convert.ToUInt32(str, 2).ToString();  //状态帧计数
            str = str1.Substring(str1.Length - 40, 8); //和校验数
            this.textBox41.Text = Convert.ToByte(str, 2).ToString();
            UInt32[] center_fre = new UInt32[8];  //8个通道的中心频率
            UInt32[] code_rate = new UInt32[8]; //8个通道的码速率
            // 下面是 遥控、测量、下行测量、下行遥控通道的中心频率和码速率
            int count = 25;
            center_fre[0] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 100;
            center_fre[1] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 176;
            center_fre[2] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 226;
            center_fre[3] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 276;
            center_fre[4] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 326;
            center_fre[5] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 374;
            center_fre[6] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 393;
            center_fre[7] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            //码速率
            count = 60;
            code_rate[0] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 135;
            code_rate[1] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 210;
            code_rate[2] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 260;
            code_rate[3] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 310;
            code_rate[4] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 360;
            code_rate[5] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 387;
            code_rate[6] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            count = 406;
            code_rate[7] = Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2);
            // 下面是 遥控测量、下行遥控遥测 载波伪码 多普勒 （载波-中心频率）（伪码-码速率） 
            str = str1.Substring(24, 32);
            this.textBox14.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[0]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 32, 32);
            this.textBox13.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[0]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 2 * 32, 32);
            this.textBox15.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[1]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 3 * 32, 32);
            this.textBox16.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[1]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 4 * 32, 32);
            this.textBox19.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[2]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 4 * 32, 32);
            this.textBox20.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[2]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 6 * 32, 32);
            this.textBox23.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[3]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 7 * 32, 32);
            this.textBox24.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[3]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 8 * 32, 32);
            this.textBox27.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[4]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 9 * 32, 32);
            this.textBox28.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[4]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 10 * 32, 32);
            this.textBox31.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[5]) * p1 * m / power).ToString("f6");
            str = str1.Substring(24 + 11 * 32, 32);
            this.textBox32.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[5]) * p1 * m / power).ToString("f6");
            string[] down_rx = new string[3];
            str = str1.Substring(24 + 12 * 32, 32);
            this.textBox39.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[6]) * p1 * m / power).ToString("f6");//下行测量载波
            down_rx[0] = this.textBox39.Text;
            str = str1.Substring(24 + 13 * 32, 32);
            this.textBox40.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[6]) * p1 * m / power).ToString("f6");//下行测量伪码
            down_rx[1] = this.textBox40.Text;
            str = str1.Substring(24 + 14 * 32, 32);
            this.textBox35.Text = ((long)(Convert.ToUInt32(str, 2) - (long)center_fre[7]) * p1 * m / power).ToString("f6");//下行遥测载波
            str = str1.Substring(24 + 15 * 32, 32);
            this.textBox36.Text = ((long)(Convert.ToUInt32(str, 2) - (long)code_rate[7]) * p1 * m / power).ToString("f6");//下行遥测伪码
            //遥控1锁定、SNR、捕获时间，共占28字节
            str = str1.Substring(24 + 16 * 32 + 1, 1);
            if (str == "1")
            {
                this.pictureBox2.BackColor = Color.Green;
                this.pictureBox26.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox2.BackColor = Color.Red;
                this.pictureBox26.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 16 * 32 + 2, 1);
            if (str == "1")
            {
                this.pictureBox3.BackColor = Color.Green;
                this.pictureBox27.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox3.BackColor = Color.Red;
                this.pictureBox27.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 16 * 32 + 3, 1);
            if (str == "1")
            {
                this.pictureBox4.BackColor = Color.Green;
                this.pictureBox28.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox4.BackColor = Color.Red;
                this.pictureBox28.BackColor = Color.Red;
            }
            str = str1.Substring(28 + 16 * 32, 8);
            this.textBox11.Text = Convert.ToByte(str, 2).ToString();//SNR
            this.textBox44.Text = Convert.ToByte(str, 2).ToString();//TC SNR
            //this.textBox11.Text = "ssssss";
            str = str1.Substring(36 + 16 * 32, 16);
            this.textBox12.Text = Convert.ToUInt32(str, 2).ToString();//捕获时间
            //遥控2锁定、SNR、捕获时间
            str = str1.Substring(52 + 16 * 32 + 1, 1);
            if (str == "1")
            { this.pictureBox5.BackColor = Color.Green; }
            else
            { this.pictureBox5.BackColor = Color.Red; }
            str = str1.Substring(52 + 16 * 32 + 2, 1);
            if (str == "1")
            { this.pictureBox6.BackColor = Color.Green; }
            else
            { this.pictureBox6.BackColor = Color.Red; }
            str = str1.Substring(52 + 16 * 32 + 3, 1);
            if (str == "1")
            { this.pictureBox7.BackColor = Color.Green; }
            else
            { this.pictureBox7.BackColor = Color.Red; }
            str = str1.Substring(56 + 16 * 32, 8);
            this.textBox18.Text = Convert.ToByte(str, 2).ToString();
            str = str1.Substring(64 + 16 * 32, 16);
            this.textBox17.Text = Convert.ToUInt16(str, 2).ToString();
            //测量1锁定、SNR、捕获时间
            str = str1.Substring(24 + 28 * 2 + 16 * 32 + 1, 1);
            if (str == "1")
            { this.pictureBox8.BackColor = Color.Green; }
            else
            { this.pictureBox8.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 2 + 16 * 32 + 2, 1);
            if (str == "1")
            { this.pictureBox9.BackColor = Color.Green; }
            else
            { this.pictureBox9.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 2 + 16 * 32 + 3, 1);
            if (str == "1")
            { this.pictureBox10.BackColor = Color.Green; }
            else
            { this.pictureBox10.BackColor = Color.Red; ; }
            str = str1.Substring(24 + 28 * 2 + 4 + 16 * 32, 8);
            this.textBox22.Text = Convert.ToByte(str, 2).ToString();
            str = str1.Substring(24 + 28 * 2 + 4 + 8 + 16 * 32, 16);
            this.textBox21.Text = Convert.ToUInt16(str, 2).ToString();
            //测量2锁定、SNR、捕获时间
            str = str1.Substring(24 + 28 * 3 + 16 * 32 + 1, 1);
            if (str == "1")
            { this.pictureBox11.BackColor = Color.Green; }
            else
            { this.pictureBox11.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 3 + 16 * 32 + 2, 1);
            if (str == "1")
            { this.pictureBox12.BackColor = Color.Green; }
            else
            { this.pictureBox12.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 3 + 16 * 32 + 3, 1);
            if (str == "1")
            { this.pictureBox13.BackColor = Color.Green; }
            else
            { this.pictureBox13.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 3 + 4 + 16 * 32, 8);
            this.textBox26.Text = Convert.ToByte(str, 2).ToString();
            str = str1.Substring(24 + 28 * 3 + 4 + 8 + 16 * 32, 16);
            this.textBox25.Text = Convert.ToUInt16(str, 2).ToString();
            //测量3锁定、SNR、捕获时间
            str = str1.Substring(24 + 28 * 4 + 16 * 32 + 1, 1);
            if (str == "1")
            { this.pictureBox14.BackColor = Color.Green; }
            else
            { this.pictureBox14.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 4 + 16 * 32 + 2, 1);
            if (str == "1")
            { this.pictureBox15.BackColor = Color.Green; }
            else
            { this.pictureBox15.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 4 + 16 * 32 + 3, 1);
            if (str == "1")
            { this.pictureBox16.BackColor = Color.Green; }
            else
            { this.pictureBox16.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 4 + 4 + 16 * 32, 8);
            this.textBox30.Text = Convert.ToByte(str, 2).ToString();
            str = str1.Substring(24 + 28 * 4 + 4 + 8 + 16 * 32, 16);
            this.textBox29.Text = Convert.ToUInt16(str, 2).ToString();
            //测量4锁定、SNR、捕获时间
            str = str1.Substring(24 + 28 * 5 + 16 * 32 + 1, 1);
            if (str == "1")
            { this.pictureBox17.BackColor = Color.Green; ; }
            else
            { this.pictureBox17.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 5 + 16 * 32 + 2, 1);
            if (str == "1")
            { this.pictureBox18.BackColor = Color.Green; }
            else
            { this.pictureBox18.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 5 + 16 * 32 + 3, 1);
            if (str == "1")
            { this.pictureBox19.BackColor = Color.Green; }
            else
            { this.pictureBox19.BackColor = Color.Red; }
            str = str1.Substring(24 + 28 * 5 + 4 + 16 * 32, 8);
            this.textBox34.Text = Convert.ToByte(str, 2).ToString();
            str = str1.Substring(24 + 28 * 5 + 4 + 8 + 16 * 32, 16);
            this.textBox33.Text = Convert.ToUInt16(str, 2).ToString();

            //下行测量锁定、SNR
            str = str1.Substring(24 + 28 * 6 + 16 * 32 + 1, 1);
            if (str == "1")
            {
                this.pictureBox20.BackColor = Color.Green;
                this.pictureBox29.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox20.BackColor = Color.Red;
                this.pictureBox29.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 28 * 6 + 16 * 32 + 2, 1);
            if (str == "1")
            {
                this.pictureBox21.BackColor = Color.Green;
                this.pictureBox30.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox21.BackColor = Color.Red;
                this.pictureBox30.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 28 * 6 + 16 * 32 + 3, 1);
            if (str == "1")
            {
                this.pictureBox22.BackColor = Color.Green;
                this.pictureBox32.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox22.BackColor = Color.Red;
                this.pictureBox32.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 28 * 6 + 4 + 16 * 32, 8);
            this.textBox42.Text = Convert.ToByte(str, 2).ToString();
            down_rx[0] = this.textBox42.Text;

            List<RX_signal> rx_signal = new List<RX_signal>();
            rx_signal.Add(new RX_signal() { name = "名称", Ranging = "下行测量状态" });
            rx_signal.Add(new RX_signal() { name = "载波NCO", Ranging = down_rx[0] });
            rx_signal.Add(new RX_signal() { name = "伪码NCO", Ranging = down_rx[1] });
            rx_signal.Add(new RX_signal() { name = "SNR", Ranging = down_rx[2] });
            this.dataGridView4.DataSource = rx_signal;
            //下行遥测锁定、SNR
            str = str1.Substring(24 + 28 * 6 + 12 + 16 * 32 + 1, 1);
            if (str == "1")
            {
                this.pictureBox23.BackColor = Color.Green;
                this.pictureBox34.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox23.BackColor = Color.Red;
                this.pictureBox34.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 28 * 6 + 12 + 16 * 32 + 2, 1);
            if (str == "1")
            {
                this.pictureBox24.BackColor = Color.Green;
                this.pictureBox33.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox24.BackColor = Color.Red;
                this.pictureBox33.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 28 * 6 + 12 + 16 * 32 + 3, 1);
            if (str == "1")
            {
                this.pictureBox25.BackColor = Color.Green;
                this.pictureBox31.BackColor = Color.Green;
            }
            else
            {
                this.pictureBox25.BackColor = Color.Red;
                this.pictureBox31.BackColor = Color.Red;
            }
            str = str1.Substring(24 + 28 * 6 + 12 + 4 + 16 * 32, 8);
            this.textBox38.Text = Convert.ToByte(str, 2).ToString();
            this.textBox6.Text = Convert.ToByte(str, 2).ToString();
        }
        #endregion

        #region 遥控数据处理
        Error_Rate_Statistics TC1_statistics = new Error_Rate_Statistics();
        public void TC1_Handle(int i, byte[] combine_data, string combineString, int frame_length)
        {
            byte[] frame = new byte[frame_length];
            for (int j = 0; j < frame_length; j++)
            {
                frame[j] = combine_data[i + j];        //此frame为遥控帧 包含了EB 90 ；
            }
            TC1_statistics.frame = frame;
            TC1_statistics.sum_verify = TC1_statistics.verify();
            TC1_statistics.single_str = combineString.Substring(i * 8 + 40, frame_length * 8);
            TC1_statistics.combine_str = TC1_statistics.last_str + TC1_statistics.single_str;
            bool connect = false;
            double last_rate = 0;

            if (TC1_statistics.decodedIsset && TC1_statistics.single_str.Length > TC1_statistics.frame_head.Length)  //帧头不长于一帧,而且开始解调
            {

                for (int jj = TC1_statistics.begin_pos; jj < TC1_statistics.single_str.Length; )
                {
                    if (TC1_statistics.combine_str.Substring(jj, TC1_statistics.frame_head.Length) == TC1_statistics.frame_head && this.IsHandleCreated)
                    {
                        try
                        {
                            TC1_statistics.decode_frame = TC1_statistics.combine_str.Substring(jj, TC1_statistics.frame_length * 8);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("error:" + ex);
                            connect = true;
                            TC1_statistics.begin_pos = jj; //当长度不够时，要记住当时截取的开始位置
                            break;
                        }
                        jj = jj + TC1_statistics.frame_length * 8;
                        if (jj > TC1_statistics.single_str.Length) { TC1_statistics.begin_pos = jj - TC1_statistics.single_str.Length; } //截取到下一帧，那么记住下次截取位置
                        if (TC1_statistics.combine_str.Substring(jj, TC1_statistics.frame_head.Length) == TC1_statistics.frame_head)
                        {
                            locked tc1_locked = new locked(locked_show);
                            this.BeginInvoke(tc1_locked, new object[] { this.pictureBox35, this.pictureBox38, true });
                        }
                        else
                        {
                            locked tc1_locked = new locked(locked_show);
                            this.BeginInvoke(tc1_locked, new object[] { this.pictureBox35, this.pictureBox38, false });
                            TC1_statistics.losing_lock_num = TC1_statistics.losing_lock_num + 1;
                        }

                        if (TC1_statistics.statistics_begin)
                        {
                            TC1_statistics.statistics();
                            if (TC1_statistics.error_code_sum > last_rate)
                            {
                                this.textBox50.Text = this.textBox50.Text + "wrong! ";
                            }
                            last_rate = TC1_statistics.error_code_sum;
                        }
                        tc1_mess tc1_mess_show = new tc1_mess(tc1_show);
                        locked_num losing_lock = new locked_num(losing_lock_num_show);
                        this.BeginInvoke(tc1_mess_show, new object[] { TC1_statistics.count, TC1_statistics.decode_frame, TC1_statistics.receive_sum, TC1_statistics.error_code_sum, TC1_statistics.error_code_rate });
                        this.BeginInvoke(losing_lock, new object[] { this.textBox55, TC1_statistics.losing_lock_num });
                    }
                    else
                    {
                        jj = jj + 1;
                    }
                }

            }
            if (!connect)
            {
                TC1_statistics.last_str = TC1_statistics.single_str;
            }
            else
            {
                TC1_statistics.last_str = TC1_statistics.combine_str;
            }
        }
        public void locked_show(PictureBox picturebox1, PictureBox picturebox2, bool islocked)
        {
            if (islocked)
            {
                picturebox1.BackColor = Color.Green;
                picturebox2.BackColor = Color.Green;
            }
            else
            {
                picturebox1.BackColor = Color.Red;
                picturebox2.BackColor = Color.Red;
            }
        }
        public void losing_lock_num_show(TextBox textbox, int num)
        {
            textbox.Text = num.ToString();
        }

        public void tc1_show(int count, string str, int sum, int code, double code_rate)
        {
            string new_str = null;
            for (int i = 0; i < str.Length / 8; i++)
            {
                new_str = new_str + " " + (Convert.ToByte(str.Substring(i * 8, 8), 2)).ToString("x2");
            }
            if (Regex.Matches(this.richTextBox2.Text, "\n").Count > 10)
            {
                this.richTextBox2.Text = this.richTextBox2.Text.Remove(0, this.richTextBox2.Text.IndexOf("\n") + 1);
            }
            this.richTextBox2.Text = this.richTextBox2.Text + new_str + "\n";
            this.richTextBox2.Select(this.richTextBox2.TextLength, 0);
            this.richTextBox2.ScrollToCaret();
            this.textBox43.Text = count.ToString();  //此处的遥控帧计数是指解调到的遥控帧
            this.textBox3.Text = sum.ToString();
            this.textBox4.Text = code.ToString();
            this.textBox5.Text = code_rate.ToString("f6");
            str_richbox = this.richTextBox2.Text;
            this.textBox52.Text = Regex.Matches(str_richbox, "\n").Count.ToString();
        }
        bool tc1_write_begin = false;
        private void button19_Click(object sender, EventArgs e) //依据设置参数开始遥控解调
        {
            TC1_statistics.decodedIsset = !TC1_statistics.decodedIsset;
            if (TC1_statistics.decodedIsset && method.remove_char(this.textBox2.Text) != null && method.remove_char(this.textBox1.Text) != null)
            {
                TC1_statistics.frame_length = Convert.ToUInt16(this.textBox2.Text);
                TC1_statistics.frame_head = method.stringtobitstring(method.remove_char(this.textBox1.Text));
                this.button19.Text = "停止解调";
            }
            if (!TC1_statistics.decodedIsset)
            {
                this.button19.Text = "开始解调";
                TC1_statistics.count = 0;//停止解调后把解调到的遥控帧设为0
                TC1_statistics.losing_lock_num = 0;//停止解调后把失锁数设为0
            }
        }
        private bool button18_Click(object sender, EventArgs e) //保存遥控信息
        {
            tc1_write_begin = !tc1_write_begin;
            saveFileDialog1.Filter = "*.txt|*.txt";
            if (tc1_write_begin == true)
            {
                //sw_tc1 = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "遥控原始数据保存.txt");
                //sw_tc1.Write(data)
                this.button18.Text = "停止保存信息";
            }
            else
            {
                //sw_tc.Close();
                this.button18.Text = "保存遥控信息";
            }
            return tc1_write_begin;
        }

        private void button17_Click(object sender, EventArgs e) //开始遥控统计
        {
            TC1_statistics.statistics_begin = !TC1_statistics.statistics_begin;
            if (TC1_statistics.statistics_begin && this.richTextBox7.Text != null)
            {
                TC1_statistics.receive_sum = 0;
                TC1_statistics.error_code_sum = 0;
                TC1_statistics.error_code_rate = 0;
                try
                {
                    TC1_statistics.base_string = method.stringtobitstring(method.remove_char(this.richTextBox7.Text));
                    this.button17.Text = "停止统计";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("异常: " + ex);
                }
            }
            else
            {
                this.textBox3.Text = "0";
                this.textBox4.Text = "0";
                this.textBox5.Text = "0";
                this.button17.Text = "开始统计";
            }
        }
        #endregion

        #region 遥测数据处理

        Error_Rate_Statistics TM_statistics = new Error_Rate_Statistics();
        public void TM_Handle(int i, byte[] combine_data, string combineString, int frame_length, int frame_count)
        {
            byte[] frame = new byte[frame_length];

            for (int j = 0; j < frame_length; j++)
            {
                frame[j] = combine_data[i + j];
            }
            bool connect = false;
            TM_statistics.frame = frame;
            TM_statistics.sum_verify = TM_statistics.verify();
            TM_statistics.single_str = combineString.Substring(i * 8 + 40, frame_length * 8);
            TM_statistics.combine_str = TM_statistics.last_str + TM_statistics.single_str;

            if (TM_statistics.decodedIsset && TM_statistics.single_str.Length > TM_statistics.frame_head.Length)  //帧头不长于一帧,而且串接了两帧
            {
                for (int jj = 0; jj < TM_statistics.single_str.Length; )
                {
                    if (TM_statistics.combine_str.Substring(jj, TM_statistics.frame_head.Length) == TM_statistics.frame_head && this.IsHandleCreated)
                    {
                        try
                        {
                            TM_statistics.decode_frame = TM_statistics.combine_str.Substring(jj, TM_statistics.frame_length * 8);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("error:" + ex);
                            connect = true;
                            break;
                        }
                        jj = jj + TM_statistics.frame_length * 8;
                        if (jj > TM_statistics.single_str.Length) { TM_statistics.begin_pos = jj - TM_statistics.single_str.Length; } //截取到下一帧，那么记住下次截取位置
                        if (TM_statistics.combine_str.Substring(jj, TM_statistics.frame_head.Length) == TM_statistics.frame_head)
                        {
                            locked tm_locked = new locked(locked_show);
                            this.BeginInvoke(tm_locked, new object[] { this.pictureBox37, this.pictureBox31, true });
                        }
                        else
                        {
                            locked tm_locked = new locked(locked_show);
                            this.BeginInvoke(tm_locked, new object[] { this.pictureBox37, this.pictureBox31, false });
                            TM_statistics.losing_lock_num = TM_statistics.losing_lock_num + 1;
                        }
                        if (TM_statistics.statistics_begin)
                        {
                            TM_statistics.statistics();
                            if (TM_statistics.error_code_sum > 0)
                            {
                                string frame_string = null;
                                for (int j = 0; j < TM_statistics.decode_frame.Length / 8; j++)
                                {
                                    frame_string = frame_string + Convert.ToByte(TM_statistics.decode_frame.Substring(j * 8, 8), 2).ToString("x2") + " ";
                                }
                                sw_tm_decode.WriteLine(frame_count.ToString() + " " + frame_string + " wrong" + frame_count.ToString());
                            }
                            else
                            {
                                string frame_string = null;
                                for (int j = 0; j < TM_statistics.decode_frame.Length / 8; j++)
                                {
                                    frame_string = frame_string + Convert.ToByte(TM_statistics.decode_frame.Substring(j * 8, 8), 2).ToString("x2") + " ";
                                }
                                sw_tm_decode.WriteLine(frame_count.ToString() + " " + frame_string);
                            }

                        }

                        tc1_mess tc1_mess_show = new tc1_mess(tm_show);
                        this.BeginInvoke(tc1_mess_show, new object[] { TM_statistics.count, TM_statistics.decode_frame, TM_statistics.receive_sum, TM_statistics.error_code_sum, TM_statistics.error_code_rate });
                        locked_num losing_lock = new locked_num(losing_lock_num_show);
                        this.BeginInvoke(losing_lock, new object[] { this.textBox56, TM_statistics.losing_lock_num });
                    }
                    else { jj = jj + 1; }
                }
            }
            if (!connect)
            {
                TM_statistics.last_str = TM_statistics.single_str;
            }
            else
            {
                TM_statistics.last_str = TM_statistics.combine_str;
            }
        }
        string str_richbox = "123";
        public void tm_show(int count, string str, int sum, int code, double code_rate)
        {
            string new_str = null;
            if (Regex.Matches(this.richTextBox4.Text, "\n").Count > 10)
            {
                this.richTextBox4.Text = this.richTextBox4.Text.Remove(0, this.richTextBox4.Text.IndexOf("\n") + 1);
            }
            for (int i = 0; i < str.Length / 8; i++)
            {
                new_str = new_str + " " + (Convert.ToByte(str.Substring(i * 8, 8), 2)).ToString("x2");
            }
            this.richTextBox4.Text = this.richTextBox4.Text + new_str + "\n";//Environment.NewLine;
            this.richTextBox4.Select(this.richTextBox4.TextLength, 0);
            this.richTextBox4.ScrollToCaret();
            str_richbox = this.richTextBox4.Text;
            //this.textBox52.Text = count.ToString();
            this.textBox52.Text = Regex.Matches(str_richbox, "\n").Count.ToString();
            //this.textBox52.Text = this.richTextBox4.Text.Length.ToString();
            this.textBox9.Text = sum.ToString();
            this.textBox8.Text = code.ToString();
            this.textBox7.Text = code_rate.ToString("f6");
        }
        private void button28_Click(object sender, EventArgs e)//依据设置参数开始遥测解调
        {
            TM_statistics.decodedIsset = !TM_statistics.decodedIsset;
            if (TM_statistics.decodedIsset && method.remove_char(this.textBox51.Text) != null && method.remove_char(this.textBox10.Text) != null)
            {
                try
                {
                    TM_statistics.frame_length = Convert.ToUInt16(this.textBox10.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("未设置遥测解调参数" + ex);
                }
                TM_statistics.frame_head = method.stringtobitstring(method.remove_char(this.textBox51.Text));
                this.button28.Text = "停止解调";
            }

            if (!TM_statistics.decodedIsset)
            {
                this.button28.Text = "开始解调";
                TM_statistics.count = 0;//停止解调后把解调到的遥控帧设为0
                TM_statistics.losing_lock_num = 0;//停止解调后把失锁数设为0
            }
        }
        bool tm_write_begin = false;
        private void button29_Click(object sender, EventArgs e) //保存遥测信息
        {
            tm_write_begin = !tm_write_begin;
            if (tm_write_begin == true)
            {
                this.button29.Text = "停止保存信息";
            }
            if (!tm_write_begin == true)
            {
                this.button29.Text = "保存遥测信息";
            }
        }
        private void button30_Click(object sender, EventArgs e) //开始遥测解调统计
        {
            TM_statistics.statistics_begin = !TM_statistics.statistics_begin;
            if (TM_statistics.statistics_begin && this.richTextBox1.Text != null)
            {
                TM_statistics.receive_sum = 0;
                TM_statistics.error_code_sum = 0;
                TM_statistics.error_code_rate = 0;
                try
                {
                    TM_statistics.base_string = method.stringtobitstring(method.remove_char(this.richTextBox1.Text));
                    this.button30.Text = "停止统计";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("异常: " + ex);
                }
            }
            else
            {
                this.textBox9.Text = "0";
                this.textBox8.Text = "0";
                this.textBox7.Text = "0";
                this.button30.Text = "开始统计";
            }

        }
        #endregion

        #region 测量数据处理
        public void RG_Handle(int i, byte[] combine_data, string combineString, int frame_length)
        {
            string str = null;
            byte[] frame = new byte[frame_length];
            str = combineString.Substring(i * 8, frame_length * 8);
            for (int cc = 0; cc < frame_length; cc++)
            {
                frame[cc] = combine_data[i + cc];
            }
            up_mess up_mess_show = new up_mess(Range_show);
            this.BeginInvoke(up_mess_show, new object[] { str });

            //string frame_string = null;
            //for (int j = 0; j < frame_length; j++)
            //{
            //    frame_string = frame_string + frame[j].ToString() + " ";
            //}
            //sw.WriteLine(frame_string); 
        }
        #endregion

        //下行帧计数

        byte last_RG_frame_count = 0;
        public void Range_show(string strall)  //测量帧显示
        {
            int RG_frame_count = 0;
            int i = 0;
            string str1 = null;
            string str = null;
            #region//下行测量帧显示
            byte[] data = new byte[] { 8, 2, 50, 7, 13, 90, 90, 90, 90, 24, 16, 20 }; //500位测量帧
            byte[] RR_data = new byte[] { 1, 1, 1, 1, 1, 1, 4, 8, 2, 9, 5, 10, 16, 30 };//每个通道90位
            byte[] R_data = new byte[] { 1, 1, 1, 1, 1, 1, 8, 9, 5, 10, 16, 30 }; //通道内的有效位
            str = strall.Substring((3 + 63) * 8, 63 * 8);   //str为下行测量帧字符串
            string uplink_str = strall.Substring(3 * 8, 63 * 8);
            //去掉空白位
            for (int j = 0; j < 4; j++)
            {
                str = str.Remove(80 + 86 * j + 6, 4);
                uplink_str = uplink_str.Remove(80 + 86 * j + 6, 4);  //去掉空闲位
            }
            for (int j = 0; j < 4; j++)
            {
                str = str.Remove(80 + 84 * j + 14, 2);
                uplink_str = uplink_str.Remove(80 + 84 * j + 14, 2); //去掉空闲位
                //uplink_str = uplink_str.Remove(80 + 84 * j + 14, 2);
            }
            string downlink_str = str;  //定义下行测量帧
            string[,] arr = new string[R_data.Length, 4];
            string[,] arr_up = new string[R_data.Length, 4];
            int sum = 0;
            for (int m = 0; m < R_data.Length; m++)
            {
                for (int n = 0; n < 4; n++)
                {
                    arr[m, n] = str.Substring(80 + 84 * n + sum, R_data[m]);  //下行测量帧有效位变为数组
                    arr_up[m, n] = uplink_str.Substring(80 + 84 * n + sum, R_data[m]);//上行测量帧有效位变为数组
                }
                sum = sum + R_data[m];
            }
            str1 = str.Substring(0, data[0]);
            i = data[0];
            this.textBox48.Text = Convert.ToByte(str1, 2).ToString(); //帧计数
            str1 = str.Substring(i, data[1]);
            i = i + data[1];
            if (RG_frame_count == 0)
            {
                last_RG_frame_count = Convert.ToByte(str1, 2);
            }
            else
            {
                int abs = Math.Abs(last_RG_frame_count - Convert.ToByte(str1, 2));
                if (abs > 1 && abs != 255) { this.textBox50.Text = "出现丢包！"; }
                last_RG_frame_count = Convert.ToByte(str1, 2);
            }
            RG_frame_count = RG_frame_count + 1;

            this.textBox45.Text = Convert.ToByte(str1, 2).ToString();//通道数
            str1 = str.Substring(i, data[2]);
            i = i + data[2];
            this.textBox46.Text = Convert.ToUInt64(str1, 2).ToString("x"); //采样时刻
            str1 = str.Substring(i, data[3]);
            i = i + data[3];
            this.textBox47.Text = Convert.ToByte(str1, 2).ToString();//AGC电压         
            str1 = str.Substring(i, data[4]); //chip相位
            i = i + data[4];
            this.textBox49.Text = Convert.ToUInt32(str1, 2).ToString();
            string modify = null;
            for (int kk = 0; kk < 4; kk++)
            {
                modify = arr[R_data.Length - 1, kk].Substring(0, 1);
                arr[R_data.Length - 1, kk] = modify + modify + arr[R_data.Length - 1, kk];  //星上多普勒频偏
            }
            //*****************************************
            string[] enterclose = new string[] { "名称", "R1", "R2", "R2", "R4" };
            string[] Name = new string[]{ "伪码锁定", "载波锁定", "位同步", "帧同步", "前一采样伪码锁定", "前一采样载波锁定", "信噪比", "测量帧位计数",
                                "伪码周期数", "伪码相位", "伪码CHIP相位", "载波多普勒频偏" };
            List<R_Signal> r_signal = new List<R_Signal>();
            for (int j = 0; j < Name.Length; j++)
            {
                r_signal.Add(new R_Signal()
                    {
                        name = Name[j],
                        R1 = Convert.ToUInt32(arr[j, 0], 2).ToString(),
                        R2 = Convert.ToUInt32(arr[j, 1], 2).ToString(),
                        R3 = Convert.ToUInt32(arr[j, 2], 2).ToString(),
                        R4 = Convert.ToUInt32(arr[j, 3], 2).ToString()
                    }
                    );
            }

            dataGridView2.DataSource = r_signal;
            dataGridView2.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;  //调整行高
            //dataGridView2.ColumnHeadersHeight = 20;  //设置列标题高度
            dataGridView2.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
            #endregion

            #region//测距计算
            str = uplink_str;  //str为上行测距帧
            string[] up_doppler = new string[4];
            for (int kk = 0; kk < 4; kk++)
            {
                up_doppler[kk] = str.Substring(80 + kk * 84 + 54, 30);  //上行多普勒频偏
            }

            for (int kk = 0; kk < 4; kk++)
            {
                modify = up_doppler[kk].Substring(0, 1);
                up_doppler[kk] = modify + modify + up_doppler[kk];  //地面（上行）多普勒频偏 
            }
            this.textBox50.Text = (Convert.ToInt32(str.Substring(80 + 14, 9), 2)).ToString();//下行通道1测量帧内计数
            byte[] data1 = new byte[] { 9, 5, 10, 16 };
            sum = 14;//8+6*1
            string[,] arr1 = new string[4, 4];
            string[,] arr2 = new string[4, 4];
            double[,] arr1_data = new double[4, 4];
            double[,] arr2_data = new double[4, 4];
            for (int n = 0; n < arr1.GetLength(1); n++)
            {
                for (int m = 0; m < arr1.GetLength(0); m++)
                {
                    arr1[m, n] = str.Substring(80 + m * 84 + sum, data1[n]);//地面（上行）有效字符数组
                    arr1_data[m, n] = (double)Convert.ToUInt32(arr1[m, n], 2);//地面（上行）有效数组
                    arr2[m, n] = downlink_str.Substring(80 + m * 84 + sum, data1[n]);//星上（下行）有效字符数组
                    arr2_data[m, n] = (double)Convert.ToUInt32(arr2[m, n], 2);//星上（下行）有效数组
                }
                sum = sum + data1[n];
            }
            List<R_Signal> rx_signal = new List<R_Signal>();
            string[] name_rx = new string[] { "距离(m)", "速度(m/s)", "速度", "星地时差" };
            rx_signal.Add(
                   new R_Signal()
                   {
                       name = "名称",
                       R1 = "通道1",
                       R2 = "通道2",
                       R3 = "通道3",
                       R4 = "通道4"
                   }
                   );

            for (int jj = 0; jj < 6; jj++)
            {
                rx_signal.Add(new R_Signal()
                    {
                        name = Name[jj + 6],
                        R1 = Convert.ToUInt32(arr_up[jj + 6, 0], 2).ToString(),
                        R2 = Convert.ToUInt32(arr_up[jj + 6, 1], 2).ToString(),
                        R3 = Convert.ToUInt32(arr_up[jj + 6, 2], 2).ToString(),
                        R4 = Convert.ToUInt32(arr_up[jj + 6, 3], 2).ToString()
                    });
            }

            double[] code_rate = new double[4];
            double[] info_rate = new double[4];
            for (int m = 0; m < 4; m++)
            {
                string str_coderate = null;
                string str_inforate = null;
                for (int n = 0; n < 4; n++)
                {
                    str_coderate = str_coderate + method.Fill_Zero(Convert.ToString(GlobalVar.b[21 + GlobalVar.channel_length * 2 + 39 + GlobalVar.channel_length * m + n], 2), 8);
                    str_inforate = str_inforate + method.Fill_Zero(Convert.ToString(GlobalVar.b[21 + GlobalVar.channel_length * 2 + 56 + GlobalVar.channel_length * m + n], 2), 8);
                }
                code_rate[m] = (long)((double)(Convert.ToUInt32(str_coderate, 2)) / power * p1 * 1000000);
                info_rate[m] = (long)(code_rate[m] / ((double)(Convert.ToUInt32(str_inforate, 2))) / 1023);
            }
            double[] up_tx = new double[4];  //上行测量时刻
            double[] down_tx = new double[4];//下行测量时刻
            for (int kk = 0; kk < 4; kk++)  //此处没有采用kk
            {
                up_tx[kk] = arr1_data[0, 0] / info_rate[0] + arr1_data[0, 1] / code_rate[0] * 1023 + arr1_data[0, 2] / code_rate[0] + arr1_data[0, 3] / code_rate[0] / Math.Pow(2, 16);
                down_tx[kk] = arr2_data[0, 0] / info_rate[0] + arr2_data[0, 1] / code_rate[0] * 1023 + arr2_data[0, 2] / code_rate[0] + arr2_data[0, 3] / code_rate[0] / Math.Pow(2, 16);
            }
            double c = 3 * Math.Pow(10, 8);
            double[] dist_data = new double[4];
            for (int kk = 0; kk < dist_data.Length; kk++)
            {
                dist_data[kk] = (up_tx[kk] - down_tx[kk]) * c / 2;
            }
            rx_signal.Add(
                new R_Signal()
                {
                    name = name_rx[0],
                    R1 = ((up_tx[0] - down_tx[0]) * c / 2).ToString(), //计算距离
                    R2 = ((up_tx[1] - down_tx[1]) * c / 2).ToString(),
                    R3 = ((up_tx[2] - down_tx[2]) * c / 2).ToString(),
                    R4 = ((up_tx[3] - down_tx[3]) * c / 2).ToString()
                }
                );
            GlobalVar.dot_num = GlobalVar.dot_num + 1;
            GlobalVar.dot_rx1.Add(((up_tx[0] - down_tx[0]) * c / 2));
            double f_rf1 = 2206.08 * Math.Pow(10, 6);
            double f_rf2 = 2031.432 * Math.Pow(10, 6);

            double[] fd1 = new double[4];
            double[] fd2 = new double[4];
            double[] speed_data = new double[4];
            for (int kk = 0; kk < 4; kk++)
            {
                fd1[kk] = (double)Convert.ToInt32(up_doppler[kk], 2) / 1000.0;
                fd2[kk] = (double)Convert.ToInt32(arr[R_data.Length - 1, kk], 2) / 1000.0;
                speed_data[kk] = (c * ((f_rf1 * f_rf2 - (f_rf2 + fd2[kk]) * (f_rf1 + fd1[0])) / (f_rf1 * f_rf2 + (f_rf2 + fd2[kk]) * (f_rf1 + fd1[0]))));
            }
            rx_signal.Add(
                   new R_Signal()
                   {
                       name = name_rx[1],
                       R1 = (c * ((f_rf1 * f_rf2 - (f_rf2 + fd2[0]) * (f_rf1 + fd1[0])) / (f_rf1 * f_rf2 + (f_rf2 + fd2[0]) * (f_rf1 + fd1[0])))).ToString(),  //速度

                       R2 = (c * ((f_rf1 * f_rf2 - (f_rf2 + fd2[1]) * (f_rf1 + fd1[0])) / (f_rf1 * f_rf2 + (f_rf2 + fd2[1]) * (f_rf1 + fd1[0])))).ToString(),

                       R3 = (c * ((f_rf1 * f_rf2 - (f_rf2 + fd2[2]) * (f_rf1 + fd1[0])) / (f_rf1 * f_rf2 + (f_rf2 + fd2[2]) * (f_rf1 + fd1[0])))).ToString(),

                       R4 = (c * ((f_rf1 * f_rf2 - (f_rf2 + fd2[3]) * (f_rf1 + fd1[0])) / (f_rf1 * f_rf2 + (f_rf2 + fd2[3]) * (f_rf1 + fd1[0])))).ToString()
                   }
                   );
            this.dataGridView3.DataSource = rx_signal;
            /*rx1_curve.receive_data(rx1_curve.list0_speed, rx1_curve.list0_dist, speed_data, dist_data);
            rx2_curve.receive_data(rx2_curve.list1_speed, rx2_curve.list1_dist, speed_data, dist_data);
            rx3_curve.receive_data(rx3_curve.list2_speed, rx3_curve.list2_dist, speed_data, dist_data);
            rx4_curve.receive_data(rx4_curve.list3_speed, rx4_curve.list3_dist, speed_data, dist_data);*/
            #endregion
        }







        //public void load_data_to_datagridview(DataGridView dataGridView, byte[] tc_data)
        //{
        //    dataGridView.Rows.Clear();
        //    string[] str_tc = new string[17];
        //    for (int kk = 0; kk < 16; kk++)
        //    {
        //        str_tc[kk + 1] = " ";
        //    }
        //    if (tc_data.Length % 16 == 0)
        //    {

        //        for (int i = 0; i < tc_data.Length / 16; i++)
        //        {
        //            str_tc[0] = i.ToString("x2");
        //            dataGridView.Rows.Add(str_tc);
        //            for (int j = 1; j < str_tc.Length; j++)
        //            {
        //                dataGridView.Rows[i].Cells[0].Value = str_tc[0];
        //                dataGridView.Rows[i].Cells[j].Value = tc_data[i * 16 + j - 1].ToString("x2");
        //                dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
        //            }

        //        }
        //    }
        //    else
        //    {
        //        for (int i = 0; i < tc_data.Length / 16 + 1; i++)
        //        {
        //            str_tc[0] = i.ToString("x2");
        //            dataGridView.Rows.Add(str_tc);
        //            if (i < tc_data.Length / 16)
        //            {
        //                for (int j = 1; j < str_tc.Length; j++)
        //                {
        //                    dataGridView.Rows[i].Cells[0].Value = str_tc[0];
        //                    dataGridView.Rows[i].Cells[j].Value = tc_data[i * 16 + j - 1].ToString("x2");
        //                    dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
        //                }
        //            }
        //            else
        //            {
        //                for (int j = 1; j < tc_data.Length % 16 + 1; j++)
        //                {
        //                    dataGridView.Rows[i].Cells[0].Value = str_tc[0];
        //                    dataGridView.Rows[i].Cells[j].Value = tc_data[i * 16 + j - 1].ToString("x2");
        //                    dataGridView.Rows[i].Cells[j].Style.BackColor = Color.LightGray;
        //                }
        //            }
        //        }
        //    }
        //}


        public void notify(string str)
        {
            this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine + DateTime.Now.ToLongTimeString() + " " + str;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            UdpClient client = null;
            IPAddress remoteIP = IPAddress.Parse("10.129.41.96");
            int remotePort = 19200;
            IPEndPoint remotePoint = new IPEndPoint(remoteIP, remotePort);//实例化一个远程端点   
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 18200));
            string filename = null;
            byte[] b1 = new byte[2114];//2114:打开的bin文件长度
            //client.Client.SetSocketOption(SocketOptionLevel.Socket,SocketOptionName.DontRoute,true);
            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer, 200000);
            //给buffer预留的最大长度
            filename = System.AppDomain.CurrentDomain.BaseDirectory + "param_cap.bin";
            bool exist1 = File.Exists(filename);//是否存在.bin文件，是则返回true
            if (exist1)
            {
                FileStream fr1 = File.OpenRead(filename);
                fr1.Read(b1, 0, b1.Length);
                byte[] b3 = new byte[1057];
                for (int i = 0; i < b3.Length; i++)
                {
                    b3[i] = b1[i];
                }
                client.Send(b3, b3.Length, remotePoint);
                for (int i = 0; i < b3.Length; i++)
                {
                    b3[i] = b1[i + 1057];
                }
                client.Send(b3, b3.Length, remotePoint);
                fr1.Close();
            }
            byte[] b2 = new byte[149];
            filename = System.AppDomain.CurrentDomain.BaseDirectory + "param_trk.bin";
            bool exist2 = File.Exists(filename);
            if (exist2)
            {
                FileStream fr2 = File.OpenRead(filename);
                fr2.Read(b2, 0, b2.Length);
                fr2.Close();
                client.Send(b2, b2.Length, remotePoint);
            }
            client.Close();
        }

        private void Main_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            int totalConfig = GlobalVar.rfConfig + GlobalVar.configNum * 6 + GlobalVar.b_num2;
            byte[] lastCon = new byte[totalConfig];//全部配置信息，先六通道再射频
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < GlobalVar.configNum; j++)
                {
                    lastCon[j + i * GlobalVar.configNum] = configByte[i][j];
                }
            }
            for (int k = 0; k < GlobalVar.b_num2; k++)
            {
                lastCon[lastCon.Length - GlobalVar.b_num2 + k + 1] = GlobalVar.control[k];
            }
            saveFileDialog1.Filter = "*.txt|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, lastCon);                
            }           
            System.Environment.Exit(0);
        }

 




















































    }
}
