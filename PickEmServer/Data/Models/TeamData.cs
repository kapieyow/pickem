using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class TeamData
    {
        [Identity]
        public string TeamCode { get; internal set; }
        public string ShortName { get; internal set; }
        public string LongName { get; internal set; }
        public string NcaaNameSeo { get; internal set; }
        public string theSpreadName { get; internal set; }
        public string icon24FileName { get; internal set; }
    }
}
