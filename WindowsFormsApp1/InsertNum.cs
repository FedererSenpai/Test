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
    public partial class InsertNum : Form
    {
        DatosInsert di;
        public InsertNum(DatosInsert di)
        {
            InitializeComponent();
            this.di = di;
        }

        private void BindData(DatosInsert di)
        {
            this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            this.numericUpDown2.DataBindings.Add("Text", di, "Maximo");
            this.numericUpDown3.DataBindings.Add("Text", di, "Minimo");
            this.numericUpDown4.DataBindings.Add("Text", di, "Decimalesmax");
            this.checkBox1.DataBindings.Add("Checked", di, "NumDecimal");
            this.checkBox2.DataBindings.Add("Checked", di, "NoCero");
            this.checkBox3.DataBindings.Add("Checked", di, "Autoincremento");
            this.checkBox4.DataBindings.Add("Checked", di, "Rango");
            this.textBox1.DataBindings.Add("Text", di, "Texto");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            switch ((sender as RadioButton).Name)
            {
                case "radioButton1":
                    if (this.radioButton1.Checked)
                    {
                        this.groupBox1.Enabled = true;
                        this.groupBox2.Enabled = false;
                        di.Aleatorio = true;
                    }
                    break;
                case "radioButton2":
                    if (this.radioButton2.Checked)
                    {
                        this.groupBox1.Enabled = false;
                        this.groupBox2.Enabled = true;
                        di.Aleatorio = false;
                    }
                    break;
            }
        }

        private void InsertNum_Load(object sender, EventArgs e)
        {
            BindData(di);
            if (di.Aleatorio)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;

        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            switch((sender as CheckBox).Text)
            {
                case "Decimal":
                    if (this.checkBox1.Checked)
                        this.panel2.Enabled = true;
                    else
                        this.panel2.Enabled = false;
                    break;
            }
        }

    }
}
