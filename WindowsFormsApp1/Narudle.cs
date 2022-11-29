using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Narudle : Form
    {
        Random r = new Random();
        TextBox tb = new TextBox();
        string[] personajesNaruto = new string[] { "NARUTO", "SASUKE", "SAKURA", "KAKASHI", "MINATO", "OROCHIMARU", "OBITO", "AKAMARU", "ASUMA", "BORUTO", "ITACHI", "MADARA", "DANZO", "KUSHINA", "CHOJI", "ROCKLEE", "SARADA", "MITSUKI", "KONOHAMARU", "HINATA", "KIBA", "SHINO", "KURENAI", "SHIKAMARU", "INO", "NEJI", "TENTEN", "MIGHTGUY", "GAARA", "KANKURO", "TEMARI", "ZABUZA", "HAKU", "KABUTO", "ZETSU", "KISAME", "KONAN", "NAGATO", "DEIDARA", "HIDAN", "KAKUZU", "SASORI", "KAGUYA", "IRUKA", "HASHIRAMA", "TSUANDE", "TOBIRAMA", "JIRAIYA", "SAI", "YAMATO", "KILLERBEE", "KURAMA", "KARIN", "JUGO", "RIN", "SHISUI"};
        string[] personajesOnePiece = new string[] { "LUFFY", "NAMI","ZORO","SANJI","CHOPPER","ROBIN", "FRANKY", "BROOK", "JINBE", "ARLONG", "FOXY", "AOKIJI", "SENGOKU", "AKAINU", "KIZARU", "MIHAWK", "VIVI", "CROCODILE", "GARP", "SMOKER", "SHANKS", "ICEBURG", "ROB", "ENEL", "DOFLAMINGO", "BELLAMY", "KAIDO", "BIGMOM", "SHIROHIGE", "KUROHIGE", "LAW", "KID", "MORIA", "ODEN", "YAMATO", "ROGER", "ACE", "SABO", "BUGGY", "HANCOCK", "KUMA", "VEGAPUNK", "FUJITORA", "KOBY", "MOMONOSUKE", "RAYLEIGH", "DRAGON", "KATAKURI", "HAWKINS", "BARTOLOMEO", "ROSINANTE", "UTA", "MARCO", "KING", "QUEEN" };
        private static int puntero = 0;
        TextBox[][] tbl;
        private static int countf = 0;
        private static string guess = string.Empty;

        public Narudle()
        {
            InitializeComponent();
            guess = GetRandomPersonaje();
            tbl = new TextBox[6][];
            CrearCajas(guess.Length);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string GetRandomPersonaje()
        {
            string[] ss = personajesNaruto.Concat(personajesOnePiece).ToArray();
            ss = ss.OrderBy(x => r.Next()).ToArray();
            return ss[r.Next(ss.Count() - 1)];
        }

        private void CrearCajas(int longi)
        {
            int y = 200;
            int x = 200;
            for (int i = 0; i < 6; i++)
            {
                x = (Screen.PrimaryScreen.Bounds.Width - 100 * longi - 50 * longi) / 2;
                tbl[i] = new TextBox[longi];
                for (int j = 0; j < longi; j++)
                {
                    x += 150;
                    tbl[i][j]= CrearLetra(x, y);
                }
                y += 150;
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
            tb.Font = new System.Drawing.Font("Microsoft Sans Serif", 65F, FontStyle.Bold);
            tb.MinimumSize = new Size(100, 100);
            tb.Location = new Point(x, y);
            tb.Name = $"{x}-{y}";
            this.Controls.Add(tb);
            return tb;
        }

        private void Narudle_KeyDown(object sender, KeyEventArgs e)
        {
            bool result = false;
            if(e.KeyValue == (int)Keys.Enter)
            {
                if (puntero % guess.Length == 0)
                    result = CheckGuess();
                if (result && countf == 6)
                {
                    MessageBox.Show("Fin");
                    Application.Exit();
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
                if (puntero < guess.Length)
                {
                    tbl[countf][puntero].Text = e.KeyCode.ToString();
                    puntero++;
                }
            }

        } 

        private bool CheckGuess()
        {
            string s = string.Concat(tbl[countf].ToList().Select(x=>x.Text));
            for(int i = 0; i < guess.Length; i++)
            {
                if (guess[i].ToString() == tbl[countf][i].Text)
                    tbl[countf][i].BackColor = Color.DarkOrange;
            }
            for (int i = 0; i < guess.Length; i++)
            {
                if(tbl[countf][i].BackColor != Color.DarkOrange)
                {
                    tbl[countf][i].BackColor = Color.Black;
                    int a = guess.ToCharArray().Where(x => (char)x == tbl[countf][i].Text.First()).Count();
                    int b = tbl[countf].Where(x => (x.BackColor == Color.DarkOrange || x.BackColor == Color.LightBlue) && x.Text == tbl[countf][i].Text).Count();
                    if (a != b)
                        tbl[countf][i].BackColor = Color.LightBlue;
                }
            }
                if (personajesNaruto.Concat(personajesOnePiece).ToArray().Contains(s))
            {
                countf++;
                puntero = 0;
                if (s.Equals(guess))
                {
                    MessageBox.Show("true");
                    return true;
                }
                else
                {
                    MessageBox.Show("false");
                    return true;
                }
            }
            else
            {
                for (int i = puntero - guess.Length; i < puntero; i++)
                {
                        tbl[countf][i].BackColor = Color.Red;
                }
                MessageBox.Show("error");
                return false;
            }
        }

    }

}
