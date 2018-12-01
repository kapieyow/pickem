using Marten.Schema;
using PickEmServer.App;
using System;
using System.Collections.Generic;


namespace PickEmServer.Data.Models
{
    public class LeagueData
    {
        [Identity]
        public string LeagueCode { get; set; }
        public string LeagueTitle { get; set; }

        public int CurrentWeekRef { get; set; }
        public string NcaaSeasonCodeRef { get; set; }
        public PickemScoringTypes PickemScoringType { get; set; }
        public string SeasonCodeRef { get; set; }

        public List<LeaguePlayerData> Players { get; set; }
        public List<LeagueWeekData> Weeks { get; set; }
        public List<PlayerScoreSubtotalData> PlayerSeasonScores { get; set; }
    }
}
