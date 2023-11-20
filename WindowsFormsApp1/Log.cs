using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public static class Log
    {
        private static Mutex accesoSerializado = new Mutex();
        private static string rutalog = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Cursor.log");
        private static string rutaerror = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Error.log");
        public static void EscribirError(string sPilaLlamadas, string sMensajeError)
        {
            //si no hay ruta puesta en el config, significa q no quiere guardar en logs de la aplicación
            bool liberarMutex = false;
            try
            {
                if (accesoSerializado.WaitOne(1000, false))
                {
                    liberarMutex = true;
                    if (!string.IsNullOrEmpty(rutaerror))
                    {
                        FileInfo oFileInfo = new FileInfo(rutaerror);
                        if (oFileInfo.Exists)
                        {
                            if (oFileInfo.Length / 1024 / 1024 >= (long)50)
                            {
                                oFileInfo.CopyTo(rutaerror.Replace(".log", DateTime.Now.ToString("ddMMyyyyHHmmss") + ".old"), true);
                                oFileInfo.Delete();
                            }
                        }
                        StreamWriter oStreamWriter;
                        oStreamWriter = File.AppendText(rutaerror);
                        //montamos el mensaje a escribir en el log
                        StringBuilder sMensaje = new StringBuilder();
                        sMensaje.AppendLine("\r" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToString("HH:mm:ss:fff"));
                        sMensaje.AppendLine("Mensaje de Error: " + sMensajeError);
                        if (!string.IsNullOrEmpty(sPilaLlamadas))
                        {
                            sMensaje.AppendLine("Pila de llamadas: ");
                            sMensaje.AppendLine(sPilaLlamadas);
                        }
                        sMensaje.AppendLine("-----------------------------------------------------------------------");
                        //Escribe una linea de texto
                        oStreamWriter.WriteLine(sMensaje.ToString());
                        oStreamWriter.Flush();
                        //cerramos el archivo de texto
                        oStreamWriter.Close();
                        accesoSerializado.ReleaseMutex();
                        liberarMutex = false;
                    }
                }
            }
            catch { }
            finally
            {
                if (liberarMutex)
                {
                    accesoSerializado.ReleaseMutex();
                }
            }
        }

        public static void EscribirLog(string mensaje)
        {
            bool liberarMutex = false;
            try
            {

                if (accesoSerializado.WaitOne(3000, false))
                {
                    liberarMutex = true;
                    FileInfo oFileInfo = new FileInfo(rutalog);
                    if (oFileInfo.Exists)
                    {
                        if (oFileInfo.Length / 1024 / 1024 >= (long)50)
                        {
                            oFileInfo.CopyTo(rutalog.Replace(".log", DateTime.Now.ToString("ddMMyyyyHHmmss") + ".old") , true);
                            oFileInfo.Delete();
                        }
                    }
                    StreamWriter sw = new StreamWriter(rutalog, true, Encoding.Default);
                    string linea = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss:fff") + ":  " + mensaje;
                    sw.WriteLine(linea);
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.EscribirError(ex.StackTrace, ex.Message);
            }
            finally
            {
                if (liberarMutex)
                    accesoSerializado.ReleaseMutex();
            }
        }

    }
}
