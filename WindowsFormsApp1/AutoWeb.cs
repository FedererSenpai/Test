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
using Swan;
using System.Xml.Linq;


namespace WindowsFormsApp1
{
    public partial class AutoWeb : Base
    {
        public bool ready = true;
        public bool cerrar = false;
        private string script;
        private string file;
        public AutoWeb()
        {
                InitializeComponent();
                this.chromiumWebBrowser1.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromiumWebBrowser1_LoadingStateChanged);
                this.Load += Start;
                chromiumWebBrowser1.LoadUrl("https://myanimelist.net/animelist/FedererMagic?order=5&status=2");
        }

        public AutoWeb(string url, string script, string file, bool cerrar)
        {
            this.script = script;
            this.file = file;
            this.cerrar = cerrar;
            InitializeComponent();
            this.chromiumWebBrowser1.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromiumWebBrowserFile_LoadingStateChanged);
            chromiumWebBrowser1.LoadUrl(url);
        }

        private async void chromiumWebBrowserFile_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                while (!chromiumWebBrowser1.CanExecuteJavascriptInMainFrame)
                    Thread.Sleep(10);

                await chromiumWebBrowser1.EvaluateScriptAsync("window.scrollTo(0, document.body.scrollHeight);");

                JavascriptResponse res = await chromiumWebBrowser1.EvaluateScriptAsync(script);
                string s = await chromiumWebBrowser1.GetBrowser().MainFrame.GetSourceAsync();
                
                if (s.Contains("Ups! Parece que algo no va bien..."))
                {
                    ChangeOpacity(1, false);
                    return;
                }

                await Task.Delay(1000); 

                try
                {
                    if (!string.IsNullOrEmpty(file))
                    {
                        SaveResultFile(file, Convert.ToString(res.Result), false);
                    }
                }
                catch(IOException ex)
                {
                    MessageBox.Show(file);
                }

                await Task.Delay(500);

                Cerrar();
                return;
            }
        }

        public AutoWeb (string url, bool cerrar, string script)
        {
            InitializeComponent();
            this.cerrar = cerrar;
            //this.Opacity = 0;
            this.chromiumWebBrowser1.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromiumWebBrowser1Senpai_LoadingStateChanged);
            chromiumWebBrowser1.LoadUrl(url);
            this.script = script;
        }
        public HtmlDocument GetDoc()
        {
            Task<string> task = chromiumWebBrowser1.GetSourceAsync();
            task.Await();
            string s = task.Result;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(s);
            task.Dispose();
            return doc;
        }
        private void Start(object sender, EventArgs e)
        {
            AddMenu("Senpai", new EventHandler(Senpai));
            AddMenu("Json", new EventHandler(Json));
            this.BringToFront();
        }

        public override void Initialize()
        {
            AddMenuItems(new EventHandler(Senpai), new EventHandler(Json), new EventHandler(Source));
        }

        private void Source(object sender, EventArgs e)
        {
            chromiumWebBrowser1.GetSourceAsync().ContinueWith(taskHtml =>
            {
                var html = taskHtml.Result;
            });
             JavascriptResponse res = chromiumWebBrowser1.EvaluateScriptAsync ("document.getElementsByClassName('sui-AtomButton sui-AtomButton--primary sui-AtomButton--outline sui-AtomButton--center sui-AtomButton--link sui-AtomButton--circular')[0].innerHTML;").Result;
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

                await chromiumWebBrowser1.EvaluateScriptAsync("window.scrollTo(0, document.body.scrollHeight);");

                await chromiumWebBrowser1.EvaluateScriptAsync(script);
                string s = await chromiumWebBrowser1.GetBrowser().MainFrame.GetSourceAsync();

                if (s.Contains("Ups! Parece que algo no va bien..."))
                {
                    ChangeOpacity(1, false);
                    return;
                }

                if (Owner is MAL)
                {
                    MAL.doc = new HtmlDocument();
                    MAL.doc.LoadHtml(s);
                    Thread.Sleep(500);
                    if (MAL.doc.Text.Contains("myanimelist"))
                        Cerrar();
                    return;
                }
                else if(Owner is Cars)
                {
                    if (!s.Contains("sui-AtomCard sui-AtomCard--responsive") || !s.Contains("sui-AtomButton sui-AtomButton--primary sui-AtomButton--outline sui-AtomButton--center sui-AtomButton--link sui-AtomButton--circular"))
                    {
                        //JavascriptResponse res = await chromiumWebBrowser1.EvaluateScriptAsync("document.body;");
                        //Cars.doc = new HtmlDocument();
                        //Cars.doc.LoadHtml(Convert.ToString(res.Result));
                        //Thread.Sleep(500);
                        Cerrar();
                        return;
                    }
                    Cars.doc = new HtmlDocument();
                    Cars.doc.LoadHtml(s);
                    Thread.Sleep(500);
                    Cerrar();
                    return;
                }
                else
                {
                    Thread.Sleep(500);
                    Cerrar();
                    return;
                }
            }
        }

        delegate void OnOpacity(int opacity, bool reload);
        private void ChangeOpacity(int opacity, bool reload)
        {
            if (this.InvokeRequired)
            {
                OnOpacity del = new OnOpacity(ChangeOpacity);
                this.Invoke(del, opacity, reload);
            }
            else
            {
                this.Opacity = opacity;
                if(reload)
                chromiumWebBrowser1.Reload();
            }
        }

        delegate void OnCerrar();
        private void Cerrar()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    OnCerrar del = new OnCerrar(Cerrar);
                    this.Invoke(del);
                }
                else
                {
                    if (Owner is Web)
                    {
                        WriteTempFile("Address.txt", chromiumWebBrowser1.GetSourceAsync().Result);
                        Thread.Sleep(500);
                    }
                    if (cerrar)
                        this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
