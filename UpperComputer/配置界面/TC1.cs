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
    public partial class TC1 : Channel
    {
        public TC1(): base()
        {
            InitializeComponent();
        }

        private void TC1_Load(object sender, EventArgs e)
        {
            this.chanelindex = 0;
            this.Text = "通道"+StartForm.chanelTotal+" TC1";
        }
    }
}
