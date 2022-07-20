using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class Tiempos : Form
    {
        
        public Tiempos()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string s;
            string url = textBox1.Text;
            string hora;
            DateTime tiempo = new DateTime();
            DateTime tiempoanterior;
            StreamReader sr = new StreamReader(url);
            int i = 0;
            while ((s = sr.ReadLine()) != null)
            {
                try
                {
                    i++;
                    if (Regex.IsMatch(s, @"\d{2}:\d{2}:\d{2}\.\d+"))
                    {
                        tiempoanterior = tiempo;
                        hora = Regex.Match(s, @"\d{2}:\d{2}:\d{2}\.\d+").Value;
                        tiempo = DateTime.ParseExact(hora, "HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
                        if(tiempo.Subtract(tiempoanterior).TotalSeconds > 1)
                        {
                            MessageBox.Show(i.ToString());
                        }
                    }
                }
                catch { }
            }
            MessageBox.Show("fin");
        }
    }
}
