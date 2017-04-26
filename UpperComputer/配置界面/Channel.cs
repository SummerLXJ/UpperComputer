using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace UpperComputer
{
    public partial class Channel : Form
    {
        public Channel()
        {
            InitializeComponent();
        }
        Method method = new Method();
        double power = System.Math.Pow(2, 32);
        public double m = 1000000.0;
        public double p1 = 80.0;
        public double pow_int = -1.54;
        public byte[] control = new byte[GlobalVar.controlNum];
        private void Channel_Load(object sender, EventArgs e)
        {
            this.control = MainForm._control;
            foreach (System.Windows.Forms.Control gb in this.Controls)
            {
                if (gb is System.Windows.Forms.GroupBox)
                {
                    foreach (System.Windows.Forms.Control control in gb.Controls)
                    {
                        if (control is System.Windows.Forms.TextBox)
                        {
                            System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)control;
                            tb.ReadOnly = true;
                        }
                        if (control is OnOffBtn)
                        {
                            OnOffBtn cb = (OnOffBtn)control;
                            cb.Enabled = false;
                        }
                        if (control is System.Windows.Forms.ComboBox)
                        {
                            System.Windows.Forms.ComboBox comb = (System.Windows.Forms.ComboBox)control;
                            comb.Enabled = false;
                        }
                        if (control is System.Windows.Forms.RadioButton)
                        {
                            System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)control;
                            rb.Enabled = false;
                        }
                    }
                }
            }
        }

        #region 点击修改参数
        private void onOffBtn4_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control gb in this.Controls)
            {
                if (gb is System.Windows.Forms.GroupBox)
                {
                    foreach (System.Windows.Forms.Control control in gb.Controls)
                    {
                        if (control is System.Windows.Forms.TextBox)
                        {
                            System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)control;
                            tb.ReadOnly = !tb.ReadOnly;
                        }
                        if (control is UpperComputer.OnOffBtn)
                        {
                            OnOffBtn cb = (OnOffBtn)control;
                            cb.Enabled = !cb.Enabled;
                        }
                        if (control is System.Windows.Forms.ComboBox)
                        {
                            System.Windows.Forms.ComboBox comb = (System.Windows.Forms.ComboBox)control;
                            comb.Enabled = !comb.Enabled;
                        }
                        if (control is System.Windows.Forms.RadioButton)
                        {
                            System.Windows.Forms.RadioButton rb = (System.Windows.Forms.RadioButton)control;
                            rb.Enabled = !rb.Enabled;
                        }
                    }
                }
            }
        }
        #endregion

        #region 控制信号下发
        public int control_signal_index = 4; //该参数为遥控通道1的开关对应的index
        private void onOffBtn1_Click(object sender, EventArgs e)//载波
        {
            string str;
            str = Convert.ToString(MainForm._control[control_signal_index], 2);
            str = method.Fill_Zero(str, 8);
            if (onOffBtn1.Checked == true)
            {
                str = str.Remove(1, 1);
                str = str.Insert(1, "1");
            }
            else
            {
                str = str.Remove(1, 1);
                str = str.Insert(1, "0");
            }
            control[control_signal_index] = Convert.ToByte(str, 2);
            control[control.Length - 3] = method.sum_verify(control);
            method.Send_Control(control);
        }

        private void onOffBtn2_Click(object sender, EventArgs e)//伪码
        {
            string str;
            str = Convert.ToString(control[control_signal_index], 2);
            str = method.Fill_Zero(str, 8);
            if (onOffBtn2.Checked == true)
            {
                str = str.Remove(2, 1);
                str = str.Insert(2, "1");
            }
            else
            {
                str = str.Remove(5, 1);
                str = str.Insert(5, "0");
            }
            control[control_signal_index] = Convert.ToByte(str, 2);
            control[control.Length - 3] = method.sum_verify(control);
            method.Send_Control(control);
        }

        private void onOffBtn3_Click(object sender, EventArgs e)
        {
            string str;
            str = Convert.ToString(control[control_signal_index], 2);
            str = method.Fill_Zero(str, 8);
            if (onOffBtn3.Checked == true)
            {
                str = str.Remove(3, 1);
                str = str.Insert(3, "1");
            }
            else
            {
                str = str.Remove(4, 1);
                str = str.Insert(4, "0");
            }
            control[control_signal_index] = Convert.ToByte(str, 2);
            control[control.Length - 3] = method.sum_verify(control);
            method.Send_Control(control);
        }

        #endregion

        #region 每个通道需要设置的参数
        //以遥控通道1为基准，每个通道参数相差76
        public int count = 3;
        public int infuence = 50;
        //public byte channel_index = 1;
        #endregion

        #region 参数转化为字节
        public void byte_calculate(int length)
        {
            for (int i = 0; i < length; i++)
            {
                GlobalVar.b[count + i] = 0;
            }
            count = count + length;
        }
        public void byte_calculate(string str, int length)//把输入字符串转化为位数标准的配置参数位并赋给b
        {
            str = method.Fill_Zero(str, 8 * length);
            for (int i = 0; i < length; i++)
            {
                GlobalVar.b[count + i] = Convert.ToByte(str.Substring(8 * i, 8), 2);
            }
            count = count + length;
        }
        #endregion

        #region 参数重置
        public void center_fre()
        {
            //try
            //{
            string str = null;
            //遥控1中频变动标识
            GlobalVar.b[count] = Convert.ToByte("cc", 16);
            count = count + 1;
            //输出电平 2字节
            str = Convert.ToString((int)(Math.Round(Math.Pow(2, 16) / (Math.Pow(10, (pow_int - Convert.ToDouble(this.textBox1.Text)) / 20)))), 2);
            //str = Convert.ToString(Convert.ToUInt32(this.textBox1.Text, 10), 2);
            byte_calculate(str, 2);
            //中心频率 4字节
            long ipart;
            ipart = (long)Math.Round(Convert.ToDouble(this.textBox2.Text) / p1 * power);
            str = Convert.ToString(ipart, 2);
            byte_calculate(str, 4);
            //频率工作模式 1字节
            switch (this.comboBox1.Text)
            {
                case "归零": GlobalVar.b[count] = 0;
                    break;
                case "固定频率": GlobalVar.b[count] = 1;
                    break;
                case "正弦波": GlobalVar.b[count] = 2;
                    break;
                case "三角波": GlobalVar.b[count] = 3;
                    break;
                case "过航捷曲线": GlobalVar.b[count] = 4;
                    break;
                case "线性扫描": GlobalVar.b[count] = 5;
                    break;
                default: GlobalVar.b[count] = 0;
                    break;
            }
            count = count + 1;
            //多普勒范围 4字节
            ipart = (long)Math.Round(Convert.ToDouble(this.textBox3.Text) * power / p1 / m);
            long doppler_range = ipart;
            str = Convert.ToString(ipart, 2);
            byte_calculate(str, 4);
            //预置多普勒 4字节
            if (Convert.ToDouble(this.textBox4.Text) >= 0)
            {
                str = Convert.ToString((long)Math.Round(Convert.ToDouble(this.textBox4.Text) * power / p1 / m), 2);
            }
            else
            {
                str = Convert.ToString((long)Math.Round(power - Math.Abs(Convert.ToDouble(this.textBox4.Text)) * power / p1 / m), 2);
            }
            byte_calculate(str, 4);
            //多普勒变化率 4字节
            if (this.comboBox1.Text == this.comboBox1.Items[2].ToString())
            {
                str = Convert.ToString((long)Math.Round((Convert.ToDouble(this.textBox5.Text) * power * 2047 / Convert.ToDouble(this.textBox3.Text) / m / p1)), 2);
            }
            else
            {
                str = Convert.ToString((long)Math.Round(Convert.ToDouble(this.textBox5.Text) * power / p1 / p1 * (Math.Pow(2, 16) - 1) / m / m), 2);
            }
            byte_calculate(str, 4);
            //多普勒加速度 4字节
            str = Convert.ToString(Convert.ToUInt32(this.textBox6.Text), 2);
            byte_calculate(str, 4);
            //正弦扫频补偿因子 4字节
            if (this.comboBox1.Text == this.comboBox1.Items[2].ToString())
            {
                double sine_coff = Convert.ToDouble(this.textBox5.Text) * 2047 / Math.Pow(10, 12) * power * Math.Pow(2, 16) / p1 / p1;
                str = Convert.ToString((long)sine_coff, 2);
                byte_calculate(str, 4);
            }
            else
            {
                byte_calculate(4);
            }
            //载波伪码相干开关
            if (this.checkBox1.Checked == true) { GlobalVar.b[count] = 1; }
            else { GlobalVar.b[count] = 0; }
            count = count + 1;
            //}
            //catch
            //{
            //    MessageBox.Show("请输入正确格式数据");
            //}
        }

        public void pseudo_code()
        {
            //try
            //{
            string str = null;
            //伪码变动标识
            GlobalVar.b[count] = Convert.ToByte("cc", 16);
            count = count + 1;
            //预置码组8字节  
            str = Convert.ToString(Convert.ToUInt16(this.textBox7.Text, 16), 2);
            byte_calculate(str, 2);
            str = Convert.ToString(Convert.ToUInt16(this.textBox8.Text, 16), 2);
            byte_calculate(str, 2);
            str = Convert.ToString(Convert.ToUInt16(this.textBox9.Text, 16), 2);
            byte_calculate(str, 2);
            str = Convert.ToString(Convert.ToUInt16(this.textBox10.Text, 16), 2);
            byte_calculate(str, 2);
            //码速率4字节
            str = Convert.ToString((long)Math.Round(Convert.ToDouble(this.comboBox2.Text) * power / p1), 2);
            byte_calculate(str, 4);
            //预置码多普勒4字节，与载波伪码相干开关状态有关
            UInt32 middle_par = 0;
            double d_middle_par = 0;
            string str1 = null;
            for (int i = 5; i < 9; i++)
            {
                str1 = str1 + method.Fill_Zero(Convert.ToString(GlobalVar.b[i], 2), 8);//计算上行频率
            }
            if (GlobalVar.b[infuence] == 0)
            {
                byte_calculate(4);
            }
            else
            {
                if (Convert.ToDouble(this.textBox4.Text) >= 0)
                {
                    middle_par = (UInt32)(Math.Round(Convert.ToDouble(this.textBox4.Text) * Convert.ToDouble(this.comboBox2.Text) / Convert.ToUInt32(str1, 2) * power / p1));
                }
                else
                {
                    middle_par = (UInt32)(Math.Round(power - Math.Abs(Convert.ToDouble(this.textBox4.Text)) * Convert.ToDouble(this.comboBox2.Text) / Convert.ToUInt32(str1, 2) * power / p1));
                }
                str = method.Fill_Zero(Convert.ToString(middle_par, 2), 32);
                byte_calculate(str, 4);
            }
            //码多普勒变化率4字节  与多普勒范围以及多普勒变化率相关
            if (GlobalVar.b[infuence] == 0)
            {
                byte_calculate(4);
            }
            else
            {
                if (this.comboBox1.Text == this.comboBox1.Items[3].ToString())
                {
                    d_middle_par = Convert.ToDouble(this.textBox5.Text) * Convert.ToDouble(this.comboBox2.Text) * m / Convert.ToUInt32(str1, 2);
                    middle_par = (UInt32)(Math.Round(d_middle_par / p1 / m / m / p1 * power * power));
                    str = Convert.ToString(middle_par, 2);
                    byte_calculate(str, 4);
                }
                else if (this.comboBox1.Text == this.comboBox1.Items[2].ToString())
                {
                    middle_par = (UInt32)(Math.Round(power * 2047 * Convert.ToDouble(this.textBox5.Text) / Convert.ToDouble(this.textBox3.Text) / p1 / m));
                    str = Convert.ToString(middle_par, 2);
                    byte_calculate(str, 4);
                }
                else
                {
                    byte_calculate(4);
                }
            }
            //码多普勒范围4字节 
            if (GlobalVar.b[infuence] == 0)
            {
                byte_calculate(4);
            }
            else
            {
                middle_par = (UInt32)(Math.Round(Convert.ToDouble(this.textBox3.Text) * Convert.ToDouble(this.comboBox2.Text) * m / Convert.ToUInt32(str1, 2) * power / p1 / m));
                str = method.Fill_Zero(Convert.ToString(middle_par, 2), 32);
                byte_calculate(str, 4);
            }
            //码多普勒补偿因子4字节  
            if (GlobalVar.b[infuence] == 0)
            {
                middle_par = (UInt32)(Math.Round(Convert.ToDouble(this.comboBox2.Text) * m / Convert.ToUInt32(str1, 2) * Math.Pow(2, 16)));
            }
            else
            {
                if (this.comboBox1.Text == this.comboBox1.Items[2].ToString())
                {
                    middle_par = (UInt32)(Math.Round(Convert.ToDouble(textBox5.Text) * Convert.ToDouble(this.comboBox2.Text) * m / Convert.ToUInt32(str1, 2) * 2047 / p1 / m * power / p1 / m * power));
                }
                else
                {
                    middle_par = 0;
                }
            }
            str = Convert.ToString(middle_par, 2);
            byte_calculate(str, 4);
            //}
            //catch
            //{
            //    MessageBox.Show("请输入正确格式数据");
            //}
        }

        public virtual void information()
        {
            //try
            //{
            string str = null;
            //信息层变动标识
            GlobalVar.b[count] = Convert.ToByte("cc", 16);
            count = count + 1;
            //信息速率4字节
            double info_rate = Convert.ToDouble(textBox15.Text) / p1 * power / m;
            str = Convert.ToString((UInt32)(Math.Round(Convert.ToDouble(textBox15.Text) / p1 * power / m)), 2);
            byte_calculate(str, 4);
            //信息来源1字节,01：随即数据块（多项式生成）02：外部定义固定格式数据块（4096Byte）03：外部定义（实时输入）
            switch (this.comboBox3.Text)
            {
                case "随机数据块": GlobalVar.b[count] = 1;
                    break;
                case "外部数据块": GlobalVar.b[count] = 2;
                    break;
                case "外部定义": GlobalVar.b[count] = 3;
                    break;
                default: GlobalVar.b[count] = 1;
                    break;
            }
            count = count + 1;
            //数据信息多项式4字节
            str = Convert.ToString(Convert.ToUInt16(this.textBox16.Text, 16), 2);
            byte_calculate(str, 2);
            str = Convert.ToString(Convert.ToUInt16(this.textBox17.Text, 16), 2);
            byte_calculate(str, 2);
            //信息空白位选择模式，0F：空闲填“0101”；F0：空闲填“0000”
            GlobalVar.b[count] = Convert.ToByte(this.textBox18.Text, 16);
            count = count + 1;
            //信息加扰开关,0：加扰;1：不加扰
            if (this.radioButton1.Checked == true) { GlobalVar.b[count] = 0; }
            else { GlobalVar.b[count] = 1; }
            count = count + 1;
            //信息层补偿因子4字节
            string str1 = null;
            for (int i = 5; i < 9; i++)
            {
                str1 = str1 + method.Fill_Zero(Convert.ToString(GlobalVar.b[i], 2), 8);//计算上行频率
            }
            //str = Convert.ToString(((long)Math.Round(info_rate * power / Convert.ToUInt32(str1, 2))), 2);
            //byte_calculate( str, 4);
            byte_calculate(4);
            //干扰模式1字节
            if (this.radioButton3.Checked == true) { GlobalVar.b[count] = 0; }
            else { GlobalVar.b[count] = 1; }
            count = count + 1;
            //}
            //catch
            //{
            //    MessageBox.Show("请输入正确格式数据");
            //}
        }

        public void null_deal_with()
        {
            foreach (System.Windows.Forms.Control gb in this.Controls)
            {
                if (gb is System.Windows.Forms.GroupBox)
                {
                    foreach (System.Windows.Forms.Control control in gb.Controls)
                    {
                        if (control is System.Windows.Forms.TextBox)
                        {
                            System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)control;
                            if (tb.Text == "")
                            {
                                tb.Text = "1";
                            }
                        }
                        if (control is System.Windows.Forms.ComboBox)
                        {
                            System.Windows.Forms.ComboBox cb = (System.Windows.Forms.ComboBox)control;
                            if (cb.Text == "")
                            {
                                cb.Text = cb.Items[0].ToString();
                            }
                        }
                    }
                }
            }

        }
        public void parameter_down()  //21~87
        {
            //通道标示
            //GlobalVar.b[count] = Convert.ToByte(channel_index.ToString()+channel_index.ToString(), 16);
            count = count + 1;
            center_fre();
            pseudo_code();
            information();
        }

        #endregion

        #region 点击下发参数
        private void button1_Click(object sender, EventArgs e)
        {
            null_deal_with();
            //count = 3 + GlobalVar.rf_num + (channel_index - 1) * GlobalVar.channel_length;
            parameter_down();
            this.onOffBtn4.Checked = false;
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
                MessageBox.Show("配置文件未下发！");
            }*/
        }
        #endregion

        private void Channel_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }


        #region 参数配置
        string text(int length, string[] bs)//生成单个控件显示的text
        {
            string str = null;
            for (int i = 0; i < length; i++)
            {
                str = str + bs[count + i];
            }
            count = count + length;
            return Convert.ToUInt32(str, 2).ToString();
        }
        public void Channel_Config(byte[] b, string[] bs)  //每个通道的控件参数显示
        {
            count = 3;
            //通道标识
            count = count + 1;
            //中频变动标识
            count = count + 1;
            //输出电平
            this.textBox1.Text = text(2, bs);
            //中心频率，按公式反推
            this.textBox2.Text = text(4, bs);//((int)(Math.Round(Convert.ToUInt32(text(4, bs)) / power * p1))).ToString();
            switch (b[count])//频率工作模式
            {
                case 0: this.comboBox1.Text = this.comboBox1.Items[0].ToString();
                    break;
                case 1: this.comboBox1.Text = this.comboBox1.Items[1].ToString();
                    break;
                case 2: this.comboBox1.Text = this.comboBox1.Items[2].ToString();
                    break;
                case 3: this.comboBox1.Text = this.comboBox1.Items[3].ToString();
                    break;
                case 4: this.comboBox1.Text = this.comboBox1.Items[4].ToString();
                    break;
                case 5: this.comboBox1.Text = this.comboBox1.Items[5].ToString();
                    break;
                default: this.comboBox1.Text = "没选定工作模式";
                    break;
            }
            count = count + 1;
            //多普勒范围、预置多普勒、多普勒变化率、多普勒加速度
            this.textBox3.Text = text(4, bs);
            this.textBox4.Text = text(4, bs);
            this.textBox5.Text = text(4, bs);
            this.textBox6.Text = text(4, bs);
            //正弦扫描补偿
            count = count + 4;
            //过捷航
            count = count + 1;
            //载波伪码相干开关
            switch (b[count])
            {
                case 0: this.checkBox1.Checked = false;
                    break;
                case 1: this.checkBox1.Checked = true;
                    break;
                default: this.checkBox1.Checked = false;
                    break;
            }
            count = count + 1;
            //码组变动标识
            count = count + 1;
            //预置码组8字节
            this.textBox7.Text = b[count].ToString("x2") + b[count + 1].ToString("x2");//多项式1
            this.textBox8.Text = b[count + 2].ToString("x2") + b[count + 3].ToString("x2");//多项式2
            this.textBox9.Text = b[count + 4].ToString("x2") + b[count + 5].ToString("x2");//初相1
            this.textBox10.Text = b[count + 6].ToString("x2") + b[count + 7].ToString("x2");//初相2
            count = count + 8;
            //码速率4字节
            this.comboBox2.Text = this.comboBox2.Items[b[count]].ToString();
            //预置码多普勒4字节
            this.textBox11.Text = text(4, bs);
            //码多普勒变化率4字节
            this.textBox12.Text = text(4, bs);
            //码多普勒范围4字节
            this.textBox13.Text = text(4, bs);
            //码多普勒补偿因子4字节
            this.textBox14.Text = text(4, bs);
            //信息层变动标识
            count = count + 1;
            //信息速率4字节
            this.textBox15.Text = text(4, bs);
            /*if (b[infuence] == 1)
            {
                this.textBox15.Text = ((Convert.ToDouble(this.comboBox2.Text) * m * m / Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2) / 1023)).ToString();
            }
            if (b[infuence] == 0)
            {
                this.textBox15.Text = ((int)Math.Round(m * p1 * Convert.ToUInt32(bs[count] + bs[count + 1] + bs[count + 2] + bs[count + 3], 2) / power)).ToString();
            }*/
            count = count + 4;
            //信息来源1字节,01：随机数据块（多项式生成）02：外部定义固定格式数据块（4096Byte）03：外部定义（实时输入）
            switch (b[count])
            {
                case 1: this.comboBox3.Text = this.comboBox3.Items[0].ToString();
                    break;
                case 2: this.comboBox3.Text = this.comboBox3.Items[1].ToString();
                    break;
                case 3: this.comboBox3.Text = this.comboBox3.Items[2].ToString();
                    break;
                default: this.comboBox3.Text = "没有选定数据来源！";
                    break;
            }
            count = count + 1;
            //数据信息多项式4字节
            this.textBox16.Text = b[count].ToString("x2") + b[count + 1].ToString("x2");
            this.textBox17.Text = b[count + 2].ToString("x2") + b[count + 3].ToString("x2");
            count = count + 4;
            //信息空白位选择模式，0F：空闲填“0101”；F0：空闲填“0000”
            this.textBox18.Text = b[count].ToString("x2");
            count = count + 1;
            //信息加扰开关,0：加扰;1：不加扰
            switch (b[count])
            {
                case 0: this.radioButton1.Checked = true;
                    break;
                case 1: this.radioButton2.Checked = true;
                    break;
                default: this.radioButton2.Checked = true;//当没有时如何选择，即默认的不加扰
                    break;
            }
            count = count + 1;
            //92.93.94.95为信息层补偿因子
            //count = count + 4;
            //干扰模式1字节
            /*if (b[count] == 0)
            {
                this.radioButton3.Checked = true;
            }
            else
            {
                this.radioButton4.Checked = true;
            }*/

            count = count + 1;
            //码型
            count = count + 1;
        }

        #endregion

        private void label2_Click(object sender, EventArgs e)
        {

        }



    }
}
