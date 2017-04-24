using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace UpperComputer
{
    public partial class RF_set : Form
    {
        Method method = new Method();       
        public RF_set()
        {
            InitializeComponent();          
        }
        private void RF_set_Load(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control control in this.Controls)
            {
                if (control is System.Windows.Forms.TextBox)
                {
                    System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)control;
                    tb.ReadOnly = true;
                }
            }
        }        

        public void button1_Click(object sender, EventArgs e)
        {
            rf_parameter_down();
            checkBox1.Checked = false;
            GlobalVar.b[GlobalVar.b.Length - 3] = method.sum_verify(GlobalVar.b);
            method.SendHandle();
            //method.sendCheckThread();
            /*if (method.Send_Check(GlobalVar.b))
            {
                method.no_change_set(GlobalVar.b);
                MessageBox.Show("配置文件已下发！");
            }
            else
            {
                MessageBox.Show("配置文件未成功接收,请重新下发！");
            }  */          
        }

        public void rf_parameter_down()
        {
            string str;
            //Method method = new Method();
            GlobalVar.b[4] = Convert.ToByte("cc", 16);
            str = Convert.ToString(Convert.ToUInt32(this.textBox1.Text, 10), 2);
            str = method.Fill_Zero(str, 32);
            GlobalVar.b[5] = Convert.ToByte(str.Substring(0, 8), 2);//每八位二进制字符转化为一个bit数值
            GlobalVar.b[6] = Convert.ToByte(str.Substring(8, 8), 2);
            GlobalVar.b[7] = Convert.ToByte(str.Substring(16, 8), 2);
            GlobalVar.b[8] = Convert.ToByte(str.Substring(24, 8), 2);
            GlobalVar.b[9] = Convert.ToByte(textBox2.Text, 10);//增益
            str = Convert.ToString(Convert.ToUInt32(this.textBox3.Text, 10), 2);
            str = method.Fill_Zero(str, 32);//下行射频频点
            GlobalVar.b[10] = Convert.ToByte(str.Substring(0, 8), 2);
            GlobalVar.b[11] = Convert.ToByte(str.Substring(8, 8), 2);
            GlobalVar.b[12] = Convert.ToByte(str.Substring(16, 8), 2);
            GlobalVar.b[13] = Convert.ToByte(str.Substring(24, 8), 2);
            GlobalVar.b[14] = Convert.ToByte(textBox4.Text, 10);
            str = Convert.ToString(Convert.ToUInt16(this.textBox5.Text, 10), 2);
            str = method.Fill_Zero(str, 16);
            GlobalVar.b[15] = Convert.ToByte(str.Substring(0, 8), 2);
            GlobalVar.b[16] = Convert.ToByte(str.Substring(8, 8), 2);
            str = Convert.ToString(Convert.ToUInt32(this.textBox6.Text, 10), 2);
            str = method.Fill_Zero(str, 16);
            GlobalVar.b[17] = Convert.ToByte(str.Substring(0, 8), 2);
            GlobalVar.b[18] = Convert.ToByte(str.Substring(8, 8), 2);
            str = Convert.ToString(Convert.ToUInt32(this.textBox7.Text, 10), 2);
            str = method.Fill_Zero(str, 16);
            GlobalVar.b[19] = Convert.ToByte(str.Substring(0, 8), 2);
            GlobalVar.b[20] = Convert.ToByte(str.Substring(8, 8), 2);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox1.Checked ==false)//未点击修改，遍历这个界面所有的控件，只能读
            {
                foreach (System.Windows.Forms.Control control in this.Controls)
                {
                    if (control is System.Windows.Forms.TextBox)
                    {
                        System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)control;
                        tb.ReadOnly = true;
                    }
                }
            }
            if (this.checkBox1.Checked == true)
            {
                foreach (System.Windows.Forms.Control control in this.Controls)
                {
                    if (control is System.Windows.Forms.TextBox)
                    {
                        System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)control;
                        tb.ReadOnly = false;
                    }
                }
            }
        }

        private void RF_set_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
             
    }
}
