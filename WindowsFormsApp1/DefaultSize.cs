using Org.BouncyCastle.Asn1.Sec;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace WindowsFormsApp1
{
    internal class DefaultSize
    {
        public static void Show()
        {
            string path = Path.Combine(Application.StartupPath, "Result", "DefaultSize.txt");
            List<ControlProperties> l = new List<ControlProperties>();
            l.Add(new ControlProperties(new ProgressBar()));
            l.Add(new ControlProperties(new TextBox()));
            File.Delete(path);
            File.WriteAllText(path, JsonConvert.SerializeObject(l,Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        
        }

        private class ControlProperties
        {
    
            public ControlProperties(object c)
            {
                control = c;
            }

            private object control;
            private string name;
            private string height;
            private string width;
            private string x;
            private string y;

            public object Control { get => control; set => control = value; }
            public string Name => control.GetType().Name;
            public string Height => GetPropertyString(control, "Height");
            public string Width => GetPropertyString(control, "Width");
            public string X => GetPropertyString(control, "Location.X");
            public string Y => GetPropertyString(control, "Location.Y");

        }

        private static string GetPropertyString(object src, string propName)
        {
            object o = GetPropertyValue(src, propName);
            return o == null ? string.Empty : o.ToString();
        }

        private static object GetPropertyValue(object src, string propName)
        {
            if(src == null) return null;

            if (propName.Contains("."))//complex type nested
            {
                var temp = propName.Split(new char[] { '.' }, 2);
                return GetPropertyValue(GetPropertyValue(src, temp[0]), temp[1]);
            }
            else
            {
                var prop = src.GetType().GetProperty(propName);
                return prop != null ? prop.GetValue(src, null) : null;
            }
        }

    }
}

