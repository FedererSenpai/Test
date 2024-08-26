using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class DibalForm : Base
    {
        DataSet datosExcel;
        int i = 0;
        Timer t = new Timer();
        Socket WorkingSocket;

        public DibalForm(int index)
        {
            InitializeComponent();
            tabControl1.SelectedIndex = index;
        }

        private void MyLoad(object sender, EventArgs e)
        {
            //Page1();
            return;
            int i = 1;
            foreach(string line in File.ReadAllLines(@"C:\Users\dzhang\Downloads\Logs\EnvioModificaciones_log\20240212_78_Scale 78_10.237.40.178_1.log"))
            {
                if (line.Contains("78L250"))
                {
                    llllllll.Items.Add(i.ToString() + "- " + line);
                    i++;
                }
            }
            Dibal.Desencriptar(System.Text.Encoding.ASCII.GetBytes("C.y.....^.w.,...,...^.]...B..}..3$...V........C...r.T;....@..>P..w@..nt.a.m.`.....L......$....f.#.......3$...>P.p........g=...T.5.L.G...s.x.#....zG.kV-.....c.a...S......bw.P...n.w..YC..9?.......!...n...6..V....!...n...6..V..}.........>..V..$.}.Z....eF...8......u#.K...kV-."));
            Page1();
        }

        private void MyShown(object sender, EventArgs e)
        {
            Page6();
            //WorkingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //WorkingSocket.Connect("192.168.150.64", 9090);
            /*t.Interval = 60000;
            t.Tick += new EventHandler(Page2);
            t.Start();*/
        }

        private void Page1()
        {
            string tmp = Path.Combine(Path.GetTempPath(), "Activitytmp.xlsx");
            File.Copy(@"C:\Daniel\VS\Test\WindowsFormsApp1\bin\Activity\resources\Activity.xlsx", tmp, true);
            IExcelDataReader excelReader = null;
            using (FileStream stream = File.Open(tmp, FileMode.Open, FileAccess.Read))
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
            wwwww.MaxDate = DateTime.Now;
            wwwww.MinDate = new DateTime(2023, 12, 19);
            wwwww.ValueChanged += new EventHandler(ValueChanged);
            wwwww.Value = DateTime.Now.AddDays(-1);
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            DateTime dt = wwwww.Value;
            string year = dt.Year.ToString();
            string month = dt.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
            string day = dt.ToString(1);
            List<Tuple<DateTime?, DateTime?>> mytime = new List<Tuple<DateTime?, DateTime?>>();
            bool startup = true;
            DateTime? lastdate = null;
            TimeSpan total = new TimeSpan();
            if (datosExcel.Tables.Contains(dt.Year.ToString()))
            {
                DataTable dataTable = datosExcel.Tables[year];
                if (dataTable.Columns.Contains(month))
                {
                    int column = dataTable.Columns[month].Ordinal;
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row[column + 1].ToString().StartsWith(day))
                        {

                            if (row[column].ToString().Equals("StartUp"))
                            {
                                string time = row[column + 1].ToString().Substring(4);
                                if (!startup)
                                {
                                    mytime.Add(new Tuple<DateTime?, DateTime?>(lastdate, null));
                                    lastdate = Convert.ToDateTime(time);
                                }
                                else
                                {
                                    lastdate = Convert.ToDateTime(time);
                                }
                                startup = false;
                            }
                            else if (row[column].ToString().Equals("ShutDown"))
                            {
                                string time = row[column + 1].ToString().Substring(4);
                                if (startup)
                                {
                                    mytime.Add(new Tuple<DateTime?, DateTime?>(null, Convert.ToDateTime(time)));
                                    lastdate = Convert.ToDateTime(time);
                                }
                                else
                                {
                                    mytime.Add(new Tuple<DateTime?, DateTime?>(lastdate, Convert.ToDateTime(time)));
                                    lastdate = Convert.ToDateTime(time);
                                }
                                startup = true;
                            }
                        }
                    }
                }
            }
            ffffffffff.Items.Clear();
            foreach (Tuple<DateTime?, DateTime?> mytuple in mytime)
            {
                ffffffffff.Items.Add($"{mytuple.Item1.ToString(2)} <-> {mytuple.Item2.ToString(2)} ::::: {(mytuple.Item2 - mytuple.Item1).ToReadableString()};");
                total = total.AddDifference(mytuple.Item1, mytuple.Item2);
            }
            ffffffffff.Items.Add($"Total ::::: {total.ToReadableString()}");
        }

        private void Page2(object sender, EventArgs e)
        {
            Random rmyr = new Random();
            switch (rmyr.Next(1, 10))
            {
                case 1:
                    t.Enabled = false;
                    /*UpdateSW*/
                    if (!File.Exists(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Updated") && !File.Exists(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Update"))
                    {
                        using (FileStream fs = File.Create(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Update")) { }
                        llllllll.Items.Add("Uninstall => " + DateTime.Now.ToString());
                        llllllll.Refresh();
                        Application.DoEvents();
                        int j = 1;
                        while(!File.Exists(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Updated") && File.Exists(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Update") && j < 300)
                        {
                            System.Threading.Thread.Sleep(1000);
                            j++;
                        }
                        if (File.Exists(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Updated"))
                            File.Delete(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Updated");
                        if (File.Exists(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Update"))
                            ExtensionMethods.MoveFile(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Update", @"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\Timeout");
                    }
                    Random ag = new Random();
                    switch(ag.Next(1, 5))
                    {
                        case 1: //CS
                            File.Copy(@"C:\Users\PCIndustrial\Desktop\SDK_Gadisa_2.20\SDK_Gadisa_2.20\CS1100_GA_164M.zip", @"C:\SW1100\UpdateSW\CS1100_GA_164M.zip");
                            llllllll.Items.Add(@"C:\SW1100\UpdateSW\CS1100_GA_164M.zip => " + DateTime.Now.ToString());
                            break;
                        case 2: //Pers
                            File.Copy(@"C:\Users\PCIndustrial\Desktop\SDK_Gadisa_2.20\SDK_Gadisa_2.20\Personalizador_2.20_GA.zip", @"C:\SW1100\UpdateSW\Personalizador_2.20_GA.zip");
                            llllllll.Items.Add(@"C:\SW1100\UpdateSW\Personalizador_2.20_GA.zip => " + DateTime.Now.ToString());
                            break;
                        case 3: //CS + Pers
                            File.Copy(@"C:\Users\PCIndustrial\Desktop\SDK_Gadisa_2.20\SDK_Gadisa_2.20\CS1100_GA_164M.zip", @"C:\SW1100\UpdateSW\CS1100_GA_164M.zip");
                            File.Copy(@"C:\Users\PCIndustrial\Desktop\SDK_Gadisa_2.20\SDK_Gadisa_2.20\Personalizador_2.20_GA.zip", @"C:\SW1100\UpdateSW\Personalizador_2.20_GA.zip");
                            llllllll.Items.Add(@"C:\SW1100\UpdateSW\CS1100_GA_164M.zip + Personalizador_2.20_GA.zip => " + DateTime.Now.ToString());
                            break;
                        default:
                            break;
                    }
                    if (!File.Exists(@"C:\SW1100\UpdateSW\" + i.ToString()))
                    {
                        i++;
                        using (FileStream fs = File.Create(@"C:\SW1100\UpdateSW\" + i.ToString())) { }
                        llllllll.Items.Add(@"C:\SW1100\UpdateSW\" + i.ToString() + " => " + DateTime.Now.ToString());
                    }
                    llllllll.Refresh();
                    Application.DoEvents();
                    int p = 1;
                    int f = Directory.GetFiles(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\ExportFilesFTP\UpdateSWResults").Length;
                    while (Directory.GetFiles(@"C:\xampp\htdocs\Dibal\DibalTransfer\TransferServer\data\queue\ftp\ExportFilesFTP\UpdateSWResults").Length == f && p < 1200)
                    {
                        System.Threading.Thread.Sleep(1000);
                        p++;
                    }

                    t.Enabled = true;
                    break;
                case 2:
                    /*ImportFiles*/


                    if (!File.Exists(@"C:\SW1100\ImportFiles\ItemFiles\Nuevo documento de texto.txt"))
                    {
                        using (StreamWriter sw = File.CreateText(@"C:\SW1100\ImportFiles\ItemFiles\Nuevo documento de texto.txt"))
                        {
                            Random rr = new Random();
                            string text = string.Join(";", new string[] { rr.Next(1, 9999).ToString(), rr.Next(1, 9999).ToString(), 20.RandomString(), 1.ToString(), 1.ToString(), rr.Next(1, 9999).ToString() });
                            sw.WriteLine(text);
                            i++;
                            llllllll.Items.Add(text + "_" + i.ToString() + " => " + DateTime.Now.ToString());
                        }
                    }
                    break;
                case 3:
                    /*ImportImages    */
                    Random r = new Random();
                    int orig = r.Next(1, 1752);
                    int dst = r.Next(1, 9999);
                    File.Copy(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Images", orig.ToString("000000") + ".jpg"), Path.Combine(@"C:\SW1100\ImportFiles\ItemImgFiles", dst.ToString("000000") + ".jpg"), true);
                    i++;
                    llllllll.Items.Add("Orig_" + orig + "_Dst_" + dst + "_" + i.ToString() + " => " + DateTime.Now.ToString());

                    break;
                case 4:
                     /*Publi*/
                    Random rrr = new Random();
                    List<string> ims = new List<string>();
                    for (int j = 0; j < 10; j++)
                    {
                        int origg = rrr.Next(1, 1752);
                        ims.Add(origg.ToString("000000") + ".jpg");
                        File.Copy(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Images", origg.ToString("000000") + ".jpg"), Path.Combine(@"C:\SW1100\Publicidad\CARNE", origg.ToString("000000") + ".jpg"), true);
                    }
                    i++;
                    llllllll.Items.Add(string.Join(";", ims) + "_" + i.ToString() + " => " + DateTime.Now.ToString());
            
                    break;
                case 5:
                    Random dag = new Random();
                    int s = dag.Next(1, 4);
                    WebRequest request = WebRequest.Create("http://192.168.150.65:3000/turnomatic/turnomatic.php?id=127.0.0.1_" + s + "&action=turnIncrement");
                    request.Credentials = CredentialCache.DefaultCredentials;
                    request.Timeout = 10000;
                    request.Headers = new WebHeaderCollection();
                    request.Method = "POST";
                    request.ContentType = "application/json";
                    request.GetResponse();
                    i++;
                    llllllll.Items.Add("http://192.168.150.65:3000/turnomatic/turnomatic.php?id=127.0.0.1_" + s + "&action=turnIncrement_" + i.ToString() + " => " + DateTime.Now.ToString());
                    break;
                default:
                    i++;
                    llllllll.Items.Add("Wait_" + i.ToString() + " => " + DateTime.Now.ToString());
                    break;
            }
            /*DibalUpdateSW       
            if (System.Diagnostics.Process.GetProcessesByName("chrome").Length == 0 && !File.Exists(@"C:\xampp\htdocs\Dibal\DibalTransfer\Actions\DibalUpdateSW\data\device\downloaded\000000000000_1_T_S_D_C2_update.zip"))
            {
                File.Copy(@"C:\xampp\htdocs\Dibal\DibalTransfer\Actions\DibalUpdateSW\data\device\000000000000_1_T_S_D_C2_update.zip", @"C:\xampp\htdocs\Dibal\DibalTransfer\Actions\DibalUpdateSW\data\device\downloaded\000000000000_1_T_S_D_C2_update.zip");
                i++;
                llllllll.Items.Add(@"C:\xampp\htdocs\Dibal\DibalTransfer\Actions\DibalUpdateSW\data\device\downloaded\000000000000_1_T_S_D_C2_update.zip_" + i.ToString() + " => " + DateTime.Now.ToString());
            }*/





        }

        private void StartListening()
        {
            try
            {
                IPEndPoint localEP = new IPEndPoint(IPAddress.Any, Convert.ToInt32(WorkingSocket.RemoteEndPoint.ToString().Split(':')[1]));

                WorkingSocket = new Socket(localEP.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                WorkingSocket.Bind(localEP);
                WorkingSocket.Listen(10);

                listBox1.Items.Add("Listen to: " + localEP.ToString());

            }
            catch (Exception e)
            {
                listBox1.Items.Add(e.Message);
            }
        }

        public class StateObject
        {
            public Socket workSocket = null;
            public const int BufferSize = 1024;
            public byte[] buffer = new byte[BufferSize];
        }

        private void AcceptCallback(IAsyncResult ar)
        {
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                listBox1.Items.Add("Socket connected to: " + handler.RemoteEndPoint.ToString());

                //Create the state object.
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
            }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            if (handler.Connected)
            {
                // Read data from the client socket.
                SocketError se = SocketError.AccessDenied;
                int read = handler.EndReceive(ar, out se);

                // Data was read from the client socket.
                if (read > 0)
                {
                    string comand = System.Text.Encoding.Default.GetString(state.buffer, 0, read);
                    if (this.InvokeRequired)
                    {
                        DelegateProcessMessage dpm = new DelegateProcessMessage(this.ProcessMessage);
                        this.Invoke(dpm, new object[] { "Answer: " + comand });
                    }
                    else
                    {
                        ProcessMessage("Answer: " + comand);
                    }
                }
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        private delegate void DelegateProcessMessage(string message);

        private void ProcessMessage(string message)
        {
            listBox1.Items.Add(message);
        }

        private void EndCall(IAsyncResult ar)
        {
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = WorkingSocket;
            StateObject state = new StateObject();
            state.workSocket = handler;

            handler.EndSend(ar);
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StateObject state = new StateObject();
            state.workSocket = WorkingSocket;
            foreach (string s in textBox1.Text.Split('|'))
            {
                WorkingSocket.Send(System.Text.Encoding.Default.GetBytes(s));
                //WorkingSocket.BeginSend(System.Text.Encoding.Default.GetBytes(s), 0, System.Text.Encoding.Default.GetByteCount(s), SocketFlags.None, EndCall, WorkingSocket);
            }
            WorkingSocket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            /* string result = string.Empty;
                        if (WorkingSocket != null && WorkingSocket.Connected)
                        {
                            //id++;
                            //mensaje = "#ID=" + id + mensaje; 
                            listBox1.Items.Add("Command: " + textBox1.Text);
                            WorkingSocket.Send(System.Text.Encoding.Default.GetBytes("#ID=34#PI#"));
                            WorkingSocket.BeginReceive()
                            listBox1.Send(System.Text.Encoding.Default.GetBytes("#ID=563#LI#"));
                            while (WorkingSocket.Available == 0)
                            {
                                Application.DoEvents();
                                System.Threading.Thread.Sleep(10);
                                if (System.Diagnostics.Process.GetProcessesByName("ScaleConnector").Length <= 0)
                                    throw new Exception("ScaleConnector is closed");
                            }

                            byte[] bResul = new byte[WorkingSocket.Available];
                            WorkingSocket.Receive(bResul);
                            result = System.Text.Encoding.Default.GetString(bResul);
                            listBox1.Items.Add("Answer: " + result);
                            //resul = resul.Replace("#ID=" + id, "");
                        }
                        else
                        {
                            listBox1.Items.Add("Socket not connected");
                        }*/

            /* if (WorkingSocket != null)
                 WorkingSocket.Close();*/
        }

        private void Page4()
        {
            Bitmap b = new Bitmap(richTextBox1.Width, richTextBox1.Height);
            richTextBox1.Rtf = @"{\rtf1\ansi asdfj dsaflsa f. asdf \b huevos\b0, huevospsoafhf dsafs sdafj asdf www.google.com askdghs asgfn dkajld";
            richTextBox1.DrawToBitmap(b, new Rectangle(0, 0, b.Width, b.Height));
            b = RtbToBitmap(richTextBox1);
            b.Save(@"C:\Control.bmp");
        }

        public static Bitmap RtbToBitmap(RichTextBox rtb)
        {
            rtb.Update(); // Ensure RTB fully painted
            Bitmap bmp = new Bitmap(rtb.Width, rtb.Height);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(rtb.PointToScreen(Point.Empty), Point.Empty, rtb.Size);
            }
            return bmp;
        }

        private void Page5()
        {
            ListBox lb = new ListBox();
            lb.Dock = DockStyle.Fill;
            tabPage5.Controls.Add(lb);
            string temp = Path.GetTempFileName();
                const int blockSize = 1024 * 8;
                const int blocksPerMb = (1024 * 1024) / blockSize;
                byte[] data = new byte[blockSize];
                Random rng = new Random();
                using (FileStream stream = File.OpenWrite(temp))
                {
                    // There 
                    for (int i = 0; i < blocksPerMb; i++)
                    {
                        rng.NextBytes(data);
                        stream.Write(data, 0, data.Length);
                    }
                }
            while (true)
            {
                File.Copy(temp, Path.Combine("C:\\TicketBai\\24646462", Path.GetRandomFileName() + ".xml"), true);
            }
        }

        private void Page6()
        {
            ListBox lb = new ListBox();
            lb.Dock = DockStyle.Fill;
            tabPage6.Controls.Add(lb);
            DriveInfo c = new DriveInfo("C:\\");
            FileInfo fi = new FileInfo("C:\\SW1100.zip");
            Int64 size = fi.Length;
            int cont = 1;
            lb.Items.Add($"Size => {c.AvailableFreeSpace}");
            while (c.AvailableFreeSpace > size)
            {
                File.Copy("C:\\SW1100.zip", $"C:\\Users\\CS1200\\Desktop\\Trash\\SW1100({cont}).zip");
                lb.Items.Add($"{cont} => {c.AvailableFreeSpace}");
                lb.Refresh();
                Application.DoEvents();
                cont++;
            }
        }
    }
}
