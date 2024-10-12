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
        private string prefix;
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

        enum EquipmentType
        {
            Standard,
            Extras
        }
        public Cars() : base(".json")
        {
            InitializeComponent();
            prefix = DateTime.Now.ToString("yyyyMMddHHmmss");
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
            carsList.ToFile(OwnFullPath);

            myProgressBar1.Value = 0;
            myProgressBar1.Maximum = carsList.Count;
            foreach (Car car in carsList)
            {
                GetCochesNetInfoCar(car);
                myProgressBar1.Value++;
            }
            while (Application.OpenForms.OfType<AutoWeb>().Count() > 0)
                Thread.Sleep(10);
            carsList.ToFile(OwnFullPath);

            myProgressBar1.Value = 0;
            myProgressBar1.Maximum = carsList.Count;
            foreach (Car car in carsList)
            {
                car.TechLink = File.ReadAllText(car.File);
                //File.Delete(car.File);
                if (!string.IsNullOrEmpty(car.TechLink))
                    GetCochesNetTechCar(car);
                myProgressBar1.Value++;
            }
            while (Application.OpenForms.OfType<AutoWeb>().Count() > 0)
                Thread.Sleep(10);
            carsList.ToFile(OwnFullPath);

            myProgressBar1.Value = 0;
            myProgressBar1.Maximum = carsList.Count;
            foreach (Car car in carsList)
            {
                if (!string.IsNullOrEmpty(car.TechFile))
                    ReadCochesNetTechCar(car);
                myProgressBar1.Value++;
                //File.Delete(car.TechFile);
            }
            carsList.ToFile(OwnFullPath);

            if(MessageBox.Show("Delete files", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                foreach (Car car in carsList)
                {
                    File.Delete(car.File); 
                    File.Delete(car.TechFile);
                }
            }

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
            //myProgressBar1.Value = 0;
            //myProgressBar1.Maximum = carsList.Count;
            //foreach (Car car in carsList)
            //{
            //    GetCochesNetInfoCar(car);
            //    myProgressBar1.Value++;
            //}
            //while (Application.OpenForms.OfType<AutoWeb>().Count() > 0)
            //    Thread.Sleep(10);
            //carsList.ToFile(OwnFullPath);
            //myProgressBar1.Value = 0;
            //myProgressBar1.Maximum = carsList.Count;
            //foreach (Car car in carsList)
            //{
            //    car.TechLink = File.ReadAllText(car.File);
            //    if(!string.IsNullOrEmpty(car.TechLink))
            //    GetCochesNetTechCar(car);
            //    myProgressBar1.Value++;
            //}
            //while (Application.OpenForms.OfType<AutoWeb>().Count() > 0)
            //    Thread.Sleep(10);
            //carsList.ToFile(OwnFullPath);
            //myProgressBar1.Value = 0;
            //myProgressBar1.Maximum = carsList.Count;
            //foreach (Car car in carsList)
            //{
            //    if (!string.IsNullOrEmpty(car.TechFile))
            //        ReadCochesNetTechCar(car);
            //    myProgressBar1.Value++;
            //}
            //carsList.ToFile(OwnFullPath);

            //if (MessageBox.Show("Delete files", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{
            //    foreach (Car car in carsList)
            //    {
            //        File.Delete(car.File);
            //        File.Delete(car.TechFile);
            //    }
            //}

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
                string randomFile = prefix + Path.GetRandomFileName();
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
                string randomFile = prefix + Path.GetRandomFileName();
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
            GetCochesNetEquipmentCount(doc.DocumentNode.SelectSingleNodeByClass("mt-ListModelDetails mt-ListModelDetails--paddingless"), out int c1, out int c2);
            HtmlNodeCollection equipmentNode = doc.DocumentNode.SelectNodesByClass("mt-PanelEquipment-accordion");
            int c = 0;
            foreach(HtmlNode node in equipmentNode)
            {
                HtmlNode headerNode = node.SelectSingleNodeByClass("sui-MoleculeAccordionItemHeaderButtonContent sui-MoleculeAccordionItemHeaderButtonContent--noWrap");
                string type = headerNode.InnerText;
                HtmlNodeCollection dataNode = node.SelectNodesByClass("mt-PanelEquipment-tableItem");
                if (c < c1)
                    GetCochesNetStandardEquipment(car, dataNode, type, EquipmentType.Standard);
                else
                    GetCochesNetStandardEquipment(car, dataNode, type, EquipmentType.Extras);
                c++;
            }
        }

        private void GetCochesNetEquipmentCount(HtmlNode node, out int standardEquipmentCount, out int ExtrasCount)
        {
            standardEquipmentCount = 0;
            ExtrasCount = 0;
            bool standardEquipment = false;
            bool extras = false;
            HtmlNode currentNode = node.NextSibling;
            while(currentNode != null)
            {
                if (currentNode.GetAttributeValue("class", string.Empty).Equals("mt-PanelEquipment-title"))
                {
                    standardEquipment = currentNode.InnerText.Equals("Equipamiento de serie");
                    extras = currentNode.InnerText.Equals("Extras");
                    if (!standardEquipment && !extras)
                        MessageBox.Show("Tittle." + currentNode.InnerText);
                }
                if(currentNode.GetAttributeValue("class", string.Empty).Equals("mt-PanelEquipment-accordion"))
                {
                    if (standardEquipment)
                        standardEquipmentCount++;
                    if (extras)
                        ExtrasCount++;
                }
                currentNode = currentNode.NextSibling;
            }
        }

        private void GetCochesNetStandardEquipment(Car car, HtmlNodeCollection datanode, string type, EquipmentType eType)
        {
            switch(eType)
            {
                case EquipmentType.Standard:
                    switch (type)
                    {
                        case "Ficha Técnica":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.TechnicalSheet.Add(node.InnerText);
                            break;
                        case "Multimedia y Audio":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.MultimediaAudio.Add(node.InnerText);
                            break;
                        case "Acabado Exterior":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.ExteriorFinish.Add(node.InnerText);
                            break;
                        case "Seguridad":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.Security.Add(node.InnerText);
                            break;
                        case "Información Básica":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.BasicInformation.Add(node.InnerText);
                            break;
                        case "Confort":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.Comfort.Add(node.InnerText);
                            break;
                        case "Dimensiones":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.Dimensions.Add(node.InnerText);
                            break;
                        case "Acabado Interior":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.InteriorFinish.Add(node.InnerText);
                            break;
                        case "Prestaciones":
                            foreach (HtmlNode node in datanode)
                                car.CarStandardEquipment.Capability.Add(node.InnerText);
                            break;
                        default:
                            MessageBox.Show("Standard." + type);
                            break;
                    }
                    break;
                case EquipmentType.Extras:
                    switch (type)
                    {
                        case "Confort":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.Comfort.Add(node.InnerText);
                            break;
                        case "Acabado Exterior":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.ExteriorFinish.Add(node.InnerText);
                            break;
                        case "Multimedia y Audio":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.MultimediaAudio.Add(node.InnerText);
                            break;
                        case "Seguridad":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.Security.Add(node.InnerText);
                            break;
                        case "Acabado Interior":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.InteriorFinish.Add(node.InnerText);
                            break;
                        case "Ficha Técnica":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.TechnicalSheet.Add(node.InnerText);
                            break;
                        case "Información Básica":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.BasicInformation.Add(node.InnerText);
                            break;
                        case "Otros extras":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.OtherExtras.Add(node.InnerText);
                            break;
                        case "Datos no clasificados":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.UnclassifiedData.Add(node.InnerText);
                            break;
                        case "Dimensiones":
                            foreach (HtmlNode node in datanode)
                                car.CarExtras.Dimensions.Add(node.InnerText);
                            break;
                        default:
                            MessageBox.Show("Extras." + type);
                            break;
                    }
                    break;
                default:
                    break;
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
                case "Motor y transmisión":
                    foreach (HtmlNode node in datanode)
                    {
                        string property = node.SelectSingleNodeByClass("mt-ListModelDetails-tableItem").InnerText;
                        string value = node.SelectSingleNodeByClass("mt-ListModelDetails-tableItem--strong").InnerText;
                        car.EngineProperty(property, value);
                    }
                    break;
                case "Consumo y prestaciones":
                    foreach (HtmlNode node in datanode)
                    {
                        string property = node.SelectSingleNodeByClass("mt-ListModelDetails-tableItem").InnerText;
                        string value = node.SelectSingleNodeByClass("mt-ListModelDetails-tableItem--strong").InnerText;
                        car.ConsumptionProperty(property, value);
                    }
                    break;
            }
        }

        private class Car
        {
            private bool favourite;
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
            private Engine engine = new Engine();
            private Consumption consumption = new Consumption();
            private StandardEquipment equipment = new StandardEquipment();
            private Extras extras = new Extras();

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
            public Engine CarEngine { get => engine; set => engine = value; }
            public Consumption CarConsumption { get => consumption; set => consumption = value; }
            public StandardEquipment CarStandardEquipment { get => equipment; set => equipment = value; }
            public Extras CarExtras { get => extras; set => extras = value; }
            public bool Favourite { get => favourite; set => favourite = value; }

            public class Model
            {
                private string capacity;
                private string version;
                private string doors;
                private string dimensions;
                private string trunk;
                private string seats;
                private string weight;
                private string maxHeight;
                private string totalDistance;

                public string Capacity { get => capacity; set => capacity = value; }
                public string Version { get => version; set => version = value; }
                public string Doors { get => doors; set => doors = value; }
                public string Dimensions { get => dimensions; set => dimensions = value; }
                public string Trunk { get => trunk; set => trunk = value; }
                public string Seats { get => seats; set => seats = value; }
                public string Weight { get => weight; set => weight = value; }
                public string MaxHeight { get => maxHeight; set => maxHeight = value; }
                public string TotalDistance { get => totalDistance; set => totalDistance = value; }
            }

            public class Engine
            {
                private string gearbox;
                private string fuel;
                private string maxPower;
                private string feed;
                private string configuration;
                private string watts;
                private string maxElectricalPower;
                private string electricalAutonomy;
                private string batteryCapacity;
                private string electricalEngineType;
                private string electricalConnectorType;
                private string averageElectricalConsumption;
                private string chargingTime;
                private string fastChargingTime88;
                private string fastChargingTime100;
                private string chargingAmperage;
                private string chargingVoltage;
                private string fastChargingTime92;
                private string fastChargingVoltage;
                private string fastChargingTime;

                public string Gearbox { get => gearbox; set => gearbox = value; }
                public string Fuel { get => fuel; set => fuel = value; }
                public string MaxPower { get => maxPower; set => maxPower = value; }
                public string Feed { get => feed; set => feed = value; }
                public string Configuration { get => configuration; set => configuration = value; }
                public string Watts { get => watts; set => watts = value; }
                public string MaxElectricalPower { get => maxElectricalPower; set => maxElectricalPower = value; }
                public string ElectricalAutonomy { get => electricalAutonomy; set => electricalAutonomy = value; }
                public string BatteryCapacity { get => batteryCapacity; set => batteryCapacity = value; }
                public string ElectricalEngineType { get => electricalEngineType; set => electricalEngineType = value; }
                public string ElectricalConnectorType { get => electricalConnectorType; set => electricalConnectorType = value; }
                public string AverageElectricalConsumption { get => averageElectricalConsumption; set => averageElectricalConsumption = value; }
                public string ChargingTime { get => chargingTime; set => chargingTime = value; }
                public string FastChargingTime100 { get => fastChargingTime100; set => fastChargingTime100 = value; }
                public string ChargingAmperage { get => chargingAmperage; set => chargingAmperage = value; }
                public string FastChargingTime88 { get => fastChargingTime100; set => fastChargingTime100 = value; }
                public string ChargingVoltage { get => chargingVoltage; set => chargingVoltage = value; }
                public string FastChargingTime92 { get => fastChargingTime92; set => fastChargingTime92 = value; }
                public string FastChargingVoltage { get => fastChargingVoltage; set => fastChargingVoltage = value; }
                public string FastChargingTime { get => fastChargingTime; set => fastChargingTime = value; }
            }

            public class Consumption
            {
                private string traction;
                private string acceleration;
                private string maxSpeed;
                private string urbanConsumption;
                private string extraUrbanConsumption;
                private string averageConsumption;

                public string Traction { get => traction; set => traction = value; }
                public string Acceleration { get => acceleration; set => acceleration = value; }
                public string MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
                public string UrbanConsumption { get => urbanConsumption; set => urbanConsumption = value; }
                public string ExtraUrbanConsumption { get => extraUrbanConsumption; set => extraUrbanConsumption = value; }
                public string AverageConsumption { get => averageConsumption; set => averageConsumption = value; }
            }

            public class StandardEquipment
            {
                List<string> technicalSheet = new List<string>();
                List<string> multimediaAudio = new List<string>();
                List<string> exteriorFinish = new List<string>();
                List<string> security = new List<string>();
                List<string> basicInformation = new List<string>();
                List<string> comfort = new List<string>();
                List<string> dimensions = new List<string>();
                List<string> interiorFinish = new List<string>();
                List<string> capability = new List<string>();

                public List<string> TechnicalSheet { get => technicalSheet; set => technicalSheet = value; }
                public List<string> MultimediaAudio { get => multimediaAudio; set => multimediaAudio = value; }
                public List<string> ExteriorFinish { get => exteriorFinish; set => exteriorFinish = value; }
                public List<string> Security { get => security; set => security = value; }
                public List<string> BasicInformation { get => basicInformation; set => basicInformation = value; }
                public List<string> Comfort { get => comfort; set => comfort = value; }
                public List<string> Dimensions { get => dimensions; set => dimensions = value; }
                public List<string> InteriorFinish { get => interiorFinish; set => interiorFinish = value; }
                public List<string> Capability { get => capability; set => capability = value; }
            }

            public class Extras
            {
                List<string> otherExtras = new List<string>();
                List<string> comfort = new List<string>();
                List<string> exteriorFinish = new List<string>();
                List<string> multimediaAudio = new List<string>();
                List<string> security = new List<string>();
                List<string> interiorFinish = new List<string>();
                List<string> technicalSheet = new List<string>();
                List<string> basicInformation = new List<string>();
                List<string> unclassifiedData = new List<string>();
                List<string> dimensions = new List<string>();

                public List<string> Comfort { get => comfort; set => comfort = value; }
                public List<string> ExteriorFinish { get => exteriorFinish; set => exteriorFinish = value; }
                public List<string> OtherExtras { get => otherExtras; set => otherExtras = value; }
                public List<string> MultimediaAudio { get => multimediaAudio; set => multimediaAudio = value; }
                public List<string> Security { get => security; set => security = value; }
                public List<string> InteriorFinish { get => interiorFinish; set => interiorFinish = value; }
                public List<string> TechnicalSheet { get => technicalSheet; set => technicalSheet = value; }
                public List<string> BasicInformation { get => basicInformation; set => basicInformation = value; }
                public List<string> UnclassifiedData { get => unclassifiedData; set => unclassifiedData = value; }
                public List<string> Dimensions { get => dimensions; set => dimensions = value; }
            }

            public void ModelProperty(string property, string value)
            {
                switch (property)
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
                    case "Altura máxima":
                        CarModel.MaxHeight = value;
                        break;
                    case "Distancia total":
                        CarModel.TotalDistance = value;
                        break;
                    default:
                        MessageBox.Show("Model." + property);
                        break;
                }
            }

            public void EngineProperty(string property, string value)
            {
                switch(property.Trim())
                {
                    case "Caja de cambios":
                        CarEngine.Gearbox = value;
                        break;
                    case "Combustible":
                        CarEngine.Fuel = value;
                        break;
                    case "Potencia máxima":
                        CarEngine.MaxPower = value;
                        break;
                    case "Alimentación":
                        CarEngine.Feed = value;
                        break;
                    case "Configuración":
                        CarEngine.Configuration = value;
                        break;
                    case "Wattios motor":
                        CarEngine.Watts = value;
                        break;
                    case "Wattio":
                        CarEngine.Watts = value;
                        break;
                    case "Potencia máxima eléctrica":
                        CarEngine.MaxElectricalPower = value;
                        break;
                    case "Consumo medio eléctrico WLTP":
                        CarEngine.AverageElectricalConsumption = value;
                        break;
                    case "Autonomía eléctrica WLTP":
                        CarEngine.ElectricalAutonomy = value;
                        break;
                    case "Capacidad de la batería":
                        CarEngine.BatteryCapacity = value;
                        break;
                    case "Tipo de motor eléctrico":
                        CarEngine.ElectricalEngineType = value;
                        break;
                    case "Tipo de conector eléctrico":
                        CarEngine.ElectricalConnectorType = value;
                        break;
                    case "Tiempo de carga":
                        CarEngine.ChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (100.0 kW)":
                        CarEngine.FastChargingTime100 = value;
                        break;
                    case "Amperaje de carga":
                        CarEngine.ChargingAmperage = value;
                        break;
                    case "Tiempo de carga rápida (88.0 kW)":
                        CarEngine.FastChargingTime88 = value;
                        break;
                    case "Voltaje de carga":
                        CarEngine.ChargingVoltage = value;
                        break;
                    case "Tiempo de carga rápida (92.0 kW)":
                        CarEngine.FastChargingTime92 = value;
                        break;
                    case "Voltaje de carga rápida":
                        CarEngine.FastChargingVoltage = value;
                        break;
                    case "Tiempo de carga rápida":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (74.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (115.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (150.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (22.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (101.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (102.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (135.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (83.8 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (240.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (153.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (130.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (350.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (50.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (200.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (90.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (160.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (180.0 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    case "Tiempo de carga rápida (2.3 kW)":
                        CarEngine.FastChargingTime = value;
                        break;
                    default:
                        MessageBox.Show("Engine." + property);
                        break;
                }
            }

            public void ConsumptionProperty(string property, string value)
            {
                switch(property)
                {
                    case "Tracción":
                        CarConsumption.Traction = value;
                        break;
                    case "Aceleración (0-100km/h)":
                        CarConsumption.Acceleration = value;
                        break;
                    case "Velocidad Máxima":
                        CarConsumption.MaxSpeed = value;
                        break;
                    case "Consumo urbano":
                        CarConsumption.UrbanConsumption = value;
                        break;
                    case "Consumo extraurbano":
                        CarConsumption.ExtraUrbanConsumption = value;
                        break;
                    case "Consumo medio":
                        CarConsumption.AverageConsumption = value;
                        break;
                    default:
                        MessageBox.Show("Consumption." + property);
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
