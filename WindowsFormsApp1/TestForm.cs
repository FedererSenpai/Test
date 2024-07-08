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
    public partial class TestForm : Base
    {
        string[] lines;
        int linea = 0;
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            lines = textBox1.Text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            textBox1.SelectionStart = 0;
            Timer timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(timer_tick);
            timer.Start();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            try
            {
                textBox1.Focus();
                textBox1.SelectionStart += textBox1.SelectionLength;
                textBox1.SelectionLength = lines[linea].Length + 2;
                linea++;
                textBox1.ScrollToCaret();
                Application.DoEvents();
            }
            catch(IndexOutOfRangeException ex)
            {
                (sender as Timer).Stop();
                (sender as Timer).Dispose();
            }
        }
    }
}
