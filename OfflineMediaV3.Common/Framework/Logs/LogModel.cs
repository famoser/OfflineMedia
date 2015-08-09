namespace OfflineMediaV3.Common.Framework.Logs
{
    public class LogModel
    {
        public LogLevel LogLevel { get; set; }

        public string Location { get; set; }
        public string Message { get; set; }

        public bool IsReported { get; set; }
    }
}
