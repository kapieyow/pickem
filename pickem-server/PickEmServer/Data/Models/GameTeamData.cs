using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class GameTeamData
    {
        public string TeamCodeRef { get; set; }
        public int Score { get; set; }
        public decimal ScoreAfterSpread { get; set; }
        public bool Winner { get; set; }
    }
}
