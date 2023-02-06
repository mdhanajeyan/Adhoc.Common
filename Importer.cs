using ClosedXML.Excel;
using Microsoft.VisualBasic.FileIO;
using NLog;
using NLog.Config;
using NLog.Targets;
using System.Data;

namespace Adhoc.Common
{
    public class Importer
    {

        public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Importer(Options options)
        {

        }

        public DataTable Setup()
        {
            try
            {
                ConfigureLogger(Options.LogFilePath);


                _logger.Info($"ADHOC Import process started");

                _logger.Info("".PadRight(80, '='));
                _logger.Info($"Binary version: {Options.BinaryVersion}");
                _logger.Info($"Customer: {Options.Customer}");
                _logger.Info($"Environment: {Options.Environment}");
                _logger.Info($"{"JobId",-40}{Options.JobId}");
                _logger.Info($"Import file: {Options.InputFilePath}");
                _logger.Info("".PadRight(80, '='));

                if (!File.Exists(Options.InputFilePath))
                    throw new Exception($"Input file '{Options.InputFilePath}' not found.");

                return ProcessDocument();
                //if (Options.FileFormat == PimConstants.Adhoc.JobParameter.XmlFileFormat)
                //    _document = XDocument.Load(Options.InputFilePath, LoadOptions.SetLineInfo);

            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Aborting with error.");
                throw;
            }
        }

        private DataTable ProcessDocument()
        {
            var filePath = Options.InputFilePath;
            var dir = new DirectoryInfo(Options.JobDirectory);
            DataTable dt = new DataTable();
            foreach (FileInfo fi in dir.GetFiles())
            {
                var ext = fi.Extension;

                switch (ext)
                {
                    case ".xlsx":
                        dt = GetExcelDataTable(fi.ToString());
                        break;
                    case ".csv":
                        dt = GetDataTabletFromCSVFile(filePath);
                        break;

                }
            }

            return dt;
        }

        private static DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new();

            try
            {

                using (TextFieldParser csvReader = new(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[]? colFields;

                    if (Options.ColumnHeaders)
                    {
                        colFields = csvReader.ReadFields();
                    }
                    else
                    {
                        colFields = Options.Fields;
                    }

                    foreach (string column in colFields!)
                    {
                        DataColumn datacolumn = new(column)
                        {
                            AllowDBNull = true
                        };

                        csvData.Columns.Add(datacolumn);
                    }

                    while (!csvReader!.EndOfData)
                    {
                        string[]? fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData!.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Reading CSV into Datatable " + ex);
            }
            return csvData;
        }

        public static DataTable GetExcelDataTable(string filePath)
        {
            DataTable dt = new();
            using (XLWorkbook workBook = new(filePath))
            {
                IXLWorksheet workSheet = workBook.Worksheet(1);
                bool firstRow = true;
                foreach (IXLRow row in workSheet.Rows())
                {
                    if (firstRow)
                    {
                        if (Options.ColumnHeaders)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Columns.Add(cell.Value.ToString());
                            }
                        }
                        else
                        {
                            string[] colFields = Options.Fields;
                            foreach (var col in colFields)
                            {
                                dt.Columns.Add(col);
                            }

                            dt.Rows.Add();
                            int i = 0;
                            foreach (IXLCell cell in row.Cells())
                            {
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                i++;
                            }
                        }
                        firstRow = false;
                    }
                    else
                    {
                        dt.Rows.Add();
                        int i = 0;
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                            i++;
                        }
                    }
                }
            }

            return dt;
        }

        private static void ConfigureLogger(string logFilePath)
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget("target2")
            {
                FileName = logFilePath,
                Layout = @"${date:format=dd MMM yyy HH\:mm\:ss tt} [${level:uppercase=true}]: ${message} ${exception:format=toString}"
            };
            config.AddTarget(fileTarget);
            config.AddRuleForAllLevels(fileTarget);
            LogManager.Configuration = config;
            LogManager.Configuration.AddTarget(fileTarget);
        }
    }
}