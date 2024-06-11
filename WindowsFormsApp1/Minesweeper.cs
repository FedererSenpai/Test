using PeNet.Header.Net;
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
    public partial class Minesweeper : Base
    {
        public Minesweeper()
        {
            InitializeComponent();
            this.Load += Start;
            Create();
        }

        private void Start(object sender, EventArgs e)
        {
            AddMenu("New", new EventHandler(New));
        }
        private void New(object sender, EventArgs e)
        {
            Create();
        }

        private void Reveal()
        {
            WinUtil.LockWindowUpdate(this.Handle);
            foreach(Control c in Controls)
            {
                (c as Mine).Open();
            }
            WinUtil.LockWindowUpdate(IntPtr.Zero);
        }
        public void CheckMine(object sender, OpenedEventArgs e)
        {
            if(e.Value == -1)
            {
                Reveal();
            }
            else if(e.Value == 0)
            {
                int index = Controls.IndexOf(sender as Mine);
                if (index % 16 != 0 && (Controls[index - 1] as Mine).GetState() != Mine.State.Opened)
                    (Controls[index - 1] as Mine).OpenVoid();
                if (index < 464 && index % 16 != 0 && (Controls[index + 15] as Mine).GetState() != Mine.State.Opened)
                    (Controls[index + 15] as Mine).OpenVoid();
                if (index < 464 && (Controls[index + 16] as Mine).GetState() != Mine.State.Opened)
                    (Controls[index + 16] as Mine).OpenVoid();
                if (index < 464 && index % 16 != 15 && (Controls[index + 17] as Mine).GetState() != Mine.State.Opened)
                (Controls[index + 17] as Mine).OpenVoid();
                if (index % 16 != 15 && (Controls[index + 1] as Mine).GetState() != Mine.State.Opened)
                (Controls[index + 1] as Mine).OpenVoid();
                if (index > 15 && index % 16 != 15 && (Controls[index - 15] as Mine).GetState() != Mine.State.Opened)
                (Controls[index - 15] as Mine).OpenVoid();
                if (index > 15 && (Controls[index - 16] as Mine).GetState() != Mine.State.Opened)
                (Controls[index - 16] as Mine).OpenVoid();
                if (index > 15 && index % 16 != 0 && (Controls[index - 17] as Mine).GetState() != Mine.State.Opened)
                (Controls[index - 17] as Mine).OpenVoid();
            }
            else
            {
                int index = Controls.IndexOf(sender as Mine);
                int count = 0;
                if (index % 16 != 0 && (Controls[index - 1] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index < 464 && index % 16 != 0 && (Controls[index + 15] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index < 464 && (Controls[index + 16] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index < 464 && index % 16 != 15 && (Controls[index + 17] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index % 16 != 15 && (Controls[index + 1] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index > 15 && index % 16 != 15 && (Controls[index - 15] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index > 15 && (Controls[index - 16] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if (count < e.Value && index > 15 && index % 16 != 0 && (Controls[index - 17] as Mine).GetState() == Mine.State.Flagged)
                    count++;
                if(count == e.Value)
                {
                    if (index % 16 != 0 && (Controls[index - 1] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index - 1] as Mine).OpenMine();
                    if (index < 464 && index % 16 != 0 && (Controls[index + 15] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index + 15] as Mine).OpenMine();
                    if (index < 464 && (Controls[index + 16] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index + 16] as Mine).OpenMine();
                    if (index < 464 && index % 16 != 15 && (Controls[index + 17] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index + 17] as Mine).OpenMine();
                    if (index % 16 != 15 && (Controls[index + 1] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index + 1] as Mine).OpenMine();
                    if (index > 15 && index % 16 != 15 && (Controls[index - 15] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index - 15] as Mine).OpenMine();
                    if (index > 15 && (Controls[index - 16] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index - 16] as Mine).OpenMine();
                    if (index > 15 && index % 16 != 0 && (Controls[index - 17] as Mine).GetState() == Mine.State.Closed)
                        (Controls[index - 17] as Mine).OpenMine();
                }
                Refresh();
            }
        }
        private void Create()
        {
            Controls.Clear();
            for (int i = 1; i <= 30; i++)
            {
                for (int j = 1; j <= 16; j++)
                {
                    Mine m = new Mine();
                    m.Location = new Point(i * 16, j * 16);
                    m.OnOpened += new Mine.OpenedEventHandler(CheckMine);
                    Controls.Add(m);
                }
            }
            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                bool set = false;
                do
                {
                    int index = r.Next(480);
                    if ((Controls[index] as Mine).Value == int.MinValue)
                    {
                        (Controls[index] as Mine).Value = -1;
                        set = true;
                    }
                }
                while (!set);
            }
            foreach (Control c in Controls)
            {
                if ((c as Mine).Value == int.MinValue)
                {
                    int count = 0;
                    int index = Controls.IndexOf(c);
                    if (index % 16 != 0 && (Controls[index - 1] as Mine).Value == -1)
                        count++;
                    if (index < 464 && index % 16 != 0 && (Controls[index + 15] as Mine).Value == -1)
                        count++;
                    if (index < 464 && (Controls[index + 16] as Mine).Value == -1)
                        count++;
                    if (index < 464 && index % 16 != 15 && (Controls[index + 17] as Mine).Value == -1)
                        count++;
                    if (index % 16 != 15 && (Controls[index + 1] as Mine).Value == -1)
                        count++;
                    if (index > 15 && index % 16 != 15 && (Controls[index - 15] as Mine).Value == -1)
                        count++;
                    if (index > 15 && (Controls[index - 16] as Mine).Value == -1)
                        count++;
                    if (index > 15 && index % 16 != 0 && (Controls[index - 17] as Mine).Value == -1)
                        count++;
                    (c as Mine).Value = count;
                }
            }
        }
    }
}
