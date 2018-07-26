using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class LeagueData
    {
        [Identity]
        public string LeagueCode { get; internal set; }
        public string LeagueTitle { get; internal set; }
    }
}
