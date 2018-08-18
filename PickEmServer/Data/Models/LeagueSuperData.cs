using Marten.Schema;
using System;
using System.Collections.Generic;


namespace PickEmServer.Data.Models
{
    public class LeagueSuperData
    {
        [Identity]
        public string LeagueCode { get; set; }
        public string LeagueTitle { get; set; }

        public string SeasonCodeRef { get; set; }

        public List<string> PlayerTagRefs { get; set; }
        public List<LeagueSuperWeekData> Weeks { get; set; }
        public List<PlayerScoreSubtotalData> PlayerSeasonScores { get; set; }
    }
}
