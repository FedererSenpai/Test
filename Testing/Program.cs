using ExcelDataReader;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testing
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyAppContext());
        }
    }

    class MyAppContext : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        int number = 0;
        string root = "T:\\17-RegistroPruebas";
        string end => "." + number.ToString("000");
        string folder = string.Empty;
        string testingf = "01.-Testing Scope";
        string defectsf = "04.-Defects to Correct";
        string testingfile = string.Empty;
        string defectsfile = string.Empty;
        byte[] hash;
        DataSet datosExcel;
        int lastdefect = 0;

        public MyAppContext()
        {
            try
            {
                MenuItem planMenuItem = new MenuItem("Testing Plan", new EventHandler(TestingPlan));
                MenuItem defectMenuItem = new MenuItem("Defects", new EventHandler(Defects));
                MenuItem selectMenuItem = new MenuItem("Select", new EventHandler(Select));
                MenuItem folderMenuItem = new MenuItem("Folder", new EventHandler(Folder));
                MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));

                notifyIcon.Icon = Properties.Resources.dna_test;
                notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                    { planMenuItem, defectMenuItem, selectMenuItem, folderMenuItem, exitMenuItem});
                notifyIcon.Visible = true;

                number = Properties.Settings.Default.Number;
                lastdefect = Properties.Settings.Default.LastDefect;
                new ToastContentBuilder().AddText(Convert.ToString(number + "-" + lastdefect)).Show();
                if (number <= 0 || !Directory.GetDirectories(root).Any(x => x.EndsWith(end)))
                {
                    Select(this, null);
                }
                else
                {
                    folder = Directory.GetDirectories(root).Single(x => x.EndsWith(end));
                    string directory = Path.Combine(folder, testingf);
                    testingfile = Directory.GetFiles(directory).Single(x => Path.GetFileName(x).StartsWith("Testing Plan") && x.EndsWith(".xlsx"));
                    directory = Path.Combine(folder, defectsf);
                    defectsfile = Directory.GetFiles(directory).Single(x => Path.GetFileName(x).StartsWith("Defects") && x.EndsWith(".xlsx"));
                }
                Timer t = new Timer();
                t.Tick += Check;
                t.Interval = 60000;
                t.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        void TestingPlan(object sender, EventArgs e)
        {
            string tmp = Path.Combine(Path.GetTempPath(), "TestingPlan.xlsx");
            File.Copy(testingfile, tmp, true);
            Process.Start(tmp);
        }

        void Defects(object sender, EventArgs e)
        {
            Process.Start(defectsfile);
        }

        void Select(object sender, EventArgs e)
        {
            Form1 f = new Form1(root);
            f.ShowDialog();
            folder = f.folder;
            number = int.Parse(folder.Split('.').Last());
            string directory = Path.Combine(folder, testingf);
            testingfile = Directory.GetFiles(directory).Single(x => Path.GetFileName(x).StartsWith("Testing Plan") && x.EndsWith(".xlsx"));
            directory = Path.Combine(folder, defectsf);
            defectsfile = Directory.GetFiles(directory).Single(x => Path.GetFileName(x).StartsWith("Defects") && x.EndsWith(".xlsx"));
            Leer();
            lastdefect = datosExcel.Tables["Defects"].Rows.IndexOf(datosExcel.Tables["Defects"].Rows.Cast<DataRow>().Last(x => !string.IsNullOrEmpty(Convert.ToString(x[1])))) - 5;
            f.Dispose();
        }

        void Folder(object sender, EventArgs e)
        {
            Process.Start(folder);
        }

        void Exit(object sender, EventArgs e)
        {
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            Properties.Settings.Default.Number = number;
            Properties.Settings.Default.LastDefect = lastdefect;
            Properties.Settings.Default.Save();
            notifyIcon.Visible = false;
            Application.Exit();
        }

        void Check(object sender, EventArgs e)
        {
            byte[] newhash;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(defectsfile))
                {
                    newhash = md5.ComputeHash(stream);
                }
            }
            if(hash == null || !Enumerable.SequenceEqual(hash, newhash))
            {
                hash = newhash;
                Avisar();
            }
        }

        void Avisar()
        {
            Leer();
            for(int i = lastdefect + 6; i < datosExcel.Tables["Defects"].Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(datosExcel.Tables["Defects"].Rows[i][1])))
                    new ToastContentBuilder().AddText(Convert.ToString(datosExcel.Tables["Defects"].Rows[i][1])).Show();
            }
            lastdefect = datosExcel.Tables["Defects"].Rows.IndexOf(datosExcel.Tables["Defects"].Rows.Cast<DataRow>().Last(x => !string.IsNullOrEmpty(Convert.ToString(x[1])))) - 5;
        }

        void Leer()
        {
            IExcelDataReader excelReader = null;
            using (FileStream stream = File.Open(defectsfile, FileMode.Open, FileAccess.Read))
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

                datosExcel = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
            }
        }
    }
}
