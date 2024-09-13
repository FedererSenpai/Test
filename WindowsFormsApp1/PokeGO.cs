using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Swan;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WindowsFormsApp1
{
    public partial class PokeGO : Base
    {
        string baseURL = "https://pogoapi.net";
        string documentation => baseURL + "/documentation/";
        List<string> endpoints = new List<string>();

        public List<string> Endpoints { get => endpoints; set
            {
                endpoints = value;
                listBox1.DataSource = endpoints;
            }
        }

        public PokeGO()
        {
            InitializeComponent();
        }

        private void PokeGO_Load(object sender, EventArgs e)
        {
            AddMenu("Update", new EventHandler(Update));
            Endpoints = ExtensionMethods.FileToList<string>(ResultFile(FilePath));
        }

        private void Update(object sender, EventArgs e)
        {
            WebClient web = new WebClient();
            web.DownloadStringAsync(new Uri(documentation));
            web.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Downloaded);
        }

        private void Downloaded(object sender, DownloadStringCompletedEventArgs e)
        {          
            string content = e.Result;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            List<string> tmpList = new List<string>();
            try
            {
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//h2[text() = 'GET ']"))
                {
                    tmpList.Add(node.SelectSingleNode(".//code").InnerText);
                }
                Endpoints = new List<string>(tmpList);
                tmpList.ToFile(ResultFile(FilePath));
            }
            catch(Exception ex)
            {
                ex.Log(this);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string ep = baseURL + Convert.ToString(listBox1.SelectedItem);
            WebClient web = new WebClient();
            web.DownloadStringAsync(new Uri(ep));
            web.DownloadStringCompleted += new DownloadStringCompletedEventHandler(JsonDownloaded);
        }

        private void JsonDownloaded(object sender, DownloadStringCompletedEventArgs e)
        {
            TextBox tb = new TextBox();
            tb.Multiline = true;
            tb.Text = e.Result.AsJSON();
            tb.Dock = DockStyle.Fill;
            tb.ScrollBars = ScrollBars.Both;
            MyControl mc = new MyControl(tb);
            mc.WindowState = FormWindowState.Maximized;
            mc.ShowDialog();
        }
    }
}
