using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Assembly : Form
    {
        const string repos = @"C:\Users\dzhang\source\repos";
        public List<Proyecto> lista = new List<Proyecto>();
        Image si = WindowsFormsApp1.Properties.Resources.comprobado;
        Image no = WindowsFormsApp1.Properties.Resources.comprobado;

        public Assembly()
        {
            InitializeComponent();
            dataGridView1.Columns[0].DataPropertyName = "Carpeta";
            dataGridView1.Columns[1].DataPropertyName = "Encontrado";
            dataGridView1.Columns[2].DataPropertyName = "Version";
            dataGridView1.Columns[3].DataPropertyName = "NuevaVersion";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.SelectedPath = repos;
            fd.ShowNewFolderButton = false;
            if (fd.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fd.SelectedPath))
            {
                string carpeta = fd.SelectedPath;
                lista.Clear();
                foreach (string dir in Directory.GetDirectories(carpeta))
                {
                    Proyecto proyecto = new Proyecto();
                    proyecto.Carpeta = new FileInfo(dir).Name;
                    lista.Add(proyecto);
                    foreach (string subdir in Directory.GetDirectories(dir))
                    {
                        if (new FileInfo(subdir).Name.Equals("Properties"))
                        {
                            proyecto.Encontrado = si;
                            foreach (string file in Directory.GetFiles(subdir))
                            {
                                if (new FileInfo(file).Name.Equals("AssemblyInfo.cs"))
                                {
                                    proyecto.Fichero = file;
                                    string tmp = file + ".tmp";
                                    if (File.Exists(tmp))
                                        File.Delete(tmp);
                                    File.Move(file, tmp);
                                    string[] lines = File.ReadAllLines(tmp);
                                    string[] newlines = new string[lines.Length];
                                    int i = 0;
                                    foreach (string line in lines)
                                    {
                                        if (line.Contains("assembly: AssemblyVersion") || line.Contains("assembly: AssemblyFileVersion"))
                                        {
                                            proyecto.Version = line.Split('"')[1];
                                            proyecto.Nuevaversion = SubirVersion(proyecto.Version);
                                            newlines[i] = line.Replace(proyecto.Version, proyecto.Nuevaversion);
                                        }
                                        else
                                            newlines[i] = line;
                                        i++;
                                    }
                                    File.WriteAllLines(file, newlines);
                                }
                            }
                            break;
                        }
                    }
                }
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = lista;
                dataGridView1.Refresh();
            }
        }

        private string SubirVersion(string version)
        {
            string nuevaversion = version;
            string[] versiones = version.Split('.');
            int i = versiones.Length;
            if (versiones.All(x => int.TryParse(x, out int num)))
            {
                int revision = Convert.ToInt16(versiones.Last().Trim());
                if (revision >= 26)
                {
                    revision = 1;
                    int build = Convert.ToInt16(versiones[i - 2].Trim());
                    build++;
                    versiones[i - 2] = build.ToString();
                }
                else
                    revision++;
                versiones[i - 1] = revision.ToString();
                nuevaversion = string.Join(".", versiones);
            }
            return nuevaversion;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(lista[e.RowIndex].Fichero));
            Process.Start("notepad.exe", lista[e.RowIndex].Fichero); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach(Proyecto p in lista)
            {
                if (!string.IsNullOrEmpty(p.Fichero))
                    File.Delete(p.Fichero + ".tmp");
            }
        }
    }

    public class Proyecto
    {
        private string carpeta;
        private Image encontrado;
        private string version;
        private string nuevaversion;
        private string fichero;

        public string Carpeta { get => carpeta; set => carpeta = value; }
        public Image Encontrado { get => encontrado; set => encontrado = value; }
        public string Version { get => version; set => version = value; }
        public string Nuevaversion { get => nuevaversion; set => nuevaversion = value; }
        public string Fichero { get => fichero; set => fichero = value; }
    }
}
