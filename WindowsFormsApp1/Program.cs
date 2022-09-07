using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Insert());
        }

        private static void Round()
        {
            if (!File.Exists(@"C:\Users\dzhang\Desktop\Daniel\Archivos\Round.txt"))
                File.Create(@"C:\Users\dzhang\Desktop\Daniel\Archivos\Round.txt");
            StringBuilder sw = new StringBuilder();
            sw.AppendLine("MidpointRounding.AwayFromZero: Cuando un número está comprendido entre otros dos, se redondea hacia el númeromás cercano y más alejado de cero.");
            sw.AppendLine();
            for (int i = 0; i < 10; i++)
            {
                sw.AppendLine($"Valor: {(decimal)50.5 + i / (decimal)100}");
                sw.AppendLine($"Valor Redondeado: {Math.Round((decimal)50.5 + i / (decimal)100, 1, MidpointRounding.AwayFromZero)}");
            }
            sw.AppendLine();
            sw.AppendLine("---------------------------------------------------------");
            sw.AppendLine();
            sw.AppendLine("MidpointRounding.ToEven: Cuando un número está comprendido entre otros dos, se redondea hacia el número par más cercano.");
            sw.AppendLine();
            for (int i = 0; i < 10; i++)
            {
                sw.AppendLine($"Valor: {(decimal)50.5 + i / (decimal)100}");
                sw.AppendLine($"Valor Redondeado: {Math.Round((decimal)50.5 + i / (decimal)100, 1, MidpointRounding.ToEven)}");
            }
            sw.AppendLine();
            sw.AppendLine();
            sw.AppendLine();
            sw.AppendLine("---------------------------------------------------------");
            sw.AppendLine();
            for (int i = 0; i < 10; i++)
            {
                sw.AppendLine($"Valor: {(decimal)50.4 + i / (decimal)100}");
                sw.AppendLine($"Valor Redondeado: {Math.Round((decimal)50.4 + i / (decimal)100, 1, MidpointRounding.AwayFromZero)}");
            }
            sw.AppendLine();
            sw.AppendLine("---------------------------------------------------------");
            sw.AppendLine();
            sw.AppendLine("MidpointRounding.ToEven: Cuando un número está comprendido entre otros dos, se redondea hacia el número par más cercano.");
            sw.AppendLine();
            for (int i = 0; i < 10; i++)
            {
                sw.AppendLine($"Valor: {(decimal)50.4 + i / (decimal)100}");
                sw.AppendLine($"Valor Redondeado: {Math.Round((decimal)50.4 + i / (decimal)100, 1, MidpointRounding.ToEven)}");
            }
            File.WriteAllText(@"C:\Users\dzhang\Desktop\Daniel\Archivos\Round.txt", sw.ToString());
        }
    }
}
