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
    public partial class InsertBool : Form
    {
        DatosInsert di;

        public InsertBool(DatosInsert di)
        {
            InitializeComponent();
            this.di = di;
        }

        public void BindData(DatosInsert di)
        {
            this.checkBox1.DataBindings.Add("Checked", di, "Aleatorio");
            this.checkBox2.DataBindings.Add("Checked", di, "Repetir");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InsertText_Load(object sender, EventArgs e)
        {
            BindData(di);
            if (di.TrueFalse)
            {
                this.radioButton1.Checked = true;
                //this.radioButton2.Checked = false;
            }
            else
            {
                this.radioButton2.Checked = true;
                //this.radioButton1.Checked = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if((sender as CheckBox).Checked)
            {
                this.radioButton1.Enabled = false;
                this.radioButton2.Enabled = false;
                this.checkBox2.Enabled = true;
            }
            else
            {
                this.radioButton1.Enabled = true;
                this.radioButton2.Enabled = true;
                this.checkBox2.Enabled = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            di.TrueFalse = this.radioButton1.Checked;
        }
    }
}
