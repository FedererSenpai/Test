using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApp1
{
    public partial class NombrarImagenes : Form
    {
        public NombrarImagenes()
        {
            InitializeComponent();
            CrearCombo();
        }

        private void CrearCombo()
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add(new DataColumn("Tipo"));
            DataRow dr = ds.Tables[0].NewRow();
            dr[0] = "Imagenes";
            ds.Tables[0].Rows.Add(dr);
            comboBox1.ValueMember = ds.Tables[0].Columns["Tipo"].ToString();
            comboBox1.DisplayMember = ds.Tables[0].Columns["Tipo"].ToString();
            comboBox1.DataSource = ds.Tables[0];
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string [] files = Directory.GetFiles(textBox1.Text);
            int i = 1;
            int j = 0;
            ImageList imageList = new ImageList();
            CheckForIllegalCrossThreadCalls = false;
            //while (j < 20000)
            //{
            //    try
            //    {
            //        string f = files[0];
            //        string oldname = f.Split('\\').Last();
            //        imageList.ImageSize = new Size(108, 108);
            //        imageList.ColorDepth = ColorDepth.Depth32Bit;
            //        Image img1 = Image.FromFile(f);
            //        imageList.Images.Add(img1);
            //        textBox2.Text = j.ToString();
            //        this.listView1.Items.Add(j.ToString("000000") + ".jpg");
            //        this.listView1.Items[j].ImageIndex = j;
            //        j++;
            //       // await Task.Run(() =>
            //        //{
            //            this.listView1.View = View.LargeIcon;
            //            this.listView1.LargeImageList = imageList;
            //            this.listView1.Sorting = SortOrder.Ascending;
            //            this.listView1.Sort();
            //        //});
            //    }
            //    catch (Exception ex){ MessageBox.Show(""); }
            //}
            bool error = true;
            foreach (string f in files)
            {
               
                try
                {
                    string oldname = f.Split('\\').Last();
                    while (error)
                    {
                        if (!File.Exists(f.Replace(oldname, i.ToString("000000") + ".jpg")))
                        {
                            File.Move(f, f.Replace(oldname, i.ToString("000000") + ".jpg"));
                            error = false;
                        }
                        else
                            i++;
                    }
                    i++;
                    error = true;
                }
                catch { }
            }
        }
    }
}
