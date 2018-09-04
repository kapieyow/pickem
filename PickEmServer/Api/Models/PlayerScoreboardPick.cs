using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class PlayerScoreboardPick
    {
        public string AwayTeamIconFileName { get; internal set; }
        public int AwayTeamLosses { get; internal set; }
        public string AwayTeamLongName { get; internal set; }
        public int AwayTeamRank { get; internal set; }
        public int AwayTeamScore { get; internal set; }
        public int AwayTeamWins { get; internal set; }
        public string GameStatusDescription { get; internal set; }
        public GameStates GameState { get; internal set; }
        public string HomeTeamIconFileName { get; internal set; }
        public int HomeTeamLosses { get; internal set; }
        public string HomeTeamLongName { get; internal set; }
        public int HomeTeamRank { get; internal set; }
        public int HomeTeamScore { get; internal set; }
        public int HomeTeamWins { get; internal set; }
        public int GameId { get; internal set; }
        public PickTypes Pick { get; internal set; }
        public PickStates PickState { get; internal set; }
        public decimal Spread { get; internal set; }
        public SpreadDirections SpreadDirection { get; internal set; }
    }
}
