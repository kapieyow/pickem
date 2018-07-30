using Marten.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class WeekScoreboardData
    {
        [Identity]
        public int WeekScoreboardId { get; set; }

        public string SeasonCodeRef { get; set; }
        public string LeagueCodeRef { get; set; }
        public int WeekNumberRef { get; set; }
        
        public List<int> PlayersPicksRefs { get; set; }
    }
}
