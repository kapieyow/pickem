using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class LeagueWeekData
    {
        public int WeekNumberRef { get; set; }

        public List<LeagueGameData> Games { get; set; }

        public List<PlayerScoreSubtotalData> PlayerWeekScores { get; set; }
    }
}
