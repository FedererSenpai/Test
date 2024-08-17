using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using CefSharp;
using CefSharp.WinForms;
using System.Net.Http;
using System.Net;
using System.IO;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Text.Json;
using System.Threading;
using System.Collections;
using CefSharp.DevTools.DOM;
using System.Runtime.Remoting.Messaging;


namespace WindowsFormsApp1
{
    public partial class AutoWeb : Base
    {
        public bool ready = true;
        public bool cerrar = false;
        public AutoWeb()
        {
                InitializeComponent();
                this.chromiumWebBrowser1.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromiumWebBrowser1_LoadingStateChanged);
                this.Load += Start;
                chromiumWebBrowser1.LoadUrl("https://myanimelist.net/animelist/FedererMagic?order=5&status=2");
        }

        public AutoWeb (string url, bool cerrar)
        {
            InitializeComponent();
            this.cerrar = cerrar;
            //this.Opacity = 0;
            this.chromiumWebBrowser1.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromiumWebBrowser1Senpai_LoadingStateChanged);
            chromiumWebBrowser1.LoadUrl(url);
        }
        public HtmlDocument GetDoc()
        {
            Task<string> task = chromiumWebBrowser1.GetSourceAsync();
            task.Start();
            task.Wait();
            string s = task.Result;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);
            return doc;
        }
        private void Start(object sender, EventArgs e)
        {
            AddMenu("Senpai", new EventHandler(Senpai));
            AddMenu("Json", new EventHandler(Json));
            this.BringToFront();
        }

        private async void Json(object sender, EventArgs e)
        {
            string s = await chromiumWebBrowser1.GetSourceAsync();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);
            HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//tbody[@class='list-item']");
            List<AnimeURL> t = new List<AnimeURL>();
            foreach (HtmlNode n in col)
            {
                HtmlNode y = n.SelectSingleNode(".//td[@class='data title clearfix']").SelectSingleNode(".//a[@class='link sort']");
                t.Add(new AnimeURL() { Name = y.InnerText, Url = y.GetAttributeValue("href", string.Empty) });
            }
            t.ToFile(Path.Combine(ResultPath, "MAL", $"FedererSenpai.json"));
        }
        private async void Senpai(object sender, EventArgs e)
        {
            string json = File.ReadAllText(Path.Combine(ResultPath, "MAL", $"FedererSenpai.json"));
            List<Anime> animelist = json.JsonToList<Anime>();

            ready = false;
            foreach (Anime a in animelist)
            {
                chromiumWebBrowser1.LoadUrl(a.URL);
                Thread.Sleep(1000);
                Application.DoEvents();
            }
            ready = true;
        }

        private async void chromiumWebBrowser1Senpai_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                while (!chromiumWebBrowser1.CanExecuteJavascriptInMainFrame)
                    Thread.Sleep(10);

                string script = "document.getElementsByClassName(\" css-47sehv\")[0].click();";
                JavascriptResponse res = await chromiumWebBrowser1.EvaluateScriptAsync(script);
                string s = await chromiumWebBrowser1.GetSourceAsync();
                MAL.doc = new HtmlDocument();
                MAL.doc.LoadHtml(s);
                Thread.Sleep(500);
                if (MAL.doc.Text.Contains("myanimelist"))
                    Cerrar();
            }
        }

        delegate void OnCerrar();
        private void Cerrar()
        {
            if(this.InvokeRequired)
            {
                OnCerrar del = new OnCerrar(Cerrar);
                this.Invoke(del);
            }
            else
            {
                WriteTempFile("Address.txt", chromiumWebBrowser1.GetSourceAsync().Result);
                Thread.Sleep(500);
                if (cerrar)
                    this.Close();
            }
        }

        private async void chromiumWebBrowser1_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if(!e.IsLoading)
            {
                while(!chromiumWebBrowser1.CanExecuteJavascriptInMainFrame)
                    Thread.Sleep(100);

                string script = "document.getElementsByClassName(\" css-47sehv\")[0].click();";
                JavascriptResponse res = await chromiumWebBrowser1.EvaluateScriptAsync(script);

                if (res.Success && res.Result != null)
                {
                    var startDate = res.Result;
                }
                script = "window.scrollTo(0, document.body.scrollHeight);";
                res = await chromiumWebBrowser1.EvaluateScriptAsync(script);

                    if (res.Success && res.Result != null)
                    {
                        var startDate = res.Result;
                    }

                Thread.Sleep(1000);

                script = "window.scrollTo(0, document.body.scrollHeight);";
                res = await chromiumWebBrowser1.EvaluateScriptAsync(script);

                if (res.Success && res.Result != null)
                { 
                    var startDate = res.Result;
                }

                Thread.Sleep(1000);

                string s = await chromiumWebBrowser1.GetSourceAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(s);
                HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//tbody[@class='list-item']");
                List<AnimeURL> t = new List<AnimeURL>();
                foreach (HtmlNode n in col)
                {
                    HtmlNode y = n.SelectSingleNode(".//td[@class='data title clearfix']").SelectSingleNode(".//a[@class='link sort']");
                    t.Add(new AnimeURL() { Name = y.InnerText, Url = y.GetAttributeValue("href", string.Empty) });
                }

                if (t.Count > 600)
                ExtensionMethods.WriteToFile(Path.Combine(ResultPath, "MAL", $"FedererSenpai.json"), t.ToJson());
                else
                {
                    script = "window.scrollTo(0, document.body.scrollHeight);";
                    res = await chromiumWebBrowser1.EvaluateScriptAsync(script);

                    if (res.Success && res.Result != null)
                    {
                        var startDate = res.Result;
                    }
                }
            }

        }

    }

    public class AnimeURL
    {
        private string name;
        private string url;

        public string Name { get => name; set => name = value; }
        public string Url { get => url; set => url = "https://myanimelist.net" + value; }
    }

}
