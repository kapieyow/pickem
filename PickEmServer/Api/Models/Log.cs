namespace PickEmServer.Api.Models
{
    public class Log
    {
        public int Id { get; set; }
        public string Component { get; set; }
        public string LogMessage { get; set; }
        public string LogLevel { get; set; }
    }
}
