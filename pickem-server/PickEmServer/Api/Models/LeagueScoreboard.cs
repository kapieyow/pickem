using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class LeagueScoreboard
    {
        public List<int> WeekNumbers { get; internal set; }
        public List<PlayerSeasonScoreboard> PlayerScoreboards { get; internal set; }
    }
}
