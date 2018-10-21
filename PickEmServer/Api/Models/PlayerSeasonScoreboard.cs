using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class PlayerSeasonScoreboard
    {
        public string PlayerTag { get; internal set; }
        public int Wins { get; internal set; }

        public List<WeekScore> WeeklyScores { get; internal set; }

    }
}
