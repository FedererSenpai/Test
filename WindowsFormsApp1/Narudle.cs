using CefSharp.DevTools.Database;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Swan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Narudle : Base
    {
        Random r = new Random();
        TextBox tb = new TextBox();
        string[] personajesNaruto = new string[] { "NARUTO", "SASUKE", "SAKURA", "KAKASHI", "MINATO", "OROCHIMARU", "OBITO", "AKAMARU", "ASUMA", "BORUTO", "ITACHI", "MADARA", "DANZO", "KUSHINA", "CHOJI", "ROCKLEE", "SARADA", "MITSUKI", "KONOHAMARU", "HINATA", "KIBA", "SHINO", "KURENAI", "SHIKAMARU", "INO", "NEJI", "TENTEN", "MIGHTGUY", "GAARA", "KANKURO", "TEMARI", "ZABUZA", "HAKU", "KABUTO", "ZETSU", "KISAME", "KONAN", "NAGATO", "DEIDARA", "HIDAN", "KAKUZU", "SASORI", "KAGUYA", "IRUKA", "HASHIRAMA", "TSUANDE", "TOBIRAMA", "JIRAIYA", "SAI", "YAMATO", "KILLERBEE", "KURAMA", "KARIN", "JUGO", "RIN", "SHISUI"};
        string[] personajesOnePiece = new string[] { "LUFFY", "NAMI","ZORO","SANJI","CHOPPER","ROBIN", "FRANKY", "BROOK", "JINBE", "ARLONG", "FOXY", "AOKIJI", "SENGOKU", "AKAINU", "KIZARU", "MIHAWK", "VIVI", "CROCODILE", "GARP", "SMOKER", "SHANKS", "ICEBURG", "ROB", "ENEL", "DOFLAMINGO", "BELLAMY", "KAIDO", "BIGMOM", "SHIROHIGE", "KUROHIGE", "LAW", "KID", "MORIA", "ODEN", "YAMATO", "ROGER", "ACE", "SABO", "BUGGY", "HANCOCK", "KUMA", "VEGAPUNK", "FUJITORA", "KOBY", "MOMONOSUKE", "RAYLEIGH", "DRAGON", "KATAKURI", "HAWKINS", "BARTOLOMEO", "ROSINANTE", "UTA", "MARCO", "KING", "QUEEN" };
        private static int puntero = 0;
        TextBox[][] tbl;
        private static int countf = 0;
        private static Character guess;
        private string OldPath;
        private List<Character> characters;

        public Narudle()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private Character GetRandomPersonaje()
        {
            string[] ss = personajesNaruto.Concat(personajesOnePiece).ToArray();
            ss = ss.OrderBy(x => r.Next()).ToArray();
            return characters.ElementAt(r.Next(characters.Count() - 1));
        }

        private void CrearCajas(int longi)
        {
            int y = 50;
            int x = 200;
            for (int i = 0; i < 6; i++)
            {
                x = (Screen.PrimaryScreen.Bounds.Width - 75 * longi - 25 * longi) / 2;
                tbl[i] = new TextBox[longi];
                for (int j = 0; j < longi; j++)
                {
                    tbl[i][j]= CrearLetra(x, y);
                    x += 100;
                }
                y += 100;
            }
        }

        private TextBox CrearLetra(int x, int y)
        {
            TextBox tb = new TextBox();
            tb.Enabled = false;
            tb.TextAlign = HorizontalAlignment.Center;
            tb.MaxLength = 1;
            tb.BorderStyle = BorderStyle.None;
            tb.BackColor = Color.Red;
            tb.ForeColor = Color.WhiteSmoke;
            tb.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, FontStyle.Bold);
            tb.Size = new Size(75, 75);
            tb.Location = new Point(x, y);
            tb.Name = $"{x}-{y}";
            this.Controls.Add(tb);
            return tb;
        }

        private void Narudle_KeyDown(object sender, KeyEventArgs e)
        {
            bool result = false;
            string g = guess.FirstName;
            if(e.KeyValue == (int)Keys.Enter)
            {
                if (puntero % g.Length == 0)
                    result = CheckGuess();
                if (result || countf == 6)
                {
                    ShowAnswer();
                }
            }
            else if (e.KeyValue == (int)Keys.Back)
            {
                if (puntero > 0)
                    puntero--;
                tbl[countf][puntero].Text = string.Empty;
            }
            else if (e.KeyValue > (int)Keys.A || e.KeyValue < (int)Keys.Z)
            {
                if (puntero < g.Length)
                {
                    tbl[countf][puntero].Text = e.KeyCode.ToString();
                    puntero++;
                }
            }

        } 

        private bool CheckGuess()
        {
            string s = string.Concat(tbl[countf].ToList().Select(x=>x.Text));
            string g = guess.FirstName.ToUpper();
            if (characters.Any(x => x.FirstName.ToUpper().Equals(s)))
            {
                for (int i = 0; i < g.Length; i++)
            {
                if (g[i] == s[i])
                    tbl[countf][i].BackColor = Color.DarkOrange;
            }
            for (int i = 0; i < g.Length; i++)
            {
                if(tbl[countf][i].BackColor != Color.DarkOrange)
                {
                    tbl[countf][i].BackColor = Color.Black; 
                    int a = g.ToCharArray().Where(x => (char)x == s[i]).Count();
                    int b = tbl[countf].Where(x => (x.BackColor == Color.DarkOrange || x.BackColor == Color.LightBlue) && x.Text[0] == s[i]).Count();
                    if (a != b)
                        tbl[countf][i].BackColor = Color.LightBlue;
                }
            }

                countf++;
                puntero = 0;
                return s.Equals(g);
            }
            else
            {
                for (int i = puntero - g.Length; i < puntero; i++)
                {
                        tbl[countf][i].BackColor = Color.Red;
                }
                return false;
            }
        }

        private void ShowAnswer()
        {
            for(int i = countf; i<6;i++)
            {
                for (int j = 0; j < guess.FirstName.Length; j++)
                    tbl[i][j].BackColor = Color.WhiteSmoke;
            }
            using (WebClient client = new WebClient())
            {               
                byte[] data = client.DownloadData(new Uri(guess.Image));
                Image image = Image.FromStream(new MemoryStream(data));
                PictureBox pb = new PictureBox();
                pb.SizeMode = PictureBoxSizeMode.Zoom;
                pb.Image = image;
                pb.Dock = DockStyle.Left;
                pb.Margin = new Padding(0);
                Panel p1 = new Panel();
                p1.Dock = DockStyle.Top;
                Label l1 = new Label();
                l1.Text = guess.FullName;
                l1.Dock = DockStyle.Top;
                Label l2 = new Label();
                l2.Text = guess.Role;
                l2.Dock = DockStyle.Top;
                p1.Controls.Add(l2);
                p1.Controls.Add(l1);
                Panel p2 = new Panel();
                p2.Dock = DockStyle.Right;
                Label l3 = new Label();
                l3.Text = guess.VoiceActor;
                l3.Dock = DockStyle.Top;
                l3.TextAlign = ContentAlignment.MiddleRight;
                Label l4 = new Label();
                l4.Text = guess.Nationality;
                l4.Dock = DockStyle.Top;
                l4.TextAlign = ContentAlignment.MiddleRight;
                p2.Controls.Add(l4);
                p2.Controls.Add(l3);
                Label l5 = new Label();
                l5.Text = guess.Anime;
                l5.Dock = DockStyle.Top;
                l5.TextAlign = ContentAlignment.TopCenter;
                FlowLayoutPanel p = new FlowLayoutPanel();
                p.AutoSize = true;
                p.WrapContents = true;
                p.FlowDirection = FlowDirection.LeftToRight;
                p.Controls.Add(pb);
                p.Controls.Add(p1);
                p.Controls.Add(p2);
                FlowLayoutPanel pp = new FlowLayoutPanel();
                pp.FlowDirection = FlowDirection.TopDown;
                pp.AutoSize = true;
                pp.WrapContents = true;
                pp.Controls.Add(l5);
                pp.Controls.Add(p);
                MyControl mc = new MyControl(pp);
                mc.ShowDialog(this);
            }
        }

        private void Generate(object sender, EventArgs e)
        {
            this.Controls.Clear();
            countf = 0;
            puntero = 0;
            guess = GetRandomPersonaje();
            tbl = new TextBox[6][];
            CrearCajas(guess.FirstName.Length);
            CrearTeclado();
        }

        private void CrearTeclado()
        {
            TextBox tb = new TextBox();
            tb.Enabled = false;
            tb.TextAlign = HorizontalAlignment.Center;
            tb.MaxLength = 1;
            tb.BorderStyle = BorderStyle.None;
            tb.BackColor = Color.Red;
            tb.ForeColor = Color.WhiteSmoke;
            tb.Font = new System.Drawing.Font("Microsoft Sans Serif", 50F, FontStyle.Bold);
            tb.Size = new Size(75, 75);
            //tb.Location = new Point(x, y);
            //tb.Name = $"{x}-{y}";
            this.Controls.Add(tb);
        }

        private void Narudle_Load(object sender, EventArgs e)
        {
            AddMenu("Load", new EventHandler(Characters));
            AddMenu("Start", new EventHandler(Generate));
            AddMenu("Search", new EventHandler(Search));
            OldPath = Path.Combine(ResultPath, "Senpai", "Old");
            characters = ExtensionMethods.FileToList<Character>(FullPath.Replace("txt", "json"));
        }

        private void MyTextChanged(object sender, EventArgs e)
        {
            ListBox l = ((TextBox)sender).Parent.Controls.Find("Narudle", false).First() as ListBox;
            string find = ((TextBox)sender).Text.ToUpper();
            l.DataSource = characters.Where(x => x.FirstName.Length == guess.FirstName.Length && x.FirstName.ToUpper().Contains(find)).ToList();
        }

        private void Search(object sender, EventArgs e)
        {
            FlowLayoutPanel p = new FlowLayoutPanel();
            TextBox t = new TextBox();
            t.Margin = new Padding(0);
            t.Width = 250;
            t.TextChanged += new EventHandler(MyTextChanged);
            ListBox l = new ListBox();
            l.Name = "Narudle";
            l.Margin = new Padding(0);
            l.Width = 250;
            l.DataSource = characters.Where(x=>x.FirstName.Length == guess.FirstName.Length).ToList();
            l.ValueMember = "FirstName";
            l.DisplayMember = "FullName";
            l.Sorted = true;
            p.FlowDirection = FlowDirection.TopDown;
            p.AutoSize = true;
            p.Controls.Add(t);
            p.Controls.Add(l);
            MyControl mc = new MyControl(p);
            if(mc.ShowDialog(this) == DialogResult.OK)
            {
                string s = ((Character)l.SelectedItem).FirstName.ToUpper();
                for (int i = 0; i<s.Length; i++)
                {
                    tbl[countf][i].Text = s[i].ToString();
                }
                SendKeys.SendWait("{ENTER}");
            }
        }

        private void Characters(object sender, EventArgs e)
        {
            characters = new List<Character>();
            MyProgressBar mpb = new MyProgressBar();
            mpb.Maximum = Directory.GetFiles(OldPath).Length;
            MyControl mc = new MyControl(mpb);
            mc.Show();
            foreach(string file in Directory.GetFiles(OldPath))
            {
                try
                {
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    using (StreamReader sr = new StreamReader(file))
                    {
                        doc.LoadHtml(sr.ReadToEnd());
                    }
                    HtmlNode charNode = doc.DocumentNode.SelectSingleNode("//div[@class='detail-characters-list clearfix']");
                    foreach (HtmlNode box in charNode.SelectNodes(".//table[@width='100%']"))
                    {
                        characters.Add(new Character(box) {Anime = Path.GetFileNameWithoutExtension(file) });
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, file + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                }
                mpb.Value++;
            }
            mc.Close();
            characters.ToFile(FullPath.Replace("txt", "json"));
        }

        private class Character
        {
            private string image;
            private string firstName;
            private string lastName;
            private string role;
            private string voiceActor;
            private string nationality;
            private string anime;

            public string FirstName { get => firstName; set => firstName = value; }
            public string LastName { get => lastName; set => lastName = value; }
            public string Role { get => role; set => role = value; }
            public string VoiceActor { get => voiceActor; set => voiceActor = value; }
            public string Nationality { get => nationality; set => nationality = value; }
            public string Image { get => image; set => image = value; }
            public string Anime { get => anime; set => anime = value; }
            [JsonIgnore]
            public string FullName => (string.IsNullOrEmpty(lastName) ? string.Empty : (LastName + ", ")) + FirstName;

            public Character()
            {

            }

            public Character(string _firstName, string _lastName, string _role, string _voiceActor, string _nationality)
            {
                firstName = _firstName;
                lastName = _lastName;
                role = _role;
                voiceActor = _voiceActor;
                nationality = _nationality;
            }

            public Character(HtmlNode node)
            {
                Image = node.SelectSingleNode(".//img").GetAttributeValue("data-src", string.Empty);
                GetName(node.SelectSingleNode(".//h3[@class='h3_characters_voice_actors']").InnerText);
                role = node.SelectSingleNode(".//div[@class='spaceit_pad']").InnerText.Trim();
                HtmlNode subnode = node.SelectSingleNode(".//td[@class='va-t ar pl4 pr4']");
                voiceActor = subnode.SelectSingleNode(".//a").InnerText.Trim();
                nationality = subnode.SelectSingleNode(".//small").InnerText.Trim();
            }

            private void GetName(string name)
            {
                string[] n = name.Split(',');                
                if (n.Length == 1)
                    firstName = name.Trim();
                else
                {
                    firstName = n[1].Trim();
                    lastName = n[0].Trim();
                }
            }
        }

    }

}
