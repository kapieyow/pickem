
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PickEmServer.App
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GameStates
    {
        SpreadNotSet,
        SpreadSet,
        InGame,
        Final,
        Cancelled
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PickTypes
    {
        Away,
        Home,
        None
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum PickStatuses
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
