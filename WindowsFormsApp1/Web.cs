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

namespace WindowsFormsApp1
{
    public partial class Web : Form
    {
        private static string clientid = "50dfcb15aa95487f9b92990daf6bb6dd";
        private static string clientsecret = "0990c8a694a349aa8f27b6158f1f7ec7";
        private static string playlist = "0l3AESuzc6aEgpeD8sQbdp";
        private static EmbedIOAuthServer _server;
        List<string> list = new List<string>();
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
            Task t = new Task(async () => await Auth());
            t.Start();
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

            var spotify = new SpotifyClient(tokenResponse.AccessToken);
            // do calls with Spotify and save token?
            PlaylistAddItemsRequest request = new PlaylistAddItemsRequest(new List<string>() { "spotify:track:06YNVx8q2zF84s8SfGbxMC" });

            await spotify.Playlists.AddItems(playlist, request);
        }

        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var config = SpotifyClientConfig
    .CreateDefault()
    .WithAuthenticator(new ClientCredentialsAuthenticator(clientid, clientsecret));

            var spotify = new SpotifyClient(config);
            /*var fasdf = await spotify.Playlists.GetItems(playlist);
            foreach (PlaylistTrack<IPlayableItem> item in fasdf.Items)
            {
                if (item.Track is FullTrack track)
                {
                    Console.WriteLine(track.Name, track.Album.Name);
                }
            }
            ExtensionMethods.WriteToFile(Path.Combine(Application.StartupPath, "Playlist", "Otasku.json"),fasdf.Items.Select(x=>x.Track).ToJson());
            */
            PlaylistAddItemsRequest request = new PlaylistAddItemsRequest(new List<string>() { "06YNVx8q2zF84s8SfGbxMC" });
            
            await spotify.Playlists.AddItems(playlist, request);
            //Implicit Grant or Authorization Code flows.
        }
    }
}
