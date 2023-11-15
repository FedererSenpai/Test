using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FindFile : Form
    {
        List<FileInfo> files;
        string fin = Path.Combine(Application.StartupPath, "Fin");
        int count = 0;
        string folder;
        string start;
        string property;
        public FindFile()
        {
            InitializeComponent();
            comboBox1.DataSource = typeof(FileInfo).GetProperties().ToList();
            comboBox1.DisplayMember = "Name";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowNewFolderButton = false;
            fd.RootFolder = Environment.SpecialFolder.MyComputer;
            fd.ShowDialog();
            start = fd.SelectedPath;
            count = 0;
            if (File.Exists(fin))
                File.Delete(fin);
            var sw = new Stopwatch();
            sw.Start();
            BackgroundWorker bw = new BackgroundWorker();
            Task t = new Task(GetFiles);
            t.Start();
            while(!File.Exists(fin))
            {
                label1.Text = sw.Elapsed.TotalSeconds.ToString("0.00");
                label2.Text = count.ToString();
                label3.Text = folder;
                Application.DoEvents();
            }
            sw.Stop();
            label2.Text = string.Format("Found {0} files in {1} seconds", files.Count, sw.Elapsed.TotalSeconds);
            listBox1.DataSource = files.Where(x=>x.Name.Equals(textBox1.Text)).Select(x=>((PropertyInfo)comboBox1.SelectedValue).GetValue(x, null)).ToList();
        }

            public void GetFiles()
            {
                var di = new DirectoryInfo(start);
                var directories = di.GetDirectories();
                files = new List<FileInfo>();
                foreach (var directoryInfo in directories)
                {
                    try
                    {
                        GetFilesFromDirectory(directoryInfo.FullName, files);
                    }
                    catch (Exception)
                    {
                    }
                }
            File.Create(fin);
            }


            private void GetFilesFromDirectory(string directory, List<FileInfo> files)
            {
                var di = new DirectoryInfo(directory);
                var fs = di.GetFiles("*", SearchOption.TopDirectoryOnly);
            folder = directory;
            files.AddRange(fs);
            count += fs.Count();
                var directories = di.GetDirectories();
                foreach (var directoryInfo in directories)
                {
                    try
                    {
                        GetFilesFromDirectory(directoryInfo.FullName, files);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
