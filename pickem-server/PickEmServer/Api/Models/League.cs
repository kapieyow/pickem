using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class League
    {
        public int CurrentWeekRef { get; internal set; }
        public string LeagueCode { get; internal set; }
        public string LeagueTitle { get; internal set; }
        public string NcaaSeasonCodeRef { get; set; }
        public PickemScoringTypes PickemScoringType { get; set; }
        public string SeasonCodeRef { get; set; }
    }
}
