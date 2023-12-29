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
    public partial class BBDD : Form, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, e);
        }

        private string database;
        private string datatable;
        private string server;

        public BBDD()
        {
            InitializeComponent();
            textBox1.DataBindings.Add("Text", this, "Database");
            textBox2.DataBindings.Add("Text", this, "Datatable");
            textBox3.DataBindings.Add("Text", this, "Server");
            database = "sys_datos";
            datatable = "dat_";
            server = "localhost";
        }

        public string Database { get => database; set
            {
                database = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Database"));
            }
        }
        public string Datatable { get => datatable; set
            {
                datatable = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Datatable"));
            }
        }

        public string Server { get => server; set
            {
                server = value;
                InvokePropertyChanged(new PropertyChangedEventArgs("Server"));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BBDD_Load(object sender, EventArgs e)
        {
            this.ActiveControl = textBox3;
        }

        private void textBox1_Focus(object sender, EventArgs e)
        {
            (sender as TextBox).SelectionStart = (sender as TextBox).Text.Length;
            (sender as TextBox).SelectionLength = 0;
        }

    }
}
