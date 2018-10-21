using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class PlayerScoreboard
    {
        public int Games { get; internal set; }
        public int GamesPicked { get; internal set; }
        public int GamesWon { get; internal set; }
        public int GamesLost { get; internal set; }
        public int GamesPending { get; internal set; }

        public List<GameScoreboard> GamePickScoreboards { get; internal set; }
    }
}
