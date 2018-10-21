using PickEmServer.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PickEmServer.Api.Models
{
    public class PickScoreboard
    {
        public string PlayerTag { get; internal set; }
        public PickTypes Pick { get; internal set; }
        public PickStates PickState { get; internal set; }
        public string PickedTeamIconFileName { get; internal set; }
        public string PickedTeamLongName { get; internal set; }
    }
}
