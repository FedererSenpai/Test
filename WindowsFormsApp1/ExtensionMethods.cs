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
    }
}
