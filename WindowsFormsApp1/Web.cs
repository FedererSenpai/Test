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
using System.Security.Policy;
using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using RiotSharp.Endpoints.SpectatorEndpoint;
using System.Diagnostics;
using Swan;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using CefSharp.DevTools.Audits;
using System.Threading;
using System.Net.Mail;
using static WindowsFormsApp1.Web;
using Microsoft.Office.Interop.Excel;
using System.Net;
using Swan.Logging;

namespace WindowsFormsApp1
{
    public partial class Web : Base
    {
        private static Web web;
        private static string clientid = "50dfcb15aa95487f9b92990daf6bb6dd";
        private static string clientsecret = "0990c8a694a349aa8f27b6158f1f7ec7";
        private static string playlist = "0l3AESuzc6aEgpeD8sQbdp";
        private static string userid = "316b4swabpspq4guu4yzvz5jnz4q";
        private static EmbedIOAuthServer _server;
        private static SpotifyClient spotify;
        private static List<Spotify> list = new List<Spotify>();
        private static List<Anime> error = new List<Anime>();
        private static string youtubeid = "481114111248-u5ipv6go48scmilhra8r6dmrafp3fvhq.apps.googleusercontent.com";
        public Web() 
        {
            InitializeComponent();
            //webBrowser1.ScriptErrorsSuppressed = true;
            //webBrowser1.Navigate("www.google.com");
            //webBrowser1.DocumentCompleted += Loaded;

            this.Load += Start;
            web = this;
        }

        private void Start(object sender, EventArgs e)
        {
            AddMenu("Load", new EventHandler(Search));
            AddMenu("Test", new EventHandler(Test));
            AddMenu("Create", new EventHandler(CreatePlaylist));
            AddMenu("Result", new EventHandler(ShowResult));
            AddMenu("Senpai", new EventHandler(Senpai));
            AddMenu("Kpoop", new EventHandler(Kpoop));
            AddMenu("Add", new EventHandler(Add));
            Task t = new Task(async () => await Auth());
            t.Start();
            this.BringToFront();
            web.Opacity = 1;
        }

        private static string ChooseSeason()
        {
            string season = string.Empty;
            ComboBox cb = new ComboBox();
            cb.DataSource = Directory.GetFileSystemEntries(Path.Combine(web.ResultPath, "MAL")).Select(x=>Path.GetFileNameWithoutExtension(x)).ToList();
            MyControl mc = new MyControl(cb);
            if(mc.isOK())
            {
                season =  mc.GetComboBox().ToString();
                mc.Close();
            }
            return season;
        }

        private async Task Senpai(bool add)
        {
            string name = "FedererSenpaiList";
            string path = Path.Combine(web.ResultPath, "MAL", $"{name}.json");
            List<Anime> l = JsonConvert.DeserializeObject<List<Anime>>(File.ReadAllText(path)).Where(x => x.Song != "No opening theme" && x.Song != "No ending theme").ToList();
            web.myProgressBar1.Maximum = l.Count;

            foreach (Anime anime in l)
            {
                Spotify spoti = ProcessSpotify(await SearchTrack(anime.Song, anime.Artist), anime.Artist, anime.Song);
                if (spoti == null)
                {
                    error.Add(anime.Clone());
                    myProgressBar1.Value++;
                    continue; 
                }
                spoti.Anime = anime.Clone();
                if (anime.Header.Equals("Continuing"))
                    spoti.Add = false;
                list.Add(spoti);
                myProgressBar1.Value++;
            }
            if(add)
            {
                JsonConvert.DeserializeObject<List<Spotify>>(Path.Combine(web.ResultPath, "Spotify", $"anime {name} mix.json")).Union(list).ToList().ToFile(Path.Combine(web.ResultPath, "Spotify", $"anime {name} mix.json"));
            }
            else
            {
                list.ToFile(Path.Combine(web.ResultPath, "Spotify", $"anime {name} mix.json"));
            }
            dataGridView1.DataSource = list;
            dataGridView1.Tag = name;
            //foreach(List<string> l in list.Select(x => "spotify:track:" + x.Id).ToList().Split(100))
            //{
            //await spotify.Playlists.AddItems(pl.Id, new PlaylistAddItemsRequest(l));
            //}
            web.WindowState = FormWindowState.Maximized;
        }
        private async void Senpai(object sender, EventArgs e)
        {
            await Senpai(false);
        }

        private async void Search(object sender, EventArgs e)
        {
            string name = ChooseSeason();
            if (string.IsNullOrEmpty(name))
                return;
            if ((await spotify.Search.Item(new SearchRequest(SearchRequest.Types.Playlist, name))).Playlists.Items.Any(x=>x.Owner.Id == userid))
            {
                if (MessageBox.Show("Ya existe. Continuar?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                    return;
            }
            //FullPlaylist pl = await spotify.Playlists.Create(userid, new PlaylistCreateRequest($"Anime {name.FirstLetterToUpperCase()} Mix"));
            //playlist = pl.Id;
            string path = Path.Combine(web.ResultPath, "MAL", $"{name}.json");
            List<Anime> l = JsonConvert.DeserializeObject<List<Anime>>(File.ReadAllText(path)).Where(x => x.Song != "No opening theme" && x.Song != "No ending theme").ToList();
            web.myProgressBar1.Maximum = l.Count;
            foreach (Anime anime in l)
            {
                Spotify spoti = ProcessSpotify(await SearchTrack(anime.Song, anime.Artist), anime.Artist, anime.Song);
                spoti.Anime = anime.Clone();
                if (anime.Header.Equals("Continuing"))
                    spoti.Add = false;
                list.Add(spoti);
                myProgressBar1.Value++;
            }
            list.ToFile(Path.Combine(web.ResultPath, "Spotify", $"anime {name} mix.json"));
            dataGridView1.DataSource = list;
            dataGridView1.Tag = name;
            //foreach(List<string> l in list.Select(x => "spotify:track:" + x.Id).ToList().Split(100))
            //{
            //await spotify.Playlists.AddItems(pl.Id, new PlaylistAddItemsRequest(l));
            //}
            web.WindowState = FormWindowState.Maximized;
        }

        private static async void CreatePlaylist(object sender, EventArgs e)
        { 
            FullPlaylist pl = await spotify.Playlists.Create(userid, new PlaylistCreateRequest($"Anime {web.dataGridView1.Tag.ToString().FirstLetterToUpperCase()} Mix"));
            playlist = pl.Id;

            List<Spotify> list = (web.dataGridView1.DataSource as List<Spotify>).Where(x=>x.Add).ToList();
            foreach (List<string> l in list.Select(x => "spotify:track:" + x.Id).ToList().Split(100))
            {
                await spotify.Playlists.AddItems(pl.Id, new PlaylistAddItemsRequest(l));
            }
        }

        public static async Task Auth()
        {
            _server = new EmbedIOAuthServer(new Uri("http://localhost:5543/callback"), 5543);
            await _server.Start();

            _server.AuthorizationCodeReceived += OnAuthorizationCodeReceived;
            _server.ErrorReceived += OnErrorReceived;

            var request = new LoginRequest(_server.BaseUri, clientid, LoginRequest.ResponseType.Code)
            {
                Scope = new List<string> { Scopes.UserReadEmail, Scopes.PlaylistModifyPublic, Scopes.PlaylistModifyPrivate }
            };
            BrowserUtil.Open(request.ToUri());
        }

        private static async Task OnAuthorizationCodeReceived(object sender, AuthorizationCodeResponse response)
        {
            await _server.Stop();

            var config = SpotifyClientConfig.CreateDefault();
            var tokenResponse = await new OAuthClient(config).RequestToken(
              new AuthorizationCodeTokenRequest(
                clientid, clientsecret, response.Code, new Uri("http://localhost:5543/callback")
              )
            );

            spotify = new SpotifyClient(tokenResponse.AccessToken);
            // do calls with Spotify and save token?
            PlaylistAddItemsRequest request = new PlaylistAddItemsRequest(new List<string>() { "spotify:track:06YNVx8q2zF84s8SfGbxMC" });

            SendKeys.SendWait("^{w}"); // close tab.
            Thread.Sleep(100); 
            WinUtil.FocusProcess("WindowsFormsApp1");
            Thread.Sleep(100);

        }

        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }

        private async void Test(object sender, EventArgs e)
        {
            return;
            SearchRequest request = new SearchRequest(SearchRequest.Types.Track, "Girlfriend");
            var asfd = await spotify.Search.Item(request);
            await spotify.Playlists.AddItems(playlist, new PlaylistAddItemsRequest(new List<string>() { "spotify:track:" + asfd.Tracks.Items.First().Id }));
            //var sdafgklh= asfd.Tracks.Items.Min(x => x.Artists.Min(y => ExtensionMethods.Distance(y.Name, "AvrilLavigne")));
            await spotify.Playlists.Create("316b4swabpspq4guu4yzvz5jnz4q", new PlaylistCreateRequest("Prueba"));
        }

        private async void Add(object sender, EventArgs e)
        {
            WebRequest request = WebRequest.Create($"https://accounts.spotify.com/api/token");
            request.ContentType = "application/x-www-form-urlencoded";
            //request.Headers.Add($"Basic {clientid}: {clientsecret}");
            request.Method = "POST";
            string stringData = $"grant_type=client_credentials&client_id={clientid}&client_secret={clientsecret}"; // place body here
            var data = Encoding.Default.GetBytes(stringData); // note: choose appropriate encoding
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(data, 0, data.Length);
            }

            WebResponse response = request.GetResponse();
            AccessToken accessToken;
            using(StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responsestring = reader.ReadToEnd();
                accessToken = JsonConvert.DeserializeObject<AccessToken>(responsestring);
            }

            request = WebRequest.Create($"https://api.spotify.com/v1/users/{userid}/playlists");
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Authorization", $"Bearer {accessToken.access_token}");
            response = request.GetResponse();
            PlaylistList playlist;
            using(StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responsestring = reader.ReadToEnd();
                playlist = JsonConvert.DeserializeObject<PlaylistList>(responsestring);
            }

            ComboBox cb = new ComboBox();
            cb.DisplayMember = "name";
            cb.DataSource = playlist.items;
            MyControl mc = new MyControl(cb);
            if (mc.isOK())
            {            
                List<Spotify> list = (web.dataGridView1.DataSource as List<Spotify>).Where(x => x.Add).ToList();
                foreach (List<string> l2 in list.Select(x => "spotify:track:" + x.Id).ToList().Split(100))
                {
                    await spotify.Playlists.AddItems((cb.SelectedItem as Item).id, new PlaylistAddItemsRequest(l2));
                }

                return;

                AutoWeb aw = new AutoWeb("https://myanimelist.net", false, "");
                aw.ShowDialog();
                List<Anime> l = MAL.GetAnimes("Address.txt").Where(x => x.Song != "No opening theme" && x.Song != "No ending theme").ToList();
                web.myProgressBar1.Maximum = l.Count;
                foreach (Anime anime in l)
                {
                    WebRequest request2 = WebRequest.Create($"https://api.spotify.com/v1/search?q=remaster%2520track%3A{anime.Song}%2520artist%3A{anime.Artist}&type=track&market=JP");
                    request2.ContentType = "application/UTF-8";
                    request2.Method = "GET";
                    request.Headers.Add("Authorization", $"Bearer {accessToken.access_token}");
                    WebResponse response2 = request2.GetResponse();
                    using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                    {
                        string responsestring = reader.ReadToEnd();
                    }
                    Spotify spoti = ProcessSpotify(await SearchTrack(anime.Song, anime.Artist), anime.Artist, anime.Song);
                    if (spoti == null)
                    {
                        error.Add(anime.Clone());
                        myProgressBar1.Value++;
                        continue;
                    }
                    spoti.Anime = anime.Clone();
                    list.Add(spoti);
                    myProgressBar1.Value++;
                }
                dataGridView1.DataSource = list;
                    dataGridView1.Tag = "Address";
                    web.WindowState = FormWindowState.Maximized;
            }

        }
        private async void Kpoop(object sender, EventArgs e)
        {
            Paging<PlaylistTrack<IPlayableItem>> kpopmix = await spotify.Playlists.GetItems("37i9dQZF1EQpesGsmIyqcW");
            Paging <PlaylistTrack<IPlayableItem>> kpoop = await spotify.Playlists.GetItems("2w4i6mlSsBMOE3BwVEKxux");
            List<string> kpopmixlist = kpopmix.Items.Select(x => ((FullTrack)x.Track).Uri).ToList();
            List<string> kpooplist = kpoop.Items.Select(x => ((FullTrack)x.Track).Uri).ToList();
            List<string> addlist;
            while (kpopmix.Next != null)
            {
                kpopmix = await spotify.NextPage(kpopmix);
                kpopmixlist.AddRange(kpopmix.Items.Select(x => ((FullTrack)x.Track).Uri).ToList());
            }
            while (kpoop.Next != null)
            {
                kpoop = await spotify.NextPage(kpoop);
                kpooplist.AddRange(kpoop.Items.Select(x => ((FullTrack)x.Track).Uri).ToList());
            }
            addlist = kpopmixlist.Except(kpooplist).ToList();
            PlaylistAddItemsRequest request = new PlaylistAddItemsRequest(addlist);
            if(request.Uris.Count > 0)
                await spotify.Playlists.AddItems("2w4i6mlSsBMOE3BwVEKxux", request);
            MessageBox.Show(addlist.Count().ToString() + " items added.");
        }
        private async Task<SearchResponse> SearchTrack(string title, string artist)
        {
            try
            {
                SearchRequest request = new SearchRequest(SearchRequest.Types.Track, title) { Market = "JP"};
                return await spotify.Search.Item(request);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private Spotify ProcessSpotify(SearchResponse response, string artist, string song)
        {
            try
            {
                FullTrack track = response.Tracks.Items.FirstOrDefault(x => x.Artists.Select(y => y.Name.ToLower()).Contains(artist.ToLower()) || x.Artists.Any(y=>artist.ToLower().Contains(y.Name.ToLower())));
                bool add = true;
                if (track == null)
                {
                    track = response.Tracks.Items.FirstOrDefault(x=>x.Name.ToLower().Contains(song.ToLower()) || song.ToLower().Contains(x.Name.ToLower()));
                    add = false;
                }
                if (track == null)
                {
                    track = response.Tracks.Items.First();
                    add = false;
                }
                return new Spotify() { Track = track.Name, Author = track.Artists.ToListString("Name"), Id = track.Id, Add = add };
            }
            catch
            {
                return null;
            }
        }

        private void dataGridView1_CellFormating(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Spotify s = dataGridView1.Rows[e.RowIndex].DataBoundItem as Spotify;
            if (!s.Add)
                e.CellStyle.BackColor = Color.LightSalmon;
            if (s.Anime.Header.Equals("Continuing"))
                e.CellStyle.BackColor = Color.LightBlue;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string header = dataGridView1.Columns[e.ColumnIndex].HeaderText;
                if (header.Equals("Anime"))
                {
                    Spotify s = dataGridView1.Rows[e.RowIndex].DataBoundItem as Spotify;
                    Process.Start(new ProcessStartInfo("cmd", "/c start chrome.exe \"" + s.Anime.URL + "\""));
                }
            }
            catch
            {
                return;
            }
            dataGridView1.Refresh();
            ((List<Spotify>)dataGridView1.DataSource).ToFile(Path.Combine(web.ResultPath, "Spotify", $"anime {dataGridView1.Tag} mix.json"));
        }
        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (dataGridView1.Columns[e.ColumnIndex].GetType() == typeof(DataGridViewButtonColumn))
                {
                    string header = dataGridView1.Columns[e.ColumnIndex].HeaderText;
                    if (header.Equals("Change"))
                    {
                        Process.Start("cmd.exe", "/C taskkill /IM chrome.exe /F");
                        Spotify s = dataGridView1.Rows[e.RowIndex].DataBoundItem as Spotify;
                        string uri = $"https://open.spotify.com/search/{s.Anime.Song.Replace(" ", "%20")}%20{s.Anime.Artist.Replace(" ", "%20")}/tracks";
                        Process.Start(new ProcessStartInfo("cmd", "/c start chrome.exe \"" + uri + "\""));
                        
                        //BrowserUtil.Open(new Uri(uri));
                        /*SearchResponse response = await SearchTrack(s.Anime.Song, s.Anime.Artist);
                        ComboBox cb = new ComboBox();
                        cb.DataSource = response.Tracks.Items.Select(x => $"{x.Name} ({string.Join(",", x.Artists.Select(y => y.Name))}) [{x.Id}]").ToList();
                        MyControl mc = new MyControl(cb);
                        if (mc.isOK())
                        {
                            FullTrack track = response.Tracks.Items.Single(x => x.Id == mc.GetComboBox().ToString().Split(new char[] { '[', ']' })[1]);
                            s.Modify(new Spotify() { Track = track.Name, Author = track.Artists.First().Name, Id = track.Id, Add = true, Anime = s.Anime });
                            dataGridView1.Refresh();
                        }*/
                    }
                    else if (header.Equals("Preview"))
                    {
                        Spotify s = dataGridView1.Rows[e.RowIndex].DataBoundItem as Spotify;
                        FullTrack track = await spotify.Tracks.Get(s.Id);
                        string preview = track.PreviewUrl;
                        /*if (!string.IsNullOrEmpty(preview))
                        {
                            WebBrowser wb = new WebBrowser();
                            wb.Navigate(preview);
                            wb.Navigate(preview);
                        }*/
                        if (!string.IsNullOrEmpty(track.Uri))
                        {
                            s.Modify(new Spotify() { Track = track.Name, Author = track.Artists.ToListString("Name"), Id = track.Id, Add = true, Anime = s.Anime });
                            dataGridView1.Refresh();
                        }
                    }
                }
            }
            catch
            {
                return;
            }
            dataGridView1.Refresh();
            ((List<Spotify>)dataGridView1.DataSource).ToFile(Path.Combine(web.ResultPath, "Spotify", $"anime {dataGridView1.Tag} mix.json"));
        }

        private static void ShowResult(object sender, EventArgs e)
        {
            string result = ChooseResult();
            if (string.IsNullOrEmpty(result))
                return;
            string path = Path.Combine(web.ResultPath, "Spotify", $"{result}.json");
            web.dataGridView1.DataSource = JsonConvert.DeserializeObject<List<Spotify>>(File.ReadAllText(path)).GroupBy(x=>x.Anime).Select(y=>y.First()).ToList();
            web.dataGridView1.Tag = result.TrimStart("anime").TrimEnd("mix").Trim();
            web.WindowState = FormWindowState.Maximized;
        }

        private static string ChooseResult()
        {
            string season = string.Empty;
            ComboBox cb = new ComboBox();
            cb.DataSource = Directory.GetFileSystemEntries(Path.Combine(web.ResultPath, "Spotify")).Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
            MyControl mc = new MyControl(cb);
            if (mc.isOK())
            {
                season = mc.GetComboBox().ToString();
                mc.Close();
            }
            return season;
        }

    }
    public class Spotify
    {
        private Anime anime;
        private string track;
        private string author;
        private string id;
        private bool add;

        public Anime Anime { get => anime; set => anime = value; }
        public string Id { get => id; set => id = value; }
        public string Track { get => track; set => track = value; }
        public string Author { get => author; set => author = value; }
        public bool Add { get => add; set => add = value; }

        public void Modify(Spotify s)
        {
            Track = s.Track;
            Author = s.Author;
            Id = s.Id;
            Add = true;
            Anime = s.Anime.Clone();
        }
    }

    public class AccessToken
    {

        public string access_token;
        public string token_type;
        public int expires_in;
    }

    public class ExternalUrls
    {
        public string spotify { get; set; }
    }

    public class SpotifyImage
    {
        public int? height { get; set; }
        public string url { get; set; }
        public int? width { get; set; }
    }

    public class Item
    {
        public bool collaborative { get; set; }
        public string description { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<SpotifyImage> images { get; set; }
        public string name { get; set; }
        public Owner owner { get; set; }
        public object primary_color { get; set; }
        public bool @public { get; set; }
        public string snapshot_id { get; set; }
        public Tracks tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Owner
    {
        public string display_name { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class PlaylistList
    {
        public string href { get; set; }
        public int limit { get; set; }
        public object next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
        public List<Item> items { get; set; }
    }

    public class Tracks
    {
        public string href { get; set; }
        public int total { get; set; }
    }
}
