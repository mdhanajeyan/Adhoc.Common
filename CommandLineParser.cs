namespace Adhoc.Common
{
    public class CommandLineParser
    {
        public int JobId { get; private set; }
        public bool DebugMode { get; private set; }

        public void ParseArguments(string[] args)
        {
            List<string> mandatoryArgs = new List<string>() { "jobid" };
            for (var i = 0; i < args.Length; ++i)
            {
                string key = args[i].Substring(1, args[i].IndexOf('=') - 1).Trim().ToLower();
                string value = args[i].Substring(args[i].IndexOf('=') + 1);
                switch (key.ToLower())
                {
                    case "jobid":
                        JobId = Int32.Parse(value);
                        break;
                    case "debugging":
                        switch (value.ToLower())
                        {
                            case "true":
                            case "yes":
                            case "1":
                                DebugMode = true;
                                break;
                            default:
                                DebugMode = false;
                                break;
                        }
                        break;
                }
                mandatoryArgs.Remove(key);
            }
            if (mandatoryArgs.Count > 0)
            {
                string missingKeys = string.Empty;
                foreach (var key in mandatoryArgs)
                {
                    if (!string.IsNullOrEmpty(missingKeys)) missingKeys += ", ";
                    missingKeys += key;
                }

                /*
                using (EventLog eventLog = new EventLog("Application"))
                {
                    string error = "Required command line arguments not found: " + missingKeys;
                    eventLog.Source = Program.EVENT_SOURCE;
                    eventLog.WriteEntry(error, EventLogEntryType.Error);
                    throw new Exception(error);
                }
                */
            }
        }
    }
}
