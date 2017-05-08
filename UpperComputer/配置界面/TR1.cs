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
    public partial class TR1 : Channel
    {

        public TR1(): base(MainForm._CHANNEL)
        {
            InitializeComponent();
        }
        private void RG1_Load(object sender, EventArgs e)
        {
            this.chanelindex = 2;
            this.Text = "通道" + StartForm.chanelTotal + " TR1";

        }     
    }
}
