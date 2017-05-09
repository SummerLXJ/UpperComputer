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
            configByte1 = LoadConfigFile(filename,configByte1);
            filename = System.AppDomain.CurrentDomain.BaseDirectory + "C1控制信号.txt";
            control1 = LoadControlFile(filename);
            control1 = method.FillHeadTail(control1, "34");
            this._control = control1;
            this._configByte = configByte1;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            configByte1 = this._configSave1;
            //control1 = this._controlSave1;
            try
            {
                string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C1配置信息.txt";
                SaveConfigFile(filename, configByte1);
            }
            catch { };
        }
    }
}
