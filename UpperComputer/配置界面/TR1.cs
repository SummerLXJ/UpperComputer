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

        public TR1(): base(1,2)
        {
            InitializeComponent();
        }
        private void RG1_Load(object sender, EventArgs e)
        {
            this.channelindex = 2;
            this.Text = "通道" + StartForm.chanelTotal + " TR1";

        }     
    }
}
