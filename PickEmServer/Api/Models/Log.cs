namespace PickEmServer.Api.Models
{
    public class Log
    {
        public int Id { get; internal set; }
        public string Component { get; internal set; }
        public string LogMessage { get; internal set; }
        public string LogLevel { get; internal set; }
    }
}
