using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.IO;
using HtmlAgilityPack;
using Org.BouncyCastle.Asn1.Crmf;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace WindowsFormsApp1
{
    static class ExtensionMethods
    {
        private enum weekday
        {
            D = 0,
            L = 1,
            M = 2,
            X = 3,
            J = 4,
            V = 5,
            S = 6
        }

        public static List<T> ToList<T>(this DataRow[] datarows) 
            where T : new()
        {
            List<T> list = new List<T>();
            foreach(DataRow dr in datarows)
            {
                list.Add(dr.ToObject<T>());
            }
            return list;
        }

        public static T ToObject<T>(this DataRow dataRow)
    where T : new()
        {
            T item = new T();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                PropertyInfo property = GetProperty(typeof(T), column.ColumnName);

                if (property != null && dataRow[column] != DBNull.Value && dataRow[column].ToString() != "NULL")
                {
                    property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
                }
            }

            return item;
        }

        private static PropertyInfo GetProperty(Type type, string attributeName)
        {
            PropertyInfo property = type.GetProperty(attributeName);

            if (property != null)
            {
                return property;
            }

            return type.GetProperties()
                 .Where(p => p.IsDefined(typeof(DisplayAttribute), false) && p.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == attributeName)
                 .FirstOrDefault();
        }

        public static object ChangeType(object value, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
            }

            return Convert.ChangeType(value, type);
        }

        public static bool AreAllSame<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

            using (var enumerator = enumerable.GetEnumerator())
            {
                var toCompare = default(T);
                if (enumerator.MoveNext())
                {
                    toCompare = enumerator.Current;
                }

                while (enumerator.MoveNext())
                {
                    if (toCompare != null && !toCompare.Equals(enumerator.Current))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static string ToString (this DateTime dt, int format)
        {
            string datetime = string.Empty;
            switch(format)
            {
                case 0:
                    datetime = ((weekday)(int)dt.DayOfWeek).ToString() + dt.ToString("dd HH:mm:ss");
                    break;
            }
            return datetime;
        }

        public static int NextRow(this _Worksheet w, int column) 
        {
            int row = 0;
            for (int i = 1; i < 1000; i++)
            {
                if (w.Cells[i, column].Value == null)
                {
                    row = i;
                    break;
                }
                    
            }
            return row;
        }

        public static List<T> JsonToList<T>(this string s)
        {
            return JsonConvert.DeserializeObject<List<T>>(s);
        }

        public static string ToJson<T>(this List<T> l)
        {
            return JsonConvert.SerializeObject(l);
        }

        public static void ToFile(this string content, string filePath)
        {
            WriteToFile(filePath, content);
        }

        public static void WriteToFile(string filePath, string content)
        {
            System.IO.FileInfo file = new System.IO.FileInfo(filePath);
            file.Directory.Create(); // If the directory already exists, this method does nothing.
            System.IO.File.WriteAllText(file.FullName, content);
        }

        public static string CheckFileName(this string file)
        {
            if (file.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
            {
                foreach(char c in Path.GetInvalidFileNameChars())
                {
                    file = file.Replace(c.ToString(), string.Empty);
                }
            }
            return file;
        }

        public static int Count(this HtmlNode node, string exp)
        {
            if (node.SelectSingleNode(".//td[@width='84%']") != null)
                return node.SelectNodes(".//td[@width='84%']").Count();
            else
                return 0;
        }

        public static int Distance(string s1, string s2)
        {
            if (s1.Length == 0)
            {
                return s2.Length;
            }
            else if (s2.Length == 0)
            {
                return s1.Length;
            }
            else if (s1[0] == s2[0])
            {
                return Distance(s1.Substring(1), s2.Substring(1));
            }
            else
            {
                return 1 - Enumerable.Min(new int[] { Distance(s1.Substring(1), s2), Distance(s2.Substring(1), s1), Distance(s1.Substring(1), s2.Substring(1)) });
            }
        }

        public static void ToFile<T>(this List<T> l, string path)
        {
            l.ToJson().ToFile(path);
        }

        public static string TrimStart(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        public static string TrimEnd(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;

            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, target.Length - trimString.Length);
            }

            return result;
        }

        public static List<List<T>> Split<T>(this List<T> list, int size)
        {
            List<List<T>> bigList = new List<List<T>>();
            int max = size;
            int i = 0;
            while(i<list.Count)
            {
                List<T> smallList = new List<T>();
                if (max > list.Count)
                    max = list.Count;
                while(i < max)
                {
                    smallList.Add(list[i]);
                    i++;
                }
                bigList.Add(smallList);
                max += size;
            }
            return bigList;
        }

        public static void Center(this Control c)
        {
            if (c.Parent == null)
                return;
            c.Location = new System.Drawing.Point((c.Parent.Width - c.Width) / 2 , (c.Parent.Height - c.Height) / 2);
        }

        public static bool isOK(this Form f)
        {
            return f.ShowDialog() == DialogResult.OK;
        }

        public static string FirstLetterToUpperCase(this string s)
        {
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static void AutoResize(this Control c)
        {
            if (c is ComboBox)
            {
                ComboBox cb = c as ComboBox;
                Graphics g = cb.CreateGraphics();
                int max = cb.Width;
                foreach (object o in (List<string>)cb.DataSource)
                {
                    float width = g.MeasureString(cb.GetItemText(o), cb.Font).Width;
                    if (width > max)
                        max = (int)width;
                }
                cb.Width = max;
            }
        }

        public static int ToInt(this DayOfWeek dow)
        {
            return (int)(dow + 6) % 7;
        }
    }
}
