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
using System.Xml;

namespace WindowsFormsApp1
{
    public partial class Cursor : Form
    {
        Timer t;
        public Cursor()
        {
            InitializeComponent();
            Log.EscribirLog("Start");
            t = new Timer();
            t.Interval = 1000;
            t.Tick += new EventHandler(this.CheckCursor);
            t.Start();
        }

        public void CheckCursor(object sender, EventArgs args)
        {
            t.Tick -= new EventHandler(this.CheckCursor);
            try
            {
                foreach (Process p in Process.GetProcessesByName("AccesoBalanzaPC"))
                {
                    string file = p.MainModule.FileName;
                    if (string.Compare(Directory.GetParent(Directory.GetParent(file).FullName).FullName, @"C:\Users\dzhang\Source\Repos\BalanzaPC\BalanzaPC\TecladoBasico\bin", StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        if (GuardarClaveOtroPrograma("Cursor", "1", p))
                        {
                            if (Directory.GetParent(file).Name == "Release")
                                {
                                Process exe = new Process();
                                p.StartInfo.FileName = file;  // just for example, you can use yours.
                                p.Start();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.EscribirError(ex.StackTrace, ex.Message);
            }
            t.Tick += new EventHandler(this.CheckCursor);
        }

        private static bool GuardarClaveOtroPrograma(string clave, string valor, Process p, string nombreFicheroConfig = "")
        {
            string ficConfigPrograma = nombreFicheroConfig;
            if (string.IsNullOrEmpty(ficConfigPrograma))
                ficConfigPrograma = @"C:\Users\dzhang\source\repos\BalanzaPC\BalanzaPC\TecladoBasico\app.config";
            if (File.Exists(ficConfigPrograma))
            {
                XmlDocument configXml = new XmlDocument();
                configXml.Load(ficConfigPrograma);

                XmlNode n;
                n = configXml.SelectSingleNode("configuration/appSettings" + "/add[@key=\"" + clave + "\"]");
                if (n != null)
                {
                    if (n.Attributes["value"].InnerText != valor)
                    {
                        System.Threading.Thread.Sleep(1000);
                        int i = 0;
                        p.Kill();
                        while (!p.HasExited && i < 1000)
                        {

                        }
                        System.Threading.Thread.Sleep(1000);
                        n.Attributes["value"].InnerText = valor;
                        configXml.Save(ficConfigPrograma);
                        Log.EscribirLog("Cursor");
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
