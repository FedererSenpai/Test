using ExcelDataReader;
using Org.BouncyCastle.Cms;
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
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Text.RegularExpressions;

namespace WindowsFormsApp1
{
    public partial class MAL : Base
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
        static List<Anime> animes = new List<Anime>();
        static List<string> nodes = new List<string>();
        static string[] header = new string[] { "Name", "URL", "Type", "Song", "Artist" };
        static string season;

        private void LoadDefault()
        {
            comboBox1.DataSource = Enum.GetValues(typeof(Seasons));
            comboBox1.SelectedItem = GetSeason();
            numericUpDown1.Value = DateTime.Now.Year;
        }

        public void Read()
        {

            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 })
            {
                string downloadString = client.DownloadString($"https://myanimelist.net/anime/season/{numericUpDown1.Value}/{comboBox1.SelectedItem}");
                ExtensionMethods.WriteToFile(Path.Combine(FolderPath, "api.txt"), downloadString);
                doc.LoadHtml(downloadString);
            }

            ProcessSeason(doc.DocumentNode);
            //Write(season);
            return;
        }

        public void ProcessSeason(HtmlNode doc)
        {
            HtmlNodeCollection list = doc.SelectNodes("//div[@class='js-anime-category-producer seasonal-anime js-seasonal-anime js-anime-type-all js-anime-type-1']");
            progressBar1.Maximum = list.Count();
            Anime anime;
            foreach (HtmlNode node in list)
            {
                anime = new Anime();
                HtmlNode mynode = node.SelectNodes(".//h2[@class='h2_anime_title']").First();
                anime.URL = mynode.FirstChild.GetAttributeValue("href", string.Empty);
                anime.Name = node.SelectSingleNode(".//span[@class='js-title']").InnerText;
                ProcessAnime(anime.Clone());
                UpdateProgress(progressBar1);
            }

            /*string[] lines = File.ReadAllLines(Path.Combine(Application.StartupPath, "api.txt"));
            progressBar1.Maximum = lines.Length;
            Anime anime = new Anime();
            foreach (string line in lines)
            {
                try
                {
                    Node(line, ref anime);
                }
                catch (Exception ex)
                {
                    try
                    {
                        Node(line.Replace("</div>", string.Empty), ref anime);
                    }
                    catch (Exception exex)
                    {

                    }
                }
                UpdateProgress(progressBar1);
            }*/
            ExtensionMethods.WriteToFile(Path.Combine(ResultPath,"MAL", $"{season}.json"), animes.ToJson());
            string asuidfha = animes.ToJson();
        }
         
        private void GetChild(HtmlNodeCollection list)
        {
            foreach(HtmlNode node in list)
            {
                if(!nodes.Contains(node.Name))
                {
                    nodes.Add(node.Name);
                }
                if(node.Name.Equals("h2") && node.InnerText.Equals("Opening Theme"))
                {
                    
                }
                if(node.Name.Equals("table"))
                {

                }
                GetChild(node.ChildNodes);
            }
        }

        private void ProcessAnime(Anime animeBase)
        {
            string path = Path.Combine(Application.StartupPath, season, $"{animeBase.Name}.txt".CheckFileName());
            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 })
            {
                string downloadString = client.DownloadString(animeBase.URL);
                ExtensionMethods.WriteToFile(path, downloadString);
                doc.LoadHtml(downloadString);
            }
            Anime anime;
            HtmlNode openingNode = doc.DocumentNode.SelectSingleNode("//div[@class='theme-songs js-theme-songs opnening']");
            HtmlNode endingNode = doc.DocumentNode.SelectSingleNode("//div[@class='theme-songs js-theme-songs ending']");

            if (openingNode.SelectNodes(".//td").Any(x => x.InnerText.Contains("No opening themes")))
            {
                anime = animeBase.Clone();
                anime.Type = "Opening";
                anime.Song = "No opening theme";
                animes.Add(anime.Clone());
            }
            else if (openingNode.SelectSingleNode(".//td[@width='84%']") != null)
            {
                foreach (HtmlNode subnode in openingNode.SelectNodes(".//td[@width='84%']"))
                {
                    anime = animeBase.Clone();
                    anime.Type = "Opening";

                    anime.Song = subnode.InnerText;
                    if (subnode.SelectSingleNode(".//span[@class='theme-song-index']") != null)
                    {
                        anime.Song = anime.Song.Replace(subnode.SelectSingleNode(".//span[@class='theme-song-index']").InnerText, string.Empty);
                    }
                    if (subnode.SelectSingleNode(".//span[@class='theme-song-episode']") != null)
                    {
                        anime.Song = anime.Song.Replace(subnode.SelectSingleNode(".//span[@class='theme-song-episode']").InnerText, string.Empty);
                    }

                    if (subnode.SelectSingleNode(".//span[@class='theme-song-artist']") == null)
                    {
                        anime.Artist = subnode.InnerText;
                    }
                    else
                    {
                        anime.Artist = subnode.SelectSingleNode(".//span[@class='theme-song-artist']").InnerText;
                    }
                    animes.Add(anime.Clone());
                }
            }

            if (endingNode.SelectNodes(".//td").Any(x => x.InnerText.Contains("No ending themes")))
            {
                anime = animeBase.Clone();
                anime.Type = "Ending";
                anime.Song = "No ending theme";
                animes.Add(anime.Clone());
            }
            else if (endingNode.SelectSingleNode(".//td[@width='84%']") != null)
            {
                foreach (HtmlNode subnode in endingNode.SelectNodes(".//td[@width='84%']"))
                {
                    anime = animeBase.Clone();
                    anime.Type = "Ending";
                    anime.Song = subnode.InnerText;
                    if (subnode.SelectSingleNode(".//span[@class='theme-song-index']") != null)
                    {
                        anime.Song = anime.Song.Replace(subnode.SelectSingleNode(".//span[@class='theme-song-index']").InnerText, string.Empty);
                    }
                    if (subnode.SelectSingleNode(".//span[@class='theme-song-episode']") != null)
                    {
                        anime.Song = anime.Song.Replace(subnode.SelectSingleNode(".//span[@class='theme-song-episode']").InnerText, string.Empty);
                    }

                    if (subnode.SelectSingleNode(".//span[@class='theme-song-artist']") == null)
                    {
                        anime.Artist = subnode.InnerText;
                    }
                    else
                    {
                        anime.Artist = subnode.SelectSingleNode(".//span[@class='theme-song-artist']").InnerText;
                    }
                    animes.Add(anime.Clone());
                }
            }

            /*GetChild(doc.DocumentNode.SelectSingleNode("//h2[.='Opening Theme']").ChildNodes);
            GetChild(doc.DocumentNode.ChildNodes);

            string[] lines = File.ReadAllLines(path);
            progressBar2.Maximum = lines.Length;
            string tipo = string.Empty;

            bool anadir = false;

            foreach (string line in lines)
            {
                try
                {
                    if (NodeAnime(line, ref anime))
                        anadir = true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        NodeAnime(line.Replace("&nbsp;", string.Empty), ref anime);
                    }
                    catch (Exception exex)
                    {

                    }
                }                
                UpdateProgress(progressBar2);
                if(anadir)
                {
                    animes.Add(anime);
                    anime = animeBase;
                }
            }*/
        }

        private void ResetProgress(ProgressBar pb)
        {
            pb.Value = 0;
            pb.Update();
        }

        private void UpdateProgress(ProgressBar pb)
        {
            pb.Value = pb.Value + 1;
            pb.Update();
            Application.DoEvents();
            Thread.Sleep(1);
        }

        private void Node(string line, ref Anime anime)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(line);
            XmlNode newNode = doc.DocumentElement;

            if (newNode.Name.Equals("h2") && newNode.Attributes["class"] != null && newNode.Attributes["class"].Value == "h2_anime_title")
            {
                links.Add(newNode.FirstChild.Attributes["href"].Value);
                anime.URL = links.Last();
            }

            if (newNode.Name.Equals("span") && newNode.Attributes["class"] != null && newNode.Attributes["class"].Value == "js-title")
            {
                names.Add(newNode.InnerText);
                anime.Name = names.Last();
                ProcessAnime(anime);
            }
        }
         private bool NodeAnime(string line, ref Anime anime)
        {
            if(line.Contains("theme-song-index"))
            {

            }
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(line);
            XmlNode newNode = doc.DocumentElement;

            if (newNode.Name.Equals("h2") && newNode.InnerText.Equals("Opening Theme"))
            {
                anime.Type = "Opening";
            }
            if (newNode.Name.Equals("h2") && newNode.InnerText.Equals("Ending Theme"))
            {
                anime.Type = "Ending";
            }

            if(newNode.FirstChild.Name.Equals("span"))
            { 
            }

            return false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ResetProgress(progressBar1);
            season = $"{comboBox1.SelectedItem} {numericUpDown1.Value}";
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
            CreateExcelSheet(path, sheet, !datosExcel.Tables.Contains(sheet));
        }

        private static void CreateExcelSheet(string FileName, string sheetName, bool create)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(FileName);
                if (create)
                {
                    xlWorkbook.Sheets.Add();
                    xlWorksheet = xlWorkbook.Sheets["Hoja1"];
                    xlWorksheet.Name = sheetName;
                }
                else
                {
                    xlWorksheet = xlWorkbook.Sheets[sheetName];
                }
                Excel.Range xlRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[1, 1], xlWorksheet.Cells[1, 5]];
                xlRange.Font.Bold = true;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.BorderAround2(Weight: Excel.XlBorderWeight.xlThick);
                xlRange.Value = header;
                xlRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[2, 1], xlWorksheet.Cells[2 + names.Count, 2]];
                xlRange.Value = ListsToArray();
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

    public class Anime
    {
        private string name = string.Empty;
        private string url = string.Empty;
        private string type = string.Empty;
        private string song = string.Empty;
        private string artist = string.Empty;

        public string Name { get => name; set => name = value; }
        public string URL { get => url; set => url = value; }
        public string Type { get => type; set => type = value; }
        public string Song { get => song; set => song = value; }
        public string Artist { get => artist; set => artist = value; }

        public Anime Clone()
        {
            Anime anime = new Anime() { Name = this.name, URL = this.url, Type = this.type, Song = this.song, Artist = this.artist };
            if (!string.IsNullOrEmpty(anime.Artist))
            {
                anime.Artist = System.Web.HttpUtility.HtmlDecode(anime.Artist).Replace("\n", string.Empty).Replace("\"", string.Empty);
            }
            if (!string.IsNullOrEmpty(anime.Song))
            {
                anime.Song = System.Web.HttpUtility.HtmlDecode(anime.Song).Replace("\n", string.Empty).Replace("\"", string.Empty);

                if (!string.IsNullOrEmpty(anime.Artist) && anime.Artist != anime.Song)
                {
                    anime.Song = anime.Song.Replace(anime.Artist, string.Empty);
                    anime.Artist = anime.Artist.Trim();
                }
                else
                {
                    if (anime.Song.Split(new string[] { " by " }, StringSplitOptions.None).Length == 2)
                    {
                        anime.Song = anime.Song.Substring(0, anime.Song.IndexOf(" by "));
                        anime.Artist = anime.Artist.Substring(anime.Artist.IndexOf(" by "));
                    }
                }

                if (Regex.IsMatch(anime.Song, @"^S?\d+:"))
                    anime.Song = anime.Song.Replace(Regex.Match(anime.Song, @"^S?\d+:").Value, string.Empty);
                if (Regex.IsMatch(anime.Song, @"^\d*:"))
                    anime.Song = anime.Song.Replace(Regex.Match(anime.Song, @"^\d*:").Value, string.Empty);
                if (Regex.IsMatch(anime.Song, @"\(ep.*\)"))
                    anime.Song = anime.Song.Replace(Regex.Match(anime.Song, @"\(ep.*\)").Value, string.Empty);
                if (Regex.IsMatch(anime.Artist, @"\(ep.*\)"))
                    anime.Artist = anime.Artist.Replace(Regex.Match(anime.Artist, @"\(ep.*\)").Value, string.Empty);

            }
            anime.Song = anime.Song.Trim();
            anime.Artist = anime.Artist.Trim().TrimStart("by").Trim();
            return anime;
        }
    }
}

