using PickEmServer.App;
using System;


namespace PickEmServer.Api.Models
{
    public class Game
    {
        public int GameId { get; internal set; }
        public DateTime LastUpdated { get; internal set; }
        public GameStates GameState { get; internal set; }
        public DateTime GameStart { get; internal set; }
        public string CurrentPeriod { get; internal set; }
        public TimeSpan TimeClock { get; internal set; }
        public Boolean NeutralField { get; internal set; }
        public Spread Spread { get; internal set; }

        public TeamScore AwayTeam { get; internal set; }
        public TeamScore HomeTeam { get; internal set; }

    }
}
