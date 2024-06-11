using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Mine : UserControl
    {
        private State state = State.Closed;
        private int value = int.MinValue;
        private Brush[] fores = new Brush[9]
        {
            Brushes.Transparent,
            Brushes.Blue,
            Brushes.Green,
            Brushes.Red,
            Brushes.DarkBlue,
            Brushes.DarkRed,
            Brushes.Turquoise,
            Brushes.Black,
            Brushes.Gray
        };

        public int Value { get => value; set => this.value = value; }

        public enum State
        {
            Opened = 1,
            Closed = 2,
            Flagged = 3,
            WrongFlagged = 4,
            Exploded = 5
        }

        public delegate void OpenedEventHandler(object sender, OpenedEventArgs e);
        public event OpenedEventHandler OnOpened;
        public Mine()
        {
            InitializeComponent();
            MouseClick += new MouseEventHandler(MouseClick_Event);
        }

        public void PerformClick()
        {
            MouseClick_Event(this, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        }
        private void MouseClick_Event(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if (state == State.Flagged)
                    state = State.Closed;
                else
                    state = State.Flagged;
            }
            else if(e.Button == MouseButtons.Left)
            {
                //Enabled = false;
                if (value == -1)
                    state = State.Exploded;
                else
                    state = State.Opened;
                OnOpened(this, new OpenedEventArgs(Value, state));
            }
            Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (state == State.Exploded)
                g.DrawImage(Properties.Resources.MineExploded, new RectangleF(0, 0, Width, Height));
            else if (state == State.WrongFlagged)
            {
                g.DrawImage(Properties.Resources.flagwrong, new RectangleF(0, 0, Width, Height));
            }
            else if (state == State.Opened)
            {
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                    g.FillRectangle(Brushes.LightGray, new RectangleF(0, 0, Width, Height));
                    if (value < 0)
                    {
                        g.DrawImage(Properties.Resources.Mine, new RectangleF(0, 0, Width, Height));
                    }
                    else
                    {
                        g.DrawString(value.ToString(), new Font("Calibri", 10, FontStyle.Bold), fores[value], e.ClipRectangle, format);
                    }
            }
            else
            {
                g.FillPolygon(Brushes.WhiteSmoke, new Point[] { new Point(0, 0), new Point(Width, 0), new Point(0, Height) });
                g.FillPolygon(Brushes.Gray, new Point[] { new Point(Width, Height), new Point(Width, 0), new Point(0, Height) });
                if (state == State.Flagged)
                {
                    g.DrawImage(Properties.Resources.flag, new RectangleF(2, 2, Width - 4, Height - 4));
                }
                else
                {
                    g.FillRectangle(Brushes.LightGray, new RectangleF(2, 2, Width - 4, Height - 4));
                }
            }
            //MessageBox.Show(string.Format("{0} {1} {2}", Color.DarkGray.R, Color.DarkGray.G, Color.DarkGray.B));
        }

        public State GetState()
        {
            return state;
        }

        public void OpenVoid()
        {
            state = State.Opened;
            if(value == 0)
            OnOpened(this, new OpenedEventArgs(Value, state));
        }

        public void OpenMine()
        {
            state = value == -1 ? State.Exploded : State.Opened;
            if (value < 1)
                OnOpened(this, new OpenedEventArgs(Value, state));
        }
        public void Open()
        {
            if(value == -1 && state != State.Flagged && state != State.Exploded)
                state = State.Opened;
            if (value >= 0 && state == State.Flagged)
                state = State.WrongFlagged;
            Enabled = false;
        }
    }

    public class OpenedEventArgs : EventArgs
    {
        int value;
        Mine.State state;
        public int Value { get => value;}
        public Mine.State State { get => state; }

        public OpenedEventArgs(int _value, Mine.State _state)
        {
            value = _value;
            state = _state;
        }
    }
}
