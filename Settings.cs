namespace Adhoc.Common
{
    public class Settings
    {
        // Example: Production, QA, Adrian's Machine
        public string Environment { get; set; }

        // Example: Dorman, Cequent, etc
        public string Customer { get; set; }
        public string XsdDirectory { get; set; }

        // Read from section ConnectionStrings
        public string PdxConnection { get; set; }
        public int CommandTimeoutInSeconds { get; set; }
        public string ErrorMailRecipient { get; set; }
        public bool OnlyProcessAppsWithParam { get; set; }
               
    }
}
