using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using RestSharp;
using System.Net.Http.Formatting;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Management;

namespace WindowsFormsApp1
{
    public partial class Tiempos : Form
    {
        List<IntPtr> lh = new List<IntPtr>();
        public enum aaa
        {
            a = 1
        }

        public enum bbb
        {
            b = 1,
            c = 2
        }

        public Tiempos()
        {
            InitializeComponent();

        }

        private void c()
        {
            try
            {
                foreach (string test in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Basurilla\CS Lentitud Carga\188A\Usuario"))
                {
                    if (!test.Contains("5"))
                        continue;
                    foreach (string num in Directory.GetDirectories(test))
                    {
                        foreach (string file in Directory.GetFiles(test))
                        {
                            if (file.Contains("Hilo"))
                                continue;
                            System.IO.File.Move(file, file.Replace(".txt", "Hilo.txt"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                textBox4.Text = "Mensaje: " + ex.Message + Environment.NewLine;
                textBox4.Text += "Stack: " + ex.StackTrace;
            }
        }

        private void d()
        {
            string file = @"C:\Users\dzhang\source\repos\BalanzaPC\BalanzaPC\Mantenimientos\frmUsuario.Designer.cs";
            string file2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Designer.txt";
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                StreamWriter sw = new StreamWriter(file2);
                foreach (string linea in File.ReadLines(file))
                {
                    if (linea.Contains("Location") || linea.Contains("Size") || linea.Contains("Font"))
                        sw.WriteLine(linea);
                }
                sw.Flush();
                sw.Close();
            }
            else
            {
                foreach (string linea in File.ReadLines(file))
                {
                    if (linea.Contains($"{textBox2.Text}.Location") || linea.Contains($"{textBox2.Text}.Size"))
                        textBox4.Text += linea + Environment.NewLine;
                }
            }
            MessageBox.Show("Fin");
        }
        
        private void f()
        {
            string file = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Ventas.log";
            string file2 = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Ventas.txt";
            StreamWriter sw = new StreamWriter(file2);
            foreach (string linea in File.ReadLines(file))
            {
                if (linea.Contains("HV"))
                    sw.WriteLine(linea.Split(':').Last().Trim());
            }
            sw.Flush();
            sw.Close();
        }

        private void b()
        {
            int total = 0;
            int contador = 0;
            string lineaanterior = string.Empty;
            int tiempoanterior = 0;
            textBox4.Text = string.Empty;
            textBox4.Text += textBox2.Text + "=>" + textBox3.Text + Environment.NewLine;
            try
            {
                foreach (string test in Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Basurilla\CS Lentitud Carga\Refresh\Test1"))
                {
                    //foreach (string num in Directory.GetDirectories(test))
                    //{
                        foreach (string file in Directory.GetFiles(test))
                        {
                            if (file.Contains(textBox2.Text))
                                foreach (string linea in File.ReadLines(file))
                                {
                                    if (linea.Contains(textBox3.Text))
                                    {
                                        string tiempo = Regex.Match(linea, @"\d+").Value;
                                        tiempoanterior = Convert.ToInt32(Regex.Match(lineaanterior, @"\d+").Value);
                                        if (checkBox1.Checked)
                                        {
                                            textBox4.Text += Path.GetDirectoryName(file) + "=>" + (Convert.ToInt32(tiempo) - tiempoanterior) + " ms" + Environment.NewLine;
                                            total += Convert.ToInt32(tiempo) - tiempoanterior;
                                        }
                                        else
                                        {
                                            textBox4.Text += Path.GetDirectoryName(file) + "=>" + (tiempo) + " ms" + Environment.NewLine;
                                            total += Convert.ToInt32(tiempo);
                                        }
                                        contador++;
                                    }
                                    lineaanterior = linea;
                                }
                        }
                    //}
                }
                textBox4.Text += "Total => " + total + " ms" + Environment.NewLine;
                textBox4.Text += "Media => " + total / contador + " ms";
            }
            catch(Exception ex)
            {
                textBox4.Text = "Mensaje: " + ex.Message + Environment.NewLine;
                textBox4.Text += "Stack: " + ex.StackTrace;
            }
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

        private void CheckTeamViewer()
        {
            List<Process> lp = new List<Process>();
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.Equals("TeamViewer"))
                    lp.Add(p);
                p.Refresh();
            }
            Thread.Sleep(1);
            int c = WinUtil.GetWindowCount(lp.First().Id, out List<IntPtr> l);
            textBox4.Text += c.ToString() + Environment.NewLine;
            textBox4.Text += lp.First().HandleCount.ToString() + Environment.NewLine;
            textBox4.Text += lp.First().MainWindowHandle.ToString() + Environment.NewLine;
            textBox4.Text += lp.First().Handle.ToString() + Environment.NewLine;
            textBox4.Text += lp.First().SafeHandle.ToString() + Environment.NewLine;
            lh.Add(lp.First().MainWindowHandle);
            lh = lh.Distinct().ToList<IntPtr>();
            if (c>1)
            {
                foreach(IntPtr i in l)
                {
                    textBox4.Text += $"{lp.First().MainWindowHandle} == {i}" + Environment.NewLine;

                    if (!lh.Contains(i))
                        WinUtil.CloseWindow(i);
                }
            }
            lh = lh.Intersect(l).ToList<IntPtr>();
            foreach (ProcessThread t in lp.First().Threads)
            {
                t.Dispose();
            }
        }

        private void Drive()
        {
            var searcher = new ManagementObjectSearcher(
    "root\\CIMV2",
    "SELECT * FROM Win32_MappedLogicalDisk");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Win32_MappedLogicalDisk instance");
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("Access: {0}", queryObj["Access"]);
                Console.WriteLine("Availability: {0}", queryObj["Availability"]);
                Console.WriteLine("BlockSize: {0}", queryObj["BlockSize"]);
                Console.WriteLine("Caption: {0}", queryObj["Caption"]);
                Console.WriteLine("Compressed: {0}", queryObj["Compressed"]);
                Console.WriteLine("ConfigManagerErrorCode: {0}", queryObj["ConfigManagerErrorCode"]);
                Console.WriteLine("ConfigManagerUserConfig: {0}", queryObj["ConfigManagerUserConfig"]);
                Console.WriteLine("CreationClassName: {0}", queryObj["CreationClassName"]);
                Console.WriteLine("Description: {0}", queryObj["Description"]);
                Console.WriteLine("DeviceID: {0}", queryObj["DeviceID"]);
                Console.WriteLine("ErrorCleared: {0}", queryObj["ErrorCleared"]);
                Console.WriteLine("ErrorDescription: {0}", queryObj["ErrorDescription"]);
                Console.WriteLine("ErrorMethodology: {0}", queryObj["ErrorMethodology"]);
                Console.WriteLine("FileSystem: {0}", queryObj["FileSystem"]);
                Console.WriteLine("FreeSpace: {0}", queryObj["FreeSpace"]);
                Console.WriteLine("InstallDate: {0}", queryObj["InstallDate"]);
                Console.WriteLine("LastErrorCode: {0}", queryObj["LastErrorCode"]);
                Console.WriteLine("MaximumComponentLength: {0}", queryObj["MaximumComponentLength"]);
                Console.WriteLine("Name: {0}", queryObj["Name"]);
                Console.WriteLine("NumberOfBlocks: {0}", queryObj["NumberOfBlocks"]);
                Console.WriteLine("PNPDeviceID: {0}", queryObj["PNPDeviceID"]);

                if (queryObj["PowerManagementCapabilities"] == null)
                    Console.WriteLine("PowerManagementCapabilities: {0}", queryObj["PowerManagementCapabilities"]);
                else
                {
                    UInt16[] arrPowerManagementCapabilities = (UInt16[])(queryObj["PowerManagementCapabilities"]);
                    foreach (UInt16 arrValue in arrPowerManagementCapabilities)
                    {
                        Console.WriteLine("PowerManagementCapabilities: {0}", arrValue);
                    }
                }
                Console.WriteLine("PowerManagementSupported: {0}", queryObj["PowerManagementSupported"]);
                Console.WriteLine("ProviderName: {0}", queryObj["ProviderName"]);
                Console.WriteLine("Purpose: {0}", queryObj["Purpose"]);
                Console.WriteLine("QuotasDisabled: {0}", queryObj["QuotasDisabled"]);
                Console.WriteLine("QuotasIncomplete: {0}", queryObj["QuotasIncomplete"]);
                Console.WriteLine("QuotasRebuilding: {0}", queryObj["QuotasRebuilding"]);
                Console.WriteLine("SessionID: {0}", queryObj["SessionID"]);
                Console.WriteLine("Size: {0}", queryObj["Size"]);
                Console.WriteLine("Status: {0}", queryObj["Status"]);
                Console.WriteLine("StatusInfo: {0}", queryObj["StatusInfo"]);
                Console.WriteLine("SupportsDiskQuotas: {0}", queryObj["SupportsDiskQuotas"]);
                Console.WriteLine("SupportsFileBasedCompression: {0}", queryObj["SupportsFileBasedCompression"]);
                Console.WriteLine("SystemCreationClassName: {0}", queryObj["SystemCreationClassName"]);
                Console.WriteLine("SystemName: {0}", queryObj["SystemName"]);
                Console.WriteLine("VolumeName: {0}", queryObj["VolumeName"]);
                Console.WriteLine("VolumeSerialNumber: {0}", queryObj["VolumeSerialNumber"]);

            }
        }

        private async void Service()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
            using (FileStream f = File.Create(@"C:\SendFile.zip"))
            {
                HttpWebRequest requ = HttpWebRequest.CreateHttp("http://localhost:8888/files/send?file=3");
            requ.Timeout = 1000000;
            requ.AllowReadStreamBuffering = false;
                requ.KeepAlive = false;
                requ.ReadWriteTimeout = 1000000;
            var res = (HttpWebResponse)requ.GetResponse();
            using (Stream s = res.GetResponseStream())
            {

                    s.CopyTo(f);

            }
            using (Stream s = res.GetResponseStream())
            {
                    s.CopyTo(f);
            }
            }
            ZipFile.CreateFromDirectory(@"\\192.168.151.81\c$\SW1100\PicturesDibal\GALERIA\PLU", @"\\192.168.151.81\c$\SW1100\PicturesDibal\GALERIA\PLU.zip");
            Directory.Move(@"\\192.168.151.81\c$\SW1100\PicturesDibal\GALERIA\PLU", @"C:\SW1100\PicturesDibal\GALERIA\PLU");
            byte[] b = new byte[128859748];
            var a = MediaTypeFormatter.GetDefaultValueForType(typeof(Image));
            HttpClient client = new HttpClient();
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, "http://192.168.150.68:8888/files/send?file=3");
            HttpResponseMessage mess = await client.SendAsync(req);
            byte[] bytes = await mess.Content.ReadAsByteArrayAsync();
            var cliente = new RestClient("http://192.168.150.68:8888/files/send?file=3");
            var request = new RestRequest("http://192.168.150.68:8888/files/send?file=3", Method.Get);
            RestResponse response = cliente.Execute(request);

        }

        private void QR()
        {
            string s = "{A|532|YUCA|1|84|0,85|3|1|0|15|2|}";
            string[] texto = s.Split(new char[2] { '\r', '\n' }, StringSplitOptions.None);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QR();
            return;
            Drive();
            return;
            try
            {
                Service();
            }
            catch(Exception ex)
            {

            }
            return;
            b();
            return;
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

        private void button2_Click(object sender, EventArgs e)
        {
            c();
            return;
        }

    }
}
