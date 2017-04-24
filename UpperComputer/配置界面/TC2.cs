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
    public partial class TC2 : Channel
    {
        public TC2()
        {
            InitializeComponent();
        }

        private void TC2_Load(object sender, EventArgs e)
        {
            this.count = 3 + GlobalVar.rf_num + GlobalVar.channel_length;
            this.infuence = 50 + GlobalVar.channel_length;
            //this.channel_index = 2;
            this.control_signal_index = 11;
        }
    }
}
