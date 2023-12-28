using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace WindowsFormsApp1
{
    public partial class Web : Form
    {
        public Web()
        {
            InitializeComponent();
            webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(Loaded);
            webBrowser1.Navigate("www.google.com");
        }

        private void Loaded(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(webBrowser1.Url.Host.Equals("www.google.com"))
            {
                
                var asdopg  = webBrowser1.Document.GetElementById("id").GetElementsByTagName("textarea");
            }
        }
    }
}
