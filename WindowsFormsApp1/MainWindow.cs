using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class MainWindow : Base
    {
        string currentnamespace;
        public MainWindow()
        {
            InitializeComponent();
            currentnamespace = Assembly.GetExecutingAssembly().GetName().Name + ".";
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            AddMenu("New", new EventHandler(New_Click));
            foreach (string images in Directory.GetFiles(ImagePath))
            {
                string name = Path.GetFileNameWithoutExtension(images);
                if (CheckForm(name))
                {
                    ToolStripMenuItem tsmi = new ToolStripMenuItem(name, null, new EventHandler(ToolStripMenuItem_Click));
                    tsmi.BackColor = Color.LightGray;
                    tsmi.Margin = new Padding(0);
                    tsmi.Padding = new Padding(0);
                    tsmi.MouseEnter += new EventHandler(ToolStripMenuItem_MouseEnter);
                    tsmi.MouseLeave += new EventHandler(ToolStripMenuItem_MouseLeave);
                    tsmi.ToolTipText = images;
                    menuStrip1.Items.Add(tsmi);
                }
            }
        }

        private bool CheckForm(string type)
        {
            var form = Type.GetType(currentnamespace + type, false, true);
            if (form == null)
                return false;
            bool isBaseForm = typeof(Base).IsAssignableFrom(form);
            return isBaseForm;
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                string name = (sender as ToolStripItem).Text;
                object form = Activator.CreateInstance(Type.GetType(currentnamespace + name, false, true));
                (form as Base).ShowDialog();
                if(!this.IsDisposed)
                    this.Show();
            }
            catch(Exception ex) { MessageBox.Show(this, ex.Message); }
        }

        private void New_Click(object sender, EventArgs e)
        {
            this.Hide();
            TextBox tb = new TextBox() { Width = this.Width - 100 };
            MyControl mc = new MyControl(tb) { };
            if(mc.isOK())
            {
                try
                { 
                    string name = tb.Text;
                    object form = Activator.CreateInstance(Type.GetType(currentnamespace + name, false, true));
                    (form as Base).ShowDialog();
                }
                catch(Exception ex)
                {

                }
            }
            this.Show();
        }

        private void ToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = (sender as ToolStripMenuItem).ToolTipText;
        }

        private void ToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = null;
        }

    }
}
