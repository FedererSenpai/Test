using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using System.Windows.Forms;
using System.IO;
using System.Data;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.Globalization;

namespace WindowsFormsApp1
{
    public static class Activity
    {
         static string path;
        static DataSet datosExcel;
        public static void WriteStart()
        {
            Write("StartUp");
        }

        public static void WriteShutDown()
        {
            Write("ShutDown");
        }

        private static void Write(string type)
        {
            path = Path.Combine(Application.StartupPath, "resources", "Activity.xlsx");
            if (!File.Exists(path))
                WriteResourceToFile(Properties.Resources.Activity, path);
            IExcelDataReader excelReader = null;
                string fileExtension = Path.GetExtension(path);
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                if (fileExtension == ".xls")
                {
                    excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (fileExtension == ".xlsx")
                {
                    excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

            datosExcel = excelReader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });
            }
            DateTime dt = DateTime.Now;
            string year = dt.Year.ToString();
            string month = dt.ToString("MMMM", CultureInfo.CreateSpecificCulture("es"));
            if (!datosExcel.Tables.Contains(year))
            {
                CreateExcelSheet(path, year);
                CreateExcelColumn(path, year, month);
            }
            else if (!datosExcel.Tables[year].Columns.Contains(month))
            {
                CreateExcelColumn(path, year, month);
            }
            WriteExcelCell(path, year, dt.Month, type);
        }

        private static void WriteExcelCell(string FileName, string sheetName, int columnName, string cellValue)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(FileName);
                xlWorksheet = (Excel.Worksheet)xlWorkbook.Sheets[sheetName];
                Excel.Range xlRange = xlWorksheet.UsedRange;

                int column = xlRange.Columns.Count - 1;
                int row = xlWorksheet.NextRow(column);
                xlRange = xlWorksheet.Cells[row, column];
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.Value = cellValue;

                xlRange = xlWorksheet.Cells[row, column + 1];
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.Value = DateTime.Now.ToString(0);

                xlWorksheet.Columns.AutoFit();
            }
            finally
            {
                if (xlWorkbook != null)
                {
                    xlWorkbook.Close(true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                }
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
        }

        private static void CreateExcelColumn(string FileName, string sheetName, string columnName)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(FileName);
                xlWorksheet = (Excel.Worksheet)xlWorkbook.Sheets[sheetName];
                Excel.Range xlRange = xlWorksheet.UsedRange;
                if(xlRange.Count == 1)
                {
                    xlRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[1, 1], xlWorksheet.Cells[1, 2]];
                }
                else
                {
                    int column = xlRange.Columns.Count;
                    xlRange = (Excel.Range)xlWorksheet.Range[xlWorksheet.Cells[1, column + 1], xlWorksheet.Cells[1, column + 2]];
                }
                xlRange.Merge();
                xlRange.Font.Bold = true;
                xlRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                xlRange.BorderAround2(Weight: Excel.XlBorderWeight.xlThick);
                xlRange.Value = columnName.ToUpper();
            }
            finally
            {
                if (xlWorkbook != null)
                {
                    xlWorkbook.Close(true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                }
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
        }

        private static void CreateExcelSheet(string FileName, string sheetName)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(FileName);
                xlWorksheet = (Excel.Worksheet)xlWorkbook.ActiveSheet;
                if (xlWorkbook.Sheets.Count == 1 && xlWorksheet.UsedRange.Count == 1 && xlWorksheet.UsedRange.Value == null)
                {
                    xlWorksheet.Name = sheetName;
                }
                else
                {
                    xlWorkbook.Sheets.Add();
                    xlWorksheet = xlWorkbook.Sheets[xlWorkbook.Sheets.Count];
                    xlWorksheet.Name = sheetName;
                }
                Excel.Range xlRange = xlWorksheet.UsedRange;
            }
            finally
            {   
                 if (xlWorkbook != null)
                {
                    xlWorkbook.Close(true);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkbook);
                }
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
            }
        }

        public static void WriteResourceToFile(byte[] resourceName, string fileName)
        {
              System.IO.File.WriteAllBytes(fileName, resourceName);
        }

       
    }
}
