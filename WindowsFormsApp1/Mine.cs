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
        public static bool revealing = false;
        public static int opens = 0;
        public static int flags = 99;
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
        public State MyState
        {
            get => state;
            set
            {
                if (value != state)
                {
                    if(value == State.Opened)
                        opens++;
                    if (value == State.Flagged)
                        flags--;
                    if (state == State.Flagged)
                        flags++;
                }
                state = value;
                if (value == State.Opened && !revealing)
                    OnOpened();
                Refresh();
            }
        }

        public enum State
        {
            Opened = 1,
            Closed = 2,
            Flagged = 3,
            WrongFlagged = 4,
            Exploded = 5
        }

        public delegate void OpenedEventHandler(object sender, OpenedEventArgs e);
        public event OpenedEventHandler Opened;
        protected virtual void OnOpened()
        {
            Opened?.Invoke(this, new OpenedEventArgs(Value, MyState));
        }
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
                if (MyState == State.Opened)
                    return;
                if (MyState == State.Flagged)
                    MyState = State.Closed;
                else
                    MyState = State.Flagged;
            }
            else if(e.Button == MouseButtons.Left)
            {
                if (state == State.Flagged)
                    return;
                //Enabled = false;
                if (value == -1)
                    MyState = State.Exploded;
                else
                    MyState = State.Opened;
                OnOpened();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (MyState == State.Exploded)
                g.DrawImage(Properties.Resources.MineExploded, new RectangleF(0, 0, Width, Height));
            else if (MyState == State.WrongFlagged)
            {
                g.DrawImage(Properties.Resources.flagwrong, new RectangleF(0, 0, Width, Height));
            }
            else if (MyState == State.Opened)
            {
                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                    g.FillRectangle(Brushes.DarkGray, new RectangleF(0, 0, Width, Height));
                    if (value < 0)
                    {
                        g.DrawImage(Properties.Resources.Mine, new RectangleF(0, 0, Width, Height));
                    }
                    else
                    {
                        g.DrawString(value.ToString(), new Font("Calibri", 13, FontStyle.Bold), fores[value], e.ClipRectangle, format);
                    }
            }
            else
            {
                g.FillPolygon(Brushes.WhiteSmoke, new Point[] { new Point(0, 0), new Point(Width, 0), new Point(0, Height) });
                g.FillPolygon(Brushes.Gray, new Point[] { new Point(Width, Height), new Point(Width, 0), new Point(0, Height) });
                if (MyState == State.Flagged)
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
            return MyState;
        }

        public void OpenVoid()
        {
            MyState = State.Opened;
            if(value == 0)
            OnOpened();
        }

        public void OpenMine()
        {
            MyState = value == -1 ? State.Exploded : State.Opened;
            if (value < 1)
                OnOpened();
        }
        public void Open()
        {
            if(value == -1 && MyState != State.Flagged && MyState != State.Exploded)
                MyState = State.Opened;
            if (value >= 0 && MyState == State.Flagged)
                MyState = State.WrongFlagged;
            Enabled = false;
        }

        public void Flag()
        {
            if (value == -1)
                MyState = State.Flagged;
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
