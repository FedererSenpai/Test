using CefSharp.DevTools.Page;
using PeNet.Header.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    public partial class Minesweeper : Base
    {
        private Panel p3;
        private Panel p2;
        private Timer t1;
        private Timer t2;
        private Label l1;
        private Label l2;
        private Stopwatch sw;
        public Minesweeper()
        {
            InitializeComponent();
            this.Load += Start;
            Create();
            Width = 665;
            Height = 451;
            MinimumSize = new Size(665, 451);
        }

        private void Start(object sender, EventArgs e)
        {
            AddMenu("New", new EventHandler(New));
        }
        private void New(object sender, EventArgs e)
        {
            t1.Dispose();
            t2.Dispose();
            Mine.flags = 99;
            Mine.opens = 0;
            Create();
        }

        private void Reveal()
        {
            t1.Stop();
            Mine.revealing = true;
            foreach(Control c in p2.Controls)
            {
                (c as Mine).Open();
            }
        }
        public void CheckMine(object sender, OpenedEventArgs e)
        {
            if(e.Value == -1)
            {
                Reveal();
            }
            else if(e.Value == 0)
            {
                int index = p2.Controls.IndexOf(sender as Mine);
                if (index % 16 != 0 && (p2.Controls[index - 1] as Mine).GetState() != Mine.State.Opened)
                    (p2.Controls[index - 1] as Mine).OpenVoid();
                if (index < 464 && index % 16 != 0 && (p2.Controls[index + 15] as Mine).GetState() != Mine.State.Opened)
                    (p2.Controls[index + 15] as Mine).OpenVoid();
                if (index < 464 && (p2.Controls[index + 16] as Mine).GetState() != Mine.State.Opened)
                    (p2.Controls[index + 16] as Mine).OpenVoid();
                if (index < 464 && index % 16 != 15 && (p2.Controls[index + 17] as Mine).GetState() != Mine.State.Opened)
                (p2.Controls[index + 17] as Mine).OpenVoid();
                if (index % 16 != 15 && (p2.Controls[index + 1] as Mine).GetState() != Mine.State.Opened)
                (p2.Controls[index + 1] as Mine).OpenVoid();
                if (index > 15 && index % 16 != 15 && (p2.Controls[index - 15] as Mine).GetState() != Mine.State.Opened)
                (p2.Controls[index - 15] as Mine).OpenVoid();
                if (index > 15 && (p2.Controls[index - 16] as Mine).GetState() != Mine.State.Opened)
                (p2.Controls[index - 16] as Mine).OpenVoid();
                if (index > 15 && index % 16 != 0 && (p2.Controls[index - 17] as Mine).GetState() != Mine.State.Opened)
                (p2.Controls[index - 17] as Mine).OpenVoid();
            }
            else
            {
                int index = p2.Controls.IndexOf(sender as Mine);
                int count = 0;
                if (index % 16 != 0 && (p2.Controls[index - 1] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index < 464 && index % 16 != 0 && (p2.Controls[index + 15] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index < 464 && (p2.Controls[index + 16] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index < 464 && index % 16 != 15 && (p2.Controls[index + 17] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index % 16 != 15 && (p2.Controls[index + 1] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index > 15 && index % 16 != 15 && (p2.Controls[index - 15] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index > 15 && (p2.Controls[index - 16] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index > 15 && index % 16 != 0 && (p2.Controls[index - 17] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if(count == e.Value)
                {
                    if (index % 16 != 0 && (p2.Controls[index - 1] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index - 1] as Mine).OpenMine();
                    if (index < 464 && index % 16 != 0 && (p2.Controls[index + 15] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index + 15] as Mine).OpenMine();
                    if (index < 464 && (p2.Controls[index + 16] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index + 16] as Mine).OpenMine();
                    if (index < 464 && index % 16 != 15 && (p2.Controls[index + 17] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index + 17] as Mine).OpenMine();
                    if (index % 16 != 15 && (p2.Controls[index + 1] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index + 1] as Mine).OpenMine();
                    if (index > 15 && index % 16 != 15 && (p2.Controls[index - 15] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index - 15] as Mine).OpenMine();
                    if (index > 15 && (p2.Controls[index - 16] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index - 16] as Mine).OpenMine();
                    if (index > 15 && index % 16 != 0 && (p2.Controls[index - 17] as Mine).GetState() == Mine.State.Closed)
                        (p2.Controls[index - 17] as Mine).OpenMine();
                }
                Update();
            }
        }
        private void Create()
        {
            Controls.Clear();
            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.RowCount = 2;
            tlp.ColumnCount = 1;
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 90));
            tlp.Dock = DockStyle.Fill;
            Panel p1 = new Panel();
            p1.Dock = DockStyle.Fill;
            Button b = new Button();
            b.Anchor = AnchorStyles.None;
            p1.Controls.Add(b);
            b.Top = (p1.Height - b.Height) / 2;
            b.Left = (p1.Width - b.Width) / 2;
            b.Height = 30;
            b.Width = 30;
            b.Image = Properties.Resources.feliz;
            b.ImageAlign = ContentAlignment.MiddleCenter;
            b.Click += new EventHandler(New);
            l2 = new Label();
            l2.Text = 0.ToString("000");
            l2.ForeColor = Color.Red;
            l2.Font = new Font("Calibri", 18, FontStyle.Bold);
            l2.TextAlign = ContentAlignment.MiddleRight;
            l1 = new Label();
            l1.Left = 30;
            l1.Text = 99.ToString("000");
            l1.ForeColor = Color.Red;
            l1.Font = new Font("Calibri", 18, FontStyle.Bold);
            p1.Controls.Add(l1);
            p1.Controls.Add(l2);
            p3 = new Panel();
            p3.Dock = DockStyle.Fill;
            p3.Margin = new Padding(10);
            p2 = new Panel();
            p2.Margin = new Padding(0);
            p2.Width = 630;
            p2.Height = 336;
            p3.Controls.Add(p2);
            tlp.Controls.Add(p1);
            tlp.Controls.Add(p3);
            Controls.Add(tlp);
            p2.Left = (p3.Width - p2.Width) / 2;
            p2.Top = (p3.Height - p2.Height) / 2;
            l2.Left = p2.Width - l2.Width - 30;
            l2.Top = (p1.Height - l2.Height) / 2;
            l1.Top = (p1.Height - l1.Height) / 2;
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    Mine m = new Mine();
                    m.Location = new Point(i * 21, j * 21);
                    m.Opened += new Mine.OpenedEventHandler(CheckMine);
                    p2.Controls.Add(m);
                }
            }
            Random r = new Random();
            for (int i = 0; i < 99; i++)
            {
                bool set = false;
                do
                {
                    int index = r.Next(480);
                    if ((p2.Controls[index] as Mine).Value == int.MinValue)
                    {
                        (p2.Controls[index] as Mine).Value = -1;
                        set = true;
                    }
                }
                while (!set);
            }
            foreach (Control c in p2.Controls)
            {
                if ((c as Mine).Value == int.MinValue)
                {
                    int count = 0;
                    int index = p2.Controls.IndexOf(c);
                    if (index % 16 != 0 && (p2.Controls[index - 1] as Mine).Value == -1)
                        count++;
                    if (index < 464 && index % 16 != 0 && (p2.Controls[index + 15] as Mine).Value == -1)
                        count++;
                    if (index < 464 && (p2.Controls[index + 16] as Mine).Value == -1)
                        count++;
                    if (index < 464 && index % 16 != 15 && (p2.Controls[index + 17] as Mine).Value == -1)
                        count++;
                    if (index % 16 != 15 && (p2.Controls[index + 1] as Mine).Value == -1)
                        count++;
                    if (index > 15 && index % 16 != 15 && (p2.Controls[index - 15] as Mine).Value == -1)
                        count++;
                    if (index > 15 && (p2.Controls[index - 16] as Mine).Value == -1)
                        count++;
                    if (index > 15 && index % 16 != 0 && (p2.Controls[index - 17] as Mine).Value == -1)
                        count++;
                    (c as Mine).Value = count;
                }
            }
            t1 = new Timer();
            t1.Interval = 1000;
            t1.Tick += new EventHandler(Tiempo);
            t1.Start();
            t2 = new Timer();
            t2.Interval = 100;
            t2.Tick += new EventHandler(Flags);
            t2.Start();
            sw = new Stopwatch();
            sw.Start();         
            /*foreach(Control c in p2.Controls)
            {
                if ((c as Mine).Value != -1)
                    (c as Mine).PerformClick();
                System.Threading.Thread.Sleep(100);
            }*/
        }

        private void Flags(object sender, EventArgs e)
        {
            if(Mine.opens >= 381)
            {
                End();
            }
            l1.Text = Mine.flags.ToString("000");
        }

        private void End()
        {
            t1.Stop();
            t2.Stop();
            sw.Stop();
            foreach (Control c in p2.Controls)
            {
                (c as Mine).Flag();
            }
            MessageBox.Show($"Time: {(sw.ElapsedMilliseconds / 1000.0).ToString("##0.00")}");
        }
        private void Tiempo(object sender, EventArgs e)
        {
            int.TryParse(l2.Text, out int t);
            t++;
            l2.Text = t.ToString("000");
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if(p2 != null && p3 != null)
            {
            p2.Left = (p3.Width - p2.Width) / 2;
            p2.Top = (p3.Height - p2.Height) / 2;
            }
        }

    }
}
