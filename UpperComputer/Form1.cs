using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace UpperComputer
{
    public partial class Form1 : UpperComputer.MainForm
    {
        public Form1():base(1)
        {
            InitializeComponent();
        }
        Method method = new Method();
        byte[] control1 = new byte[GlobalVar.controlNum];
        byte[][] configByte1 = new byte[GlobalVar.chanelCount][];

        private void Form1_Load(object sender, EventArgs e)
        {
            this.CHANNEL = 1;
            for (int i = 0; i < GlobalVar.chanelCount; i++)
            {
                configByte1[i] = new byte[GlobalVar.configNum];
            }
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + "C1配置信息.txt";
            string strline = null;
            ArrayList blist = new ArrayList();
            StreamReader sr;
            try
            {
                sr = new StreamReader(filename);
                while ((strline = sr.ReadLine()) != null)
                {
                    blist.Add(strline);
                }
                string[] myArr = (string[])blist.ToArray(typeof(string));
                for (int i = 0; i < configByte.Length; i++)
                {
                    for (int j = 0; j < GlobalVar.configNum; j++)
                    {
                        configByte1[i][j] = Convert.ToByte(myArr[i], 16);
                    }
                }
                for (int i = 0; i < GlobalVar.chanelCount; i++)
                {
                    configuration(i + 1, configByte1[i]);
                }
            }
            catch (Exception ex)
            {
                for (int i = 0; i < configByte.Length; i++)
                {
                    for (int j = 0; j < GlobalVar.configNum; j++)
                    {
                        configByte1[i][j] = 0;
                    }
                }
                MessageBox.Show("初始配置信息不存在，请自行配置！\n" + "错误：\n" + ex);
            }
            filename = System.AppDomain.CurrentDomain.BaseDirectory + "C1控制信号.txt";
            blist = new ArrayList();
            sr = new StreamReader(filename);
            while ((strline = sr.ReadLine()) != null)
            {
                blist.Add(strline);
            }
            try
            {
                string[] myArr2 = (string[])blist.ToArray(typeof(string));
                for (int i = 0; i < control.Length; i++)
                {
                    control1[i] = Convert.ToByte(myArr2[i], 16);
                }
            }
            catch
            {
                for (int i = 0; i < control.Length; i++)
                {
                    control1[i] = 0;
                }
            }
            control1 = method.FillHeadTail(control, "34");
            this._control = control1;
            this._configByte = configByte1;
        }
        private void Form1_Click(object sender, EventArgs e)
        {
            this.CHANNEL = 1;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            configByte1 = this._configSave1;
            control1 = this._controlSave1; 
            byte[] lastCon = new byte[GlobalVar.configNum * 6];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < GlobalVar.configNum; j++)
                {
                    lastCon[j + i * GlobalVar.configNum] = configByte1[i][j];
                }
            }
            StreamWriter sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C1配置信息.txt");
            for (int i = 0; i < lastCon.Length; i++)
            {
                sw.WriteLine(lastCon[i].ToString("x2"));
            }
            sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C1控制信号.txt");
            for (int i = 0; i < GlobalVar.controlNum; i++)
            {
                sw.WriteLine(control1[i].ToString("x2"));
            }
            sw.Close();
        }
    }
}
