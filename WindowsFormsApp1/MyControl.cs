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
    public partial class MyControl : Form
    {
        Control control;

        public Control Control { get => control; set => control = value; }

        public MyControl(Control c)
        {
            InitializeComponent();
            Control = c;
            control.AutoResize();
            this.panel4.Controls.Add(Control);
            if (control.Width > this.Width)
                this.Width = control.Width + 50;
            control.Center();
        }

        private void MyControl_Load(object sender, EventArgs e)
        {
            button1.Center();
            button2.Center();
        }

        public object GetComboBox()
        {
            if (this.Control is ComboBox)
                return ((ComboBox)Control).SelectedValue;
            return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
