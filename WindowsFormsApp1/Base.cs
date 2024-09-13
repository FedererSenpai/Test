using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Base : Form
    {
        private string folderPath = Application.StartupPath;
        private string resultPath = string.Empty;
        private string filePath = string.Empty;
        private string tempPath = string.Empty;       
        public Base()
        {
            InitializeComponent();
            FilePath = GetType().Name + ".txt";
            ResultPath = Path.Combine(FolderPath, "Result");
        }

        public string FolderPath {
            get
            {
                if (folderPath.EndsWith("Debug") || folderPath.EndsWith("Release"))
                    return Directory.GetParent(Application.StartupPath).FullName;
                else
                    return folderPath;
            }
            set => folderPath = value; }
        public string ResultPath { get => resultPath; set => resultPath = value; }
        public string FilePath { get => filePath; set => filePath = value; }

        public string FullPath => Path.Combine(resultPath, FilePath);

        public string TempPath { get => Path.Combine(ResultPath, "tmp");}

        public string ImagePath { get => Path.Combine(ResultPath, "Images"); }
        private void Base_Load(object sender, EventArgs e)
        {
            this.Menu = new MainMenu();
            MenuItem item = new MenuItem("Menu") { Name = "Menu"};
            MenuItem window = new MenuItem("Window") { Name = "Window" };
            this.Menu.MenuItems.Add(item);
            this.Menu.MenuItems.Add(window);
            item.MenuItems.Add("Folder", new EventHandler(Folder_Click));
            window.MenuItems.Add("Image", new EventHandler(Image_Click));
            window.MenuItems.Add("Close", new EventHandler(Close_Click));
            window.MenuItems.Add("Exit", new EventHandler(Exit_Click));
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
        }
        public void AddMenu(string menu, EventHandler e)
        {
            this.Menu.MenuItems["Menu"].MenuItems.Add(menu, e);
        }

        private void Folder_Click(object sender, EventArgs e)
        {
            Process.Start(FolderPath);
        }

        private void Image_Click(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            Application.DoEvents();
            string path = Path.Combine(ResultPath, "Images", Name + ".png");
            ExtensionMethods.ScreenShot(this.Bounds, path);
        }
        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public virtual void WriteTempFile(string file, string content)
        {
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(Path.Combine(TempPath,file), content, Encoding.UTF8);
        }

        public virtual string ReadTempFile(string file)
        {
            return File.ReadAllText(Path.Combine(TempPath, file));
        }

        public string ResultFile(string file, bool subfolder = true)
        {
            if(subfolder)
                return Path.Combine(ResultPath, GetType().Name, file);
            else
                return Path.Combine(ResultPath, file);
        }

        public void SaveResultFile(string file, string content, bool subfolder = true)
        {
            if (File.Exists(file))
                File.Delete(file);
            File.WriteAllText(ResultFile(file, subfolder), content, Encoding.UTF8);
        }
    }
}
