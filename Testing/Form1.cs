using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testing
{
    public partial class Form1 : Form
    {
        public string folder;
        public Form1(string folder)
        {
            InitializeComponent();
            this.folder = folder;
            MyLoad();
        }

        private void MyLoad()
        {
            DirectoryInfo di = new DirectoryInfo(folder);
            comboBox1.DataSource = di.GetDirectories();
            comboBox1.DisplayMember = "Name";
            comboBox1.ValueMember = "FullName";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedIndex > -1)
            {
                this.folder = Convert.ToString(comboBox1.SelectedValue);
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Abort;
            }
            this.Hide();
        }

    }
}
