using NAudio.CoreAudioApi;
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
    public partial class Archivos : Form
    {
        int[] iii;
        public Archivos()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sound.test();
            return;
            Random r = new Random(100);
            while (true)
            {
                int i = r.Next();
            }
            try
            {
                IEnumerator afds = new List<string>() { "asfkl", "sfa", "sfkañd", "aaaaa", "sfakjhng", "sdiaopgh" }.GetEnumerator();
                while(afds.MoveNext())
                {
                    try
                    {
                        MessageBox.Show(afds.Current.ToString());
                        if (afds.Current.ToString().Equals("aaaaa"))
                            throw new Exception();
                    }
                    catch(Exception ex)
                    {
                        while(afds.MoveNext())
                        {
                            MessageBox.Show(afds.Current.ToString());
                        }
                        throw ex;
                    }
                    finally
                    {
                        MessageBox.Show("Fin");
                    }
                }
                NetworkCredential nc = new NetworkCredential(@"C, S1200", "CS1200");
                CredentialCache cc = new CredentialCache();
                cc.Add(new Uri(@"\\192.168.150.66"), "Basic", nc);
                string[] f = Directory.GetDirectories(@"X:\SW1100");
                FolderBrowserDialog fd = new FolderBrowserDialog();
                fd.RootFolder = Environment.SpecialFolder.Personal;
                fd.SelectedPath =  @"X:\\SW1100\";
                fd.ShowDialog();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
