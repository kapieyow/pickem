using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class TeamWeekStats
    {
        public int WeekNumberRef { get; set; }
        public int? FbsRank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
