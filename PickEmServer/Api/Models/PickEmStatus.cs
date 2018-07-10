namespace PickEmServer.Api.Models
{
    public class PickEmStatus
    {
        public PickEmStatus() { }

        public string Database { get; internal set; }
        public string DatabaseHost { get; internal set; }
        public string Product { get; internal set; }
        public string ProductVersion { get; internal set; }
        public string RuntimeEnvironment { get; internal set; }
    }
}
