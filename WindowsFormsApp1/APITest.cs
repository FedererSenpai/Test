using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net.Http.Formatting;
using System.Collections;

namespace WindowsFormsApp1
{
    public partial class APITest : Form
    {
        static HttpClient client = new HttpClient();
        string maxidticket;
        public bool subir = false;
        string idticket;
        string respuesta;
        ResponseScaleWS resp = new ResponseScaleWS(ResponseScaleWS.respuestas.OK, string.Empty);
        public APITest()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            ResponseScaleWS res = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseScaleWS>("{ \"Result\":\"OK\",\"Description\":\"Connected\"}");
            //StreamReader sr = new StreamReader(@"C:\Users\dzhang\Desktop\json.json");
            //string texto = sr.ReadToEnd();
            //string respues = PostMethod("http://localhost:8888/tickets/insert", texto, 5000);
            string respues = GetMethod("http://10.2.11.103:8888/infos/status", 2500);
            textBox1.Text = "http://nortmaticcloudlab2.nortconsulting.com/nortmaticapi/api/";
            NortMaticRequest r = new NortMaticRequest();
            r.AuthSecret = "AAAAA";
            r.SecretKey = "BBBBB";
            r.UserName = "CCCCC";
            r.UserCode = "DDDDD";
            r.AuthSecret = "EEEEE";
            NormaticResponse s = new NormaticResponse();
            s.IsError = true;
            s.Message = "ZZZZZ";
            s.ResponseError = new NormaticCounterResponse() { Currentcountervalue = 10, Requestedcountervalue = 20 };
            s.ResponseSuccess = new NormaticCounterResponse() { Currentcountervalue = 30, Requestedcountervalue = 40 };
            List<NortMaticRequest> asfd = new List<NortMaticRequest>();
            asfd.Add(r);
            asfd.ToFile(@"C:\pruebajson.json");
            List<NormaticResponse> aghdf = new List<NormaticResponse>();
            aghdf.Add(s);
            aghdf.ToFile(@"C:\pruebajson2.json");
            List<BajaRequest> dasdf = new List<BajaRequest>();
            dasdf.Add(new BajaRequest());
            dasdf.Add(new BajaRequest());
            dasdf.Add(new BajaRequest());
            dasdf.ToFile(@"C:\pruebabaja.json");
        }

        public static string PostMethod(string url, string data, int timeout = 1500)
        {
            string resp = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            string json = data;
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Timeout = timeout;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return resp;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();
                            return responseBody;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle error
                return resp;
            }
        }

        public static string GetMethod(string url, int timeout = 1500)
        {
            string resp = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Timeout = timeout;
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) return resp;
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            string responseBody = objReader.ReadToEnd();
                            return responseBody;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                // Handle error
                return resp;
            }
        }

        public async Task ReinicarAPI()
        {
            try
            { 
            HttpResponseMessage mes = await client.PostAsJsonAsync("config/restart", string.Empty);
            if (mes.IsSuccessStatusCode)
            {
                    respuesta = await mes.Content.ReadAsAsync<string>();
            }
            else
            {
                    respuesta = "KO => " + mes.StatusCode.ToString();
            }
        }
            catch (Exception ex)
            {
                respuesta = "KO => " + ex.Message;
            }
        }

        public async Task LlamarStatus()
        {
            try
            {
                HttpResponseMessage mes = await client.GetAsync("info/status");
                if (mes.IsSuccessStatusCode)
                {
                    respuesta = await mes.Content.ReadAsAsync<string>();
                    StreamWriter sw = new StreamWriter(@"C:\Users\dzhang\Desktop\jsonstatus.json");
                    sw.WriteLine(respuesta);
                    sw.Close();
                }
                else
                {
                    respuesta = "KO => " + mes.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                respuesta = "KO => " + ex.Message;
            }
        }

        public async Task LlamarGetTicket()
        {
            try
            {
                HttpResponseMessage mes = await client.GetAsync("Tickets/maxidticket/?update=true");
                if (mes.IsSuccessStatusCode)
                {
                    label1.Text = await mes.Content.ReadAsAsync<string>();
                    if (label1.Text.Contains("Argumento no válido"))
                    {
                        mes = await client.GetAsync("Tickets/maxidticket/?update=false");
                        if (mes.IsSuccessStatusCode)
                        {
                            label1.Text = await mes.Content.ReadAsAsync<string>();
                        }
                        else
                        {
                            label1.Text = "KO => " + mes.StatusCode.ToString();
                        }
                    }
                }
                else
                {
                    label1.Text = "KO => " + mes.StatusCode.ToString();
                }
                maxidticket = label1.Text;
            }
            catch(Exception ex)
            {
                label1.Text = "KO => " + ex.Message;
            }
        }

        public async Task LlamarGetTicketTemp()
        {
            try
            { 
            HttpResponseMessage mes = await client.GetAsync($"Tickets/tojson/?query=select%20*%20from%20dat_ticket_cabecera%20limit%201;SELECT%20*%20FROM%20dat_ticket_linea%20d%20where%20idticket%20=%20(select%20idticket%20from%20dat_ticket_cabecera%20limit%201);SELECT%20*%20FROM%20dat_ticket_forma_pago%20where%20idticket%20=%20(SELECT%20idticket%20from%20dat_ticket_cabecera%20limit%201);");
                if (mes.IsSuccessStatusCode)
            {
                    respuesta = await mes.Content.ReadAsAsync<string>();
                    resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseScaleWS>(respuesta);
                    if (long.TryParse(resp.Description, out long id))
                        idticket = id.ToString();
                    else
                        subir = false;
            }
            else
            {
                    respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseScaleWS(ResponseScaleWS.respuestas.KO, mes.StatusCode.ToString()));
                    subir = false;
                }
            }
            catch(TaskCanceledException ex)
            {
                respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseScaleWS(ResponseScaleWS.respuestas.KO, ex.Message));
            }
            catch (Exception ex)
            {
                respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseScaleWS(ResponseScaleWS.respuestas.KO, ex.Message));

                subir = false;
            }
        }

        public async Task LlamarPostConfig(string server)
        {
            try
            {
                /*Dictionary<string, string> d = new Dictionary<string, string>()
                {
                    {"logpath", @"C:\SW1100\BalanzaPC\log\" },
                    {"cnnstring", $"Server={server}; Database=sys_datos; User Id=user; Password=dibal;Connection TimeOut = 10000; Port=3306; provider=Provider MySQL;" },
                    {"idempresa", "1" },
                    {"idtienda", "1" },
                    {"idbalanza", "1" }
                };
                HttpResponseMessage response = await client.PostAsJsonAsync("Config/Initialize", string.Empty);
                if (response.IsSuccessStatusCode)
                {
                        respuesta = await response.Content.ReadAsAsync<string>();
                }
                else
                {
                        respuesta = "KO => " + response.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    respuesta = "KO => " + ex.Message;
                }*/
                string res = PostMethod("http://localhost:8888/config/initialize", string.Empty, 2500);
                object o = Type.GetType("WindowsFormsApp1.ResponseScaleWS");
            }
            catch { }
        }

        public async Task LlamarPostTicket()
        {
            try
            {
                StreamReader sr = new StreamReader($@"C:\Users\dzhang\Desktop\jsonlist\json{idticket}.json");
                string json = await sr.ReadToEndAsync();


                DataSet ds = Newtonsoft.Json.JsonConvert.DeserializeObject<DataSet>(json);
                if(ds.Tables[0].Rows.Count == 0)
                {
                    subir = false;
                }
                //await ParsearDataset(ds);
                HttpResponseMessage response = await client.PostAsJsonAsync("Tickets/insert", json);
            if (response.IsSuccessStatusCode)
            {
                    respuesta = await response.Content.ReadAsAsync<string>();
                    StreamWriter sw = new StreamWriter(@"C:\Users\dzhang\Desktop\jsonticket.json");
                    sw.WriteLine(respuesta);
                    sw.Close();
                }
            else
            {
                    respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseScaleWS(ResponseScaleWS.respuestas.KO, response.StatusCode.ToString()));
                    subir = false;

            }
            }
            catch (TaskCanceledException ex)
            {
                respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseScaleWS(ResponseScaleWS.respuestas.OK, ex.Message));
            }
            catch (Exception ex)
            {
                respuesta = Newtonsoft.Json.JsonConvert.SerializeObject(new ResponseScaleWS(ResponseScaleWS.respuestas.KO, ex.Message));

                subir = false;
            }
        }

        public async Task LlamarPostCampos()
        {
            try
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("Tickets/campos", string.Empty);
                if (response.IsSuccessStatusCode)
                {
                    label1.Text = await response.Content.ReadAsAsync<string>();
                }
                else
                {
                    label1.Text = "KO => " + response.StatusCode.ToString();
                }
            }
            catch (Exception ex)
            {
                label1.Text = "KO => " + ex.Message;
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            await LlamarPostTicket(); //insert
            resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseScaleWS>(respuesta);
            label1.Text = resp.Description;
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            await LlamarGetTicketTemp(); //tojson
            resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseScaleWS>(respuesta);
            label1.Text = resp.Description;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            LlamarPostConfig("localhost");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LlamarPostConfig("192.168.151.62");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LlamarGetTicket(); //Maxidticket
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LlamarPostCampos(); //campos cabecera y lineas
        }

        private async void button6_Click(object sender, EventArgs e)
        {
            int i = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            label1.Text = "Start";
            subir = true;
            while(subir)
            {
                try
                {
                    await LlamarGetTicketTemp();
                    resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseScaleWS>(respuesta);
                    label1.Text = resp.Description;
                    Application.DoEvents();
                    Thread.Sleep(2000);
                    Application.DoEvents();
                    await LlamarPostTicket();
                    resp = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseScaleWS>(respuesta);
                    label1.Text = resp.Description;
                    Application.DoEvents();
                    Thread.Sleep(2000);
                    Application.DoEvents();
                    i++;
                }
                catch
                {
                    subir = false;
                }
            }
            label1.Text = "fin " + i + " " + sw.ElapsedMilliseconds;
            sw.Stop();
            Application.DoEvents();
            Thread.Sleep(2000);
            Application.DoEvents();
            /*button1.BackColor = Color.LightGreen;
            button1_Click(button1, null);
            button1.BackColor = SystemColors.Control;

            Application.DoEvents();
            Thread.Sleep(1000);
            Application.DoEvents();

            if (label1.Text.StartsWith("KO"))
            {
                MessageBox.Show("button1_Click");
                return;
            }

            button5.BackColor = Color.LightGreen;
            button5_Click(button5, null);
            button5.BackColor = SystemColors.Control;

            Application.DoEvents();
            Thread.Sleep(1000);
            Application.DoEvents();

            if (label1.Text.StartsWith("KO"))
            {
                MessageBox.Show("button5_Click");
                return;
            }

            button4.BackColor = Color.LightGreen;
            button4_Click(button4, null);
            button4.BackColor = SystemColors.Control;

            Application.DoEvents();
            Thread.Sleep(1000);
            Application.DoEvents();

            if (label1.Text.StartsWith("KO"))
            {
                MessageBox.Show("button4_Click");
                return;
            }

            button2.BackColor = Color.LightGreen;
            button2_Click(button2, null);
            button2.BackColor = SystemColors.Control;

            Application.DoEvents();
            Thread.Sleep(1000);
            Application.DoEvents();

            if (label1.Text.StartsWith("KO"))
            {
                MessageBox.Show("button2_Click");
                return;
            }

            button1.BackColor = Color.LightGreen;
            button1_Click(button1, null);
            button1.BackColor = SystemColors.Control;

            Application.DoEvents();
            Thread.Sleep(1000);
            Application.DoEvents();

            if (label1.Text.StartsWith("KO"))
            {
                MessageBox.Show("button1_Click _2");
                return;
            }

            button3.BackColor = Color.LightGreen;
            button3_Click(button3,null);
            button3.BackColor = SystemColors.Control;

            Application.DoEvents();
            Thread.Sleep(1000);
            Application.DoEvents();

            if (label1.Text.StartsWith("KO"))
            {
                MessageBox.Show("button3_Click");
                return;
            }
            */
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ReinicarAPI();

            Application.DoEvents();
            Thread.Sleep(5000);
            Application.DoEvents();

            LlamarStatus();

            Application.DoEvents();
            Thread.Sleep(1000);
            Application.DoEvents();
        }

        private async void button9_Click(object sender, EventArgs e)
        {
            string result = SendPeticionGet(textBox1.Text + "check/ping");
            HttpResponseMessage response = await client.GetAsync("check/ping");
            if (response.IsSuccessStatusCode)
            {
                button9.ForeColor = await response.Content.ReadAsAsync<bool>() ? Color.Green : Color.Red;
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    client.BaseAddress = new Uri("http://localhost:8888/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    client.Timeout = TimeSpan.FromMilliseconds(10000);
                    break;
                case 1:
                    client.BaseAddress = new Uri(textBox1.Text);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("*/*"));
                    client.Timeout = TimeSpan.FromMilliseconds(10000);
                    break;
            }
        }

        private async void button10_Click(object sender, EventArgs e)
        {
            textBox2.Text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject< NormaticResponse>(SendPeticion(textBox1.Text + "counter/getvalue", JsonConvert.SerializeObject(new NortMaticRequest() { SecretKey = "c78ece5b-313d-4a14-94ca-386adcb243d2", ResponseType = 0 }))), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        private async void button11_Click(object sender, EventArgs e)
        {
            textBox2.Text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<NormaticResponse>(SendPeticion(textBox1.Text + "counter/next", JsonConvert.SerializeObject(new NortMaticRequest() { SecretKey = "c78ece5b-313d-4a14-94ca-386adcb243d2", ResponseType = 0 }))), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        private async void button12_Click(object sender, EventArgs e)
        {
            textBox2.Text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<NormaticResponse>(SendPeticion(textBox1.Text + "counter/recall", JsonConvert.SerializeObject(new NortMaticRequest() { SecretKey = "c78ece5b-313d-4a14-94ca-386adcb243d2", ResponseType = 0 }))), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

        }

        private async void button13_Click(object sender, EventArgs e)
        {
            textBox2.Text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<NormaticResponse>(SendPeticion(textBox1.Text + "counter/previous", JsonConvert.SerializeObject(new NortMaticRequest() { SecretKey = "c78ece5b-313d-4a14-94ca-386adcb243d2", ResponseType = 0 }))), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

        }

        private async void button14_Click(object sender, EventArgs e)
        {
            textBox2.Text = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<NormaticResponse>(SendPeticion(textBox1.Text + "counter/reset", JsonConvert.SerializeObject(new NortMaticRequest() { SecretKey = "c78ece5b-313d-4a14-94ca-386adcb243d2", ResponseType = 0 }))), Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

        }

        private static string SendPeticionGet(string peticion)
        {
            string responseFromServer = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(peticion);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 5000;
                request.Headers = new WebHeaderCollection();

                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                Log.EscribirError(ex.StackTrace, ex.Message);
            }
            return responseFromServer;
        }

        private static string SendPeticion(string peticion, string data)
        {
            string responseFromServer = string.Empty;
            try
            {
                WebRequest request = WebRequest.Create(peticion);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 10000;
                request.Headers = new WebHeaderCollection();
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                var bytes = Encoding.ASCII.GetBytes(data);

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, data.Length);
                }
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                response.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                Log.EscribirError(ex.StackTrace, ex.Message);
            }
            return responseFromServer;
        }

        private void button15_Click(object sender, EventArgs e)
        {
            textBox3.Text = SendPeticion("http://192.168.150.100:3000/turnomatic/getData.php?ASJSON&id=127.0.0.1_1", string.Empty);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text))
            {
                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                t.Interval = 60000;
                t.Tick += new EventHandler(Timer_tick);
                t.Start();
            }
            else
            {
                string serverViewIP = textBox4.Text.Split(':')[0];
                string sectionID = textBox4.Text.Split(':')[1];
                SendPeticion("http://" + serverViewIP + ":3000/turnomatic/turnomatic.php?id=127.0.0.1_" + sectionID + "&action=turnIncrement", string.Empty);
                textBox3.Text += "http://" + serverViewIP + ":3000/turnomatic/turnomatic.php?id=127.0.0.1_" + sectionID + "&action=turnIncrement" + " =>" + DateTime.Now.ToString() + Environment.NewLine;
            }
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            Hashtable table = new Hashtable();
            table.Add("0","192.168.151.69:1");
            table.Add("1","192.168.151.65:2");
            table.Add("2","10.2.11.99:3");
            table.Add("3", "192.168.150.61:1");
            table.Add("4", "192.168.151.69:2");
            table.Add("5", "10.2.11.103:3");
            table.Add("6", "10.2.11.99:1");
            table.Add("7", "10.2.11.103:2");
            table.Add("8", "192.168.150.62:3");
            string randomkey = new Random().Next(table.Count).ToString();
            string serverViewIP = table[randomkey].ToString().Split(':')[0];
            string sectionID = table[randomkey].ToString().Split(':')[1];
            //string serverViewIP = textBox4.Text;
            //string sectionID = "1"; // new Random().Next(2).ToString();
            SendPeticion("http://" + serverViewIP + ":3000/turnomatic/turnomatic.php?id=127.0.0.1_" + sectionID + "&action=turnIncrement", string.Empty);
            textBox3.Text += "http://" + serverViewIP + ":3000/turnomatic/turnomatic.php?id=127.0.0.1_" + sectionID + "&action=turnIncrement" + " =>" + DateTime.Now.ToString() + Environment.NewLine;
            if(textBox3.Lines.Count() >= 20)
            textBox3.Lines = textBox3.Lines.Skip(15).ToArray();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            string serverIP = "192.168.150.221";
            string serverID = "1";
            string checkout_number = "1";
            SendPeticion("http://" + serverIP + ":3000/SingleQueue/request.php?action=openCheckout&id=" + serverID + "&checkoutNumber=" + checkout_number + "&value=1", string.Empty);
            textBox3.Text += "http://" + serverIP + ":3000/SingleQueue/request.php?action=openCheckout&id=" + serverID + "&checkoutNumber=" + checkout_number + "&value=1" + " =>" + DateTime.Now.ToString() + Environment.NewLine;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            string serverIP = "192.168.150.221";
            string serverID = "1";
            string checkout_number = "1";
            string arrow = "1";
            SendPeticion("http://" + serverIP + ":3000/SingleQueue/request.php?action=freeCheckout&id=" + serverID + "&checkoutNumber=" + checkout_number + "&arrow=" + arrow + "&value=1", string.Empty);
            textBox3.Text += "http://" + serverIP + ":3000/SingleQueue/request.php?action=freeCheckout&id=" + serverID + "&checkoutNumber=" + checkout_number + "&arrow=" + arrow + "&value=1" + " =>" + DateTime.Now.ToString() + Environment.NewLine;
        }
    }

    class ResponseScaleWS
    {
        public enum respuestas
        {
            OK,
            KO
        }

        public string Result { get; set; }
        public string Description { get; set; }

        public ResponseScaleWS(respuestas resultado, string descripcion)
        {
            Result = resultado.ToString();
            Description = descripcion;
        }
    }

    public class NortMaticRequest
    {
        private string secretKey;
        private string authSecret;
        private string userCode;
        private string userName;
        private int? responseType;

        [JsonRequired]
        [JsonProperty("secretKey")]
        public string SecretKey { get => secretKey; set => secretKey = value; }
        [JsonProperty("responseType", NullValueHandling = NullValueHandling.Ignore)]
        public int? ResponseType { get => responseType; set => responseType = value; }
        [JsonProperty("authSecret", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthSecret { get => authSecret; set => authSecret = value; }
        [JsonProperty("userCode", NullValueHandling = NullValueHandling.Ignore)]
        public string UserCode { get => userCode; set => userCode = value; }
        [JsonProperty("userName", NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get => userName; set => userName = value; }
    }

    public class NormaticResponse
    {
        private NormaticCounterResponse responseSuccess;
        private NormaticCounterResponse responseError;
        private bool isError;
        private string message;

        [JsonProperty("responseSuccess", NullValueHandling = NullValueHandling.Ignore)]
        public NormaticCounterResponse ResponseSuccess { get => responseSuccess; set => responseSuccess = value; }
        [JsonProperty("responseError", NullValueHandling = NullValueHandling.Ignore)]
        public NormaticCounterResponse ResponseError { get => responseError; set => responseError = value; }
        [JsonProperty("isError")]
        public bool IsError { get => isError; set => isError = value; }
        [JsonProperty("message")]
        public string Message { get => message; set => message = value; }
    }

    public class NormaticCounterResponse
    {
        private int currentcountervalue;
        private int requestedcountervalue;

        [JsonProperty("currentcountervalue")]
        public int Currentcountervalue { get => currentcountervalue; set => currentcountervalue = value; }
        [JsonProperty("requestedcountervalue")]
        public int Requestedcountervalue { get => requestedcountervalue; set => requestedcountervalue = value; }
    }

    public class BajaRequest
    {
        private int idTransaccion = 1;
        private int codigoArticulo = 345978;
        private string nombreArticulo = "qwertyuiop";
        private string tipoArticulo = "pesado";
        private decimal cantidad = 1.234M;
        private DateTime fechaHora = DateTime.Now;
        private string tipoBaja = "asdfghjkl";
        private string modificado = "A";

        public int IdTransaccion { get => idTransaccion; set => idTransaccion = value; }
        public int CodigoArticulo { get => codigoArticulo; set => codigoArticulo = value; }
        public string NombreArticulo { get => nombreArticulo; set => nombreArticulo = value; }
        public string TipoArticulo { get => tipoArticulo; set => tipoArticulo = value; }
        public decimal Cantidad { get => cantidad; set => cantidad = value; }
        public DateTime FechaHora { get => fechaHora; set => fechaHora = value; }
        public string TipoBaja { get => tipoBaja; set => tipoBaja = value; }
        public string Modificado { get => modificado; set => modificado = value; }
    }
}
