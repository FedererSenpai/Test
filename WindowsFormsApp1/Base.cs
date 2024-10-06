using Renci.SshNet;
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
        private string script;

        public Base() : this(".txt")
        {
            
        }

        public Base(string extension = ".txt")
        {
            InitializeComponent();
            FilePath = GetType().Name + extension;
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
        public string ResultPath { get => resultPath; set => resultPath = value; } //Ruta carpeta resultados C:\Daniel\VS\Test\WindowsFormsApp1\bin\Result
        public string FilePath { get => filePath; set => filePath = value; } //Fichero form Base.txt

        public string FullPath => Path.Combine(resultPath, FilePath); //Ruta fichero form C:\Daniel\VS\Test\WindowsFormsApp1\bin\Result\Base.txt

        public string TempPath { get => Path.Combine(ResultPath, "tmp"); } //Ruta carpeta temporal C:\Daniel\VS\Test\WindowsFormsApp1\bin\Result\tmp

        public string ImagePath { get => Path.Combine(ResultPath, "Images"); } //Ruta carpeta imagenes C:\Daniel\VS\Test\WindowsFormsApp1\bin\Result\Images

        public string OwnPath => Path.Combine(ResultPath, GetType().Name);//Ruta carpeta form C:\Daniel\VS\Test\WindowsFormsApp1\bin\Result\Base
        public string OwnFullPath => Path.Combine(OwnPath, FilePath); //Ruta fichero form C:\Daniel\VS\Test\WindowsFormsApp1\bin\Result\Base\Base.txt

        public string Script { get => script; set => script = value; }

        private void Base_Load(object sender, EventArgs e)
        {
            this.Menu = new MainMenu();
            MenuItem item = new MenuItem("Menu") { Name = "Menu" };
            MenuItem window = new MenuItem("Window") { Name = "Window" };
            this.Menu.MenuItems.Add(item);
            this.Menu.MenuItems.Add(window);
            item.MenuItems.Add("Folder", new EventHandler(Folder_Click));
            window.MenuItems.Add("Image", new EventHandler(Image_Click));
            window.MenuItems.Add("Close", new EventHandler(Close_Click));
            window.MenuItems.Add("Exit", new EventHandler(Exit_Click));
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            Initialize();
        }

        public virtual void AddMenuItems(params object[] items)
        {
            if (items is null) return;
            foreach(EventHandler item in items)
            {
                AddMenu(item.Method.Name, item);
            }
        }
            
        public virtual void Initialize()
        {
            AddMenuItems(null);
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
            File.WriteAllText(Path.Combine(TempPath, file), content, Encoding.UTF8);
        }

        public virtual string ReadTempFile(string file)
        {
            return File.ReadAllText(Path.Combine(TempPath, file));
        }

        public string ResultFile(string file, bool subfolder = true) //Fichero en carpeta resultados C:\Daniel\VS\Test\WindowsFormsApp1\bin\Result\Fichero.txt
        {
            if (subfolder)
                return Path.Combine(ResultPath, GetType().Name, file);
            else
                return Path.Combine(ResultPath, file);
        }

        public void SaveResultFile(string file, string content, bool subfolder = true) //Guardar fichero en carpeta resultados
        {
            string f = ResultFile(file, subfolder);
            try
            {
                if (File.Exists(f))
                    File.Delete(f);
            }
            catch(IOException ex)
            {
                bool closed = false;
                int i = 0;
                while(!closed && i < 30)
                {
                    try
                    {
                        using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            stream.Close();
                        }
                        closed = true;
                    }
                    catch (IOException)
                    {
                        i++;
                        Thread.Sleep(100);
                    }
                }
                if (closed)
                {
                    if (File.Exists(f))
                        File.Delete(f);
                }
                else
                {
                    f = GetFileNumbered(f, 1);
                }
            }
            File.WriteAllText(f, content, Encoding.UTF8);
        }

        private string GetFileNumbered(string file, int number)
        {
            string newFile = Path.GetFileNameWithoutExtension(file) + $"({number})" + Path.GetExtension(file);
            if (!File.Exists(newFile))
                return newFile;
            return GetFileNumbered(file, number++);
        }

        public string ReadResultFile(string file, bool subfolder = true)
        {
            string content = string.Empty;
            string f = ResultFile(file, subfolder);
            if (!File.Exists(f))
                return content;
            using (StreamReader sw = new StreamReader(f, Encoding.UTF8))
            {
                content = sw.ReadToEnd();
            }
            return content;
        }

    }
}
