using CefSharp;
using CefSharp.DevTools.Emulation;
using CefSharp.WinForms;
using HtmlAgilityPack;
using Microsoft.VisualBasic.Compatibility.VB6;
using Renci.SshNet;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xml.Linq;
using static WindowsFormsApp1.DatosInsert;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace WindowsFormsApp1
{
    public partial class Cars : Base
    {
        List<Car> carsList = new List<Car>();
        const string separator = "%7C";
         
        const string cochesNet = "https://www.coches.net";
        const string carLink = ".//a[@class='mt-CardBasic-titleLink']";
        const string addPage = "&pg={0}";
        public static HtmlDocument doc = new HtmlDocument();
        private int MaxPrice = 50000;
        enum imageType
        {
            Image,
            Video,
            None
        }

        enum ArrBodyType
        {
            Berlina = 1,
            Coupe = 2,
            Cabrio = 3,
            Familiar = 4,
            Monovolumen = 5,
            SUV = 6,
            PickUp = 7
        }

        enum Fueltype2List
        {
            Diesel = 1,
            Gasolina = 2,
            Electrico = 3,
            Hibrido = 4,
            HibridoEnchufable = 5,
            GLP = 6,
            CNG = 7,
            Otros = 8
        }

        enum PriceRank
        {
            PrecioAlto = 1,
            PrecioElevado = 2,
            PrecioJusto = 3,
            BuenPrecio = 4,
            SuperPrecio = 5
        }

        enum fi
        {
            Price,
            FinancingInstalment,
            Kilometers,
            Year
        }

        enum Or
        {
            ASC = 1,
            DESC = -1
        }

        public Cars() : base(".json")
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            AddMenuItems(new EventHandler(Search), new EventHandler(Fill));
        }
      
        private string PrepareParameters(params object[] parameters)
        {
            string paramlist = string.Empty;

            foreach (object param in parameters)
            {
                paramlist += GetParam(param);
            }

            if (!string.IsNullOrEmpty(paramlist)) 
                paramlist = "/?" + paramlist.Substring(0, paramlist.Length - 3);

            return paramlist;
        }

        private string GetParam(object param)
        {
            if(param is IList)
            {
                Type tipo = param.GetType().GetGenericArguments().Single();
                string value;
                if (tipo.Name == typeof(fi).Name)
                    value = String.Join(separator, ((IList)param).Cast<object>());
                else
                    value = String.Join(separator, ((IList)param).Cast<int>());
                return tipo.Name + "=" + value + "%26";
            }
            else if (param is ValueType)
            {
                return ((ValueTuple<string, string>)param).Item1 + "=" + ((ValueTuple<string, string>)param).Item2 + "%26";
            }
            return string.Empty;
        }

        private void Buscar()
        {
            AutoWeb aw = new AutoWeb(label1.Text, true, "document.getElementById(\"didomi-notice-agree-button\").click();");
            aw.Opacity = 0;
            aw.Owner = this;
            aw.ShowDialog();
            int lastPage = GetCochesNetLastPage(doc.DocumentNode);
            myProgressBar1.Maximum = lastPage;
            carsList = new List<Car>();
            GetCochesNetCar(doc.DocumentNode, lastPage);
            foreach (Car car in carsList)
                GetCochesNetInfoCar(car);
            dataGridView1.DataSource = carsList;
            carsList.ToFile(OwnFullPath);
        }

        private string GetLink(HtmlNode node)
        {
            return node.SelectSingleNode(carLink).GetAttributeValue("href", string.Empty);
        }

        private void Search(object sender, EventArgs e)
        {
            this.label1.Text = cochesNet + "/km-0" + PrepareParameters(new List<ArrBodyType>() { ArrBodyType.SUV },
                new List<Fueltype2List>() { Fueltype2List.Electrico, Fueltype2List.Hibrido, Fueltype2List.HibridoEnchufable}, 
                new List<PriceRank>() { PriceRank.BuenPrecio, PriceRank.PrecioJusto, PriceRank.SuperPrecio},
                new List<fi>() { fi.Price }, new List<Or>() { Or.ASC}, ("MaxPrice", "50000") );
            Buscar();
        }

        private void Fill(object sender, EventArgs e)
        {
            carsList = ExtensionMethods.FileToList<Car>(OwnFullPath);
            myProgressBar1.Value = 0;
            myProgressBar1.Maximum = carsList.Count;
            foreach (Car car in carsList)
            {
                GetCochesNetInfoCar(car);
                myProgressBar1.Value++;
            }
            carsList.ToFile(OwnFullPath);
            while (Application.OpenForms.OfType<AutoWeb>().Count() > 0)
                Thread.Sleep(10);
            myProgressBar1.Value = 0;
            myProgressBar1.Maximum = carsList.Count;
            foreach (Car car in carsList)
            {
                car.TechLink = File.ReadAllText(car.File);
                File.Delete(car.File);
                GetCochesNetTechCar(car);
                myProgressBar1.Value++;
            }
            carsList.ToFile(OwnFullPath);
            while (Application.OpenForms.OfType<AutoWeb>().Count() > 0)
                Thread.Sleep(10);
            foreach(Car car in carsList)
            {
                ReadCochesNetTechCar(car);
                myProgressBar1.Value++;
                File.Delete(car.TechFile);
            }
            carsList.ToFile(OwnFullPath);
            dataGridView1.DataSource = carsList;
        }

        private void GetCochesNetCar(HtmlNode node, int lastPage, int page = 1)
        {
            try
            {
                HtmlNodeCollection carInfoList = node.SelectNodesByClass("sui-AtomCard sui-AtomCard--responsive", true);
                foreach (HtmlNode carInfo in carInfoList)
                {
                    Car car = new Car();
                    HtmlNode imageNode = carInfo.SelectSingleNodeByClass("sui-AtomVideoPlayer-youtubePlayerFrame");
                    if(imageNode != null)
                    {
                        car.ImageURL = imageNode.GetAttributeValue("src", string.Empty);
                        car.Image = imageType.Video;
                    }
                    else
                    { 
                        imageNode = carInfo.SelectSingleNodeByClass("mt-GalleryBasic-sliderImage mt-GalleryBasic-sliderImage--squared");
                        if(imageNode != null)
                        {
                            car.ImageURL = imageNode.GetAttributeValue("src", string.Empty);
                            car.Image = imageType.Image;
                        }
                    }
                    car.Link = cochesNet + carInfo.SelectSingleNodeByClass("mt-CardBasic-titleLink").GetAttributeValue("href", string.Empty);
                    car.Title = carInfo.SelectSingleNodeByClass("mt-CardBasic-title").InnerText;
                    car.Price = carInfo.SelectSingleNodeByClass("mt-CardAdPrice-cashAmount").InnerText;// ("mt-TitleBasic-title mt-TitleBasic-title--s mt-TitleBasic-title--currentColor").InnerText;
                    HtmlNode financingNode = carInfo.SelectSingleNodeByClass("mt-CardAdPrice-financedAmount");
                    if (financingNode != null) 
                    {
                        car.Financing = financingNode.SelectSingleNode(".//strong").InnerText;
                        car.Fee = carInfo.SelectSingleNodeByClass("mt-CardAdPrice-financedAmountCuota mt-CardAdPrice-financedAmountCuota--topSpaced").InnerText;// ("mt-TitleBasic-title mt-TitleBasic-title--s mt-TitleBasic-title--currentColor").InnerText;
                    }
                    HtmlNode cityNode = carInfo.SelectSingleNodeByClass("mt-CardAd-attrItemIconLabel");
                    int i = 0;
                    if (cityNode != null)
                    {
                        car.City = cityNode.InnerText;
                        i++;
                    }
                    foreach(HtmlNode attrnode in carInfo.SelectNodesByClass("mt-CardAd-attrItem"))
                    {
                        switch(i)
                        {
                            case 0:
                                car.City = attrnode.InnerText;
                                break;
                            case 1:
                                car.Fuel = attrnode.InnerText;
                                break;
                            case 2:
                                car.Year = attrnode.InnerText;
                                break;
                            case 3:
                                car.Kilometers = attrnode.InnerText;
                                break;
                        }
                        i++;
                    }
                    //GetCochesNetInfoCar(car);
                    carsList.Add(car);
                }
                myProgressBar1.Value++;
                page++;
                Thread.Sleep(1000);
                if (page <= lastPage)
                {
                    AutoWeb aw = new AutoWeb(label1.Text + string.Format(addPage, page), true, "document.getElementById(\"didomi-notice-agree-button\").click();");
                    aw.Opacity = 0;
                    aw.Owner = this;
                    aw.ShowInTaskbar = false;
                    aw.ShowDialog();
                    aw.Dispose();
                    GetCochesNetCar(doc.DocumentNode, lastPage, page);
                }
            }
            catch(Exception ex)
            {
                SaveResultFile("Exception.txt", doc.Text, true);
                throw ex ;
            }
        }

        private void GetCochesNetInfoCar(Car car)
        {
            string url = car.Link;
            string file = string.Empty;
            while(string.IsNullOrEmpty(file) || File.Exists(file))
            {
                string randomFile = Path.GetRandomFileName();
                file = ResultFile(randomFile);
            }
            AutoWeb aw = new AutoWeb(url, "Array.from(document.getElementsByClassName('sui-AtomButton sui-AtomButton--primary sui-AtomButton--outline sui-AtomButton--center sui-AtomButton--link sui-AtomButton--circular')).find(item=>item.getAttribute('type') == 'link').toString()", file, true);
            aw.Opacity = 0;
            aw.Owner = this;
            aw.ShowInTaskbar = false;
            aw.ShowDialog();
            car.File = file;
            //aw.Dispose();
            //HtmlNode buttonNode = doc.DocumentNode.SelectSingleNodeByClass("sui-AtomButton sui-AtomButton--primary sui-AtomButton--outline sui-AtomButton--center sui-AtomButton--link sui-AtomButton--circular");
            //car.TechLink = buttonNode.GetAttributeValue("href", string.Empty);
        }

        private void GetCochesNetTechCar(Car car)
        {
            string url = car.TechLink;
            string file = string.Empty;
            while (string.IsNullOrEmpty(file) || File.Exists(file))
            {
                string randomFile = Path.GetRandomFileName();
                file = ResultFile(randomFile);
            }
            AutoWeb aw = new AutoWeb(url, "Array.prototype.forEach.call(document.getElementsByClassName('sui-MoleculeAccordionItemHeaderButton sui-MoleculeAccordionItemHeaderButton--icon-position-right'), function(el) {el.click();}); document.body.getHTML();", file, true);
            //document.getElementsByClassName('mt-SharedSectionWrapper-contentWrapper')[3].getHTML();
            aw.Opacity = 0;
            aw.Owner = this;
            aw.ShowInTaskbar = false;
            aw.ShowDialog();
            car.TechFile = file;
            //aw.Dispose();
            //HtmlNode generalNode = doc.DocumentNode.SelectSingleNodeByClass("mt-SharedSectionWrapper-contentWrapper");
        }

        private int GetCochesNetLastPage(HtmlNode node)
        {
            HtmlNodeCollection pages = node.SelectNodes("//li[@class='sui-MoleculePagination-item']");
            string page = "1";
            if(pages.Any(x=>x.SelectSingleNode(".//span[@class='sui-AtomButton-rightIcon']") != null))
            {
                page = pages[pages.Count - 2].SelectSingleNode(".//span[@class='sui-AtomButton-content']").InnerText;
            }
            else
                page = pages.Last().SelectSingleNode(".//span[@class='sui-AtomButton-content']").InnerText;
            return Convert.ToInt16(page);
        }

        private void ReadCochesNetTechCar(Car car)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(File.ReadAllText(car.TechFile));
            HtmlNodeCollection generalNode = doc.DocumentNode.SelectNodesByClass("mt-ListModelDetails-accordion", true);
            if (generalNode == null && doc.DocumentNode.InnerText.Contains("¡Ups! No hemos podido encontrar la página que buscas."))
                return;
            foreach(HtmlNode node in generalNode)
            {
                HtmlNode headerNode = node.SelectSingleNodeByClass("sui-MoleculeAccordionItemHeaderButtonContent sui-MoleculeAccordionItemHeaderButtonContent--noWrap");
                string type = headerNode.InnerText;
                HtmlNodeCollection dataNode = node.SelectNodesByClass("react-AtomTable-row react-AtomTable-row--zebraStriped");
                GetCochesNetGeneralData(car, dataNode, type);
            }
        }

        private void GetCochesNetGeneralData(Car car, HtmlNodeCollection datanode, string type)
        {
            switch(type)
            {
                case "Datos del modelo y carrocería":
                    foreach(HtmlNode node in datanode)
                    {
                        string property = node.SelectSingleNodeByClass("mt-ListModelDetails-tableItem").InnerText;
                        string value = node.SelectSingleNodeByClass("mt-ListModelDetails-tableItem--strong").InnerText;
                        car.ModelProperty(property, value);
                    }
                    break;
            }
        }

        private class Car
        {
            private string title;
            private string price;
            private string financing;
            private string fee;
            private string fuel;
            private string kilometers;
            private string city;
            private string year;
            private imageType image = imageType.None;
            private string imageURL;
            private string link;
            private string techLink;
            private string file;
            private string techFile;
            private Model model = new Model();

            public string Title { get => title; set => title = value; }
            public string Price { get => price; set => price = value.Replace("&bnsp;", " "); }
            public string Financing { get => financing; set => financing = value; }
            public string Fee { get => fee; set => fee = value; }
            public string Fuel { get => fuel; set => fuel = value; }
            public string Kilometers { get => kilometers; set => kilometers = value; }
            public string City { get => city; set => city = value; }
            public string Year { get => year; set => year = value; }
            public string ImageURL { get => imageURL; set => imageURL = value; }
            public string Link { get => link; set => link = value; }
            public imageType Image { get => image; set => image = value; }
            public string TechLink { get => techLink; set => techLink = value; }
            public string File { get => file; set => file = value; }
            public string TechFile { get => techFile; set => techFile = value; }
            public Model CarModel { get => model; set => model = value; }

            public class Model
            {
                private string capacity;
                private string version;
                private string doors;
                private string dimensions;
                private string trunk;
                private string seats;
                private string weight;

                public string Capacity { get => capacity; set => capacity = value; }
                public string Version { get => version; set => version = value; }
                public string Doors { get => doors; set => doors = value; }
                public string Dimensions { get => dimensions; set => dimensions = value; }
                public string Trunk { get => trunk; set => trunk = value; }
                public string Seats { get => seats; set => seats = value; }
                public string Weight { get => weight; set => weight = value; }
            }

            public class Engine
            {

            }

            public class Consumption
            {

            }

            private class Equipment
            {

            }

            public void ModelProperty(string property, string value)
            {
                switch(property)
                {
                    case "Capacidad del depósito":
                        CarModel.Capacity = value;
                        break;
                    case "Versión":
                        CarModel.Version = value;
                        break;
                    case "Nº de puertas":
                        CarModel.Doors = value;
                        break;
                    case "Largo x ancho x alto":
                        CarModel.Dimensions = value;
                        break;
                    case "Volumen del maletero":
                        CarModel.Trunk = value;
                        break;
                    case "Nº de plazas":
                        CarModel.Seats = value;
                        break;
                    case "Peso":
                        CarModel.Weight = value;
                        break;
                }
            }
        }

        private void label1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ExtensionMethods.DefaultBrowser((sender as Label).Text);
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if (dataGridView1.Columns[e.ColumnIndex].DataPropertyName == "Link")
                {
                    Process.Start(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dataGridView1.DataSource = carsList.Where(x=>x.Title.ToLower().Contains(textBox1.Text.ToLower())).ToList();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

    }
}
