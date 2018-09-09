using System.Collections.Generic;

namespace PickEmServer.Api.Models
{
    public class PlayerScoreboard
    {
        public string PlayerTag { get; internal set; }
        public int Wins { get; internal set; }
        public List<PlayerScoreboardPick> PlayerScoreboardPicks { get; internal set; }
    }
}