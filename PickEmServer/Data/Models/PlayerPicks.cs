using Marten.Schema;
using System.Collections.Generic;

namespace PickEmServer.Data.Models
{
    public class PlayerPicks
    {

        [Identity]
        public int PlayerPicksId { get; set; }

        public string SeasonCodeRef { get; set; }
        public string LeagueCodeRef { get; set; }
        public int WeekNumberRef { get; set; }
        public string PlayerTagRef { get; set; }

        public List<GamePickData> Picks { get; set; }
    }
}
