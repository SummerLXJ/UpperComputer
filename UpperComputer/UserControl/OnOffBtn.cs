using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UpperComputer
{
    public partial class OnOffBtn : UserControl
    {
        public OnOffBtn()
        {
            InitializeComponent();

            //设置Style支持透明背景色并且双缓冲
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.Selectable, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;
            this.Size = new Size(45, 18);
        }
        public delegate void CheckedHandle(object sender, EventArgs e);
        public event CheckedHandle CheckedChanged;
        public bool isCheck = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Checked
        {
            get { return isCheck; }
            set
            {
                isCheck = value; 
                this.Invalidate();                
            }

        }
        /// <summary>
        /// 样式
        /// </summary>

        protected override void OnPaint(PaintEventArgs e)
        {
            Bitmap bitMapOn = null;
            Bitmap bitMapOff = null;

            bitMapOn = global::UpperComputer.Properties.Resources.btncheckon4;
            bitMapOff = global::UpperComputer.Properties.Resources.btncheckoff4;

            Graphics g = e.Graphics;
            Rectangle rec = new Rectangle(0, 0, this.Size.Width, this.Size.Height);

            if (isCheck)
            {
                g.DrawImage(bitMapOn, rec);

            }
            else
            {
                g.DrawImage(bitMapOff, rec);
            }
        }
        private void OnOffBtn_Click(object sender, EventArgs e)
        {

            isCheck = !isCheck;
            this.Invalidate();
        }
    }

}

