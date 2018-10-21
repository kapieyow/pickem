using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class SeasonData
    {
        [Identity]
        public string SeasonCode { get; set; }
        public string SeasonTitle { get; set; }
    }
}
