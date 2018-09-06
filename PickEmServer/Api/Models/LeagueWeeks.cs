using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class LeagueWeeks
    {
        public List<int> WeekNumbers { get; internal set; }
        public int CurrentWeekNumber { get; internal set; }
    }
}
