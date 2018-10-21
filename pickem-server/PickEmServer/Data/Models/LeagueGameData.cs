using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Data.Models
{
    public class LeagueGameData
    {
        public int GameIdRef { get; set; }

        public List<PlayerPickData> PlayerPicks { get; set; }
    }
}
