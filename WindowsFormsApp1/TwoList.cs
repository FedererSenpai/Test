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
    public partial class TwoList : Form
    {
        public TwoList()
        {
            InitializeComponent();
            listBox1.DataSource = Dibal.ag;
            listBox2.DataSource = Dibal.df;
        }
    }
}
