using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FindFile : Form
    {
        public FindFile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var b = new BuildFileList();
            var sw = new Stopwatch();
            sw.Start();
            var files = b.GetFiles();
            sw.Stop();
            label1.Text = string.Format("Found {0} files in {1} seconds", files.Count, sw.Elapsed.TotalSeconds);

        }

        public class BuildFileList
        {
            public List<FileInfo> GetFiles()
            {
                var di = new DirectoryInfo(@"C:\");
                var directories = di.GetDirectories();
                var files = new List<FileInfo>();
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
                return files;
            }


            private void GetFilesFromDirectory(string directory, List<FileInfo> files)
            {
                var di = new DirectoryInfo(directory);
                var fs = di.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                files.AddRange(fs);
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
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
