using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class PingNet : Form
    {
        public PingNet()
        {
            InitializeComponent();
            textBox1.Text = GetLocalIPAddress();
            textBox2.Text = GetSubnetMask();
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetSubnetMask()
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return unicastIPAddressInformation.IPv4Mask.ToString();
                    }
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            progressBar1.ForeColor = Color.Blue;
            Dibal.PingEvent += new Dibal.PingEventHandler(this.Progress);
            Dibal.FinishEvent += new Dibal.FinishEventHandler(this.Finish);
            Dibal.IPList = new System.Collections.ObjectModel.ObservableCollection<string>();
            Dibal.IPSegment(textBox1.Text, textBox2.Text);
            progressBar1.Maximum = (int)Dibal.Hosts().Count();
            Dibal.Ping();

        }

        private void Progress(object sender, EventArgs e)
        {
            progressBar1.Value++;
            int percent = (int)(((double)progressBar1.Value / (double)progressBar1.Maximum) * 100);
            label1.Text = percent.ToString() + "% (" + progressBar1.Value + "/" + progressBar1.Maximum + ")";
            progressBar1.Refresh();
            Application.DoEvents();
        }

        private void Finish(object sender, EventArgs e)
        {
            button1.Enabled = true;
            listBox1.DataSource = Dibal.IPList;
            listBox1.Refresh();
        }
    }
}
