using System;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;

namespace WindowsFormsApp1
{
    public class Beeper
    {

        private static bool comprobadoX5CPU = false;
        private static bool isX5CPU = false;
        private static int freq = 800;
        private static int dur = 50;

        [DllImport("inpout32.dll")]
        private extern static void Out32(short PortAddress, short Data);
        [DllImport("inpout32.dll")]
        private extern static char Inp32(short PortAddress);

        public static void Beep(int freqHz, int ms)
        {
            try
            {
                if (IsX5CPU)
                {
                    Console.Beep(freqHz, ms);
                }
                else
                {
                    int freqNumber = 1193180 / freqHz;
                    // Preparar el zumbador
                    Out32(0x43, 0xB6);
                    // Enviar la frecuencia, el byte menos significativo primero
                    Out32(0x42, (Byte)(freqNumber & 0xFF));
                    Out32(0x42, (Byte)(freqNumber >> 8));
                    Thread.Sleep(10);
                    // Para iniciar el pitido, poner a 1 los bits 0 y 1 del valor en el puerto 0x61
                    Out32(0x61, (Byte)(Convert.ToByte(Inp32(0x61)) | 0x03));
                    Thread.Sleep(ms);
                    // Parar el pitido poniendo a 0 los bits 0 y 1 del valor en el puerto 0x61
                    Out32(0x61, (Byte)(Convert.ToByte(Inp32(0x61)) & 0xFC));
                }

            }
            catch
            {
                // No hacer nada
            }
        }

        public static void Beep()
        {
            Beep(800, 20); // Equivalente a CommonData.Beeper.Beep() 
        }


        public static bool IsX5CPU
        {
            get
            {
                if (comprobadoX5CPU == false)
                {
                    ManagementObjectSearcher mosQuery = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                    ManagementObjectCollection queryCollection1 = mosQuery.Get();
                    foreach (ManagementObject manObject in queryCollection1)
                    {
                        if (Convert.ToString(manObject["Name"]).ToLower().Contains("x5-z8350"))
                            isX5CPU = true;
                        else
                            isX5CPU = false;
                        break;
                    }
                    comprobadoX5CPU = true;
                }
                return isX5CPU;
            }

        }

        public static void BeepAsync(int _freq, int _dur)
        {
            freq = _freq;
            dur = _dur;
            Thread hilo = new Thread(MetodoBeepAsync);
            hilo.Start();
        }

        public static void MetodoBeepAsync()
        {
            Beep(freq, dur);
        }
    }
}
