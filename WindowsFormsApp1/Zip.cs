using Renci.SshNet.Messages;
using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.WebParts;
using System.Windows;

namespace WindowsFormsApp1
{
    internal static class Zip
    {
        public static void ZipFiles(string[] sourceFiles)
        {
            SevenZipBase.SetLibraryPath(Path.Combine(
Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory,
"Librerias", "7z.dll"));
            
            sourceFiles = new string[1] { "C:\\Daniel\\VS\\Test\\WindowsFormsApp1\\bin\\Debug\\ZipSample\\Nuevo documento de texto.zip" };
            
            if (Path.GetExtension(sourceFiles[0]).Equals(".zip"))
            {
                Password pwform = new Password();
                pwform.ShowDialog();
                string passtext = pwform.Pass;
                pwform.Close();
                
                SevenZipExtractor extractor = new SevenZipExtractor(sourceFiles[0], passtext);
                extractor.ExtractArchive(Path.GetDirectoryName(sourceFiles[0]));
            }

            if (sourceFiles.Length == 0)
                return;

            sourceFiles = new string[1] { "C:\\Daniel\\VS\\Test\\WindowsFormsApp1\\bin\\Debug\\ZipSample\\Nuevo documento de texto.txt" };

            SevenZipCompressor compressor = new SevenZipCompressor();
            Password pw = new Password('+');
            pw.ShowDialog();
            string password = pw.Pass;
            pw.Close();
            string destinationFile = Path.Combine(Path.GetDirectoryName(sourceFiles[0]),Path.GetFileNameWithoutExtension(sourceFiles[0]) + ".zip");

            if (String.IsNullOrWhiteSpace(password))
            {
                compressor.CompressFiles(destinationFile, sourceFiles);
            }
            else
            {
                //optional
                compressor.EncryptHeaders = true;
                compressor.CompressFilesEncrypted(destinationFile, password, sourceFiles);
            }
        }

    }
}
