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
        public TC2(): base(MainForm._CHANNEL)
        {
            InitializeComponent();
        }

        private void TC2_Load(object sender, EventArgs e)
        {
            this.chanelindex = 1;
            this.Text = "通道" + StartForm.chanelTotal + " TC2";

        }
    }
}
