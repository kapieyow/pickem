using Marten.Schema;
using PickEmServer.App;
using System;

namespace PickEmServer.Data.Models
{
    public class GameData
    {
        [Identity]
        public int GameId { get; internal set; }

        public string SeasonCodeRef { get; internal set; }
        public int WeekNumberRef { get; internal set; }

        public DateTime LastUpdated { get; internal set; }
        public GameStates GameState { get; internal set; }
        public DateTime GameStart { get; internal set; }
        public string CurrentPeriod { get; internal set; }
        public TimeSpan TimeClock { get; internal set; }
        public Boolean NeutralField { get; internal set; }
        public SpreadData Spread { get; internal set; }

        public string AwayTeamCodeRef { get; internal set; }
        public string HomeTeamCodeRef { get; internal set; }
    }
}
