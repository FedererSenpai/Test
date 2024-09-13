using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Http.SelfHost;
using EmbedIO.Actions;
using EmbedIO;
using SpotifyAPI.Web.Auth;
using System.Reflection;
using RiotSharp.Endpoints.SpectatorEndpoint;
using SpotifyAPI.Web;
using System.Windows;
using System.Runtime.CompilerServices;

namespace WindowsFormsApp1
{
    public partial class Lyrics : Base
    {
        private static string clientid = "50dfcb15aa95487f9b92990daf6bb6dd";
        private static string clientsecret = "0990c8a694a349aa8f27b6158f1f7ec7";
        private static string userid = "316b4swabpspq4guu4yzvz5jnz4q";
        private static EmbedIOAuthServer _server;
        private static SpotifyClient spotify;
        private static AccessToken accessToken;
        private static System.Timers.Timer tim;

        public Lyrics()
        {
            InitializeComponent();
        }

        private void Lyrics_Load(object sender, EventArgs e)
        {
            tim = new System.Timers.Timer();
            tim.Interval = 100;
            tim.Elapsed += timer_tick;
            Task t = new Task(async () => await Auth());
            t.Start();
            GetToken();
        }

        private void GetToken()
        {
            WebRequest request = WebRequest.Create($"https://accounts.spotify.com/api/token");
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            string stringData = $"grant_type=client_credentials&client_id={clientid}&client_secret={clientsecret}"; // place body here
            var data = Encoding.Default.GetBytes(stringData); // note: choose appropriate encoding
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(data, 0, data.Length);
            }

            WebResponse response = request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string responsestring = reader.ReadToEnd();
                accessToken = JsonConvert.DeserializeObject<AccessToken>(responsestring);
            }
        }

        private void RefreshToken()
        {
            try
            {
                WebRequest request = WebRequest.Create($"https://accounts.spotify.com/api/token");
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                string stringData = $"grant_type=refresh_token&refresh_token={accessToken}&client_id={clientid}&client_secret={clientsecret}"; // place body here
                var data = Encoding.Default.GetBytes(stringData); // note: choose appropriate encoding
                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(data, 0, data.Length);
                }

                WebResponse response = request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string responsestring = reader.ReadToEnd();
                    accessToken = JsonConvert.DeserializeObject<AccessToken>(responsestring);
                }
            }
            catch (Exception ex)
            {

            }
        }

        delegate void TrackText();
        private async void GetCurrentTrack()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    TrackText del = new TrackText(GetCurrentTrack);
                    this.Invoke(del);
                }
                else
                {
                    var player = await spotify.Player.GetCurrentPlayback();
                    label1.Text = (player.Item as FullTrack).Name;
                    label2.Text = (player.Item as FullTrack).Artists.ToListString("Name");
                    Refresh();
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            catch (Exception ex)
            {

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
                Scope = new List<string> { Scopes.UserReadEmail, Scopes.PlaylistModifyPublic, Scopes.PlaylistModifyPrivate, Scopes.UserReadPlaybackState, Scopes.UserReadCurrentlyPlaying, Scopes.UserModifyPlaybackState }
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

            SendKeys.SendWait("^{w}"); // close tab.
            Thread.Sleep(100);
            WinUtil.FocusProcess("WindowsFormsApp1");
            Thread.Sleep(100);

            tim.Start();
        }

        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await _server.Stop();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            GetCurrentTrack();
        }
    }
}
