using Marten.Schema;
using PickEmServer.App;
using System;

namespace PickEmServer.Data.Models
{
    public class GameData
    {
        [Identity]
        public int GameId { get; set; }

        public string SeasonCodeRef { get; set; }
        public int WeekNumberRef { get; set; }

        public DateTime LastUpdated { get; set; }
        public GameStates GameState { get; set; }
        public DateTime GameStart { get; set; }
        public string CurrentPeriod { get; set; }
        public TimeSpan TimeClock { get; set; }
        public Boolean NeutralField { get; set; }
        public SpreadData Spread { get; set; }
        public GameLeaderTypes Leader { get; set; }
        public GameLeaderTypes LeaderAfterSpread { get; set; }

        public GameTeamData AwayTeam { get; set; }
        public GameTeamData HomeTeam { get; set; }
    }
}
