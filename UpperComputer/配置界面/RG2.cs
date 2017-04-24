using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpperComputer
{
    public partial class RG2 : Channel
    {
        public RG2()
        {
            InitializeComponent();
        }

        private void RG2_Load(object sender, EventArgs e)
        {
            this.count = 3 + GlobalVar.rf_num + GlobalVar.channel_length*3;
            this.infuence = 50 + GlobalVar.channel_length*3;
            //this.channel_index = 4;
            this.control_signal_index = 25;
        }
        Method method = new Method();
        public int information_site = 81 + 3 * GlobalVar.channel_length;
        public override void information()
        {
            base.information();
            //信息速率，4字节
            double info_rate = Convert.ToDouble(this.comboBox2.Text) * this.m / 1023 / Convert.ToDouble(textBox15.Text);
            string str = Convert.ToString((UInt32)Math.Round(info_rate), 2);
            str = method.Fill_Zero(str, 32);
            GlobalVar.b[information_site] = Convert.ToByte(str.Substring(0, 8), 2);
            GlobalVar.b[information_site + 1] = Convert.ToByte(str.Substring(8, 8), 2);
            GlobalVar.b[information_site + 2] = Convert.ToByte(str.Substring(16, 8), 2);
            GlobalVar.b[information_site + 3] = Convert.ToByte(str.Substring(24, 8), 2);
        }
    }
}
