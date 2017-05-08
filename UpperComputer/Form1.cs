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
        public Form1()
            : base(1)
        {
            InitializeComponent();
        }
        Method method = new Method();
        byte[] control1 = new byte[GlobalVar.controlNum];
        byte[][] configByte1 = new byte[GlobalVar.chanelCount][];

        private void Form1_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < GlobalVar.chanelCount; i++)
            {
                configByte1[i] = new byte[GlobalVar.configNum];
            }
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + "C1配置信息.txt";
            string strline = null;
            ArrayList blist = new ArrayList();
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //StreamReader sr;
            try
            {
                //sr = new StreamReader(filename);
                while ((strline = sr.ReadLine()) != null)
                {
                    blist.Add(strline);
                }
                string[] myArr = (string[])blist.ToArray(typeof(string));
                for (int i = 0; i < configByte.Length; i++)
                {
                    for (int j = 0; j < GlobalVar.configNum; j++)
                    {
                        configByte1[i][j] = Convert.ToByte(myArr[j + i * GlobalVar.configNum], 16);
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
            fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            sr = new StreamReader(fs, System.Text.Encoding.Default);
            try
            {
                while ((strline = sr.ReadLine()) != null)
                {
                    blist.Add(strline);
                }

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
            control1 = method.FillHeadTail(control1, "34");
            this._control = control1;
            this._configByte = configByte1;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
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
            try
            {
                string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C1配置信息.txt";
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                for (int i = 0; i < lastCon.Length; i++)
                {
                    sw.WriteLine(lastCon[i].ToString("x2"));
                }
                sw.Close();
            }
            catch { };
        }
    }
}
