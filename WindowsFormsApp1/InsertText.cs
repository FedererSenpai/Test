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

    }
}
