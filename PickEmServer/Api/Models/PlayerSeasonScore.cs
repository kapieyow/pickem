using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class PlayerSeasonScore
    {
        public Player Player { get; internal set; }
        public int Points { get; internal set; }

        public List<WeekScore> WeeklyScores { get; internal set; }

    }
}
