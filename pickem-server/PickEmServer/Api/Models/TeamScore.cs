using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class TeamScore
    {
        public Team Team { get; internal set; }
        public int Score { get; internal set; }
        public decimal ScoreAfterSpread { get; internal set; }
        public bool Winner { get; internal set; }
    }
}
