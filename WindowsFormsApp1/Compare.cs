using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Compare : Form
    {
        int index = 0;
        int max = 0;
        List<Tuple<string, List<string>, List<string>>> list = new List<Tuple<string, List<string>, List<string>>>();
        List<string> oldlist = new List<string>();
        List<string> newlist = new List<string>();

        public Compare()
        {
            InitializeComponent();
        }

        private void Compare_Load(object sender, EventArgs e)
        {
            string filename = Path.Combine(Application.StartupPath, "Result", "Git", "sys_datos.txt");
            string url = @"C:\Users\dzhang\source\repos\BalanzaPC\BalanzaPC";
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("cmd.exe", $"/C git diff > {filename}") { WorkingDirectory = url};
            p.Start();
            p.WaitForExit();
            p.Close();

            string changeline = string.Empty;
            bool first = true;
            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if(line.StartsWith("@@"))
                    {
                        if(!first)
                        {
                            list.Add(new Tuple<string, List<string>, List<string>>(changeline, oldlist, newlist));
                        }
                        else
                        {
                            first = false;
                        }
                        changeline = line;
                        oldlist = new List<string>();
                        newlist = new List<string>();
                    }
                    if(line.StartsWith("-"))
                    {
                        oldlist.Add(line.TrimStart("-").Trim());
                    }
                    if(line.StartsWith("+"))
                    {
                        newlist.Add(line.TrimStart("+").Trim());
                    }
                }
            }
            list.Add(new Tuple<string, List<string>, List<string>>(changeline, oldlist, newlist));
            max = list.Count() - 1;
            UpdateListBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //prev
            if (index == 0)
                index = max;
            else
                index--;
            UpdateListBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //next
            if (index == max)
                index = 0;
            else
                index++;
            UpdateListBox();
        }

        private void UpdateListBox()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            foreach (string s in list[index].Item2)
                listBox1.Items.Add(s);
            foreach (string s in list[index].Item3)
                listBox2.Items.Add(s);
            if(listBox1.Items.Count > 0)
            listBox1.SelectedIndex = 0;
            if(listBox2.Items.Count > 0)
            listBox2.SelectedIndex = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.Items.Count >= listBox1.SelectedIndex + 1)
                listBox2.SelectedIndex = listBox1.SelectedIndex;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count >= listBox2.SelectedIndex + 1)
                listBox1.SelectedIndex = listBox2.SelectedIndex;
        }
        //Get-ChildItem . -Attributes Directory+Hidden -ErrorAction SilentlyContinue -Filter ".git" -Recurse
    }
}
