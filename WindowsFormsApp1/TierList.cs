using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class TierList : Base
    {
        HttpClient client = new HttpClient();
        string clientid = "faa1722b79f99c0aab71630c35c75a3b";
        HttpListener listener = new HttpListener();
        string code;

        public TierList()
        {
            InitializeComponent();
            Create();
            //client.BaseAddress = new Uri("https://api.myanimelist.net/v0.20");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromMilliseconds(10000);
        }

        private void Create()
        {
            Button b = new Button();
            b.Text = "ADD";
            b.TextAlign = ContentAlignment.MiddleCenter;
            b.Font = new Font(b.Font.FontFamily, 16, FontStyle.Bold);
            b.Dock = DockStyle.Left;
            b.Click += (s, e) =>
            {
                AddElement();
            };
            Panel p = new Panel();
            p.Controls.Add(b);
            flowLayoutPanel1.Controls.Add(p);
            AddTier("Master Piece", Color.Red);
            AddTier("Joyita", Color.Orange);
            AddTier("Muy Bueno", Color.Yellow);
            AddTier("Bueno", Color.Green);
            flowLayoutPanel1.SizeChanged += (s, e) =>
            {
                foreach (Control c in flowLayoutPanel1.Controls)
                {
                    c.Width = flowLayoutPanel1.ClientSize.Width - c.Margin.Horizontal;
                }
            };
        }

        private void AddTier(string text, Color color)
        {
            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.ColumnCount = 2;
            tlp.RowCount = 1;
            //tlp.ColumnStyles[0].Width = 250;
            Label l = new Label();
            l.Text = text;
            l.BackColor = color;
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Font = new Font(l.Font.FontFamily, 16, FontStyle.Bold);
            l.Dock = DockStyle.Left;
            l.Margin = new Padding(0);
            FlowLayoutPanel flp = new FlowLayoutPanel();
            flp.FlowDirection = FlowDirection.LeftToRight;
            flp.Dock = DockStyle.Fill;
            flp.BackColor = Color.LightGray;
            flp.Margin = new Padding(0);
            flp.Padding = new Padding(0);
            tlp.Controls.Add(l);
            tlp.Controls.Add(flp);
            tlp.Width = flowLayoutPanel1.ClientSize.Width;
            tlp.Tag = l.Text;
            tlp.Name = "Tier_" + l.Text;
            flowLayoutPanel1.Controls.Add(tlp);
        }

        private void AddElement()
        {
            FlowLayoutPanel flp = new FlowLayoutPanel();
            flp.BackColor = Color.LightGray;
            flp.FlowDirection = FlowDirection.TopDown;
            flp.Dock = DockStyle.Fill;
            ComboBox cbn = new ComboBox();
            string path = path = Path.Combine(ResultPath, "MAL", "FedererSenpai.json");
            cbn.DataSource = ExtensionMethods.JsonToList<AnimeURL>(File.ReadAllText(path)).OrderBy(x=>x.Name).ToList();
            cbn.ValueMember = "Name";
            cbn.DisplayMember = "Name";
            cbn.Width = 400;
            ComboBox cbt = new ComboBox();
            cbt.DataSource = flowLayoutPanel1.Controls.Cast<Control>().Where(x=>x.Tag != null).ToList();
            cbt.DisplayMember = "Tag";
            cbt.ValueMember = "Control";
            flp.Controls.Add(cbn);
            flp.Controls.Add(cbt);
            MyControl mc = new MyControl(flp);
            mc.ShowDialog();
            
            var c = flowLayoutPanel1.Controls.Find(((Control)cbt.SelectedValue).Name, false).First();
            c.Controls[1].Controls.Add(new Label() { Text = cbn.SelectedValue.ToString(), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Left, BackColor = Color.White, AutoSize = true, Font = new Font(Label.DefaultFont.FontFamily, 16, FontStyle.Bold), Margin = new Padding(0) });
            mc.Dispose();
            flowLayoutPanel1.Refresh();
        }

        private async Task Api()
        {
            HttpResponseMessage res = await client.GetAsync("anime/21?fields=id,main_picture");
            MessageBox.Show(await res.Content.ReadAsStringAsync());
        }

        private async Task Auth()
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["response_type"] = "code";
            query["client_id"] = clientid;
            query["state"] = Guid.NewGuid().ToString();
            query["redirect_uri"] = "http://localhost:8888";
            query["code_challenge"] = PKCE();
            query["code_challenge_method"] = "plain";

            string authurl = "https://myanimelist.net/v1/oauth2/authorize?" + query.ToString();
            listener.Prefixes.Add("http://localhost:8888/");
            listener.Start();

            Process.Start(new ProcessStartInfo(authurl) {UseShellExecute = true });

            var context = await listener.GetContextAsync();
            code = context.Request.QueryString["code"];
            string state = context.Request.QueryString["state"];
            listener.Stop();
        }

        private string PKCE()
        {
            Random r = new Random();
            int length = r.Next(22, 65);
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] data = new byte[64];
            rng.GetBytes(data, 0, 64);
            string code = Convert.ToBase64String(data, 0, length).Replace("+", "-").Replace("/", "_").Replace("=", "");
            return code;
        }

        private void TierList_Load(object sender, EventArgs e)
        {
            //Auth();
        }
    }
}
