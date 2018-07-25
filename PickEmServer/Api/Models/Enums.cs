using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public enum GameStates
    {
        SpreadNotSet,
        SpreadSet,
        InGame,
        Final,
        Cancelled
    }

    public enum PickTypes
    {
        Away,
        Home,
        None
    }

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

    public enum SpreadDirections
    {
        None,
        ToAway,
        ToHome
    }
}
