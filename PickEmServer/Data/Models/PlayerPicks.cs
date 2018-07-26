using Marten.Schema;
using System.Collections.Generic;

namespace PickEmServer.Data.Models
{
    public class PlayerPicks
    {

        [Identity]
        public int PlayerPicksId { get; internal set; }

        public string SeasonCodeRef { get; internal set; }
        public string LeagueCodeRef { get; internal set; }
        public int WeekNumberRef { get; internal set; }
        public string PlayerTagRef { get; internal set; }

        public List<GamePickData> Picks { get; internal set; }
    }
}
