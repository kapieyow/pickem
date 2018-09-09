using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class WeekScoreboard
    {
        // sets of players using same model as players for now
        public List<PlayerScoreboard> PlayerScoreboards { get; internal set; }
    }
}
