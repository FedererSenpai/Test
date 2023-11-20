using Microsoft.Win32;
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
            if (args.Length == 0)
            {
                Dibal.EST();
                Application.Run(new FindFile());
            }
            else
            {
                switch (args[0])
                {
                    case "Cursor":
                        Application.Run(new Cursor());
                        break;
                    case "Assembly":
                        Application.Run(new Assembly());
                        break;
                    default:
                        Application.Exit();
                        break;
                }
            }
        }

    }
}
