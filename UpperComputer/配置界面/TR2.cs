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
    public partial class TR2 : Channel
    {
        public TR2()
        {
            InitializeComponent();
        }

        private void RG2_Load(object sender, EventArgs e)
        {
            this.chanelindex = 3;
            this.Text = "通道" + StartForm.chanelTotal + " TR2";
        }
       
    }
}
