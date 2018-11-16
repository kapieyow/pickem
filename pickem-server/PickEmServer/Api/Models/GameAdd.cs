using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class GameAdd
    {
        public int GameId { get; set; }
        public DateTime GameStart { get; set; }
        public Boolean NeutralField { get; set; }
        public string AwayTeamCode { get; set; }
        public string HomeTeamCode { get; set; }
        public string GameTitle { get; set; }
    }
}
