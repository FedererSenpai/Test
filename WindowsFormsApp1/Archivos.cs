using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Archivos : Form
    {
        public Archivos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NetworkCredential nc = new NetworkCredential(@"CS1200", "CS1200");
            CredentialCache cc = new CredentialCache();
            cc.Add(new Uri(@"\\192.168.150.66"), "Basic", nc);
            string[] f = Directory.GetDirectories(@"\\192.168.150.66\c$\SW1100");
        }
    }
}
