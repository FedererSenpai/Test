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
    public partial class Password : Form
    {
        private string pass;
        public Password(char character) : this()
        {
            textBox1.PasswordChar = character;
        }

        public Password()
        {
            InitializeComponent();
        }

        public string Pass { get => pass;}

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            pass = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            pass = string.Empty;
            DialogResult = DialogResult.Cancel;
        }
    }
}
