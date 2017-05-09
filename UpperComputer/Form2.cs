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
        public Form2():base(2)
        {
            InitializeComponent();
            
        }
        Method method = new Method();
        byte[] control2 = new byte[GlobalVar.controlNum];
        byte[][] configByte2 = new byte[GlobalVar.chanelCount][];
        private void Form2_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < GlobalVar.chanelCount; i++)
            {
                configByte2[i] = new byte[GlobalVar.configNum];
            }
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + "C2配置信息.txt";
            configByte2 = LoadConfigFile(filename,configByte2);
            filename = System.AppDomain.CurrentDomain.BaseDirectory + "C2控制信号.txt";
            control2 = LoadControlFile(filename);
            control2= method.FillHeadTail(control, "34");
            this._control = control2;
            this._configByte = configByte2;
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            configByte2 = this._configSave2;
            control2 = this._controlSave2;
            try
            {
                string filename = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "C2配置信息.txt";
                SaveConfigFile(filename, configByte2);
            }
            catch { };
        }
    }
}
