using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;

namespace WindowsFormsApp1
{
    public partial class MAL : Form
    {
        private enum Seasons
        {
            winter, spring, summer, fall
        }
        public MAL()
        {
            InitializeComponent();
        }

        static List<string> names = new List<string>();
        static List<string> links = new List<string>();
        static string[] header = new string[] { "Name", "URL", "Type", "Song", "Artist" };

        private void LoadDefault()
        {
            comboBox1.DataSource = Enum.GetValues(typeof(Seasons));
            comboBox1.SelectedItem = GetSeason();
            numericUpDown1.Value = DateTime.Now.Year;
        }

        public void Read()
        {
            using (WebClient client = new WebClient())
            {
                string downloadString = client.DownloadString($"https://myanimelist.net/anime/season/{numericUpDown1.Value}/{comboBox1.SelectedItem}");
                File.WriteAllText(Path.Combine(Application.StartupPath, "api.txt"), downloadString);
            }
            Process();
            Write($"{comboBox1.SelectedItem} {numericUpDown1.Value}");
            return;
        }

        public void Process()
        {
            string[] lines = File.ReadAllLines(Path.Combine(Application.StartupPath, "api.txt"));
            progressBar1.Maximum = lines.Length;
            int progress = 0;         
            foreach (string line in lines)
            {
                try
                {
                    Node(line);
                }
                catch (Exception ex)
                {
                    try
                    {
                        Node(line.Replace("</div>", string.Empty));
                    }
                    catch (Exception exex)
                    {

                    }
                }
                UpdateProgress();
            }
        }
        //js-anime-category-producer seasonal-anime js-seasonal-anime js-anime-type-all js-anime-type-1

        private void UpdateProgress()
        {
            progressBar1.Value = progressBar1.Value + 1;
            progressBar1.Update();
        }

        private static void Node(string line)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(line);
            XmlNode newNode = doc.DocumentElement;

            if (newNode.Name.Equals("h2") && newNode.Attributes["class"] != null && newNode.Attributes["class"].Value == "h2_anime_title")
            {
                names.Add(newNode.FirstChild.Attributes["href"].Value);
            }

            if (newNode.Name.Equals("span") && newNode.Attributes["class"] != null & newNode.Attributes["class"].Value == "js-title")
            {
                links.Add(newNode.InnerText);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Read();
        }

        private static Seasons GetSeason()
        {
            DateTime date = DateTime.Now;
            float value = (float)date.Month + date.Day / 100f;  // <month>.<day(2 digit)>    
            if (value < 3.21 || value >= 12.22) return Seasons.winter;   // Winter
            if (value < 6.21) return Seasons.spring; // Spring
            if (value < 9.23) return Seasons.summer; // Summer
            return Seasons.fall;   // Autumn
        }

        private void MAL_Load(object sender, EventArgs e)
        {
            LoadDefault();
        }

        private static void Write(string sheet)
        {
            string path = Path.Combine(Application.StartupPath, "resources", "MAL.xlsx");
            DataSet datosExcel = new DataSet();
            if (!File.Exists(path))
                WriteResourceToFile(Properties.Resources.MAL, path);
            IExcelDataReader excelReader = null;
            string fileExtension = Path.GetExtension(path);
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
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
            if (!datosExcel.Tables.Contains(sheet))
            {
                CreateExcelSheet(path, sheet);
            }
            //WriteExcelCell(path, year, dt.Month, type);
        }

        private static void CreateExcelSheet(string FileName, string sheetName)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(FileName);
                xlWorkbook.Sheets.Add();
                xlWorksheet = xlWorkbook.Sheets["Hoja1"];
                xlWorksheet.Name = sheetName;
                Excel.Range xlRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[1, 1], xlWorksheet.Cells[1, 5]];
                xlRange.Font.Bold = true;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.BorderAround2(Weight: Excel.XlBorderWeight.xlThick);
                xlRange.Value = header;
                xlRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[2, 1], xlWorksheet.Cells[2 + names.Count, 2]];
                xlRange.Value = ListsToArray(names, links);
            }
            finally
            {
                if (xlWorkbook != null)
                {
                    xlWorkbook.Close(true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                }
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
        }

        private static void CreateExcelColumn(string FileName, string sheetName, string columnName)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(FileName);
                xlWorksheet = (Excel.Worksheet)xlWorkbook.Sheets[sheetName];
            }
            finally
            {
                if (xlWorkbook != null)
                {
                    xlWorkbook.Close(true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                }
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
        }

        public static void WriteResourceToFile(byte[] resourceName, string fileName)
        {
            System.IO.File.WriteAllBytes(fileName, resourceName);
        }

        public static string[,] Transpose(string[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            string[,] result = new string[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }

        private static string[,] ListsToArray()
        {
            string[,] matrix = new string[names.Count(), 2];
            for (int c = 0; c<names.Count();c++ )
            {
                matrix[c, 0] = names[c];
                matrix[c, 1] = links[c];
                c++;
            }
            return matrix;
        }
    }
}

