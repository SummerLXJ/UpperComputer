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
        TC1 TC1 = new TC1();
        TC2 TC2 = new TC2();
        TR1 TR1 = new TR1();
        TR2 TR2 = new TR2();
        TR3 TR3 = new TR3();
        TR4 TR4 = new TR4();
        public int CHANNEL = 1;
        public static byte[] brf = new byte[GlobalVar.rfConfigNum];
        public static byte[][] configByte = new byte[GlobalVar.chanelCount][];
        public static byte[] control = new byte[GlobalVar.controlNum];
       public byte[] _control
        {
            set { control = value; }
            get { return control; }
        }
        public byte[][] _configByte
        {
            set { configByte = value; }
            get { return configByte; }
        }
        public static byte[] controlSave1 = new byte[GlobalVar.controlNum];
        public static byte[][] configSave1 = new byte[GlobalVar.chanelCount][];
        public byte[] _controlSave1
        {
            set { controlSave1 = value; }
            get { return controlSave1; }
        }
        public byte[][] _configSave1
        {
            set { configSave1 = value; }
            get { return configSave1; }
        }
        public static byte[] controlSave2 = new byte[GlobalVar.controlNum];
        public static byte[][] configSave2 = new byte[GlobalVar.chanelCount][];
        public byte[] _controlSave2
        {
            set { controlSave2 = value; }
            get { return controlSave1; }
        }
        public byte[][] _configSave2
        {
            set { configSave2 = value; }
            get { return configSave2; }
        }
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            this._configByte = method._configByte;
            this._control = method._control;
        }

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
                method.ctrChanged(control,index, "1");
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                           DateTime.Now.ToLocalTime().ToString() + " 打开" + str + "通道";
            }
            else
            {
                method.ctrChanged(control,index, "0");
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                           DateTime.Now.ToLocalTime().ToString() + " 关闭" + str + "通道";
            }
        }
        private void onOffBtn1_Click(object sender, EventArgs e)
        {
            ControlChange(1, onOffBtn1.Checked);
            control[control.Length - 3] = method.sum_verify(control);
            method.Send_Control(control);
            method.BackCheck(onOffBtn1, control, "34");
        }

        private void onOffBtn2_Click(object sender, EventArgs e)
        {
            ControlChange(2, onOffBtn2.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            method.BackCheck(onOffBtn2, control, "34");
        }
        private void onOffBtn3_Click(object sender, EventArgs e)
        {
            ControlChange(3, onOffBtn3.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            method.BackCheck(onOffBtn3, control, "34");
        }
        private void onOffBtn4_Click(object sender, EventArgs e)
        {
            ControlChange(4, onOffBtn4.Checked);
           control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            method.BackCheck(onOffBtn4, control, "34");
        }
        private void onOffBtn5_Click(object sender, EventArgs e)
        {
            ControlChange(5, onOffBtn5.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            method.BackCheck(onOffBtn5, control, "34");
        }
        private void onOffBtn6_Click(object sender, EventArgs e)
        {
            ControlChange(6, onOffBtn6.Checked);
            control[control.Length - 2] = method.sum_verify(control);
            method.Send_Control(control);
            method.BackCheck(onOffBtn6, control, "34");
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
                byte indexbyte = pb[3];
                string configstring = Convert.ToString(indexbyte, 2);
                configstring = method.Fill_Zero(configstring, 8);
                configstring = configstring.Substring(2, 6);
                int configIndex = Convert.ToInt32(configstring);//Convert.ToString(indexbyte,2).Substring(2, 6));
                configuration(configIndex, pb);
                GlobalVar.b = pb;
                fr.Close();
                this.richTextBox3.Text = this.richTextBox3.Text + Environment.NewLine +
                                           DateTime.Now.ToLocalTime().ToString()+ " 加载配置文件 " + fileName;
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
                case 3: TR1.Channel_Config(b, bs); break;
                case 4: TR2.Channel_Config(b, bs); break;
                case 5: TR3.Channel_Config(b, bs); break;
                case 6: TR4.Channel_Config(b, bs); break;
            }
        }


        private void 遥控1配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == TC1 || TC1.IsDisposed == true)
            { TC1 = new TC1(); }
            TC1.Show();
        }

        private void 遥控2配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == TC2 || TC2.IsDisposed == true)
            { TC2 = new TC2(); }
            TC2.Show();
        }

        private void 遥测1配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == TR1 || TR1.IsDisposed == true)
            { TR1 = new TR1();} 
            TR1.Show();
        }

        private void 遥测2配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == TR2 || TR2.IsDisposed == true)
            { TR2 = new TR2(); }
            TR2.Show();
        }

        private void 遥测3配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == TR3 || TR3.IsDisposed == true)
            { TR3 = new TR3(); }
            TR3.Show();
        }

        private void 遥测4配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == TR4 || TR4.IsDisposed == true)
            { TR4 = new TR4(); }
            TR4.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CHANNEL == 1)
            {
                this._configSave1 = this._configByte;
                this._controlSave1 = this._control;
            }
            if (CHANNEL == 2)
            {
                this._configSave2 = this._configByte;
                this._controlSave2 = this._control;
            }
        }
    }
}
