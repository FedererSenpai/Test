using CefSharp;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    internal static class Performance
    {
        private static List<PerformanceResult> result = new List<PerformanceResult>();
        static string s1 = "kitten";
        static string s2 = "sitting";
        static string s3 = "uninformed";
        static string s4 = "uniformed";
        public static void Compare()
        {
            var asdf = typeof(Performance).GetMethods();
            foreach (Action a in typeof(Performance).GetMethods().Where(x => x.Name.Length == 1).Select(x=>x.CreateDelegate(typeof(Action))))
                Run(a);
            ExtensionMethods.WriteToFile(Path.Combine(Application.StartupPath, "Result", "Performance.json"), JsonConvert.SerializeObject(result));
        }

        private static void Run(Action action)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            action.Invoke();
            sw.Stop();
            result.Add(new PerformanceResult() { Name = action.Method.Name, Time = sw.Elapsed });
        }

        public static void B()
        {
            int result = 0;
            for (int i = 0; i < 100000; i++)
            {
                if (s1.Length == 0)
                    result = s2.Length;
                else if (s2.Length == 0)
                    result = s1.Length;
                if (s3.Length == 0)
                    result = s4.Length;
                else if (s4.Length == 0)
                    result = s3.Length;
            }
        }

        public static void A()
        {
            int result = 0;
            for (int i = 0; i < 100000; i++)
            {
                if (Math.Min(s1.Length, s2.Length) == 0)
                {
                    result = Math.Max(s1.Length, s2.Length);
                }
                if (Math.Min(s3.Length, s4.Length) == 0)
                {
                    result = Math.Max(s3.Length, s4.Length);
                }

            }
        }
    }

    public class PerformanceResult
    {
        private string name;
        private TimeSpan time;

        public string Name { get => name; set => name = value; }
        public TimeSpan Time { get => time; set => time = value; }
    }
}
