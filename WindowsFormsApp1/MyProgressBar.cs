using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MyProgressBar : UserControl
    {
        private System.Drawing.Color progressColor = System.Drawing.Color.Green;
        private System.Drawing.Color borderColor = System.Drawing.Color.Black;
        private double value = 0;
        private double maximum = 100;
        private double minimum = 0;
        Control previousParent;
        private float borderWidth = 1;
        private bool showValue = true;

        public MyProgressBar()
        {
            InitializeComponent();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            if (Parent != previousParent)
            {
                if (Parent != null) Parent.Paint += ParentPaint;
                if (previousParent != null) previousParent.Paint -= ParentPaint;
                previousParent = Parent;
            }
        }
        private void ParentPaint(object sender, PaintEventArgs e)
        {
            using (Pen p = new Pen(BorderColor, BorderWidth))
            using (var gp = new GraphicsPath())
            {
                float halfPenWidth = BorderWidth / 2;
                var borderRect = new RectangleF(Left - halfPenWidth, Top - halfPenWidth,
                                               Width + BorderWidth, Height + BorderWidth);
                gp.AddRectangle(borderRect);
                e.Graphics.DrawPath(p, gp);
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (Parent != null) Parent.Invalidate();
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            if (Parent != null) Parent.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            Rectangle rec = new Rectangle(0, 0, (int)(Width * porcentage), Height);
            graphics.FillRectangle(new SolidBrush(progressColor), rec);
            string text = (porcentage * 100).ToString("#0.##") + "%" + PaintValue;
            graphics.MeasureString(text, Font);
            graphics.DrawString(text, Font, new SolidBrush(ForeColor), new Rectangle(0,0,Width, Height) , new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap });
        }

        public System.Drawing.Color ProgressColor { get => progressColor; set => progressColor = value; }
        public double Value { get => value; set { this.value = value; Refresh(); } }
        public double Maximum { get => maximum; set => maximum = value;  }
        public double Minimum { get => minimum; set => minimum = value; }
        [Browsable(false)]
        public double porcentage => (value - minimum) / (maximum - minimum);

        public Color BorderColor { get => borderColor; set => borderColor = value; }
        public float BorderWidth { get => borderWidth; set => borderWidth = value; }
        public bool ShowValue { get => showValue; set => showValue = value; }
        private string PaintValue => showValue ? (" (" + (minimum != 0 ? minimum.ToString("#0.##") + "/" : string.Empty)) + $"{value.ToString("#0.##")}/{maximum.ToString("#0.##")})" : string.Empty;
    }
}
