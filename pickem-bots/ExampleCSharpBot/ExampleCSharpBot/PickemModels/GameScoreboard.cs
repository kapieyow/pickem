using System;
using System.Collections.Generic;

namespace ExampleCSharpBot.PickemModels
{
    public class GameScoreboard
    {
        public string AwayTeamIconFileName { get; set; }
        public int AwayTeamLosses { get; set; }
        public string AwayTeamLongName { get; set; }
        public int AwayTeamRank { get; set; }
        public int AwayTeamScore { get; set; }
        public int AwayTeamWins { get; set; }
        public string GameCurrentPeriod { get; set; }
        public int GameId { get; set; }
        public DateTime GameStart { get; set; }
        public GameStates GameState { get; set; }
        public TimeSpan GameTimeClock { get; set; }
        public string GameTitle { get; set; }
        public string HomeTeamIconFileName { get; set; }
        public int HomeTeamLosses { get; set; }
        public string HomeTeamLongName { get; set; }
        public int HomeTeamRank { get; set; }
        public int HomeTeamScore { get; set; }
        public int HomeTeamWins { get; set; }
        public decimal Spread { get; set; }
        public SpreadDirections SpreadDirection { get; set; }
        public GameLeaderTypes Leader { get; set; }
        public GameLeaderTypes LeaderAfterSpread { get; set; }
        public int WinPoints { get; set; }

        public List<PickScoreboard> PickScoreboards { get; set; }
    }
}
