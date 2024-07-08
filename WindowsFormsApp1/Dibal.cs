using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.Xml;
using ExcelDataReader;
using System.Data;
using System.Security.Cryptography;
using System.Threading;
using System.Management;
using System.Xml.Schema;
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public static class Dibal
    {
        private static UInt32 _ip;
        private static UInt32 _mask;
        public static List<string> ag;
        public static List<string> df;
        private static Int32 claveCliente_E = 0;
        private static Int32 claveCliente_N = 0;
        private static Int32 claveCliente_D = 0;
        private static Int32 claveServidor_E = 0;
        private static Int32 claveServidor_N = 0;


        public static void IPSegment(string ip, string mask)
        {
            _ip = ip.ParseIp();
            _mask = mask.ParseIp();
        }

        public static UInt32 NumberOfHosts
        {
            get { return ~_mask + 1; }
        }

        public static UInt32 NetworkAddress
        {
            get { return _ip & _mask; }
        }

        public static UInt32 BroadcastAddress
        {
            get { return NetworkAddress + ~_mask; }
        }

        public static IEnumerable<UInt32> Hosts()
        {
            for (var host = NetworkAddress + 1; host < BroadcastAddress; host++)
            {
                yield return host;
            }
        }

        private static string ToIpString(this UInt32 value)
        {
            var bitmask = 0xff000000;
            var parts = new string[4];
            for (var i = 0; i < 4; i++)
            {
                var masked = (value & bitmask) >> ((3 - i) * 8);
                bitmask >>= 8;
                parts[i] = masked.ToString(CultureInfo.InvariantCulture);
            }
            return String.Join(".", parts);
        }

        private static UInt32 ParseIp(this string ipAddress)
        {
            var splitted = ipAddress.Split('.');
            UInt32 ip = 0;
            for (var i = 0; i < 4; i++)
            {
                ip = (ip << 8) + UInt32.Parse(splitted[i]);
            }
            return ip;
        }

        public static ObservableCollection<string> IPList;

        [Flags]
        private enum ProcessAccessFlags : uint
        {
            QueryLimitedInformation = 0x00001000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool QueryFullProcessImageName(
              [In] IntPtr hProcess,
              [In] int dwFlags,
              [Out] StringBuilder lpExeName,
              ref int lpdwSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(
         ProcessAccessFlags processAccess,
         bool bInheritHandle,
         int processId);

        static String GetProcessFilename(Process p)
        {
            int capacity = 2000;
            StringBuilder builder = new StringBuilder(capacity);
            IntPtr ptr = OpenProcess(ProcessAccessFlags.QueryLimitedInformation, false, p.Id);
            if (!QueryFullProcessImageName(ptr, 0, builder, ref capacity))
            {
                return String.Empty;
            }

            return builder.ToString();
        }
        public static void CheckSolution()
        {
            Process[] pp = Process.GetProcesses();
            string[] ss = pp.Select(z => GetProcessFilename(z)).ToArray();
            Process[] pp2 = Process.GetProcessesByName("Microsoft Visual Studio 2017");
            foreach (Process p in pp)
            {
                if(GetProcessFilename(p).Contains("Balanza") || GetProcessFilename(p).Contains("Visual Studio") || GetProcessFilename(p).Contains("VS"))
                {

                }
            }
        }

        public static void EST()
        {
#if ARGS
                        return;
#endif
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

        public static void Ping()
        {
            string web = "http://192.168.0.{0}/edit.html";
            foreach(uint host in Hosts())
            {
                var ping = new Ping();
                string shost = host.ToIpString();
                if (ping.Send(shost, 3000).Status == IPStatus.Success)
                {
                    IPList.Add(shost);
                }
                OnPing();
            }
            OnFinish();
        }

        public static void Request()
        {
            string web = "http://192.168.0.{0}/edit.html";
            for (int i = 0; i < 255; i++)
            {
                var ping = new Ping();
                if (ping.Send(new IPAddress(new byte[] { 192, 168, 0, (byte)i }), 3000).Status == IPStatus.Success)
                {
                    WebRequest request = WebRequest.Create(string.Format(web, i));
                    request.Credentials = CredentialCache.DefaultCredentials;
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Stream dataStream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        Console.WriteLine(responseFromServer);
                        reader.Close();
                        dataStream.Close();
                        response.Close();
                    }
                }
                OnPing();
            }
            OnFinish();
        }

        public static event PingEventHandler PingEvent;
        private static void OnPing()
        {
            PingEvent?.Invoke(typeof(Dibal), EventArgs.Empty);
        }
        public delegate void PingEventHandler(object sender, EventArgs e);

        public static event FinishEventHandler FinishEvent;
        private static void OnFinish()
        {
            FinishEvent?.Invoke(typeof(Dibal), EventArgs.Empty);
        }
        public delegate void FinishEventHandler(object sender, EventArgs e);

        public static void SFTP()
        {

            try
            {var host = "sftp_server_address.com";
                var port = 999;
                var username = "your_username";
                var passphrase = "your_passphrase";
                var privateKeyLocalFilePath = @"your_localpath\...\PrivateKeyOpenSSH.ppk";

                var remoteFolderPath = "/";
                var localFolferPath = @"C:\locafiles\";

                var key = File.ReadAllText(privateKeyLocalFilePath);
                var buf = new MemoryStream(Encoding.UTF8.GetBytes(key));
                var privateKeyFile = new PrivateKeyFile(buf, passphrase);
                var connectionInfo = new ConnectionInfo(host, port, username,
                    new PrivateKeyAuthenticationMethod(username, privateKeyFile));
                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();
                    var files = client.ListDirectory(remoteFolderPath);
                    foreach (var file in files)
                    {
                        Console.WriteLine(file);
                        using (var targetFile = new FileStream(Path.Combine(localFolferPath, file.Name), FileMode.Create))
                        {
                            client.DownloadFile(file.FullName, targetFile);
                            targetFile.Close();
                        }
                    }
                    client.Disconnect();
                }
            }
            catch (Exception e) when (e is SshConnectionException || e is SocketException || e is ProxyException)
            {
                Console.WriteLine($"Error connecting to server: {e.Message}");
            }
            catch (SshAuthenticationException e)
            {
                Console.WriteLine($"Failed to authenticate: {e.Message}");
            }
            catch (SftpPermissionDeniedException e)
            {
                Console.WriteLine($"Operation denied by the server: {e.Message}");
            }
            catch (SshException e)
            {
                Console.WriteLine($"Sftp Error: {e.Message}");
            }
        }

        public static void XML()
        {
            string ruta = "C:\\SW1100\\BalanzaPC\\Config\\scale_config.xml";
            if (File.Exists(ruta))
            {
                System.Xml.XmlNode n;
                System.Xml.XmlDocument config = new System.Xml.XmlDocument();
                config.Load(ruta);
                n = config.SelectSingleNode("descendant::modoTrabajo");
                string modoTrabajo = n.InnerText;
            }

            XmlDocument writer = new XmlDocument();
            XmlDeclaration documentType = writer.CreateXmlDeclaration("1.0", "utf-8", null);
            writer.AppendChild(documentType);

            XmlElement root = writer.CreateElement("PatientFile");
            writer.AppendChild(root);
        }

        public static string Password(string contraseña)
        {
            MD5CryptoServiceProvider md5;
            Byte[] TextoEnBytes;
            Byte[] HashEnBytes;

            // Create New Crypto Service Provider Object
            md5 = new MD5CryptoServiceProvider();

            //Convert the original string to array of Bytes
            TextoEnBytes = System.Text.Encoding.UTF8.GetBytes(contraseña);

            //Compute the Hash, returns an array of Bytes
            HashEnBytes = md5.ComputeHash(TextoEnBytes);

            md5.Clear();

            // Return a base 64 encoded string of the Hash value
            string passwordEncripatada = (Convert.ToBase64String(HashEnBytes));

            return passwordEncripatada;
        }

        static void printAllKLength(char[] set, int k)
        {
            int n = set.Length;
            printAllKLengthRec(set, "", n, k);
        }

        // The main recursive method
        // to print all possible 
        // strings of length k
        static void printAllKLengthRec(char[] set,
                                       String prefix,
                                       int n, int k)
        {

            // Base case: k is 0,
            // print prefix
            if (k == 0)
            {
                Console.WriteLine(prefix);
                if (Password(prefix) == "U+Nt5NAz6aPz/SD5ntgCXg==")
                    MessageBox.Show(prefix + "=>" + Password(prefix));
                return;
            }

            // One by one add all characters 
            // from set and recursively 
            // call for k equals to k-1
            for (int i = 0; i < n; ++i)
            {
                // Next character of input added
                String newPrefix = prefix + set[i];
                if (string.IsNullOrEmpty(prefix))
                {
                    Thread t = new Thread(() => printAllKLengthRec(set, newPrefix,
                                        n, k - 1));
                    t.Start();
                }
                else
                {
                    // k is decreased, because 
                    // we have added a new character
                    printAllKLengthRec(set, newPrefix,
                                            n, k - 1);
                }
            }
        }

        public static void CracK()
        {
            char[] set2 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToArray();
            for (int i = 11; i>0; i--)
            {
                printAllKLength(set2, i);
            }
        }

        public static void Codepages()
        {
            var codepages = Encoding.GetEncodings().Select(x => x.GetEncoding()).ToList();
            var names = codepages.Select(x => x.BodyName).ToList();
            var ids = codepages.Select(x => x.CodePage).ToList();
            List<string> a = new List<string>();
            List<string> b = new List<string>();
            StringBuilder sasdfao = new StringBuilder();
            for (int i =0; i<256;i++)
            {
                if(i==127)
                {

                }
                byte[] dsaf = BitConverter.GetBytes(i);
                //a.Add(Encoding.GetEncoding(1252).GetString(dsaf));
                //b.Add(Encoding.UTF8.GetString(dsaf));
                a.Add(Encoding.GetEncoding(1252).GetString(Encoding.UTF8.GetBytes(Encoding.GetEncoding(1252).GetString(dsaf))));
                b.Add(Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Encoding.GetEncoding(1252).GetString(dsaf))));
                string sda = Convert.ToString(Convert.ToChar(i));
                byte[] afsdf = new byte[1];
                Array.Copy(dsaf, afsdf, 1);
                sasdfao.Append(Encoding.GetEncoding(1251).GetString(afsdf));
                if(!Encoding.GetEncoding(1252).GetBytes(sda).SequenceEqual(Encoding.UTF8.GetBytes(sda)))
                {
                    string asdag = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(sda));
                    string addsfgs= Encoding.GetEncoding(1252).GetString(Encoding.UTF8.GetBytes(sda));
                    byte[] sadfhdsl = Encoding.GetEncoding(1252).GetBytes(sda);
                    byte[] gaogiu = Encoding.UTF8.GetBytes(sda);
                }
            }
            string dasfkhbs = sasdfao.ToString();
            Clipboard.SetText(dasfkhbs, TextDataFormat.Text);

            ag = a;
            df = b;
            List<string> c = a.Except(b).ToList();
            Coding();
        }

        public static void Coding()
        {
            string path = @"T:\17-RegistroPruebas\231204.UX ARQ.CS.STD.058.23002.23002.052\05.-Used Files\5.- Any Other\CU_Codepages\1256\art_Win1256.txt";
            StreamReader sr = new StreamReader(path, Encoding.GetEncoding("ISO-8859-6"));
            string sdfh = sr.ReadToEnd();
            string c = Encoding.UTF8.GetString(Encoding.GetEncoding(1252).GetBytes("Ñ"));
            c = Encoding.GetEncoding(1252).GetString(Encoding.UTF8.GetBytes("Ñ"));
            byte[] b1 = Encoding.UTF8.GetBytes("ñ");
            MemoryStream stream = new MemoryStream(b1);
            string s2 = new StreamReader(stream, System.Text.Encoding.GetEncoding(1252)).ReadLine();
        }

        public static void Endian()
        {
            Int16 fahlksdfh = 1;
            SByte aihfpos = (SByte)fahlksdfh;
            if(aihfpos == 1)
            {

            }
        }
        
        public static void Import()
        {
            string path = Path.Combine(@"C:\Users\DIBAL\articulo", "ArticlesDZH1252.xlsx");
            IExcelDataReader excelReader = null;
            string fileExtension = Path.GetExtension(path);
            byte [] adfs = File.ReadAllBytes(path);
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                if (fileExtension == ".xls")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileExtension == ".xlsx")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                System.Data.DataSet datosExcel = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true,
                        
                    }
                });
                path = Path.Combine(@"C:\Users\DIBAL\articulo", "ArticlesDZH65001.txt");
                foreach (DataRow dr in datosExcel.Tables[0].Rows)
                {
                    List<string> ls = dr.ItemArray.Select(x=> Convert.ToString(x)).ToList();
                    string g = string.Join(";", ls);
                    using (StreamWriter sw = new StreamWriter(path, true, new UTF8Encoding(false)))
                    {
                        sw.WriteLine(g);
                    }
                }
                path = Path.Combine(@"C:\Users\DIBAL\articulo", "ArticlesDZH1252.txt");
                foreach (DataRow dr in datosExcel.Tables[0].Rows)
                {
                    List<string> ls = dr.ItemArray.Select(x => Convert.ToString(x)).ToList();
                    string g = string.Join(";", ls);
                    using (StreamWriter sw = new StreamWriter(path, true, Encoding.GetEncoding(1252)))
                    {
                        sw.WriteLine(g);
                    }
                }
                path = Path.Combine(@"C:\Users\DIBAL\articulo", "ArticlesDZHBytes.txt");
                File.WriteAllText(path, Encoding.GetEncoding(1252).GetString(adfs), Encoding.GetEncoding(1252));
            }
        }

        public static void DateTimeMySQL()
        {
            DataSet dsTarifa = MySQL.EjecutaQuery(MySQL.Connection,"Select * from dat_tarifa");
            object o = dsTarifa.Tables[0].Rows[100]["FechaInicio"];
            string s = dsTarifa.Tables[0].Rows[100]["FechaInicio"].ToString();
            string sql = "Update dat_tarifa set FechaInicio = ?FechaInicio;";
            IDbCommand cmd = MySQL.CreateCommand(MySQL.Connection, sql);
            cmd.CommandText = sql;
            DateTime.TryParse(dsTarifa.Tables[0].Rows[0]["FechaInicio"].ToString(), out DateTime dFechaInicio);
            MySQL.AddParametro(cmd, "FechaInicio", dsTarifa.Tables[0].Rows[0]["FechaInicio"].ToString());
            cmd.ExecuteNonQuery();
        }

        public static void Reorder()
        {
            DataSet ds = MySQL.EjecutaQuery(MySQL.Connection ,"SELECT idarticulo, logopantalla FROM dat_articulo d where idseccion = 1 order by idseccion, idarticulo");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string logopantalla = NuevoLogoPantalla(dr["logopantalla"].ToString());
                string sql = "Update dat_articulo set tecladirecta = " + dr["idarticulo"].ToString() + " where idarticulo = " + dr["idarticulo"].ToString();
                MySQL.EjecutaNonQuery(sql);
            }
        }

        private static string NuevoLogoPantalla(string logopantalla)
        {
            string logo = logopantalla;
            logo = logo.Replace(@"\", @"\\");
            /*logo = logo.Replace("1_Ave", "6_Ave");
            logo = logo.Replace("2_Bebidas", "15_Bebidas");
            logo = logo.Replace("3_Cerdo", "7_Cerdo");
            logo = logo.Replace("4_Charcu", "4_Charcu");
            logo = logo.Replace("5_Congelados", "14_Congelados");
            logo = logo.Replace("6_Cordero", "8_Cordero");
            logo = logo.Replace("7_Dulces", "13_Dulces");
            logo = logo.Replace("8_Encurtidos", "12_Encurtidos");
            logo = logo.Replace("9_Frutas", "1_Frutas");
            logo = logo.Replace("10_Frutos_secos", "11_Frutos_secos");
            logo = logo.Replace("11_Horno", "10_Horno");
            logo = logo.Replace("12_Pescado", "15_Pescado");
            logo = logo.Replace("13_Queso", "9_Queso");
            logo = logo.Replace("14_Vacuno", "3_Vacuno");
            logo = logo.Replace("15_Verduras", "2_Verduras");
            logo = logo.Replace("15_Pescado", "5_Pescado");*/
            return logo;
        }

        private static void Fiscal()
        {
            Properties.Settings.Default.Fiscal = 1;
        }

        private static void Encrypt()
        {
            Aes aes = System.Security.Cryptography.Aes.Create();
            aes.CreateEncryptor();
        }

        public static void Remote()
        {
            var processToRun = new[] { "notepad.exe" };
            var connection = new ConnectionOptions();
            connection.Username = "CS1200";
            connection.Password = "CS1200";
            var wmiScope = new ManagementScope(String.Format("\\\\{0}\\root\\cimv2", "192.168.150.64"), connection);
            var wmiProcess = new ManagementClass(wmiScope, new ManagementPath("Win32_Process"), new ObjectGetOptions());
            wmiProcess.InvokeMethod("Create", processToRun);
        }

        public static void Blob()
        {
            byte[] img = (byte[])MySQL.EjecutaScalar(MySQL.NewConnection("localhost", "sys_datos_occ"), "Select Bitmap from dat_operacion_contra_cajon where idoperacion = 5846");
            using (var ms = new MemoryStream(img))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                image.Save(Path.Combine(Application.StartupPath, "Result", "image.bmp"));
            }
            new Process() { StartInfo = new ProcessStartInfo(Path.Combine(Application.StartupPath, "Result", "image.bmp")) }.Start();
        }

        public static void Schema()
        {
            DataSet ds = new DataSet();
            ds.ReadXmlSchema(@"C:\Daniel\VS\Test\WindowsFormsApp1\bin\Debug\Result\SuministroInformacion.xsd");
            using (WebClient client = new WebClient())
            {
                string s = client.DownloadString("https://prewww2.aeat.es/static_files/common/internet/dep/aplicaciones/es/aeat/tike/cont/ws/SuministroInformacion.xsd");
                s = client.DownloadString("https://prewww2.aeat.es/static_files/common/internet/dep/aplicaciones/es/aeat/tikeV1.0/cont/ws/SuministroInformacion.xsd");
                using (StreamWriter writer = new StreamWriter(Path.Combine(Application.StartupPath, "Result", "schema.xsd")))
                {
                    writer.Write(s);
                }
            }
            XmlTextReader reader = new XmlTextReader("https://prewww2.aeat.es/static_files/common/internet/dep/aplicaciones/es/aeat/tikeV1.0/cont/ws/SuministroInformacion.xsd");
            XmlSchema schema = XmlSchema.Read(reader, ValidationCallback);
            ds.ReadXml(Path.Combine(Application.StartupPath, "Result", "schema.xsd"), XmlReadMode.ReadSchema);
            ds.ReadXmlSchema(Path.Combine(Application.StartupPath, "Result", "schema.xsd"));
            ds.ReadXmlSchema(@"C:\Users\dzhang\source\repos\dibalticketbai\DibalTB\DibalTB\TicketBai.xsd");
        }

        static void ValidationCallback(object sender, ValidationEventArgs args)
        {
            if (args.Severity == XmlSeverityType.Warning)
                Console.Write("WARNING: ");
            else if (args.Severity == XmlSeverityType.Error)
                Console.Write("ERROR: ");

            Console.WriteLine(args.Message);
        }
        public static void DecryptLicense()
        {
            string a = ManagementLicenses.ModuleAccess.LeerRegistroNoEncriptado("CSW_ADV", "keycode");
            string b = ManagementLicenses.ModuleAccess.LeerRegistroEncriptado("CSW_ADV", "keycode");
            string valor = "EFC42600EB8E1800BDFF0100C19C3200CD6005001C391300466B2B0090F62100DCD11E00";
            Int32[] bufferIn = new Int32[valor.Length];
            byte[] bufferOut = new byte[valor.Length];
            int longOut = 0;

            string originalHex, alteredHex;
            int iNumber;
            int j = 0;
            for (int i = 0; i < valor.Length; i += 8)
            {
                originalHex = valor.Substring(i, 8);
                alteredHex = "";
                alteredHex += originalHex.Substring(6, 2);
                alteredHex += originalHex.Substring(4, 2);
                alteredHex += originalHex.Substring(2, 2);
                alteredHex += originalHex.Substring(0, 2);
                iNumber = Int32.Parse(alteredHex, System.Globalization.NumberStyles.HexNumber);
                bufferIn[j] = iNumber;
                j++;
            }

            WinUtil.Decrypt(bufferIn, bufferIn.GetLength(0), ref bufferOut[0], ref longOut, WinUtil.cryp_E, WinUtil.decryp_D, WinUtil.cryp_N);

            string result = Encoding.ASCII.GetString(bufferOut).Replace("\0", string.Empty);
        }

        public static void MyConvert()
        {
            object value = null;
            value = "NULL";
            int.TryParse(Convert.ToString(value), out int f);
            value = int.MinValue;
            int.TryParse(Convert.ToString(value), out int o);
            if (o == int.MinValue) { }
            object v = Convert.ToInt32(value);
        }

        public static void Multisql()
        {
            object asdg = MySQL.EjecutaQuery(MySQL.Connection, "Select idarticulo from sys_datos_dfs.dat_articulo union select idarticulo from sys_datos.dat_articulo;");
        }

        public enum MODELO_FACTURACION
        {
            NO_ESTABLECIDO = -1,
            NO_FISCAL = 0,
            TBAI = 1,
            FISCAL = 2,
            VERIFACTU = 3
        }

        public static void CheckModelo()
        {
            MODELO_FACTURACION modelo = MODELO_FACTURACION.FISCAL;
            MODELO_FACTURACION modeloaf = MODELO_FACTURACION.FISCAL;
            if(modelo == modeloaf)
            {

            }
        }

        public static string Desencriptar(byte[] mensajeServidor)
        {
            string sMensajeServidor = string.Empty;
            try
            {
                WinUtil.GenerateKeys(ref claveCliente_E, ref claveCliente_N, ref claveCliente_D);
                byte[] bufferOut = new byte[800];
            int longOut = 0;
            WinUtil.Decrypt(mensajeServidor, mensajeServidor.GetLength(0), ref bufferOut[0], ref longOut, claveCliente_E, claveCliente_D, claveCliente_N);
            foreach (byte bt in bufferOut)
                sMensajeServidor += Convert.ToChar(bt);
            return sMensajeServidor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] Encriptar(string mensajeEncriptar)
        {
            try
            {
                WinUtil.GenerateKeys(ref claveCliente_E, ref claveCliente_N, ref claveCliente_D);
                byte[] bufferOut = new byte[800];
            int longOut = 0;
            WinUtil.Encrypt(mensajeEncriptar, ref bufferOut[0], ref longOut, claveServidor_E, claveServidor_N);
            bufferOut = (byte[])ResizeArray(bufferOut, longOut);

            return bufferOut;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static System.Array ResizeArray(System.Array oldArray, int newSize)
        {
            try
            {
                int oldSize = oldArray.Length;
                System.Type elementType = oldArray.GetType().GetElementType();
                System.Array newArray = System.Array.CreateInstance(elementType, newSize);
                int preserveLength = System.Math.Min(oldSize, newSize);
                if (preserveLength > 0)
                    System.Array.Copy(oldArray, newArray, preserveLength);
                return newArray;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ErrorLogs()
        {
            List<string> errors = new List<string>();
        string path = @"C:\Daniel\VS\Test\WindowsFormsApp1\bin\Debug\Result\Logs";
            foreach(DirectoryInfo d in new DirectoryInfo(path).GetDirectories())
            {
                foreach(FileInfo f in d.GetFiles())
                {
                    using (StreamReader sr = new StreamReader(f.FullName))
                    {
                        if(sr.ReadToEnd().ToLower().Contains("error"))
                            errors.Add(f.FullName);
                    }
                }
            }
        }

        public static void Diretory(string path, List<FileInfo> files, bool first = true)
        {
            if(first)
            files = new List<FileInfo>();
            foreach(string s in Directory.GetDirectories(path))
            {
                Diretory(s, files, false);
            }
            foreach(string s in Directory.GetFiles(path))
            {
                files.Add(new FileInfo(s));
            }
            if(first)
            {
                MessageBox.Show(files.Count.ToString());
                Process p = new Process();
                p.StartInfo = new ProcessStartInfo(@"C:\xampp\htdocs\Dibal\DibalTransfer\Actions\DibalUpdateSW\data\device\current\CS1100_GA_164M\CS1100_GA_164M\5-Programa\sw1100_setup.exe", "/f");
                p.Start();
                p.WaitForExit();
                Diretory2(path, files);
            }
        }

        private static void Diretory2(string path, List<FileInfo> files, bool first = true)
        {
            foreach (string s in Directory.GetDirectories(path))
            {
                Diretory2(s, files, false);
            }
            foreach (string s in Directory.GetFiles(path))
            {
                FileInfo fi = new FileInfo(s);
                if (files.Any(f => f.FullName.Equals(fi.FullName) && f.CreationTime.Equals(fi.CreationTime)))
                    files.Remove(new FileInfo(s));
            }
            if (first)
            {
                files.ToFile("C:\\Files.txt");
                MessageBox.Show(files.Count.ToString());
            }
        }

        public static void Remove()
        {
            string s = "VALUES (asdf, adsfa, asdfsd), ";
            MessageBox.Show(s.Remove(s.Length - 2));
        }

        public static void Checksum()
        {
            string resul = "79815872347";
            resul = resul.Substring(resul.Length - 12);
            int Suma = 92;
            int sumared = Convert.ToInt32((Suma + 10) / 10) * 10;
        }

        public static void Baltty()
        {
            string[] lines = File.ReadAllLines(ExtensionMethods.DesktopFile("tx.txt"));
            List<string> newlines = new List<string>();
            foreach(string line in lines)
            {
                if(line.StartsWith("02L"))
                {
                    newlines.Add(line);
                }
            }
            File.WriteAllLines(ExtensionMethods.DesktopFile("tx.txt"), newlines);
        }

        public static void Images()
        {
            Random r = new Random(234234);
            while (true)
            {
                if (File.Exists(@"C:\Users\dzhang\Downloads\Images.jpg"))
                {
                    int name = r.Next(1000000);
                    while (File.Exists(Path.Combine(@"C:\Daniel\Basurilla\Images", name + ".jpg")))
                    {
                        name = r.Next(1000000);
                    }
                    File.Move(@"C:\Users\dzhang\Downloads\Images.jpg", Path.Combine(@"C:\Daniel\Basurilla\Images", name + ".jpg"));
                }
            }
        }

        public static void Uninstall()
        {
                while (true)
                {
                try
                {
                    if (File.Exists(@"\\192.168.150.65\ftp\Update"))
                    {
                        foreach (Process d in Process.GetProcessesByName("AccesoBalanzaPC")) { d.Kill(); }
                        foreach (Process d in Process.GetProcessesByName("RGI")) { d.Kill(); }
                        foreach (Process d in Process.GetProcessesByName("ComunicacionesBalPCInterface")) { d.Kill(); }

                        string name = Convert.ToString(Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\SW1100").GetValue("Version"));
                        if (!string.IsNullOrEmpty(name))
                        {
                            Process p2 = new Process();
                            ProcessStartInfo psi2 = new ProcessStartInfo("cmd.exe", $"/C wmic product where name='{name}' call uninstall");
                            p2.StartInfo = psi2;
                            psi2.UseShellExecute = false;
                            psi2.CreateNoWindow = false;
                            psi2.RedirectStandardInput = true;
                            psi2.RedirectStandardOutput = true;
                            psi2.RedirectStandardError = true;
                            p2.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
                            p2.ErrorDataReceived += new DataReceivedEventHandler(p_ErrorDataReceived);
                            p2.Start();
                            p2.WaitForExit();
                        }
                        Process p = new Process();
                        ProcessStartInfo psi = new ProcessStartInfo(@"C:\Users\CS1200\Desktop\SDK_Gadisa_2.19\SDK_Gadisa_2.19\CS1100_GA_164L\CS1100_GA_164L\5-Programa\sw1100_setup.exe", "/qb");
                            p.StartInfo = psi;
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = false;
                            psi.RedirectStandardError = false;
                            psi.RedirectStandardInput = false;
                            psi.RedirectStandardOutput = false;
                            p.Start();
                            p.WaitForExit();

                        File.Copy(@"C:\Users\CS1200\Desktop\CONFIGURATION_DB.exe", @"C:\SW1100\DBSQLCS\CONFIGURATION_DB.exe", true);

                        p = new Process();
                            psi = new ProcessStartInfo(@"C:\SW1100\DBSQLCS\CONFIGURATION_DB.exe", "1 0");
                            p.StartInfo = psi;
                            psi.UseShellExecute = false;
                            psi.CreateNoWindow = false;
                            psi.RedirectStandardError = false;
                            psi.RedirectStandardInput = false;
                            psi.RedirectStandardOutput = false;
                            p.Start();
                            p.WaitForExit();

                        File.Copy(@"C:\Users\CS1200\Desktop\AccesoBalanzaPC.exe.config", @"C:\SW1100\BalanzaPC\AccesoBalanzaPC.exe.config", true);
                        Process.Start(@"C:\SW1100\BalanzaPC\AccesoBalanzaPC.exe");
                        Thread.Sleep(1000);
                        Process.Start(@"C:\SW1100\Comunicaciones\ComunicacionesBalPCInterface.exe");
                        Thread.Sleep(1000);
                        File.Copy(@"C:\Users\CS1200\Desktop\LineData.xml", @"C:\SW1100\RGI\LineData.xml", true);
                        Process.Start(@"C:\SW1100\RGI\RGI.exe");
                        Thread.Sleep(1000);

                        if (File.Exists(@"\\192.168.150.65\ftp\Update"))
                            ExtensionMethods.MoveFile(@"\\192.168.150.65\ftp\Update", @"\\192.168.150.65\ftp\Updated");
                    }
                    Thread.Sleep(1000);
                }
            catch (NullReferenceException e)
                {
                    if (File.Exists(@"\\192.168.150.65\ftp\Update"))
                        ExtensionMethods.MoveFile(@"\\192.168.150.65\ftp\Update", @"\\192.168.150.65\ftp\NullReferenceError");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    if (File.Exists(@"\\192.168.150.65\ftp\Update"))
                        ExtensionMethods.MoveFile(@"\\192.168.150.65\ftp\Update", @"\\192.168.150.65\ftp\Error");
                }
            }

        }

        static void p_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            string s = e.Data;
            File.AppendAllText(Path.Combine(Application.StartupPath, "Output.txt"), DateTime.Now.ToString() + "  => " + s + Environment.NewLine);
        }

        static void p_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            string s = e.Data;
            File.AppendAllText(Path.Combine(Application.StartupPath, "Error.txt"), DateTime.Now.ToString() + "  => " + s + Environment.NewLine);
        }

        public static void Chrome()
        {
            string programFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
            string programFilesX86 = Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%");
            string sdag = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            string sdagsdfg = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string sdwag = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
            string sddagsdfg = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
            string sdsdfhag = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Google\Chrome\Application\chrome.exe";
            string sdhfdhagsdfg = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\Google\Chrome\Application\chrome.exe";
            //Chrome
            if (!File.Exists(Environment.ExpandEnvironmentVariables("%ProgramW6432%") + @"\Google\Chrome\Application\chrome.exe"))
            {
                if (!File.Exists(Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%") + @"\Google\Chrome\Application\chrome.exe"))
                {

                }
            }
        }

        public static void splitcommand()
        {
            string comand = "#ID=3040#IR##ID=4207#LS#0#5#80#0#0#0#0#";
            foreach (string mes in comand.Split(new string[] { "#ID=" }, StringSplitOptions.RemoveEmptyEntries))
            {
                MessageBox.Show("#ID=" + mes);
            }
        }

        public static void FechaArabe()
        {
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                MessageBox.Show(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
                MessageBox.Show(System.Threading.Thread.CurrentThread.CurrentUICulture.ToString());

                int rows = MySQL.EjecutaQuery(MySQL.Connection, "select * from sys_campos_etiqueta where IdModeloBalanza = 20 order by OrdenAgrupacion DESC, OrdenCampos ASC").Tables[0].Rows.Count;
                MessageBox.Show(rows.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetType().ToString() + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public static void GetFiles()
        {
            string path = "C:\\tmp3";
            string[] files = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly);
            int asfg = files.Length;
        }

        public static void CompareMD5()
        {
            string fileUN = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ts_client_UN.dll");
            byte[] bUN;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileUN))
                {
                    bUN = md5.ComputeHash(stream);
                }
            }
            string fileDI = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ts_client_DI.dll");
            byte[] bDI;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileDI))
                {
                    bDI = md5.ComputeHash(stream);
                }
            }
            bool eq = Enumerable.SequenceEqual(bUN, bDI);
        }

        public static void Testing()
        {
            int number = 144;
            string root = "T:\\17-RegistroPruebas";
            string end = "." + number.ToString("000");
            string folder = Directory.GetDirectories(root).Single(x => x.EndsWith(end));
            string testingf = "01.-Testing Scope";
            string defectsf = "04.-Defects to Correct";
            string tmp = Path.Combine(Path.GetTempPath(), "TestingPlan.xlsx");
            string directory = Path.Combine(folder, testingf);
            string file = Directory.GetFiles(directory).Single(x => Path.GetFileName(x).StartsWith("Testing Plan") && x.EndsWith(".xlsx"));
            File.Copy(file, tmp, true);
            Process.Start(tmp);

            directory = Path.Combine(folder, defectsf);
            file = Directory.GetFiles(directory).Single(x => Path.GetFileName(x).StartsWith("Defects") && x.EndsWith(".xlsx"));
            Process.Start(file);

            IExcelDataReader excelReader = null;
            DataSet datosExcel;
            using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

               datosExcel = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
            }

        }
    }
}
