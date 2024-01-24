using ExcelDataReader;
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
    public partial class UpdateAssembly : Form
    {
        const string repos = @"C:\Users\dzhang\source\repos";
        const string solutions = "Solutions";
        const string projects = "Projects";
        const string name = "Name";
        const string id = "Id";
        const string path = "Path";
        public List<Proyecto> lista = new List<Proyecto>();
        Image si = WindowsFormsApp1.Properties.Resources.comprobado;
        Image no = WindowsFormsApp1.Properties.Resources.comprobado;
        DataSet datosExcel;
        public UpdateAssembly()
        {
            InitializeComponent();
            dataGridView1.Columns[0].DataPropertyName = "Carpeta";
            dataGridView1.Columns[1].DataPropertyName = "Encontrado";
            dataGridView1.Columns[2].DataPropertyName = "Version";
            dataGridView1.Columns[3].DataPropertyName = "NuevaVersion";
        }

        private void Form_Load(object sender, EventArgs e)
        {
            IExcelDataReader excelReader = null;
            Stream stream;
            string storePath = Path.Combine(Application.StartupPath, "resources", "Assembly.xlsx");
            string fileExtension = Path.GetExtension(storePath);
            if (File.Exists(storePath))
            {
                stream = File.Open(storePath, FileMode.Open, FileAccess.Read);
            }
            else
            {
                stream = new StreamReader(new MemoryStream(Properties.Resources.Assembly)).BaseStream;
            }
            if (fileExtension == ".xls")
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else if (fileExtension == ".xlsx")
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }
            datosExcel = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });
            comboBox1.DataSource = datosExcel.Tables[solutions];
            comboBox1.DisplayMember = name;
            comboBox1.ValueMember = id;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Solution solutionRow = (comboBox1.SelectedItem as DataRowView).Row.ToObject<Solution>();
            string solutionPath;
            if(Convert.ToInt16(solutionRow.Id) == 0)
            {
                solutionPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(solutionRow.Path).FullName).FullName).FullName).FullName;
            }
            else
            {
                solutionPath = Path.Combine(repos, Convert.ToString(solutionRow.Path));
            }

            checkBox1.Checked = Convert.ToBoolean(solutionRow.Reset);
            lista.Clear();
            foreach (Project p in datosExcel.Tables[projects].Select("Id = " + solutionRow.Id).ToList<Project>())
            {
                Proyecto proyecto = new Proyecto();
                proyecto.Encontrado = no;
                string projectPath = Path.Combine(solutionPath, p.Path);
                if (Directory.Exists(projectPath))
                {
                    string carpeta = projectPath;
                    foreach (string subdir in Directory.GetDirectories(carpeta))
                    {
                        if (new FileInfo(subdir).Name.Equals("Properties"))
                        {
                            proyecto.Carpeta = new FileInfo(carpeta).Name;
                            proyecto.Encontrado = si;
                            foreach (string file in Directory.GetFiles(subdir))
                            {
                                if (new FileInfo(file).Name.Equals("AssemblyInfo.cs"))
                                {
                                    proyecto.Fichero = file;
                                    //string tmp = file + ".tmp";
                                    //if (File.Exists(tmp))
                                    //File.Delete(tmp);
                                    //File.Move(file, tmp);
                                    string[] lines = File.ReadAllLines(file);
                                    //string[] newlines = new string[lines.Length];
                                    int i = 0;
                                    foreach (string line in lines)
                                    {
                                        if (line.Contains("assembly: AssemblyVersion") || line.Contains("assembly: AssemblyFileVersion"))
                                        {
                                            proyecto.Version = line.Split('"')[1];
                                            proyecto.Nuevaversion = SubirVersion(proyecto.Version);
                                            //newlines[i] = line.Replace(proyecto.Version, proyecto.Nuevaversion);
                                        }
                                        //else
                                        //newlines[i] = line;
                                        i++;
                                    }
                                    //File.WriteAllLines(file, newlines);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (proyecto.Encontrado == no)
                    {
                        foreach (string file in Directory.GetFiles(carpeta))
                        {
                            if (new FileInfo(file).Name.Equals("AssemblyInfo.cs"))
                            {
                                proyecto.Carpeta = new FileInfo(carpeta).Name;
                                proyecto.Encontrado = si;
                                proyecto.Fichero = file;
                                //string tmp = file + ".tmp";
                                //if (File.Exists(tmp))
                                //File.Delete(tmp);
                                //File.Move(file, tmp);
                                string[] lines = File.ReadAllLines(file);
                                //string[] newlines = new string[lines.Length];
                                int i = 0;
                                foreach (string line in lines)
                                {
                                    if (line.Contains("assembly: AssemblyVersion") || line.Contains("assembly: AssemblyFileVersion"))
                                    {
                                        proyecto.Version = line.Split('"')[1];
                                        proyecto.Nuevaversion = SubirVersion(proyecto.Version);
                                        //newlines[i] = line.Replace(proyecto.Version, proyecto.Nuevaversion);
                                    }
                                    //else
                                    //newlines[i] = line;
                                    i++;
                                }
                                //File.WriteAllLines(file, newlines);
                            }
                        }
                        break;
                    }
                }
                lista.Add(proyecto);
            }
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = null;
            dataGridView1.RowCount = lista.Count();
            dataGridView1.DataSource = lista;
            dataGridView1.Refresh();
            if (lista.Select(x => x.Version).AreAllSame())
            {
                textBox1.Text = lista.First().Nuevaversion;
                this.button2.Enabled = true;
            }
        }

        private string SubirVersion(string version)
        {
            string nuevaversion = version;
            string[] versiones = version.Split('.');
            int i = versiones.Length;
            if (versiones.All(x => int.TryParse(x, out int num)))
            {
                if (radioButton2.Checked)
                {
                    int revision = Convert.ToInt16(versiones.Last().Trim());
                    if (checkBox1.Checked && revision >= 26)
                    {
                        revision = 1;
                        int build = Convert.ToInt16(versiones[i - 2].Trim());
                        build++;
                        versiones[i - 2] = build.ToString();
                    }
                    else
                        revision++;
                    versiones[i - 1] = revision.ToString();
                }
                else
                {
                    int build = Convert.ToInt16(versiones[i - 2].Trim());
                    build++;
                    versiones[i - 2] = build.ToString();
                    versiones[i - 1] = 1.ToString();
                }
            }
            nuevaversion = string.Join(".", versiones);

            return nuevaversion;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Process.Start(Path.GetDirectoryName(lista[e.RowIndex].Fichero));
            Process.Start("notepad.exe", lista[e.RowIndex].Fichero); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int total = lista.Count;
            progressBar1.Maximum = total;
            int c = 0;
            int porcentaje = 0;
            button2.Text = "0% (0/" + total +")";
            foreach(Proyecto p in lista)
            {
                string tmp = p.Fichero + ".tmp";
                if (File.Exists(tmp))
                File.Delete(tmp);
                File.Move(p.Fichero, tmp);
                string[] lines = File.ReadAllLines(tmp);
                string[] newlines = new string[lines.Length];
                int i = 0;
                foreach (string line in lines)
                {
                    if (line.Contains("assembly: AssemblyVersion") || line.Contains("assembly: AssemblyFileVersion"))
                    {
                        newlines[i] = line.Replace(p.Version, p.Nuevaversion);
                    }
                    else
                    newlines[i] = line;
                    i++;
                }
                File.WriteAllLines(p.Fichero, newlines, Encoding.UTF8);
                c++;
                progressBar1.Value = c;
                progressBar1.Update();
                porcentaje = c / total * 100;
                button2.Text = porcentaje +"% ("+c+"/" + total + ")";
                Application.DoEvents();
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
                return;
            if(radioButton1.Checked)
            {
                lista.ForEach(x => x.Nuevaversion = SubirVersion(x.Version));
            }
            else if(radioButton2.Checked)
            {
                lista.ForEach(x => x.Nuevaversion = SubirVersion(x.Version));
            }
            else
            {
                lista.ForEach(x => x.Nuevaversion = textBox1.Text);
            }
            if (lista.Select(x => x.Version).AreAllSame())
            {
                textBox1.Text = lista.First().Nuevaversion;
                this.button2.Enabled = true;
            }
            dataGridView1.Refresh();
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

    public class Solution
    {
        private int id;
        private string name;
        private string path;
        private int reset;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Path { get => path; set => path = value; }
        public int Reset { get => reset; set => reset = value; }
    }

    public class Project
    {
        private int id;
        private string name;
        private string path;

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Path { get => path; set => path = value; }
    }
}
