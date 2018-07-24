using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class LeagueScoreboard
    {
        public League League { get; internal set; }
        public Season Season { get; internal set; }

        public List<PlayerSeasonScore> PlayerScores { get; internal set; }

    }
}
