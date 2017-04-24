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
  
    public partial class Order_Set : Form
    {
        public delegate void send_length(int length,string str,string name);
        public event send_length send;
        public string str;
        public Order_Set()
        {
            InitializeComponent();
        }

        private void TC_Order_Set_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
               int length=Convert.ToInt16( this.textBox2.Text);
               string name = this.textBox1.Text;
               send(length, str, name);
               this.Close();
            }
            catch
            {
                MessageBox.Show("请输入正确的指令长度!");
            }          
        }
    }
}
