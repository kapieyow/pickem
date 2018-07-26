using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class PlayerSeasonScoreData
    {
        public string PlayerTagRef { get; internal set; }
        public int Points { get; internal set; }

        public List<WeekScoreData> WeeklyScores { get; internal set; }
    }
}
