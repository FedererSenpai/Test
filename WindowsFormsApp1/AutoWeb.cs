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
using JikanDotNet;
using System.Net.Http;
using System.Net;
using System.IO;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using System.Text.Json;
using System.Threading;
using System.Collections;


namespace WindowsFormsApp1
{
    public partial class AutoWeb : Base
    {
        public AutoWeb()
        {
            InitializeComponent();
            chromiumWebBrowser1.LoadUrl("https://myanimelist.net/animelist/FedererMagic?status=2");
            WebBrowser wb = new WebBrowser();
            wb.Dock = DockStyle.Fill;
            this.Controls.Add(wb);
            wb.Navigate($"https://myanimelist.net/animelist/FedererMagic?status=2");
            wb.ScriptErrorsSuppressed = true;
            wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(dasgsd);
            HtmlDocument doc = new HtmlDocument();
            using (WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 })
            {
                string downloadString = client.DownloadString($"https://myanimelist.net/animelist/FedererMagic?status=2");
                doc.LoadHtml(downloadString);
            }
            HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//table[@class='list-table']");
            string v = col.First().GetAttributeValue("data-items", string.Empty);
            File.WriteAllText(Path.Combine(ResultPath, "mylist.json"), v.Replace("&quot;", "\""));
            File.WriteAllText(FullPath, doc.Text);
            List<MyAnime> l = JsonSerializer.Deserialize<List<MyAnime>>(v.Replace("&quot;", "\""));
        }

        private void dasgsd(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            (sender as WebBrowser).Document.Body.ScrollIntoView(false);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml((sender as WebBrowser).DocumentText);
            HtmlNodeCollection col = doc.DocumentNode.SelectNodes("//table[@class='list-table']");
            string v = col.First().GetAttributeValue("data-items", string.Empty);
            File.WriteAllText(Path.Combine(ResultPath, "mylist.json"), v.Replace("&quot;", "\""));
            File.WriteAllText(FullPath, doc.Text);
            List<MyAnime> l = JsonSerializer.Deserialize<List<MyAnime>>(v.Replace("&quot;", "\""));

        }

        private async void chromiumWebBrowser1_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if(!e.IsLoading)
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadStringAsync(new Uri("https://myanimelist.net/animelist/FedererMagic?status=2"));
                    webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);
                }
                string script = "document.getElementsByName(\"q\")";
                script = "document.getElementsByClassName(\" css-47sehv\")[0].click();";
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
                Hashtable t = new Hashtable();
                foreach (HtmlNode n in col)
                {
                    HtmlNode y = n.SelectSingleNode(".//td[@class='data title clearfix']").SelectSingleNode(".//a[@class='link sort']");
                    t.Add(y.InnerText, y.GetAttributeValue("href", string.Empty));
                }
            }

        }

        private async void chromiumWebBrowser1_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
        }

        private void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            File.AppendAllText(FullPath ,e.Result);
        }
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
