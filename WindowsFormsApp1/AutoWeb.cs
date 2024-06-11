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


namespace WindowsFormsApp1
{
    public partial class AutoWeb : Base
    {
        public bool ready = false;
        public AutoWeb()
        {
            InitializeComponent();
            this.chromiumWebBrowser1.LoadingStateChanged += new System.EventHandler<CefSharp.LoadingStateChangedEventArgs>(this.chromiumWebBrowser1_LoadingStateChanged);
            this.Load += Start;
            chromiumWebBrowser1.LoadUrl("https://myanimelist.net/anime/32729/Orange");
        }

        public AutoWeb (string url)
        {
            InitializeComponent();
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
            this.BringToFront();
        }

        private async void Senpai(object sender, EventArgs e)
        {
            string json = File.ReadAllText(Path.Combine(ResultPath, "MAL", $"FedererSenpai.json"));
            List<Anime> animelist = json.JsonToList<Anime>();
            foreach (Anime a in animelist)
            {
                chromiumWebBrowser1.LoadUrl(a.URL);
                Thread.Sleep(1000);
                Application.DoEvents();
            }
        }

        private async void chromiumWebBrowser1Senpai_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                string script = "document.getElementsByClassName(\" css-47sehv\")[0].click();";
                JavascriptResponse res = await chromiumWebBrowser1.EvaluateScriptAsync(script);
                string s = await chromiumWebBrowser1.GetSourceAsync();
                MAL.doc = new HtmlDocument();
                MAL.doc.LoadHtml(s);
                Thread.Sleep(500);
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
                this.Close();
            }
        }

        private async void chromiumWebBrowser1_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if(!e.IsLoading)
            {
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

                script = "window.scrollTo(0, document.body.scrollHeight);";
                res = await chromiumWebBrowser1.EvaluateScriptAsync(script);

                if (res.Success && res.Result != null)
                {
                    var startDate = res.Result;
                }
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
                if(t.Count > 300)
                ExtensionMethods.WriteToFile(Path.Combine(ResultPath, "MAL", $"FedererSenpai.json"), t.ToJson());

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

    public class MyAnime
    {
        public int status;
        public int score;
        public string tags;
        public int is_rewatching;
        public int num_watched_episodes;
        public int created_at;
        public int updated_at;
        public string anime_title;
        public string anime_title_eng;
        public int anime_num_episodes;
        public int anime_airing_status;
        public int anime_id;
        public string anime_studios;
        public string anime_licensors;
        public string anime_season;
        public int anime_total_members;
        public int anime_total_scores;
        public int anime_socre_val;
        public int anime_score_diff;
        public int anime_popularity;
        public bool has_episode_video;
        public bool has_promotion_video;
        public bool has_video;
        public string video_url;
        public List<Genre> genres;
        public List<Demographic> demographics;
        public List<Theme> themes;
        public string title_localized;
        public string anime_url;
        public string anime_image_path;
        public bool is_added_to_list;
        public string anime_media_type_string;
        public string anime_mpaa_rating_string;
        public string start_date_string;
        public string finish_date_string;
        public string anime_start_date_string;
        public string anime_end_date_string;
        public string days_string;
        public string storage_string;
        public string priority_string;
        public string notes;
        public string editable_notes;
    }
    public class Genre
    {
        public int id;
        public string name;
    }
    public class Demographic
    {
        public int id;
        public string name;
    }
    public class Theme
    {
        public int id;
        public string name;
    }
}
