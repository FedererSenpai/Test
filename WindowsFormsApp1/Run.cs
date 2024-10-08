﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                case "MigracionDatos.exe":
                    RunMigracionDatos();
                    break;
                case "ClearGit.exe":
                    RunClearGit();
                    break;
                case "Vacaciones.exe":
                    RunVacaciones();
                    break;
                case "MysqlService.exe":
                    RunMysqlService();
                    break;
                case "Testing.exe":
                    RunTesting();
                    break;
                default:
                    exit = false;
                    break;
            }
            if (exit)
                Environment.Exit(0);
        }

        private static void RunMigracionDatos()
        {
            RunProgram(@"C:\MigraciónDatos\MigracionDatos.exe");
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

        private static void RunClearGit()
        {
            FolderBrowserDialog d = new FolderBrowserDialog();
            d.ShowNewFolderButton = false;
            d.SelectedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source", "repos");
            if (d.ShowDialog() == DialogResult.OK)
            {
                string folder = d.SelectedPath;
                Process p = new Process();
                ProcessStartInfo ps = new ProcessStartInfo("cmd.exe", $"/C \"cd {folder}\" & git stash");
                ps.WorkingDirectory = folder;
                ps.UseShellExecute = false;
                p.StartInfo = ps;
                p.Start();
                p.WaitForExit(100000);
            }
        }

        private static void RunVacaciones()
        {
            File.Copy(@"U:\000_INFORMACIÓN PERMANENTE\00 CONSERVAR-RESPONSABLE Inmaculada Lopez-REORGANIZACIÓN INFO\99 99 VACACIONES\0-Vacaciones 2024.xlsx", @"C:\Users\dzhang\Desktop\Vacaciones.xlsx", true);
            RunProgram(@"C:\Users\dzhang\Desktop\Vacaciones.xlsx");
        }

        private static void RunMysqlService()
        {
            bool start = true;
            bool running = false;
            while (!running)
            {
                List<ServiceController> lsc = ServiceController.GetServices().Where(s => s.ServiceName.Contains("MySQL")).ToList();
                foreach (ServiceController sc in lsc)
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        start = false;
                        running = true;
                        break;
                    }
                }
                if (start)
                {
                    lsc.First().Start();
                }
                System.Threading.Thread.Sleep(5000);
            }
        }

        private static void RunTesting()
        {
            Process.Start(@"C:\Daniel\VS\Test\Testing\bin\Release\Testing.exe");
        }
    }
}
