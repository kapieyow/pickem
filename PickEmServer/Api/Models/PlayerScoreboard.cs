using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class PlayerScoreboard
    {
        public Season Season { get; internal set; }
        public League League { get; internal set; }
        public int WeekNumber { get; internal set; }
        public Player Player { get; internal set; }

        public List<GamePick> Picks { get; internal set; }
    }
}
