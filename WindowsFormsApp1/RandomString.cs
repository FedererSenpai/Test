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
    public partial class RandomString : Form
    {
        string text;
        Random r = new Random();
        public RandomString()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            text = string.Empty;
            if(numericUpDown1.Value != null)
            {
                object g = (int)numericUpDown1.Value;
                for (int i = 0; i < (int)numericUpDown1.Value;i++)
                {
                    text += (char)r.Next(65, 123);
                }
            }
            richTextBox1.Text = text;
            object o = text.Length;
            o = richTextBox1.Text.Length;
            Clipboard.SetText(text);
        }
    }
}
