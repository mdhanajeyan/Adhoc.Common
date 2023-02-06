using Microsoft.Extensions.Configuration;
using NLog;
using Pdx.Core.Model;
using System.Reflection;
using static Pdx.Core.Data.Dapper.Managers.PdxDBManager;
using static Pricedex.Pim.Common.PimConstants;

namespace Adhoc.Common
{
    public class Options
    {
        private static Settings _appSettings;
        private static PdxJob _pdxjob;
        public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public Options(IConfiguration configuration, PdxJob pdxJob, CommandLineArguments cla)
        {
            _appSettings = configuration.GetSection("AdhocImportSettings").Get<Settings>();
            _pdxjob = pdxJob;

            BinaryVersion = Assembly.GetExecutingAssembly()!.GetName()!.Version!.ToString();

            Customer = _appSettings.Customer;
            Environment = _appSettings.Environment;
            XsdDirectotry = _appSettings.XsdDirectory;


            ConnectionString = configuration.GetSection("ConnectionStrings").Get<Settings>().PdxConnection;
            CommandTimeoutInSecond = configuration.GetSection("ConnectionStrings").Get<Settings>().CommandTimeoutInSeconds;

            CommandLineParser parser = new CommandLineParser();
            parser.ParseArguments(cla.args);
            JobId = parser.JobId;
            RetrieveJobParameters();
            InputFilePath = Path.Combine(JobDirectory, InputFilename);
            LogFilePath = Path.Combine(JobDirectory, LogFileName);
        }

        public static string BinaryVersion { get; private set; }
        public static string ConnectionString { get; private set; }
        public static int CommandTimeoutInSecond { get; private set; }
        public static string Customer { get; private set; }
        public static string Environment { get; private set; }
        public static int JobId { get; private set; }
        public static string LogFileName { get; private set; }
        public static string LogFilePath { get; private set; }
        public static string JobDirectory { get; private set; }
        public static string InputFilename { get; private set; }
        public static string InputFilePath { get; private set; }

        public static string SubmissionType { get; private set; }
        public static string XsdDirectotry { get; private set; }
        public static string PartClass { get; private set; }
        public static int ManufacturerBrandId { get; private set; }
        public static bool AllowApplicationCreation { get; set; }
        public static bool CreateCustomQualifiersFromNotes { get; set; }
        public static bool CreateCustomApps { get; set; }
        public static bool CreateParts { get; set; }
        public static int UserId { get; set; }
        public static int TradingPartnerId { get; set; }
        public static string TradingPartnerName { get; set; }
        public static int RegionId { get; set; }
        public static List<int> PartTerminologiesForManualQuanity { get; set; }
        public static string FileFormat { get; set; }
        public static string FileType { get; set; }

        public static bool UpdateRecords { get; set; }
        public static bool CreateRecords { get; set; }
        public static bool ColumnHeaders { get; set; }
        public static string ClassType { get; set; }

        public static string[] Fields { get; set; }

        private static void RetrieveJobParameters()
        {
            IEnumerable<JobParameter> parameters = _pdxjob.GetParameters(JobId);


            foreach (JobParameter param in parameters)
            {
                switch (param.Name)
                {
                    case Pdx.Core.Common.Constants.Job.Key.JobFolderPath:
                        JobDirectory = param.Value;
                        break;

                    case Pdx.Core.Common.Constants.Job.Key.UploadFileName:
                        InputFilename = param.Value;
                        break;

                    case Pdx.Core.Common.Constants.Job.Key.LogFileName:
                        LogFileName = param.Value;
                        break;

                    case JobUserParameter.UserId:
                        UserId = Convert.ToInt32(param.Value);
                        break;

                    case JobParameters.TradingPartnerId:
                        TradingPartnerId = Convert.ToInt32(param.Value);
                        break;

                    case CheckOut.TradingPartnerName:
                        TradingPartnerName = param.Value;
                        break;


                    case JobParameters.FileFormat:
                        FileFormat = param.Value;
                        break;

                    case JobParameters.FileType:
                        FileType = param.Value;
                        break;

                    case Pdx.Core.Common.Constants.Job.Key.UpdateRecords:
                        UpdateRecords = Convert.ToBoolean(param.Value);
                        break;

                    case Pdx.Core.Common.Constants.Job.Key.CreateRecords:
                        CreateRecords = Convert.ToBoolean(param.Value);
                        break;

                    case Pdx.Core.Common.Constants.Job.Key.ColumnHeaders:
                        ColumnHeaders = Convert.ToBoolean(param.Value);
                        break;

                    case Pdx.Core.Common.Constants.Job.Key.ClassType:
                        ClassType = param.Value;
                        break;

                    case Pdx.Core.Common.Constants.Job.Key.Fields:
                        var withoutQuotes = param.Value.Replace("\"", "").Replace("\t", "");
                        Fields = withoutQuotes.Split(",");
                        break;
                }
            }
        }
    }
}
