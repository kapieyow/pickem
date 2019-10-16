namespace ExampleCSharpBot.PickemModels
{
    public class League
    {
        public int CurrentWeekRef { get; set; }
        public string LeagueCode { get; set; }
        public string LeagueTitle { get; set; }
        public string NcaaSeasonCodeRef { get; set; }
        public PickemScoringTypes PickemScoringType { get; set; }
        public string SeasonCodeRef { get; set; }
    }
}
