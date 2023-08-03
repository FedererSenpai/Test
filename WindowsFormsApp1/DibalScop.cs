using PeNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DibalScop : Form
    {
        [DllImport("Dibalscop.dll")]
        static extern string DataSend2();
        public DibalScop()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pe = new PeFile(@"Dibalscop.dll");
            var functions = pe.ExportedFunctions.Select(x => x.Name).ToList();
            string response = DataSend2();
        }
    }
}
