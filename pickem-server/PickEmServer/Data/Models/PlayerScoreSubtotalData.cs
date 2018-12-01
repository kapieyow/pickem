
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class PlayerScoreSubtotalData
    {
        public string PlayerTagRef { get; set; }
        public int Points { get; set; }

        public int GamesPicked { get; set; }
        public int GamesLost { get; set; }
        public int GamesPending { get; set; }
        public int GamesWon { get; set; }
    }
}
