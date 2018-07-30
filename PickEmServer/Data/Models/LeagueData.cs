using Marten.Schema;

namespace PickEmServer.Data.Models
{
    public class LeagueData
    {
        [Identity]
        public string LeagueCode { get; set; }
        public string LeagueTitle { get; set; }
    }
}
