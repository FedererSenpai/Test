using Microsoft.VisualBasic.Compatibility.VB6;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MyDay : UserControl
    {
        private DateTime date;
        private Font dayFont = new System.Drawing.Font("Comic Sans MS", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        private Brush dayBrush = Brushes.Black;
        private string dataText = string.Empty;

        [DefaultValue(AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom)]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }

        public DateTime Date { get => date; set => date = value; }
        public Font DayFont { get => dayFont; set => dayFont = value; }
        public Brush DayBrush { get => dayBrush; set => dayBrush = value; }
        public string DataText { get => dataText; set => dataText = value; }

        Control previousParent;
        Color BorderColor = Color.Black;
        int BorderWidth = 1;
        public MyDay()
        {
            InitializeComponent();
        }

        public MyDay(DateTime _date, bool _thismonth, string _text)
        {
            date = _date;
            DataText = _text;
            if(!_thismonth)
            {
                this.dayFont = new Font(this.dayFont.FontFamily, this.dayFont.Size - 6, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
                this.dayBrush = Brushes.LightGray;
            }
            InitializeComponent();
        } 

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawString(Date.Day.ToString(), this.dayFont, this.dayBrush, e.ClipRectangle);
            g.DrawString(this.dataText, this.Font, Brushes.Black, e.ClipRectangle, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
        }

    }
}
