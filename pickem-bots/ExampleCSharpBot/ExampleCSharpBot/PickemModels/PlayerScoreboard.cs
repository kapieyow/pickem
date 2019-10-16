using System.Collections.Generic;

namespace ExampleCSharpBot.PickemModels
{
    public class PlayerScoreboard
    {
        public int Games { get; set; }
        public int GamesPicked { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int GamesPending { get; set; }
        public int Points { get; set; }

        public List<GameScoreboard> GamePickScoreboards { get; set; }
    }
}
