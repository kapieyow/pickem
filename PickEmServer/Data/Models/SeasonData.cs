using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class SeasonData
    {
        [Identity]
        public string SeasonCode { get; internal set; }
        public string SeasonTitle { get; internal set; }
    }
}
