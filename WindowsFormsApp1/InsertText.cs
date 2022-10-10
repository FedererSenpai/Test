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
    public partial class InsertText : Form
    {
        DatosInsert di;

        public InsertText(DatosInsert di)
        {
            InitializeComponent();
            this.di = di;
        }

        public void BindData(DatosInsert di)
        {
            this.numericUpDown2.DataBindings.Add("Text", di, "Longitud");
            this.numericUpDown1.DataBindings.Add("Text", di, "Minimo");
            this.numericUpDown3.DataBindings.Add("Text", di, "Maximo");
            this.checkBox1.DataBindings.Add("Checked", di, "Minusculas");
            this.checkBox2.DataBindings.Add("Checked", di, "Mayusculas");
            this.checkBox3.DataBindings.Add("Checked", di, "Numeros");
            this.checkBox4.DataBindings.Add("Checked", di, "Caracteresespeciales");
            this.checkBox5.DataBindings.Add("Checked", di, "Primermayuscula");
            this.checkBox6.DataBindings.Add("Checked", di, "Repetir");
            this.textBox1.DataBindings.Add("Text", di, "Texto");
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            switch((sender as RadioButton).Name)
            {
                case "radioButton1":
                    if(this.radioButton1.Checked)
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
                case "radioButton3":
                    if (this.radioButton3.Checked)
                    {
                        this.panel1.Enabled = true;
                        this.panel2.Enabled = false;
                        di.Fijo = true;
                    }
                        break;
                case "radioButton4":
                    if (this.radioButton4.Checked)
                    {
                        this.panel1.Enabled = false;
                        this.panel2.Enabled = true;
                        di.Fijo = false;
                    }
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InsertText_Load(object sender, EventArgs e)
        {
            BindData(di);
            if (di.Aleatorio)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;

            if (di.Fijo)
                radioButton3.Checked = true;
            else
                radioButton4.Checked = true;
            }
    }
}
