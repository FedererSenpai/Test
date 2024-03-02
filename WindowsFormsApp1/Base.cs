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
    public partial class Base : Form
    {
        private string folderPath = Application.StartupPath;
        private string resultPath = Path.Combine(Application.StartupPath, "Result");
        private string filePath = string.Empty;

        public Base()
        {
            InitializeComponent();
            FilePath = GetType().Name + ".txt";
        }

        public string FolderPath { get => folderPath; set => folderPath = value; }
        public string ResultPath { get => resultPath; set => resultPath = value; }
        public string FilePath { get => filePath; set => filePath = value; }

        public string FullPath => Path.Combine(resultPath, FilePath);
        private void Base_Load(object sender, EventArgs e)
        {
            this.Menu = new MainMenu();
            MenuItem item = new MenuItem("Menu") { Name = "Menu"};
            this.Menu.MenuItems.Add(item);
            item.MenuItems.Add("Folder", new EventHandler(Folder_Click));
        }

        public void AddMenu(string menu, EventHandler e)
        {
            this.Menu.MenuItems["Menu"].MenuItems.Add(menu, e);
        }

        private void Folder_Click(object sender, EventArgs e)
        {
            Process.Start(FolderPath);
        }
    }
}
