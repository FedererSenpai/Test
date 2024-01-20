using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class Run
    {
        public static void Execute()
        {
            bool exit = true;
            switch(System.AppDomain.CurrentDomain.FriendlyName)
            {
                case "Test.exe":
                    RunTest();
                        break;
                case "Planning.exe":
                    RunPlanning();
                    break;
                case "Activity.exe":
                    RunActivity();
                    break;
                default:
                    exit = false;
                    break;
            }
            if (exit)
                Environment.Exit(0);
        }

        private static void RunTest()
        {
            RunProgram(@"C:\Daniel\VS\Test\WindowsFormsApp1.sln");
        }

        private static void RunPlanning()
        {
            foreach (string file in Directory.GetFiles(@"T:\32.-Planning\04 Plan semanal", "SAR.*.Plan semanal.xlsx", SearchOption.TopDirectoryOnly))
                File.Copy(file, @"C:\Users\dzhang\Desktop\ControlTareasSemana_Windows.xlsx", true);
            RunProgram(@"C:\Users\dzhang\Desktop\ControlTareasSemana_Windows.xlsx");
        }

        private static void RunActivity()
        {
            string tmp = Path.Combine(Path.GetTempPath(), "Activitytmp.xlsx");
            File.Copy(@"C:\Daniel\VS\Test\WindowsFormsApp1\bin\Activity\resources\Activity.xlsx", tmp, true);
            RunProgram(tmp);
        }

        private static void RunProgram(string name)
        {
            Process p = new Process();
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = name;
            p.StartInfo = ps;
            p.Start();
        }
    }
}
