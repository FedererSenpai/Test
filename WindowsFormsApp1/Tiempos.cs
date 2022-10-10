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
using System.Threading;
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

        private void a()
        {
            System.Diagnostics.Process pr = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo prs = new System.Diagnostics.ProcessStartInfo();
            prs.Arguments = "CS";
            prs.FileName = @"..\DibalDBImport\DibalDBImport.exe";
            prs.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            prs.UseShellExecute = true;
            pr.StartInfo = prs;

            if (File.Exists(prs.FileName))
            {

                pr.Start();
                pr.WaitForExit();

                DirectoryInfo directory = new DirectoryInfo(@"..\DibalDBImport\resultado");
                if (directory.Exists)
                {
                    FileInfo myFile = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();

                    var numLineas = 0;
                    using (var reader = File.OpenText(myFile.FullName))
                    {
                        while (reader.ReadLine() != null)
                        {
                            numLineas++;
                        }
                    }

                    //Obtener el resultado de la ultima importación
                    string lineDate = File.ReadLines(myFile.FullName).Skip(numLineas - 7).Take(1).First();
                    string lineResult = File.ReadLines(myFile.FullName).Skip(numLineas - 5).Take(1).First();
                }
            }
            else
            {
                MessageBox.Show("DibalDBImportNoInstalado");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            a();
            return;

            string s;
            string url = textBox1.Text;
            string hora;
            DateTime tiempo = new DateTime();
            DateTime tiempoanterior;
            StreamReader sr = new StreamReader(url);
            StreamWriter sw = new StreamWriter(@"C:\Users\dzhang\Desktop\Renan.txt");
            int i = 0;
            while ((s = sr.ReadLine()) != null)
            {
                try
                {
                    if(s.ToLower().Contains("registro enviado"))
                    {
                        sw.WriteLine(s.Split(':').Last().Substring(1));
                    }
                    //i++;
                    //if (Regex.IsMatch(s, @"\d{2}:\d{2}:\d{2}\.\d+"))
                    //{
                    //    tiempoanterior = tiempo;
                    //    hora = Regex.Match(s, @"\d{2}:\d{2}:\d{2}\.\d+").Value;
                    //    tiempo = DateTime.ParseExact(hora, "HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
                    //    if(tiempo.Subtract(tiempoanterior).TotalSeconds > 1)
                    //    {
                    //        MessageBox.Show(i.ToString());
                    //    }
                    //}
                }
                catch { }
            }
            MessageBox.Show("fin");
        }
    }
}
