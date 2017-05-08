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
    public partial class TR4 : Channel
    {
        public TR4() : base(MainForm._CHANNEL)
        {
            InitializeComponent();
        }

        private void RG4_Load(object sender, EventArgs e)
        {
            this.chanelindex = 5;
            this.Text = "通道" + StartForm.chanelTotal + " TR4";

        }
    }
}
