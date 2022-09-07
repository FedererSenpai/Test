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
            BindData(di);
            this.di = di;
        }

        public void BindData(DatosInsert di)
        {
            this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            this.radioButton1.DataBindings.Add("Checked", di, "Aleatorio");
            this.radioButton3.DataBindings.Add("Checked", di, "Fijo");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
            //this.numericUpDown1.DataBindings.Add("Text", di, "Longitud");
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
                    }
                        break;
                case "radioButton2":
                    if (this.radioButton2.Checked)
                    {
                        this.groupBox1.Enabled = false;
                        this.groupBox2.Enabled = true;
                    }
                    break;
                case "radioButton3":
                    if (this.radioButton3.Checked)
                    {
                        this.panel1.Enabled = true;
                        this.panel2.Enabled = false;
                        this.panel3.Enabled = false;
                    }
                        break;
                case "radioButton4":
                    if (this.radioButton4.Checked)
                    {
                        this.panel1.Enabled = false;
                        this.panel2.Enabled = true;
                        this.panel3.Enabled = true;
                    }
                    break;
            }
        }
    }
}
