using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class PlayerPicks
    {
        public Player Player { get; internal set; }
        public List<GamePick> Picks { get; internal set; }
        public int WeekPoints { get; internal set; }
    }
}
