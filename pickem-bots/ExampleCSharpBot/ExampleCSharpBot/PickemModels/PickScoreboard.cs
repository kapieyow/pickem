namespace ExampleCSharpBot.PickemModels
{
    public class PickScoreboard
    {
        public string PlayerTag { get; set; }
        public PickTypes Pick { get; set; }
        public PickStates PickState { get; set; }
        public string PickedTeamIconFileName { get; set; }
        public string PickedTeamLongName { get; set; }
    }
}
