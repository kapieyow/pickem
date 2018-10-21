using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class WeekScoreboard
    {
        // TODO: all built assuming all of these lists are the same length and 
        // in the same player order
        public List<string> PlayerTags { get; internal set; }
        public List<GameScoreboard> GamePickScoreboards { get; internal set; }
        public List<PlayerWeekWins> PlayerWins { get; internal set; }
    }
}
