using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class TeamSeasonStats
    {
        public string SeasonCodeRef { get; set; }
        public List<TeamWeekStats> WeekStats { get; set; }
    }
}
