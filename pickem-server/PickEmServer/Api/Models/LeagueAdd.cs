using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class LeagueAdd
    {
        public int CurrentWeekRef { get; set; }
        public string LeagueCode { get; set; }
        public string LeagueTitle { get; set; }
        public string NcaaSeasonCodeRef { get; set; }
        public PickemScoringTypes PickemScoringType { get; set; }
        public string SeasonCodeRef { get; set; }

        public List<int> WeekNumbers { get; set; }
    }
}
