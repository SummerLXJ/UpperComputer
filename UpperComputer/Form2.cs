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
    public partial class Form2 : UpperComputer.MainForm
    {
        public Form2()
        {
            InitializeComponent();
            
        }
        Method method = new Method();
        byte[] control2 = new byte[GlobalVar.controlNum];
        byte[][] configByte2 = new byte[GlobalVar.chanelCount][];
        private void Form2_Load(object sender, EventArgs e)
        {
            this.CHANNEL = 2;
            for (int i = 0; i < GlobalVar.chanelCount; i++)
            {
                configByte2[i] = new byte[GlobalVar.configNum];
            }
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + "C2配置信息.txt";
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
                        configByte2[i][j] = Convert.ToByte(myArr[i], 16);
                    }
                }
                for (int i = 0; i < GlobalVar.chanelCount; i++)
                {
                    configuration(i + 1, configByte2[i]);
                }
            }
            catch
            {
                for (int i = 0; i < configByte.Length; i++)
                {
                    for (int j = 0; j < GlobalVar.configNum; j++)
                    {
                        configByte2[i][j] = 0;
                    }
                }
                MessageBox.Show("初始配置信息不存在，请自行配置！");
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
                    control2[i] = Convert.ToByte(myArr2[i], 16);
                }
            }
            catch
            {
                for (int i = 0; i < control.Length; i++)
                {
                    control2[i] = 0;
                }
            }
            control2= method.FillHeadTail(control, "34");
            this._control = control2;
            this._configByte = configByte2;
        }
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            configByte2 = this._configSave2;
            control2 = this._controlSave2;
            byte[] lastCon = new byte[GlobalVar.configNum * 6];
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < GlobalVar.configNum; j++)
                {
                    lastCon[j + i * GlobalVar.configNum] = configByte2[i][j];
                }
            }
            StreamWriter sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C2配置信息.txt");
            for (int i = 0; i < lastCon.Length; i++)
            {
                sw.WriteLine(lastCon[i].ToString("x2"));
            }
            sw = new StreamWriter(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C2控制信号.txt");
            for (int i = 0; i < GlobalVar.controlNum; i++)
            {
                sw.WriteLine(control2[i].ToString("x2"));
            }
            sw.Close();
        }
    }
}
