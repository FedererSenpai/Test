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
            Task t = new Task(async () => await Auth());
            t.Start();
            this.BringToFront();

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

        private async void Senpai(object sender, EventArgs e)
        {
            string name = "FedererSenpaiList";
            string path = Path.Combine(web.ResultPath, "MAL", $"{name}.json");
            List<Anime> l = JsonConvert.DeserializeObject<List<Anime>>(File.ReadAllText(path)).Where(x => x.Song != "No opening theme" && x.Song != "No ending theme").ToList();
            web.myProgressBar1.Maximum = l.Count;
            foreach (Anime anime in l)
            {
                Spotify spoti = ProcessSpotify(await SearchTrack(anime.Song), anime.Artist);
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
                Spotify spoti = ProcessSpotify(await SearchTrack(anime.Song), anime.Artist);
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
            web.WindowState = FormWindowState.Maximized;
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

        private async Task<SearchResponse> SearchTrack(string title)
        {
            try
            {
                SearchRequest request = new SearchRequest(SearchRequest.Types.Track, title);
                return await spotify.Search.Item(request);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private Spotify ProcessSpotify(SearchResponse response, string artist)
        {
            FullTrack track = response.Tracks.Items.FirstOrDefault(x=>x.Artists.Select(y=>y.Name).Contains(artist));
            bool add = true;
            if(track == null)
            {
                track = response.Tracks.Items.First();
                add = false;
            }
            return new Spotify() { Track = track.Name, Author = track.Artists.First().Name, Id = track.Id, Add = add };
        }

        private void dataGridView1_CellFormating(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Spotify s = dataGridView1.Rows[e.RowIndex].DataBoundItem as Spotify;
            if (s.Anime.Header.Equals("Continuing"))
                e.CellStyle.BackColor = Color.LightBlue;
        }

        private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].GetType() == typeof(DataGridViewButtonColumn))
            {
                string header = dataGridView1.Columns[e.ColumnIndex].HeaderText;
                if (header.Equals("Change"))
                {
                    Spotify s = dataGridView1.Rows[e.RowIndex].DataBoundItem as Spotify;
                    SearchResponse response = await SearchTrack(s.Anime.Song);
                    ComboBox cb = new ComboBox();
                    cb.DataSource = response.Tracks.Items.Select(x => $"{x.Name} ({string.Join(",", x.Artists.Select(y => y.Name))}) [{x.Id}]").ToList();
                    MyControl mc = new MyControl(cb);
                    if (mc.isOK())
                    {
                        FullTrack track = response.Tracks.Items.Single(x => x.Id == mc.GetComboBox().ToString().Split(new char[] { '[', ']' })[1]);
                        s.Modify(new Spotify() { Track = track.Name, Author = track.Artists.First().Name, Id = track.Id, Add = true, Anime = s.Anime });
                        dataGridView1.Refresh();
                    }
                }
                else if (header.Equals("Preview"))
                {
                    Spotify s = dataGridView1.Rows[e.RowIndex].DataBoundItem as Spotify;
                    FullTrack track = await spotify.Tracks.Get(s.Id);
                    string preview = track.PreviewUrl;
                    if (!string.IsNullOrEmpty(preview))
                    {
                        WebBrowser wb = new WebBrowser();
                        wb.Navigate(preview);
                        wb.Navigate(preview);
                        s.Modify(new Spotify() { Track = track.Name, Author = track.Artists.First().Name, Id = track.Id, Add = true, Anime = s.Anime });
                        dataGridView1.Refresh();
                    }
                }
            }
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
        public string Track { get => track; set => track = value; }
        public string Author { get => author; set => author = value; }
        public string Id { get => id; set => id = value; }
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
}
