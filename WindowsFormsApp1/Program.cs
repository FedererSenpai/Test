﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if KPOOP
            Application.Run(new Web());
#else
            if (args.Length == 0)
            {
                Application.Run(new MainWindow());
            }
            else
            {
                switch (args[0])
                {
                    case "Cursor":
                        Application.Run(new Cursor());
                        break;
                    case "Assembly":
                        Application.Run(new UpdateAssembly());
                        break;
                    case "Activity":
                        if (args[1].Equals("/a"))
                            Activity.WriteStart();
                        else if (args[1].Equals("/z"))
                            Activity.WriteShutDown();
                        else
                            Application.Run(new DibalForm(0));
                        break;
                    case "MAL":
                        Application.Run(new MAL());
                        break;
                    case "Insert":
                        Application.Run(new Insert());
                        break;
                    default:
                        Zip.ZipFiles(args);
                        Application.Exit();
                        break;
                }
            }
#endif
        }

   }
}
