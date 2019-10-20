using ExampleCSharpBot.PickemModels;
using System;

namespace ExampleCSharpBot
{
    public class AdamsMagicService
    {
        public static readonly GameStates[] BeforeGameStates = new [] { GameStates.SpreadNotSet, GameStates.SpreadLocked };

        public PickTypes ScoreIt(GameScoreboard gameScoreboard)
        {
            var data = new EngineData
            {
                GameScoreboard = gameScoreboard,
                Home = new TeamData {
                    TeamType = GameLeaderTypes.Home,
                    Spread = gameScoreboard.Spread,
                    SpreadDirection = gameScoreboard.SpreadDirection,
                    TeamLosses = gameScoreboard.HomeTeamLosses,
                    TeamWins = gameScoreboard.HomeTeamWins,
                    TeamRank = gameScoreboard.HomeTeamRank
                },
                Away = new TeamData {
                    TeamType = GameLeaderTypes.Away,
                    Spread = gameScoreboard.Spread,
                    SpreadDirection = gameScoreboard.SpreadDirection,
                    TeamLosses = gameScoreboard.AwayTeamLosses,
                    TeamWins = gameScoreboard.AwayTeamWins,
                    TeamRank = gameScoreboard.AwayTeamRank
                }
            };

            ApplyRule(data, (toRate, other) => toRate.TeamRank > 0 ? 1 : 0);
            ApplyRule(data, (toRate, other) => toRate.TeamRank > 0 && toRate.TeamRank < 5 ? 2 : 0);
            ApplyRule(data, (toRate, other) => toRate.TeamRank > 0 && toRate.TeamRank < other.TeamRank ? 1 : 0);
            ApplyRule(data, (toRate, other) => toRate.TeamRank > 0 && toRate.TeamRank < (other.TeamRank - 10) ? 1 : 0);
            ApplyRule(data, (toRate, other) => toRate.Spread == 0 ? 1 : 0);
            ApplyRule(data, (toRate, other) => toRate.TeamType == GameLeaderTypes.Home ? 1 : 0);
            ApplyRule(data, (toRate, other) => toRate.TeamWins > toRate.TeamLosses ? 1 : 0);
            ApplyRule(data, (toRate, other) => toRate.TeamWins > other.TeamWins ? 1 : 0);
            ApplyRule(data, (toRate, other) => toRate.TeamSpread() > 20 ? 4 : 0);

            Console.WriteLine($"Home {data.Home.Rate} Away {data.Away.Rate} {data.Away.TeamSpread()}");

            return data.Home.Rate >= data.Away.Rate 
                ? PickTypes.Home
                : PickTypes.Away;   
        }

        private void ApplyRule(EngineData data, Func<TeamData, TeamData, int> rule)
        {
            data.Home.Rate += rule(data.Home, data.Away);
            data.Away.Rate += rule(data.Away, data.Home);
        }
    }

    public class EngineData
    {
        public TeamData Home { get; set; }
        public TeamData Away { get; set; }
        public GameScoreboard GameScoreboard { get; set; }
    }

    public class TeamData
    {
        public int Rate { get; set; } = 0;
        public GameLeaderTypes TeamType { get; set; }
        public int TeamRank { get; set; }
        public int TeamWins { get; set; }
        public int TeamLosses { get; set; }
        public decimal Spread { get; set; }
        public SpreadDirections SpreadDirection { get; set; }
        
        public int TeamSpread() 
        {
            if (TeamType == GameLeaderTypes.Away
                && SpreadDirection == SpreadDirections.ToAway)
                return (int)Spread;

            if (TeamType == GameLeaderTypes.Home
                && SpreadDirection == SpreadDirections.ToHome)
                return (int)Spread;

            return 0;
        }
    }
}