﻿using System;
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
        public TR2(): base(1,3)
        {
            InitializeComponent();
        }

        private void TR2_Load(object sender, EventArgs e)
        {
            this.channelindex = 3;
            this.Text = "通道" + StartForm.chanelTotal + " TR2";
        }
       
    }
}
