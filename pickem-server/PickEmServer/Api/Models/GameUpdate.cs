using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class GameUpdate
    {
        public DateTime LastUpdated { get; set; }
        public GameStates GameState { get; set; }
        public DateTime GameStart { get; set; }
        public string CurrentPeriod { get; set; }
        public TimeSpan TimeClock { get; set; }
        public int AwayTeamScore { get; set; }
        public int HomeTeamScore { get; set; }
    }
}
