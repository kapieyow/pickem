using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class TeamStatsUpdate
    {
        public int? FbsRank { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}
