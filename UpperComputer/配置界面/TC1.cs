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
    public partial class TC1 : Channel
    {
        public TC1()
        {
            InitializeComponent();
        }

        private void TC1_Load(object sender, EventArgs e)
        {
            this.count = 21;
            this.infuence = 50;
            //this.channel_index = 1;
        }
    }
}
