using CefSharp;
using CefSharp.WinForms;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WindowsFormsApp1
{
    //PDTE: EVENTO CLICK HEADER PARA ORDENAR POR COLUMNAS.
    public partial class Flights : Base
    {
        private string urlBase => $"https://www.skyscanner.es/transporte/vuelos/{textBox1.Text}/{textBox2.Text}/?adultsv2=1&cabinclass=economy&childrenv2=&ref=home&rtn=1&preferdirects={checkBox3.Checked}&outboundaltsenabled={checkBox1.Checked}&inboundaltsenabled={checkBox2.Checked}&oym={"{0}"}&iym={"{1}"}&selectedoday=01&selectediday=01";
        List<Prices> prices = new List<Prices>();
        string prefix;
        int pricesCount = 0;
        public Flights() : base(".json")
        {
            InitializeComponent();
            prefix = DateTime.Now.ToString("yyyyMMddHHmmss");
            textBox1.Text = "bio";
            textBox2.Text = "tyoa";
        }

        public override void Initialize()
        {
            AddMenuItems(new EventHandler(Fill), new EventHandler(HomePage), new EventHandler(ToFile));
        }

        private void ToFile(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".json";
            saveFileDialog.InitialDirectory = OwnPath;
            if(saveFileDialog.ShowDialog(this) == DialogResult.OK)
                prices.ToFile(saveFileDialog.FileName);
        }

        private void HomePage(object sender, EventArgs e)
        {
            ExtensionMethods.DefaultBrowser("https://www.skyscanner.com");
        }

        private void Fill(object sender, EventArgs e)
        {
            prices = ExtensionMethods.FileToList<Prices>(OwnFullPath);
            prices = prices.OrderBy(x => x.TotalPrice).ToList();
            dataGridView1.DataSource = prices;
        }

        private DateTime GetFirstDay(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (DateTime i = GetFirstDay(dateTimePicker1.Value); i <= dateTimePicker2.Value; i = i.AddMonths(1))
            {
                for (DateTime j = i; j <= dateTimePicker2.Value; j = j.AddMonths(1))
                    GetPrices(i, j);
            }

            prices.ToFile(OwnFullPath);

            dataGridView1.DataSource = prices;
            //dataGridView1.Sort(dataGridView1.Columns["TotalPrice"], ListSortDirection.Ascending);

        }

        private void GetPrices(DateTime from, DateTime to)
        {
            label3.Text = string.Format(urlBase, from.ToString("yyMM"), to.ToString("yyMM"));
            AutoWeb aw = new AutoWeb(label3.Text, chromiumWebBrowserPrices_LoadingStateChanged, "", "");
            aw.Opacity = 0;
            aw.Owner = this;
            aw.ShowInTaskbar = false;
            aw.ShowDialog();
            while (pricesCount <= 0)
            {
                Thread.Sleep(100);
            }
            aw.Dispose();

            List<Prices> tempPrices = new List<Prices>();
            myProgressBar1.Maximum = pricesCount;
            myProgressBar1.Value = 0;
            for (int i = 0; i <= pricesCount; i++)
            {
                Prices p = new Prices() { DepartureYear = from.Year, ReturnYear = to.Year, DepartureMonth = from.Month, ReturnMonth = to.Month };
                string file = string.Empty;
                while (string.IsNullOrEmpty(file) || File.Exists(file))
                {
                    string randomFile = prefix + Path.GetRandomFileName();
                    file = ResultFile(randomFile);
                }

                aw = new AutoWeb(label3.Text, chromiumWebBrowserFlights_LoadingStateChanged, file, i == pricesCount ? string.Empty : $"if(document.getElementsByClassName('month-view-calendar outbound-calendar')[0].getElementsByClassName('BpkCalendarWeek_bpk-calendar-week__date__MTRlO bpk-calendar-week__date--none')[{i}].getElementsByClassName('price')[0].innerText != '')document.getElementsByClassName('month-view-calendar outbound-calendar')[0].getElementsByClassName('BpkCalendarWeek_bpk-calendar-week__date__MTRlO bpk-calendar-week__date--none')[{i}].firstChild.click();");
                aw.Opacity = 0;
                aw.Owner = this;
                aw.ShowInTaskbar = false;
                aw.ShowDialog();
                p.File = file;
                myProgressBar1.Value++;
                tempPrices.Add(p);
                aw.Dispose();
            }

            myProgressBar1.Maximum = tempPrices.Count;
            myProgressBar1.Value = 0;
            foreach (Prices p in tempPrices)
            {
                prices.AddRange(p.ReadPriceFile());
                myProgressBar1.Value++;
                File.Delete(p.File);
            }
        }

        private async void chromiumWebBrowserPrices_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
            AutoWeb aw = browser.TopLevelControl as AutoWeb;
            if (!e.IsLoading)
            {
                while (!browser.CanExecuteJavascriptInMainFrame)
                    Thread.Sleep(10);
                 
                JavascriptResponse res;
                string s;
                try
                {
                    await browser.EvaluateScriptAsync("window.scrollTo(0, document.body.scrollHeight);");

                    res = await browser.EvaluateScriptAsync("document.getElementsByClassName('month-view-calendar outbound-calendar')[0].getElementsByClassName('BpkCalendarWeek_bpk-calendar-week__date__MTRlO bpk-calendar-week__date--none').length");

                    if (!browser.Address.Contains("www.skyscanner.es/transporte/vuelos/"))
                    {
                        aw.ChangeOpacity(1, false);
                        return;
                    }
                }
                catch (ObjectDisposedException)
                {
                    return;
                }

                int.TryParse(Convert.ToString(res.Result), out pricesCount);
                aw.Cerrar();
                return;
            }

        }


        private async void chromiumWebBrowserFlights_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            ChromiumWebBrowser browser = sender as ChromiumWebBrowser;
            AutoWeb aw = browser.TopLevelControl as AutoWeb;
            if (!e.IsLoading)
            {
                while (!browser.CanExecuteJavascriptInMainFrame)
                    Thread.Sleep(10);

                JavascriptResponse res;
                string s;
                try
                {
                    await browser.EvaluateScriptAsync("window.scrollTo(0, document.body.scrollHeight);");
                    
                    if(checkBox3.Checked)
                        await browser.EvaluateScriptAsync("if(!document.getElementsByClassName('BpkCheckbox_bpk-checkbox__input__MTRlM')[0].checked) document.getElementsByClassName('BpkCheckbox_bpk-checkbox__input__MTRlM')[0].click();");

                    res = await browser.EvaluateScriptAsync(aw.GetScript());
                    s = await browser.GetBrowser().MainFrame.GetSourceAsync();

                    if (!browser.Address.Contains("www.skyscanner.es/transporte/vuelos/"))
                    {
                        aw.ChangeOpacity(1, false);
                        return;
                    }
                }
                catch (ObjectDisposedException)
                {
                    return;
                }


                await Task.Delay(1000);

                string file = aw.GetFile();
                try
                {
                    if (!string.IsNullOrEmpty(file) )
                    {
                        SaveResultFile(file, s, false, false);
                    }
                }
                catch (IOException ex)
                {
                    //MessageBox.Show(file);
                }

                await Task.Delay(500);

                aw.Cerrar();
                return;
            }

        }

        private void label3_Click(object sender, EventArgs e)
        {
            ExtensionMethods.DefaultBrowser(string.Format((sender as Label).Text, "01", "01"));
        }
        
        private class Prices
        {
            private int departureYear;
            private int returnYear;
            private int departureMonth;
            private int returnMonth;
            private string departureDay;
            private string returnDay;
            private string departurePrice;
            private string returnPrice;
            private string file;

            public int DepartureYear { get => departureYear; set => departureYear = value; }
            public int ReturnYear { get => returnYear; set => returnYear = value; }
            public int DepartureMonth { get => departureMonth; set => departureMonth = value; }
            public int ReturnMonth { get => returnMonth; set => returnMonth = value; }
            public string DepartureDay { get => departureDay; set => departureDay = value; }
            public string ReturnDay { get => returnDay; set => returnDay = value; }
            public string DeparturePrice { get => departurePrice; set => departurePrice = value; }
            public string ReturnPrice { get => returnPrice; set => returnPrice = value; }
            public int TotalPrice { get => GetTotalPrice(); }
            public string File { get => file; set => file = value; }
            [JsonIgnore]
            public int Days => CalculateDays();


            private int CalculateDays()
            {
                return new DateTime(ReturnYear, ReturnMonth, Convert.ToInt16(ReturnDay)).Subtract(new DateTime(DepartureYear, DepartureMonth, Convert.ToInt16(DepartureDay))).Days;
            }

            private int GetTotalPrice()
            {
                int.TryParse(DeparturePrice.Replace("€", string.Empty), out int dep);
                int.TryParse(ReturnPrice.Replace("€", string.Empty), out int ret);
                return dep + ret;
            }

            public Prices Clone()
            {
                Prices p = new Prices();
                p.DepartureYear = departureYear;
                p.ReturnYear = returnYear;
                p.DepartureMonth = departureMonth;
                p.ReturnMonth = returnMonth;
                p.DeparturePrice = departurePrice;
                p.DepartureDay = departureDay;
                p.File = file;
                return p;
            }

            public List<Prices> ReadPriceFile()
            {
                List<Prices> priceList = new List<Prices>();
                try
                {
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(System.IO.File.ReadAllText(File));
                    HtmlNode departureNode = doc.DocumentNode.SelectSingleNodeByClass("month-view-calendar outbound-calendar", true).SelectSingleNodeByClass("BpkCalendarWeek_bpk-calendar-week__date__MTRlO bpk-calendar-week__date--single");
                    DepartureDay = departureNode.SelectSingleNodeByClass("date").InnerText;
                    DeparturePrice = departureNode.SelectSingleNodeByClass("price").InnerText;
                    HtmlNode calendarNode = doc.DocumentNode.SelectSingleNodeByClass("month-view-calendar inbound-calendar", true);
                    HtmlNode dateNodes = calendarNode.SelectSingleNodeByClass("BpkCalendarGrid_bpk-calendar-grid__YjU0O month-view-grid--data-loaded");
                    if(dateNodes == null)
                        dateNodes = calendarNode.SelectSingleNodeByClass("BpkCalendarGrid_bpk-calendar-grid__YjU0O month-view-grid--data-loading");
                    HtmlNodeCollection pricesNodes = dateNodes.SelectNodesByClass("price");
                    foreach (HtmlNode node in pricesNodes)
                    {
                        HtmlNode parent = node.ParentNode;
                        Prices p = Clone();
                        p.ReturnDay = parent.SelectSingleNodeByClass("date").InnerText;
                        p.ReturnPrice = parent.SelectSingleNodeByClass("price").InnerText;
                        if (!string.IsNullOrEmpty(p.DeparturePrice) && !string.IsNullOrEmpty(p.ReturnPrice))
                            priceList.Add(p);
                    }
                }
                catch(Exception e)
                {
                    return ReadPriceFile();
                }
                return priceList;
            }
        }
    }
}
