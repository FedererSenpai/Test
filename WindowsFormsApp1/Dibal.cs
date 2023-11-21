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

namespace WindowsFormsApp1
{
    public static class Dibal
    {
        private static UInt32 _ip;
        private static UInt32 _mask;

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
    }
}
