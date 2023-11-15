using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Net : Form
    {
        Random r = new Random();
        public Net()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo psi =
new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + "Ethernet" + "\" enable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo("netsh", "interface set interface \"" + "Ethernet" + "\" disable");
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            StreamWriter s = new StreamWriter(Application.StartupPath + "\\Net.txt");
            int t = 0;
            while (true)
            {
                t = r.Next(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
                textBox3.Text = t.ToString();
                Application.DoEvents();
                sw.Restart();
                while(sw.ElapsedMilliseconds < t * 1000)
                { }
                sw.Stop();
                button3_Click(button3, e);
                s.WriteLine("Disconnected: " + DateTime.Now.ToString());
                textBox3.ForeColor = Color.Red;
                Application.DoEvents();
                t = r.Next(Convert.ToInt32(textBox1.Text), Convert.ToInt32(textBox2.Text));
                textBox3.Text = t.ToString();
                Application.DoEvents();
                sw.Restart();
                while (sw.ElapsedMilliseconds < t * 1000)
                { }
                sw.Stop();
                button2_Click(button2, e);
                s.WriteLine("Connected: " + DateTime.Now.ToString());
                textBox3.ForeColor = Color.Green;
                Application.DoEvents();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
