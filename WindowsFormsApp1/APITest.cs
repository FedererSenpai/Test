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
            client.BaseAddress = new Uri("http://localhost:8888/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromMilliseconds(10000);
            CheckForIllegalCrossThreadCalls = false;
            ResponseScaleWS res = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseScaleWS>("{ \"Result\":\"OK\",\"Description\":\"Connected\"}");
            StreamReader sr = new StreamReader(@"C:\Users\dzhang\Desktop\json.json");
            string texto = sr.ReadToEnd();
            //string respues = PostMethod("http://localhost:8888/tickets/insert", texto, 5000);
            string respues = GetMethod("http://10.2.11.103:8888/infos/status", 2500);
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
}
