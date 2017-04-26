using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace UpperComputer
{
    public partial class Form1 : UpperComputer.MainForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] control1 = new byte[GlobalVar.controlNum];
            control1[0] = 235;
            control1[1] = 125;
            control1[2] = 12;

            /*
            string fileName = System.AppDomain.CurrentDomain.BaseDirectory+"C1控制信号.txt";           
            FileStream fr = File.OpenRead(fileName);
            fr.Read(control1, 0, control1.Length);*/
            this.control = control1;
        }
    }
}
