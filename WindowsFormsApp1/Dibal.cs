using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public static class Dibal
    {
        public static void EST()
        {
            byte[] fichero;
            fichero = File.ReadAllBytes("U:\\AMA\\TX_LogosUSB.bck");
                DateTime q = DateTime.Now;
            string afosui = q.ToString("MMddyyyyhhmmss");
            string fz = q.ToString("HHmmss");
            MessageBox.Show(fz);
            string l = "15010202020102955498000002119";
            string h = l.Substring(8, 4);
            string n2 = l.Substring(8, 4).Equals("0000") ? "0000" : l.Substring(4, 4);
            string o = l.Substring(8, 4).Equals("0000") ? l.Substring(4, 4) : l.Substring(8, 4);
            string path = Application.StartupPath + "\\EST";
            string file = Directory.GetFiles(path).First(x => x.EndsWith("_tmp"));
            string f = Directory.GetFiles(path).First(x=>x.EndsWith("_last"));
            bool e = File.Exists(f);
            //File.Move(f, Path.Combine(path + "\\Procesados", Path.GetFileName(f).Replace("_last", string.Empty)));
            //File.Move(file + "_tmp", file + "_last");
            List<string> viejo = File.ReadLines(f).Where(x=>x.StartsWith("02")).ToList();
            List<string> nuevo = File.ReadLines(file).Where(x => x.StartsWith("02")).ToList();
            List<string> delta = new List<string>();
            delta.AddRange(nuevo.Except(viejo)); //Nuevos registros.
            //delta.AddRange(viejo.Except(nuevo).Where(x => !nuevo.Select(y => y.Substring(0, 36)).Contains(x.Substring(0, 36))).Select(x=>x.Substring(0,36) + "|00|"));
            delta.AddRange(viejo.Except(nuevo).Select(x => x.Substring(0, 36)).Except(nuevo.Select(y => y.Substring(0, 36))).Select(z => z + "|00|"));
            foreach(string n in nuevo)
            {
                if (!viejo.Contains(n))
                    delta.Add(n);
            }
            foreach (string v in viejo)
            {
                if (!nuevo.Contains(v))
                {
                    foreach (string n in nuevo)
                    {
                        if (n.StartsWith(v.Substring(0, 36)))
                        {
                            break;
                        }
                        if(n.Equals(nuevo.Last()))
                        {
                            delta.Add(v.Substring(0,36) + "|00|");
                        }
                    }
                }
            }
        }
    }
}
