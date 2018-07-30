using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class TeamData
    {
        [Identity]
        public string TeamCode { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string NcaaNameSeo { get; set; }
        public string theSpreadName { get; set; }
        public string icon24FileName { get; set; }
    }
}
