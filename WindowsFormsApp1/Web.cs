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
using System.Windows.Controls;
using SpotifyAPI;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using RiotSharp.Endpoints.SpectatorEndpoint;
using System.Diagnostics;
using Swan;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace WindowsFormsApp1
{
    public partial class Web : Base
    {
        private static string clientid = "50dfcb15aa95487f9b92990daf6bb6dd";
        private static string clientsecret = "0990c8a694a349aa8f27b6158f1f7ec7";
        private static string playlist = "0l3AESuzc6aEgpeD8sQbdp";
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
        }

        private void Start(object sender, EventArgs e)
        {
            AddMenu("Load", new EventHandler(Search));
            Task t = new Task(async () => await Auth());
            t.Start();
            this.BringToFront();
        }

        private async void Search(object sender, EventArgs e)
        {
            string path = Path.Combine(Application.StartupPath, "Result", "fall 2023.json");
            foreach(Anime anime in JsonConvert.DeserializeObject<List<Anime>>(File.ReadAllText(path)))
            {
                Spotify spoti = ProcessSpotify(await Search(anime.Song), anime.Artist);
                spoti.Anime = anime.Clone();
                list.Add(spoti);
            }
            list.ToFile(Path.Combine(Application.StartupPath, "Result", "anime fall 2023 mix.json"));
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

            //await spotify.Playlists.AddItems(playlist, request);
        }

        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            SearchRequest request = new SearchRequest(SearchRequest.Types.Track, "Girlfriend");
            var asfd = await spotify.Search.Item(request);
            //var sdafgklh= asfd.Tracks.Items.Min(x => x.Artists.Min(y => ExtensionMethods.Distance(y.Name, "AvrilLavigne")));
        }

        private async Task<SearchResponse> Search(string title)
        {
            SearchRequest request = new SearchRequest(SearchRequest.Types.Track, title);
            return await spotify.Search.Item(request);
        }

        private Spotify ProcessSpotify(SearchResponse response, string artist)
        {
            FullTrack track = response.Tracks.Items.FirstOrDefault(x=>x.Artists.Select(y=>y.Name).Contains(artist));
            if(track == null)
            {
                track = response.Tracks.Items.First();
            }
            return new Spotify() { Track = track.Name, Author = track.Artists.First().Name, Id = track.Id };
        }

        public class Spotify
        {
            private Anime anime;
            private string track;
            private string author;
            private string id;

            public Anime Anime { get => anime; set => anime = value; }
            public string Track { get => track; set => track = value; }
            public string Author { get => author; set => author = value; }
            public string Id { get => id; set => id = value; }
        }
    }
}
