using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class PlayerSeasonScoreData
    {
        public string PlayerTagRef { get; set; }
        public int Points { get; set; }

        public List<WeekScoreData> WeeklyScores { get; set; }
    }
}
