using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json;
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

        private void button2_Click(object sender, EventArgs e)
        {
            string file = Path.GetFileName(Convert.ToString(listBox1.SelectedItem));
            string ep = baseURL + Convert.ToString(listBox1.SelectedItem);
            WebClient web = new WebClient();
            web.DownloadFileAsync(new Uri(ep), ResultFile(file));
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

        string ConvertDashToCase(string input, bool pascal = true)
        {
            StringBuilder sb = new StringBuilder();
            bool caseFlag = pascal;
            for (int i = 0; i < input.Length; ++i)
            {
                char c = input[i];
                if (c == '_')
                {
                    caseFlag = true;
                }
                else if (caseFlag)
                {
                    sb.Append(char.ToUpper(c));
                    caseFlag = false;
                }
                else
                {
                    sb.Append(char.ToLower(c));
                }
            }
            return sb.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            decimal[,] matrix = new decimal[18, 18];
            JsonNode node = JsonObject.Parse(ReadResultFile("type_effectiveness.json"));
            foreach(TypeEffectiveness attack in Enum.GetValues(typeof(TypeEffectiveness)))
            {
                foreach (TypeEffectiveness defense in Enum.GetValues(typeof(TypeEffectiveness)))
                {
                    matrix[(int)attack, (int)defense] = node[attack.ToString()][defense.ToString()].GetValue<decimal>();
                }
            }

            ComboBox t1 = new ComboBox();
            t1.DataSource = Enum.GetValues(typeof(TypeEffectiveness));
            ComboBox t2 = new ComboBox();
            t2.DataSource = Enum.GetValues(typeof(TypeEffectiveness));
            t1.SelectedIndex = -1;
            t2.SelectedIndex = -1;
            FlowLayoutPanel p = new FlowLayoutPanel();
            p.Controls.Add(t1);
            p.Controls.Add(t2);
            MyControl mc = new MyControl(p);
            mc.ShowDialog();
            
            foreach(TypeEffectiveness attack in Enum.GetValues(typeof(TypeEffectiveness)))
            {
                if (matrix[(int)attack, (int)TypeEffectiveness.Water] > 1)
                {
                    MessageBox.Show(attack.ToString());
                }
            }
        }

        private enum TypeEffectiveness
        {
         Bug,
        Dark,
       Dragon,
        Electric,
        Fairy,
        Fighting,
        Fire,
        Flying,
        Ghost,
        Grass,
        Ground,
        Ice,
        Normal,
        Poison,
        Psychic,
        Rock,
        Steel,
        Water
        }

    }
}
