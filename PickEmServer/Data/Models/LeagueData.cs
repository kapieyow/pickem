using Marten.Schema;
using System;
using System.Collections.Generic;


namespace PickEmServer.Data.Models
{
    public class LeagueData
    {
        [Identity]
        public string LeagueCode { get; set; }
        public string LeagueTitle { get; set; }

        public string SeasonCodeRef { get; set; }

        public List<LeaguePlayerData> Players { get; set; }
        public List<LeagueWeekData> Weeks { get; set; }
        public List<PlayerScoreSubtotalData> PlayerSeasonScores { get; set; }
    }
}
