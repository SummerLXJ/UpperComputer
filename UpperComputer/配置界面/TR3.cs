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
    public partial class TR3 : Channel
    {
        public TR3()
        {
            InitializeComponent();
        }

        private void RG3_Load(object sender, EventArgs e)
        {
            this.chanelindex = 4;
            this.Text = "通道" + StartForm.chanelTotal + " TR3";

        }
    }
}
