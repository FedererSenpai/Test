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
    public partial class Dumb : Form
    {
        public Dumb()
        {
            InitializeComponent();
        }

        public void MoveControl(Control c)
        {
            Random r = new Random();
            c.Location = new Point(r.Next(100, 701), r.Next(100, 801));
        }

        public bool CheckIntersect(Control c1, Control c2)
        {
            Rectangle r = new Rectangle(c1.Location,c1.Size);
            Rectangle r2 = new Rectangle(c2.Location, c2.Size);
            return r.IntersectsWith(r2);
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            MessageBox.Show("I knew it!!!!!!!!!!!!!!!");
            Application.Exit();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            do
            {
                MoveControl(btnNo);
            }
            while (CheckIntersect(btnYes, btnNo));
        }
    }
}
