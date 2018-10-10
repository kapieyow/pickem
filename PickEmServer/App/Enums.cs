
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PickEmServer.App
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameStates
    {
        SpreadNotSet,
        SpreadLocked,
        InGame,
        Final,
        Cancelled
    }

       
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameLeaderTypes
    {
        Away,
        Home,
        None
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PickemSystemEventTypes
    {
        GameChanged,
        LeagueGameAdded,
        LeaguePlayerAdded,
        LeaguePlayerChanged,
        LeaguePlayerPickChanged,
        SpreadLocked,
        SpreadUpdated,
        SystemSettingsChanged,
        TeamStatsChanged,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PickTypes
    {
        Away,
        Home,
        None,
        Hidden // THIS SHOULD NEVER be in the database (only for UI hides)
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PickStates
    {
        Losing,
        Lost,
        None,
        Pushing,
        Pushed,
        Winning,
        Won
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SpreadDirections
    {
        None,
        ToAway,
        ToHome
    }
}
